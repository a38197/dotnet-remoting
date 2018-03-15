using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SuperSoftware.Shared;

namespace SuperSoftware.Server
{

    public interface IServerExtended : IServer {
        void Start();
        void Stop();
        void Register(string[] productFamilies, IStockManager sm, PropagateToken token);
        void Unregister(IStockManager sm, PropagateToken token);
        bool HasStockManager(IStockManager sm);
        void AddZoneServer(IServerExtended server, PropagateToken token);
        void RemoveZoneServer(string serverUid, PropagateToken token);
        void SendServerAction(PropagateToken token);
        void ReceiveServerAction(PropagateToken token);
        IServerExtended[] GetRingServerList();
        int RingIndex { get; }
    }

    internal class CachedServer
    {
        private readonly IServerExtended proxy;
        public IServerExtended ServerProxy { get { return proxy; } }

        private readonly string uid;
        public string UID { get { return uid; } }

        private readonly int index;
        public int RingIndex { get { return index; } }

        public CachedServer(IServerExtended server)
        {
            proxy = server;
            uid = server.GetUID();
            index = server.RingIndex;
        }
    }

    public class Server : MarshalByRefObject, IServerExtended
    {
        private static Logger logger = new Logger(Console.Out, true);
        static Server()
        {
            logger.Start();
        }

        private readonly CachedServer myServer;
        private readonly PropagateToken discoverToken;
        public Server() {
            var key = System.Configuration.ConfigurationManager.AppSettings[ConfigurationFactory.SERVER_RING_INDEX_KEY];
            ringIndex = Int32.Parse(key);
            discoverToken = new PropagateToken(PropagateAction.AddServer, this);
            myServer = new CachedServer(this);
        }

        public Server(ref IServerExtended singleton) : this()
        {
            singleton = this;
        }

        private string serverUid = Guid.NewGuid().ToString();
        public string GetUID()
        {
            return serverUid;
        }

        public void SendMessage(string msg)
        {
            logger.Info("Received message [{0}]", msg);
        }

        private readonly Dictionary<string, CachedServer> knownServers = new Dictionary<string, CachedServer>();
        private readonly Dictionary<string, Dictionary<string,IStockManager>> familiesMap = new Dictionary<string, Dictionary<string, IStockManager>>();

        private CachedServer nextRingServer;

        private readonly int ringIndex;
        public int RingIndex
        {
            get
            {
                return ringIndex;
            }
        }

        public void SendServerAction(PropagateToken token)
        {
            if (token == null)
                return; //Do not propagate

            token.AddHop(this);
            var servCopy = nextRingServer;
            ThreadPool.QueueUserWorkItem((result) =>
            {
                try
                {
                    logger.Debug("Propagating action {0} to server {1} on index.", token.Action.ToString(), servCopy.RingIndex);
                    servCopy.ServerProxy.ReceiveServerAction(token);
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    logger.Debug(ex, "Exception on propagate to index {0}", servCopy.RingIndex);
                    logger.Error("Error propagating action {0} to server {1} on index, removing server.", 
                        token.Action.ToString(), servCopy.RingIndex);

                    RemoveZoneServer(servCopy.UID);
                    if(servCopy.UID != nextRingServer.UID && nextRingServer.UID != this.serverUid)
                        SendServerAction(token);
                }
            });
        }

        public void ReceiveServerAction(PropagateToken token) {
            if (checkDiscovery(token)) return;

            logger.Debug("Token {0} received for action {1}", token.GetUID(), token.Action.ToString());
            if (token.HasHopedOnObject(this)) {
                logger.Debug("Token {0} already processed, discarding", token.GetUID());
                return;
            }
                

            object[] pars;
            switch (token.Action){
                case PropagateAction.AddServer:
                    AddZoneServer((IServerExtended)token.Data, token);
                    break;
                case PropagateAction.RemoveServer:
                    RemoveZoneServer((string)token.Data, token);
                    break;
                case PropagateAction.Register:
                    pars = (object[])token.Data;
                    Register((string[])pars[0], (IStockManager)pars[1],token);
                    break;
                case PropagateAction.Unregister:
                    Unregister((IStockManager)token.Data, token);
                    break;
            }
        }

        private bool checkDiscovery(PropagateToken token)
        {
            if (discoveryComplete) return false;

            lock (knownServers)
            {
                if (!discoveryComplete && (discoverToken.GetUID() == token.GetUID()))
                {
                    discoveryComplete = true;
                    logger.Info("Received discovery token. Server is on the ring.");
                    return true;
                }
            }

            return false;
            
        }

        public void Register(string[] productFamilies, IStockManager sm)
        {
            object[] pars = new object[] { productFamilies, sm };
            Register(productFamilies, sm, new PropagateToken(PropagateAction.Register, pars));
        }

        public void Register(string[] productFamilies, IStockManager sm, PropagateToken token)
        {
            logger.Info("Registering families {0} for stock manager {1}", productFamilies, sm.GetUID());
            lock (familiesMap)
            {
                foreach(string family in productFamilies)
                {
                    if (!familiesMap.ContainsKey(family))
                        familiesMap.Add(family, new Dictionary<string, IStockManager>());

                    var dic = familiesMap[family];
                    if (!dic.ContainsKey(sm.GetUID()))
                        dic.Add(sm.GetUID(), sm);
                    else
                        logger.Warn("Stock manager {0} already registered, discarding", sm.GetUID());
                    
                }
            }
            SendServerAction(token);
        }

        public void Unregister(IStockManager sm)
        {
            Unregister(sm, new PropagateToken(PropagateAction.Unregister, sm));
        }

        public void Unregister(IStockManager sm, PropagateToken token)
        {
            logger.Info("Unregistering stock manager {0}", sm.GetUID());
            lock (familiesMap)
            {
                foreach (var entry in familiesMap)
                    if (entry.Value.ContainsKey(sm.GetUID()))
                        entry.Value.Remove(sm.GetUID());
            }
            SendServerAction(token);
        }

        public bool HasStockManager(IStockManager sm)
        {
            foreach (var entry in familiesMap)
                if (entry.Value.ContainsKey(sm.GetUID()))
                    return true;
            return false;
        }

        public void AddZoneServer(IServerExtended server)
        {
            AddZoneServer(server, new PropagateToken(PropagateAction.AddServer, server));
        }

        public void AddZoneServer(IServerExtended server, PropagateToken token)
        {
            logger.Info("Adding zone server with index {0}", server.RingIndex);
            lock (knownServers)
            {
                checkRingData(server);
                if (!knownServers.ContainsKey(server.GetUID()))
                {
                    CachedServer cached = new CachedServer(server);
                    knownServers.Add(cached.UID, cached);
                }
                    
                nextRingServer = getNextServerOnRing();
            }
            SendServerAction(token);
        }

        private volatile bool discoveryComplete = false;

        private void checkRingData(IServerExtended server)
        {
            foreach (var ks in knownServers)
                if (server.RingIndex == ks.Value.RingIndex)
                    throw new ArgumentException($"RingIndex {server.RingIndex} already exists!");
        }

        public void RemoveZoneServer(string serverUid)
        {
            RemoveZoneServer(serverUid, new PropagateToken(PropagateAction.RemoveServer, serverUid));
        }

        public void RemoveZoneServer(string serverUid, PropagateToken token)
        {
            CachedServer cachedServer;
            if (!knownServers.TryGetValue(serverUid, out cachedServer))
            {
                logger.Info("Cannot remove unknown server with uid {0}", serverUid);
                return;
            }

            logger.Info("Removing zone server with index {0}", cachedServer.RingIndex);
            lock (knownServers)
            {
                knownServers.Remove(cachedServer.UID);

                if(cachedServer.UID == nextRingServer.UID)
                    nextRingServer = getNextServerOnRing();
            }

            SendServerAction(token);
        }

        private CachedServer getNextServerOnRing()
        {
            if (knownServers.Count == 0)
            {
                logger.Debug("Only this server is on ring");
                return myServer;
            }

            CachedServer next;
            if (knownServers.Count == 1)
            {
                next = knownServers.First().Value;
                logger.Debug("Next server on ring has the UID {0} and index {1}", next.UID, next.RingIndex);
                return next;
            }
                

            var sorted = from servers in knownServers.Values
                         orderby servers.RingIndex ascending
                         select servers;

            foreach (var s in sorted)
                if (s.RingIndex > ringIndex) {
                    logger.Debug("Next server on ring has the UID {0} and index {1}", s.UID, s.RingIndex);
                    return s;
                }

            next = sorted.First();
            logger.Debug("Next server on ring has the UID {0} and index {1}", next.UID, next.RingIndex);
            return next;
        }

        /**
            <summary>
                Discovery process is done by acquiring a list of possible ring servers from a config file.
                Then it asks a complete server list for the ring one server on the config file.
                A server is only considered to be in the ring if, when sending the the AddZoneServer token, it arrives with the same list of objects retreived from the server
                The first server shall not have any servers on the configuration file, so it assumes the ring is empty
            </summary>
        **/
        
        private void registerAndDiscoverRingServers()
        {
            logger.Info("Starting discovery process");
            var configInfo = getServersFromFile();
            if (configInfo.Count() == 0)
            {
                logger.Info("No servers on config file. Assuming empty ring");
                nextRingServer = myServer;
                return; //Considered first ring server
            }
                

            var ringServerProxies = getRingServers(configInfo);
            logger.Info("Got ring server list with {0} entries", ringServerProxies.Count());

            foreach (var proxy in ringServerProxies)
            {
                var server = new CachedServer(proxy);
                if (server.RingIndex == this.RingIndex)
                {
                    logger.Error("Server with ring index {0} already exist. Aborting.", server.RingIndex);
                    throw new DiscoveryException();
                }
                    

                knownServers.Add(proxy.GetUID(), server);
            }
                

            nextRingServer = getNextServerOnRing();

            logger.Info("Trying to register server on ring");
            SendServerAction(discoverToken);
        }

        private IEnumerable<IServerInfo> getServersFromFile()
        {
            logger.Debug("Getting server configuration file");
            try {
                ConfigurationFactory.GetConfiguration().LoadFromFile();
                return ConfigurationFactory.GetConfiguration().GetAllServers();
            } catch (System.IO.IOException ex)
            {
                ConfigurationFactory.GetConfiguration().AddServer("DummyName", "127.0.0.1", 1, ChannelType.Tcp);
                ConfigurationFactory.GetConfiguration().SaveToFile();
                string message = $"Unable to load configuration from file!Created dummy file at {Environment.CurrentDirectory}{Environment.NewLine}" +
                    "Please fill the configuration file with the correct information.";

                throw new ConfigurationException(message, ex);
            }
        }

        private IEnumerable<IServerExtended> getRingServers(IEnumerable<IServerInfo> config)
        {
            foreach(var info in config)
            {
                try
                {
                    var sProxy = mapServerFromConfig(info);
                    IServerExtended[] list = sProxy.GetRingServerList();
                    return list.OrderBy((value) => value.RingIndex);
                } catch (Exception ex)
                {
                    logger.Warn("Dicovery of server {0} returned exception: {1}.", info.ServerEndpoint.ToString(), ex.Message);
                    logger.Debug(ex, "Discovery Exception");
                }
            }
            throw new DiscoveryException("Unable to discover ring of servers");
        }

        private IServerExtended mapServerFromConfig(IServerInfo info)
        {
            
            var uri = String.Format("{0}://{1}:{2}/{3}", 
                info.ServerChannelType, 
                info.ServerEndpoint.Address.ToString(),
                info.ServerEndpoint.Port,
                ConfigurationFactory.SERVER_OBJECT_URI);

            logger.Debug("Trying to get server proxy from uri {0}", uri);
            return (IServerExtended)Activator.GetObject(typeof(IServerExtended), uri);
        }

        public IServerExtended[] GetRingServerList()
        {
            logger.Info("Returning server list");
            lock(knownServers){
                LinkedList<IServerExtended> copy = 
                    new LinkedList<IServerExtended>(knownServers.Values.Select((cache)=>cache.ServerProxy));

                copy.AddLast(this);
                return copy.ToArray();
            }
            
        }

        public void Start()
        {
            registerAndDiscoverRingServers();
            logger.Info("Server started with with ring id " + ringIndex);
        }

        public void Stop()
        {
            logger.Info("Sending stop signal and unregister from ring");
            if (nextRingServer != myServer)
                nextRingServer.ServerProxy.RemoveZoneServer(serverUid,
                    //Proxy will cease to exist, serializable version of IUniqueObject
                    new PropagateToken(PropagateAction.RemoveServer, serverUid)); 

            logger.Stop();
        }


        public IStockManager[] GetStockManagers(string productFamily, IUniqueObject sender)
        {
            if (familiesMap.ContainsKey(productFamily))
            {
                return (from sm in familiesMap[productFamily]
                       where sm.Key != sender.GetUID()
                       select sm.Value).ToArray();
            }

            return new IStockManager[0];
        }
    }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;
using System.IO;
using System.Collections;

namespace SuperSoftware.Shared
{
    public interface IConfiguration {
        IEnumerable<IServerInfo> GetAllServers();
        void SaveToFile();
        void SaveToFile(string fileName);
        void LoadFromFile();
        void LoadFromFile(string fileName);
        void AddServer(string name, string ip, int port, ChannelType channel);
        void RemoveServer(string name);
    }

    public static class ConfigurationFactory {
        public static IConfiguration GetConfiguration() {
            return Configuration.Instance;
        }

        public const string SERVER_RING_INDEX_KEY = "RingIndex";
        public const string SERVER_OBJECT_URI = "ZoneServer.service";
        public const string SERVER_EXTENDED_OBJECT_URI = "ZoneServerExtended.service";

        public static IChannel GetChannel(ChannelType type, int port = 0, string channelName="")
        {
            Preconditions.Check(() => type != ChannelType.Ipc ? true : channelName.Length > 0, "On Ipc channel the name is mandatory");
            Preconditions.Check(() => port > 0, "On Tcp or Http port must be greater than 0");
            switch (type)
            {
                case ChannelType.Tcp:
                    BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
                    provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                    IDictionary props = new Hashtable();
                    props["port"] = port;
                    return new TcpChannel(props, null, provider);
                case ChannelType.Http:
                    return new HttpChannel(port);
                case ChannelType.Ipc:
                    return new IpcChannel(channelName);
                default:
                    throw new ArgumentException("Invalid type");
            }
        }

        public static ChannelType GetChannelType(IChannel channel)
        {
            if (channel is TcpChannel)
            {
                return ChannelType.Tcp;
            }

            if (channel is HttpChannel)
            {
                return ChannelType.Http;
            }

            if (channel is IpcChannel)
            {
                return ChannelType.Ipc;
            }

            throw new ArgumentException("Channel not supported");
        }

        
    }

    public enum ChannelType { 
        Tcp, Http, Ipc
    }

    public sealed class Configuration : IConfiguration
    {

        //TODO: Change condifguration to load default configuration from app.config

        public static Configuration Instance = new Configuration();
        SerializationAdapter<ServerInfo[]> serializer = new SerializationAdapter<ServerInfo[]>();

        private Configuration() {
        }

        private readonly string FILENAME = "ZoneServers.config";
        private List<ServerInfo> servers = new List<ServerInfo>();

        public IEnumerable<IServerInfo> GetAllServers()
        {
            return servers;
        }

        public void LoadFromFile()
        {
            loadInternal(FILENAME);
        }

        public void LoadFromFile(string fileName)
        {
            loadInternal(fileName);
        }

        public void SaveToFile()
        {
            saveInternal(FILENAME);
        }

        public void SaveToFile(string fileName)
        {
            saveInternal(fileName);
        }

        public void AddServer(string name, string ip, int port, ChannelType channel)
        {
            var info = new ServerInfo(name, ip, port, channel);
            servers.Add(info);
        }

        public void RemoveServer(string name)
        {
            var server = servers.Find(s => s.ServerName == name);
            if (null != server) servers.Remove(server);
        }

        private void saveInternal(string fileName) {
            serializer.SerializeToFile(fileName, servers.ToArray());
        }

        private void loadInternal(string fileName)
        {
            var temp = serializer.DerializeFromFile(fileName);
            servers.Clear();
            servers.AddRange(temp);
        }

    }

    public interface IServerInfo
    {
        string ServerName { get; }
        IPEndPoint ServerEndpoint { get; }
        ChannelType ServerChannelType { get; }
    }

    [Serializable]
    public class ServerInfo : IServerInfo
    {

        public string ServerName{ get; set; }

        public string IpAddress;

        public int Port;

        public string ChannelType;
        
        public ChannelType ServerChannelType
        {
            get
            {
                return (ChannelType)Enum.Parse(typeof(ChannelType), ChannelType);
            }
        }

        public IPEndPoint ServerEndpoint
        {
            get
            {
                return new IPEndPoint(IPAddress.Parse(IpAddress), Port);
            }
        }

        private ServerInfo() { } //for serialization
        internal ServerInfo(string name, string ipAddress, int port, ChannelType channel)
        {
            Preconditions.CheckNotNull(name);
            Preconditions.CheckNotEmpty(ipAddress);
            Preconditions.Check(() => port > 0, "Port should be > 0");
            ServerName = name;
            IpAddress = IPAddress.Parse(ipAddress).ToString();
            Port = port;
            ChannelType = Enum.GetName(channel.GetType() ,channel);
        }
    }
    
}

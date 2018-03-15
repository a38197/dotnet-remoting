using System;
using System.Linq;
using System.Text;
using SuperSoftware.Shared;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Configuration;

namespace SuperSoftware.Server
{
    class Program
    {
        static void programaticStartup()
        {
            Console.WriteLine("Programatic startup");
            instance = new Server();

            var channelNumber = Int32.Parse(ConfigurationManager.AppSettings["Port"]);
            Console.WriteLine("Creating channel {0} on port {1}", ChannelType.Tcp, channelNumber);
            IChannel channel = ConfigurationFactory.GetChannel(ChannelType.Tcp, channelNumber);
            bool secure = Boolean.Parse(ConfigurationManager.AppSettings["Secure"]);
            ChannelServices.RegisterChannel(channel, secure); 

            //Todo: Define object lifetime (statefull or at least long lifetime??) See slides leases
            Console.WriteLine("Registering server");
            RemotingServices.Marshal((Server)instance, ConfigurationFactory.SERVER_OBJECT_URI, typeof(IServerExtended));
            instance.Start();
        }

        private static IServerExtended instance;

        private static bool configStartup = true;
        private const string CONFIG = "configFile", PROGRAMATIC = "staticStartup";
        static void Main(string[] args)
        {
            processArgs(args);

            if(configStartup)
                configFileStartup(); 
            else
                programaticStartup();

            Console.ReadKey();
            if(instance!=null)
                instance.Stop();
        }

        private static void processArgs(string[] args)
        {
            if (args.Length > 0 && args.Contains(CONFIG))
                configStartup = true;

            if (args.Length > 0 && args.Contains(PROGRAMATIC))
                configStartup = false;
        }

        private static void configFileStartup()
        {
            Console.WriteLine("Configuration file startup");
            Console.WriteLine("Registering service types");
            var configFile = String.Format("{0}.config", AppDomain.CurrentDomain.FriendlyName);
            RemotingConfiguration.Configure(configFile, false);
            printRemoteConfig(Console.Out);
            instantiateSingletonServer();
        }

        private static void instantiateSingletonServer()
        {
            var type = RemotingConfiguration.GetRegisteredWellKnownServiceTypes()[0];
            var channel = ChannelServices.RegisteredChannels[0];
            string uri = String.Format("{0}://127.0.0.1:{1}/{2}",
                ConfigurationManager.AppSettings["Channel"],
                ConfigurationManager.AppSettings["Port"], type.ObjectUri);
            instance = (IServerExtended)Activator.GetObject(typeof(IServerExtended), uri);
            instance.SendMessage(String.Format("Singleton Created on port channel {0} and port {1}",
                ConfigurationManager.AppSettings["Channel"],
                ConfigurationManager.AppSettings["Port"]));
            instance.Start();
        }

        private static void printRemoteConfig(System.IO.TextWriter writer)
        {
            foreach(var channel in ChannelServices.RegisteredChannels)
            {
                writer.WriteLine($"Channel {channel.ToString()} registered");
            }
            var services = RemotingConfiguration.GetRegisteredWellKnownServiceTypes();
            foreach(var service in services)
            {
                writer.WriteLine($"Registered {service.TypeName} as {service.Mode} on uri {service.ObjectUri}");
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using SuperSoftware.Shared;

namespace SuperSoftware.Client
{
    static class ClientProgram
    {
        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            configureRemoting();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm());
        }

        private static void configureRemoting()
        {
            var configFile = String.Format("{0}.config", AppDomain.CurrentDomain.FriendlyName);
            RemotingConfiguration.Configure(configFile, false);
        }

        public static IServer GetServerProxy()
        {
            WellKnownClientTypeEntry[] knownServers = RemotingConfiguration.GetRegisteredWellKnownClientTypes();
            return (IServer)Activator.GetObject(typeof(IServer), knownServers[0].ObjectUrl);
        }
        
    }
}

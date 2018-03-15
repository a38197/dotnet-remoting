using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperSoftware.Shared;

namespace SuperSoftware.Client
{
    public partial class ClientForm : Form
    {
        private readonly Client client = new Client(System.Configuration.ConfigurationManager.AppSettings["ClientName"]);
        private readonly IServer server = ClientProgram.GetServerProxy();
        private readonly Dictionary<string, CachedStockManager> remoteClients = new Dictionary<string, CachedStockManager>();

        public ClientForm()
        {
            InitializeData();   
            InitializeComponent();
            this.Text = System.Configuration.ConfigurationManager.AppSettings["ClientName"];
        }

        private const string PRODUCT_LIST = "ClientStock.xml";
        private void InitializeData()
        {
            string filePath = System.IO.Path.Combine(Environment.CurrentDirectory, PRODUCT_LIST);

            if (!System.IO.File.Exists(filePath))
                createBlankStockFile(filePath);

            loadStockFromFile(filePath);

        }

        private void createBlankStockFile(string filePath)
        {
            var stock = new Product[] { new Product("Prod1","Family1") };
            Shared.SerializationAdapter<Product[]> adapter = new Shared.SerializationAdapter<Product[]>();
            adapter.SerializeToFile(filePath, stock);
            MessageBox.Show("Não havia ficheiro de stock. Criado um dummy na diretoria " + filePath);
        }

        private void loadStockFromFile(string filePath)
        {
            Shared.SerializationAdapter<Product[]> adapter = new Shared.SerializationAdapter<Product[]>();
            var stock = adapter.DerializeFromFile(filePath);
            client.SetProducts(stock);
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            configureSize();
            configureListBox();
            configureEvents();
        }

        private void configureEvents()
        {
            client.RegisterRequestEvent(this.logRequest);
        }

        private void logRequest(StockManager.RequestType type)
        {
            Action action = () =>
            {
                lbRequests.Items.Add(sf("Received request for {0}", type.ToString()));
            };
            Invoke(action);
        }

        private void configureListBox()
        {
            lbFamilies.Items.Clear();
            lbFamilies.Items.AddRange(client.StockManager.GetProductFamilies());
        }

        private void configureSize()
        {
            MinimumSize = Size;
            MaximumSize = Size;
            MaximizeBox = false;
        }

        private void lbFamilies_SelectedIndexChanged(object sender, EventArgs e)
        {
            string family = (string)lbFamilies.SelectedItem;
            var prods = from product in client.StockManager.GetProducts(family)
                        select sf("[{0}], #{1}, ${2}",product.Name,product.Stock, product.Price);

            lbProducts.Items.Clear();
            lbProducts.Items.AddRange(prods.ToArray());
        }

        private void lbProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            string product = (string)lbProducts.SelectedItem;

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            executeAsync(() => {
                try
                {
                    server.Register(client.StockManager.GetProductFamilies(), client.StockManager);
                    MessageBox.Show("Ligado");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"Erro ao ligar");
                }
                
            }, null);
        }

        private IAsyncResult executeAsync(Action action, AsyncCallback cb)
        {
            Action async = () =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Erro ao executar a função assíncrona");
                }
            };
            return async.BeginInvoke(cb, null);
        }

        private AsyncCallback executeForeground(Action action)
        {
            return (result) =>
            {
                Invoke(action);
            };
        }

        private void btnSearchFamily_Click(object sender, EventArgs e)
        {
            List<IStockManager> toUnregister = new List<IStockManager>();
            executeAsync(() => {
                var sms = server.GetStockManagers(txtSearchFamily.Text, new UniqueObject(client.StockManager.GetUID())); //Poupa um roundtrip
                MessageBox.Show(sf("Servidor retornou {0} resultados", sms.Count()), "Obter Stock Managers");
                lock (remoteClients)
                {
                    remoteClients.Clear();
                    foreach (var sm in sms)
                    {
                        try
                        {
                            if (remoteClients.ContainsKey(sm.GetUID()))
                                continue;

                            CachedStockManager csm = new CachedStockManager(sm);
                            remoteClients.Add(csm.UID, csm);
                        }
                        catch (System.Net.Sockets.SocketException ex)
                        {
                            MessageBox.Show(ex.Message,"Erro ao ligar o stock manager");
                            toUnregister.Add(sm);
                        }
                    }
                }
                
            },
            (result)=> {
                unregisterStockManagerAsync(toUnregister);
                executeForeground(this.updateRemoteLbWithSearch)(null);
            });
        }

        private void updateRemoteLbWithSearch()
        {
            lbRemoteManagers.Items.Clear();
            string family = txtSearchFamily.Text.Trim();
            foreach(var remote in remoteClients)
            {
                if (remote.Value.Families.Contains(family))
                    lbRemoteManagers.Items.Add(sf("{0}, {1}, {2}",
                        remote.Value.Manager.StockManagerName,
                        txtSearchFamily.Text,
                        remote.Value.UID));
            }
        }

        private IAsyncResult unregisterStockManagerAsync(IEnumerable<IStockManager> managers)
        {
            Action action = () =>
            {
                foreach (var sm in managers)
                    server.Unregister(sm);
            };
            return action.BeginInvoke(null, null);
        }

        private void btnDisconect_Click(object sender, EventArgs e)
        {
            executeAsync(() => {
                try
                {
                    server.Unregister(client.StockManager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro ao desligar");
                }

            }, null);
        }

        private void btnRequestStock_Click(object sender, EventArgs e)
        {

        }

        private void lbRemoteManagers_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbRemoteProd.Items.Clear();
            var split = lbRemoteManagers.SelectedItem.ToString().Split(',');
            string manager = split[2].Trim();
            var family = split[1].Trim();

            foreach(var csm in remoteClients)
            {
                if(csm.Value.UID == manager)
                {
                    var products = csm.Value.Manager.GetProducts(family);
                    foreach(var p in products)
                    {
                        lbRemoteProd.Items.Add(sf("[{0}], #{1}, ${2}", p.Name, p.Stock, p.Price));
                    }
                }
            }
        }

        private void lbRemoteProd_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private string sf(string input, params object[] args)
        {
            return String.Format(input, args);
        }

        public void ClientForm_Closing(object sender, FormClosingEventArgs e)
        {
            server.Unregister(client.StockManager);
        }
    }
}

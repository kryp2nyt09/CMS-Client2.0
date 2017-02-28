using CMS2.BusinessLogic;
using CMS2.Client.SyncHelper;
using CMS2.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls;
using CMS2.Client.Properties;
using System.Configuration;
using CMS2.Client;
using CMS2.DataAccess;

namespace CMS2_Client
{
    public partial class Extract_Database : Telerik.WinControls.UI.RadForm
    {

        #region Properties
        private bool isSubServer { get; set; }
        private bool isLocalConnected { get; set; }
        private bool isMainConnected { get; set; }
        private string _localServer { get; set; }
        private string _localDbName { get; set; }
        private string _localUsername { get; set; }
        private string _localPassword { get; set; }

        private string _mainServer { get; set; }
        private string _mainDbName { get; set; }
        private string _mainUsername { get; set; }
        private string _mainPassword { get; set; }

        private string _localConnectionString { get; set; }
        private string _mainConnectionString { get; set; }

        private string _filter { get; set; }

        public string _branchCorpOfficeId { get; set; }

        private BranchCorpOfficeBL _bcoService;

        private Synchronization _synchronization;

        private List<BranchCorpOffice> _branchCorpOffices;
        #endregion

        #region Constructor
        public Extract_Database()
        {
            InitializeComponent();
            _bcoService = new BranchCorpOfficeBL();
        }
        #endregion

        #region Events
        private void Extract_Database_Load(object sender, EventArgs e)
        {
            this.isSubServer = true;
            this.isLocalConnected = false;
            this.isMainConnected = false;
            testMainConnection.Visible = false;
            testLocalConnection.Visible = false;
            dboBranchCoprOffice.Enabled = false;

            //_branchCorpOffices = _bcoService.FilterActive().OrderBy(x => x.BranchCorpOfficeName).ToList();

            //dboBranchCoprOffice.DataSource = _branchCorpOffices;
            //dboBranchCoprOffice.DisplayMember = "BranchCorpOfficeName";
            //dboBranchCoprOffice.ValueMember = "BranchCorpOfficeId";
            //dboBranchCoprOffice.SelectedIndex = -1;
        }

        private void LocalTestConnection_Click(object sender, EventArgs e)
        {
            if (IsDataValid_Local()) {
                GatherInputs();
                if (isSubServer)
                {
                    _localConnectionString = String.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=180;Connection Lifetime=0;Pooling=true;", _localServer, "master", _localUsername, _localPassword);
                }
                else
                {
                    _localConnectionString = String.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=180;Connection Lifetime=0;Pooling=true;", _localServer, _localDbName, _localUsername, _localPassword);

                }

                SqlConnection localConnection = new SqlConnection(_localConnectionString);
                try
                {
                    localConnection.Open();
                    isLocalConnected = true;
                    testLocalConnection.Text = "Success";
                    testLocalConnection.Visible = true;
                    testLocalConnection.ForeColor = Color.Green;
                    if (!isSubServer) {
                        loadbranchCorp(_localConnectionString);
                        dboBranchCoprOffice.Enabled = true;
                    }


                }
                catch (Exception)
                {
                    isLocalConnected = false;
                    testLocalConnection.Text = "Failed";
                    testLocalConnection.Visible = true;
                    testLocalConnection.ForeColor = Color.Red;
                }
                finally
                {
                    localConnection.Dispose();
                }
            }
        }

        private void MainTestConnection_Click(object sender, EventArgs e)
        {
            if (IsDataValid_Main()) {
                GatherInputs();
                _mainConnectionString = String.Format("Server={0};Database={1};User Id={2};Password={3};", _mainServer, _mainDbName, _mainUsername, _mainPassword);
                SqlConnection mainConnection = new SqlConnection(_mainConnectionString);
                try
                {
                    mainConnection.Open();
                    isMainConnected = true;
                    testMainConnection.Text = "Success";
                    testMainConnection.Visible = true;
                    testMainConnection.ForeColor = Color.Green;
                    loadbranchCorp(_mainConnectionString);
                    dboBranchCoprOffice.Enabled = true;
                }
                catch (Exception)
                {
                    isMainConnected = false;
                    testMainConnection.Text = "Failed";
                    testMainConnection.Visible = true;
                    testMainConnection.ForeColor = Color.Red;
                }
                finally
                {
                    mainConnection.Dispose();
                }
            }
        }

        
        private void SubServer_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            ResetAll();
            ToggleEnableDisableMainServer(true);
            isSubServer = true;
        }

        private void ClientApp_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            ResetAll();
            ToggleEnableDisableMainServer(false);
            isSubServer = false;
        }

        private void Extract_Click(object sender, EventArgs e)
        {            
            if (isSubServer && isLocalConnected && isMainConnected)
            {
                Application.DoEvents();
                ProgressLabel.Text = "Checking database if exist...";
                ProgressBar1.Value1 = ProgressBar1.Value1 + 20;
                Application.DoEvents();
                DropDatabaseIfExist();             

                ProgressLabel.Text = "Creating database...";
                ProgressBar1.Value1 = ProgressBar1.Value1 + 20;
                Application.DoEvents();
                CreateDatabase();

                ProgressLabel.Text = "Preparing database for synchronization...";
                int index = dboBranchCoprOffice.SelectedItem.ToString().IndexOf(" ");
                _filter = dboBranchCoprOffice.SelectedItem.ToString().Substring(0, index);
                _branchCorpOfficeId = dboBranchCoprOffice.SelectedValue.ToString();
                Application.DoEvents();
                GlobalVars.Sync = new Synchronization(new SqlConnection(_localConnectionString.Replace("master",_localDbName)), new SqlConnection(_mainConnectionString), dboBranchCoprOffice.SelectedValue.ToString(), _filter);
                Application.DoEvents();

                ProgressBar1.Value1 = ProgressBar1.Value1 + 30;
                Application.DoEvents();
                ProgressLabel.Text = "Writing settings to configuration settings...";
                Application.DoEvents();
                WriteToConfig(_localConnectionString.Replace("master", _localDbName));
                WriteToConfig(_mainConnectionString);
                ProgressBar1.Value1 = ProgressBar1.Value1 + 20;
                Application.DoEvents();
                System.Threading.Thread.Sleep(3000);
                this.DialogResult = DialogResult.OK;
                this.Close();
                
            }
            else if (!isSubServer && isLocalConnected)
            {
                int index = dboBranchCoprOffice.SelectedItem.ToString().IndexOf(" ");
                _filter = dboBranchCoprOffice.SelectedItem.ToString().Substring(0, index);
                _branchCorpOfficeId = dboBranchCoprOffice.SelectedValue.ToString();
                WriteToConfig(_localConnectionString.Replace("master", _localDbName));
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        #endregion

        #region Methods
        private void ToggleEnableDisableMainServer(bool Flag)
        {
            MainServer.Enabled = Flag;
            MainDbName.Enabled = Flag;
            MainUsername.Enabled = Flag;
            MainPassword.Enabled = Flag;
            MainTestConnection.Enabled = Flag;
        }

        private void WriteToConfig(string connString)
        {
            Application.DoEvents();
            var appConfigDoc = new XmlDocument();
            appConfigDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            foreach (XmlElement xElement in appConfigDoc.DocumentElement)
            {
                if (xElement.Name == "connectionStrings")
                {
                    var nodes = xElement.ChildNodes;
                    foreach (XmlElement item in nodes)
                    {
                        if (item.Attributes["name"].Value.Equals("Cms"))
                        {
                            item.Attributes["connectionString"].Value = connString;
                            break;
                        }
                    }
                    break;
                }
            }
            appConfigDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            Settings.Default.IsSynchronizationSetup = true;
            Settings.Default.LocalDbServer = _localServer;
            Settings.Default.LocalDbName = _localDbName;
            Settings.Default.LocalDbUsername = _localUsername;
            Settings.Default.LocalDbPassword = _localPassword;
            Settings.Default.CentralServerIp = _mainServer;
            Settings.Default.CentralDbName = _mainDbName;
            Settings.Default.CentralUsername = _mainUsername;
            Settings.Default.CentralPassword = _mainPassword;
            Settings.Default.Filter = _filter;
            Settings.Default.DeviceBcoId = _branchCorpOfficeId;
            ProgressLabel.Text = "Saving configuration settings...";
            Settings.Default.Save();

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["isSync"].Value = "true";
            config.AppSettings.Settings["Filter"].Value = _filter;
            config.AppSettings.Settings["BcoId"].Value = _branchCorpOfficeId;
            config.Save(ConfigurationSaveMode.Modified);

        }

        private void GatherInputs()
        {
            _localServer = LocalServer.Text;
            _localDbName = LocalDbName.Text;
            _localUsername = LocalUsername.Text;
            _localPassword = LocalPassword.Text;

            _mainServer = MainServer.Text;
            _mainDbName = MainDbName.Text;
            _mainUsername = MainUsername.Text;
            _mainPassword = MainPassword.Text;
        }

        private void ResetAll()
        {
            _localServer = "";
            _localDbName = "";
            _localUsername = "";
            _localPassword = "";

            _mainServer = "";
            _mainDbName = "";
            _mainUsername = "";
            _mainPassword = "";

            LocalServer.Text = _localServer;
            LocalDbName.Text = _localDbName;
            LocalUsername.Text = _localUsername;
            LocalPassword.Text = _localPassword;

            MainServer.Text = _mainServer;
            MainDbName.Text = _mainDbName;
            MainUsername.Text = _mainUsername;
            MainPassword.Text = _mainPassword;

            testLocalConnection.Visible = false;
            testMainConnection.Visible = false;

            dboBranchCoprOffice.Enabled = false;
        }

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Split by "GO" statements
            var statements = Regex.Split(
                    sqlScript,
                    @"^\s*GO\s*\d*\s*($|\-\-.*$)",
                    RegexOptions.Multiline |
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.IgnoreCase);

            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'));
        }

        private void DropDatabaseIfExist()
        {
            using (SqlConnection connection = new SqlConnection(_localConnectionString))
            {
                using (SqlCommand command = new SqlCommand("Select COUNT(*) from master.dbo.sysdatabases where name = '" + _localDbName + "'", connection))
                {
                    try
                    {
                        connection.Open();
                        int count = (Int32) command.ExecuteScalar();

                        if (count == 1)
                        {
                            command.CommandText = "Use master alter database[" +_localDbName +"] set single_user with rollback immediate; DROP DATABASE [" + _localDbName + "]";
                            command.ExecuteNonQuery();
                            ProgressLabel.Text = "Database was deleted.";
                        }                       
                    }
                    catch (Exception ex)
                    {
                        ProgressLabel.Text = "Unable to drop database.";
                    }
                }
            }

        }

        private void CreateDatabase()
        {
            using (SqlConnection connection = new SqlConnection(_localConnectionString))
            {
                using (SqlCommand command = new SqlCommand("Create Database " + _localDbName  , connection))
                {
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();                        
                        ProgressLabel.Text = "Database was created.";
                    }
                    catch (Exception ex)
                    {
                        ProgressLabel.Text = "Unable to create database.";
                    }
                }
            }

        }

        private void loadbranchCorp(string conString)
        {
            using (SqlConnection connectionString = new SqlConnection(conString))
            {
                try
                {
                    SqlCommand command = new SqlCommand("SELECT * FROM BranchCorpOffice ORDER BY BranchCorpOfficeName ASC", connectionString);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    dboBranchCoprOffice.ValueMember = "BranchCorpOfficeId";
                    dboBranchCoprOffice.DisplayMember = "BranchCorpOfficeName";
                    dboBranchCoprOffice.DataSource = dt;

                    connectionString.Close();
                }
                catch (Exception ex)
                {
                    // MessageBox.Show("Error occured!");
                }
            }
        }

        private bool IsDataValid_Local()
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(LocalServer.Text) || string.IsNullOrEmpty(LocalDbName.Text) || string.IsNullOrEmpty(LocalUsername.Text) || string.IsNullOrEmpty(LocalPassword.Text))
            {
                MessageBox.Show("Please fill out all fields.", "Data Error", MessageBoxButtons.OK);
                isValid = false;
            }

            return isValid;
        }

        private bool IsDataValid_Main()
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(MainServer.Text) || string.IsNullOrEmpty(MainDbName.Text) || string.IsNullOrEmpty(MainUsername.Text) || string.IsNullOrEmpty(MainPassword.Text))
            {
                MessageBox.Show("Please fill out all fields.", "Data Error", MessageBoxButtons.OK);
                isValid = false;
            }

            return isValid;
        }

        #endregion
    }
}

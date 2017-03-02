using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;
using CMS2.BusinessLogic;
using CMS2.Client.Properties;
using CMS2.Entities;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using CMS2.DataAccess;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using CMS2.Client.SyncHelper;
using System.Drawing;
using CMS2.Common;
using System.Threading;

namespace CMS2.Client
{
    public partial class CmsDbCon : Telerik.WinControls.UI.RadForm
    {

        #region Properties

        private XmlNodeList settings;

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
        private string _deviceCode { get; set; }
        public string _deviceRevenueUnitId { get; set; }

        public bool IsNeedDBSetup { get; set; }
        public bool IsFormClose { get; set; }

        private BranchCorpOfficeBL bcoService;
        private RevenueUnitTypeBL revenutUnitTypeService;
        private RevenueUnitBL revenueUnitService;

        private Synchronization _synchronization;

        private List<BranchCorpOffice> _branchCorpOffices;
        private List<RevenueUnitType> revenueUnitTypes;
        private List<RevenueUnit> revenueUnits;
        private List<SyncTables> _entities;

        private bool isProvision = true;
        private bool isDeprovisionClient = true;
        private bool IsDeprovisionServer = true;

        #endregion

        #region Constructors
        public CmsDbCon()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        private void CmsDbCon_Load(object sender, EventArgs e)
        {
            WaitingBar.StopWaiting();
            btnSaveSync.Enabled = false;

            this.isLocalConnected = false;
            this.isMainConnected = false;
            testMainConnection.Visible = false;
            testLocalConnection.Visible = false;

            bcoService = new BranchCorpOfficeBL();
            revenutUnitTypeService = new RevenueUnitTypeBL();
            revenueUnitService = new RevenueUnitBL();

            _branchCorpOffices = bcoService.FilterActive().OrderBy(x => x.BranchCorpOfficeName).ToList();
            revenueUnitTypes = revenutUnitTypeService.FilterActive().OrderBy(x => x.RevenueUnitTypeName).ToList();
            revenueUnits = revenueUnitService.FilterActive().OrderBy(x => x.RevenueUnitName).ToList();

            // Load BranchCorpOffices
            lstBco.DataSource = _branchCorpOffices;
            lstBco.DisplayMember = "BranchCorpOfficeName";
            lstBco.ValueMember = "BranchCorpOfficeId";

            // Load RevenueUnitTypes
            lstRevenueUnitType.DataSource = revenueUnitTypes;
            lstRevenueUnitType.DisplayMember = "RevenueUnitTypeName";
            lstRevenueUnitType.ValueMember = "RevenueUnitTypeId";

            // Load RevenueUnits
            lstRevenueUnit.DataSource = revenueUnits;
            lstRevenueUnit.DisplayMember = "RevenueUnitName";
            lstRevenueUnit.ValueMember = "RevenueUnitId";

            _branchCorpOfficeId = ConfigurationManager.AppSettings["BcoId"].ToString();
            _filter = ConfigurationManager.AppSettings["Filter"].ToString();
            _localConnectionString = ConfigurationManager.ConnectionStrings["Cms"].ConnectionString;
            _mainConnectionString = ConfigurationManager.ConnectionStrings["CmsCentral"].ConnectionString;

            SetChekcBoxes();
            SetEntities();
            CheckTableState();
            gridTables.DataSource = _entities;

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isLocalConnected && isMainConnected && GatherInputs())
            {
                // AppUSerSettings
                Settings.Default["LocalDbServer"] = _localServer;
                Settings.Default["LocalDbName"] = _localDbName;
                Settings.Default["LocalDbUsername"] = _localUsername;
                Settings.Default["LocalDbPassword"] = _localPassword;
                Settings.Default["CentralServerIp"] = _mainServer;
                Settings.Default["CentralDbName"] = _mainDbName;
                Settings.Default["CentralUsername"] = _mainUsername;
                Settings.Default["CentralPassword"] = _mainPassword;
                Settings.Default["DeviceCode"] = _deviceCode;
                Settings.Default["DeviceRevenueUnitId"] = _deviceRevenueUnitId;
                Settings.Default["DeviceBcoId"] = _branchCorpOfficeId;
                Settings.Default.Save();

                //app.config-appSettings
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["DeviceCode"].Value = txtDeviceCode.Text;
                config.AppSettings.Settings["RUId"].Value = _deviceRevenueUnitId;
                config.AppSettings.Settings["BcoId"].Value = _branchCorpOfficeId;
                config.Save(ConfigurationSaveMode.Modified);

                XmlDocument appConfigDoc = new XmlDocument();
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
                                item.Attributes["connectionString"].Value = _localConnectionString;
                            }
                            else if (item.Attributes["name"].Value.Equals("CmsCentral"))
                            {
                                item.Attributes["connectionString"].Value = _mainConnectionString;
                            }
                        }
                    }
                }
                appConfigDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

                Application.Restart();

            }
        }
        private void CmsDbCon_Shown(object sender, EventArgs e)
        {
            txtLocalIP.Text = Settings.Default.LocalDbServer;
            txtLocalDbName.Text = Settings.Default.LocalDbName;
            txtLocalDbUsername.Text = Settings.Default.LocalDbUsername;
            txtLocalDbPassword.Text = Settings.Default.LocalDbPassword;
            txtServerIP.Text = Settings.Default.CentralServerIp;
            txtServerDbName.Text = Settings.Default.CentralDbName;
            txtServerUsername.Text = Settings.Default.CentralUsername;
            txtServerPassword.Text = Settings.Default.CentralPassword;
            txtDeviceCode.Text = Settings.Default.DeviceCode;

            try
            {
                lstBco.SelectedValue = _branchCorpOffices.Find(x => x.BranchCorpOfficeId == Guid.Parse(Settings.Default.DeviceBcoId.ToString())).BranchCorpOfficeId;
                lstRevenueUnit.SelectedValue = revenueUnits.Find(x => x.RevenueUnitId == Guid.Parse(Settings.Default.DeviceRevenueUnitId.ToString())).RevenueUnitId;
            }
            catch (Exception ex)
            {
                Logs.ErrorLogs("CMS Settings", "CmsDBConShow", ex.Message);
            }

        }
        private void lstBco_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBco.SelectedIndex > -1 && lstRevenueUnitType.SelectedIndex > -1)
            {
                List<RevenueUnit> list = new List<RevenueUnit>();
                list = revenueUnits.Where(x => x.City.BranchCorpOffice.BranchCorpOfficeName == lstBco.SelectedItem.ToString() && x.RevenueUnitType.RevenueUnitTypeName == lstRevenueUnitType.SelectedItem.ToString()).ToList();
                PopulateRevenueUnit(list);
            }

        }
        private void PopulateRevenueUnit(List<RevenueUnit> list)
        {
            lstRevenueUnit.Items.Clear();
            lstRevenueUnit.DisplayMember = "RevenueUnitName";
            lstRevenueUnit.ValueMember = "ReveneuUnitId";
        }
        private void CmsDbCon_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.IsFormClose = true;
        }
        private void lstBco_Validated(object sender, EventArgs e)
        {
            if (lstBco.SelectedIndex > -1)
            {
                var temp = revenueUnits.Where(x => x.City.BranchCorpOffice.BranchCorpOfficeId == Guid.Parse(lstBco.SelectedValue.ToString()) && x.RevenueUnitType.RevenueUnitTypeId == Guid.Parse(lstRevenueUnitType.SelectedValue.ToString())).ToList();
                PopulateRevenueUnit(temp);
            }
        }
        private void lstRevenueUnitType_Validated(object sender, EventArgs e)
        {
            if (lstRevenueUnitType.SelectedIndex > -1)
            {
                var temp = revenueUnits.Where(x => x.City.BranchCorpOffice.BranchCorpOfficeId == Guid.Parse(lstBco.SelectedValue.ToString()) && x.RevenueUnitType.RevenueUnitTypeId == Guid.Parse(lstRevenueUnitType.SelectedValue.ToString())).ToList();
                PopulateRevenueUnit(temp);
            }
        }
        private void btnLocalTest_Click(object sender, EventArgs e)
        {
            if (IsDataValid_Local())
            {
                GatherInputs();
                _localConnectionString = String.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=180;Connection Lifetime=0;Pooling=true;", _localServer, "master", _localUsername, _localPassword);

                SqlConnection localConnection = new SqlConnection(_localConnectionString);
                try
                {
                    localConnection.Open();
                    isLocalConnected = true;
                    testLocalConnection.Text = "Success";
                    testLocalConnection.Visible = true;
                    testLocalConnection.ForeColor = Color.Green;

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
        private void btnServerTest_Click(object sender, EventArgs e)
        {
            if (IsDataValid_Main())
            {
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
        private void lstRevenueUnitType_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (lstRevenueUnitType.SelectedIndex > -1 && lstBco.SelectedIndex > -1)
            {
                List<RevenueUnit> list = new List<RevenueUnit>();
                list = revenueUnits.Where(x => x.City.BranchCorpOffice.BranchCorpOfficeId == Guid.Parse(lstBco.SelectedValue.ToString()) && x.RevenueUnitType.RevenueUnitTypeName == lstRevenueUnitType.SelectedItem.ToString()).ToList();
                PopulateRevenueUnit(list);
            }

        }
        private void LocalTestConnection_Click(object sender, EventArgs e)
        {
            if (IsDataValid_Local())
            {
                GatherInputs();

                _localConnectionString = String.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=180;Connection Lifetime=0;Pooling=true;", _localServer, _localDbName, _localUsername, _localPassword);


                SqlConnection localConnection = new SqlConnection(_localConnectionString);
                try
                {
                    localConnection.Open();
                    isLocalConnected = true;
                    testLocalConnection.Text = "Success";
                    testLocalConnection.Visible = true;
                    testLocalConnection.ForeColor = Color.Green;

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
            if (IsDataValid_Main())
            {
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
        private void chkDeprovisionClient_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (args.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
            {
                isDeprovisionClient = true;
            }
            else
            {
                isDeprovisionClient = false;
            }
        }
        private void chkDeprovisionServer_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (args.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
            {
                IsDeprovisionServer = true;
            }
            else
            {
                IsDeprovisionServer = false;
            }

        }
        private void chkProvision_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (args.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
            {
                isProvision = true;
            }
            else
            {
                isProvision = false;
            }
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isDeprovisionClient)
            {
                StartDeprovisionLocal();
            }
            if (IsDeprovisionServer)
            {
                StartDeprovisionServer();
            }
            if (isProvision)
            {
                StartProvision();
            }
            CheckTableState();
            gridTables.Refresh();
        }
        #endregion

        #region Methods
        private void SetEntities()
        {

            using (CmsContext context = new CmsContext())
            {
                ObjectContext objContext = ((IObjectContextAdapter)context).ObjectContext;
                MetadataWorkspace workspace = objContext.MetadataWorkspace;


                IEnumerable<EntityType> tables = workspace.GetItems<EntityType>(DataSpace.SSpace);

                _entities = new List<SyncTables>();

                foreach (var item in tables)
                {
                    SyncTables table = new SyncTables();
                    table.TableName = item.Name;
                    _entities.Add(table);
                }

            }
        }
        private bool IsDataValid_Local()
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(txtLocalIP.Text) || string.IsNullOrEmpty(txtLocalDbName.Text) || string.IsNullOrEmpty(txtLocalDbUsername.Text) || string.IsNullOrEmpty(txtLocalDbPassword.Text))
            {
                MessageBox.Show("Please fill out all fields.", "Data Error.", MessageBoxButtons.OK);
                isValid = false;
            }

            return isValid;
        }
        private bool IsDataValid_Main()
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(txtServerIP.Text) || string.IsNullOrEmpty(txtServerDbName.Text) || string.IsNullOrEmpty(txtServerUsername.Text) || string.IsNullOrEmpty(txtServerPassword.Text))
            {
                MessageBox.Show("Please fill out all fields.", "Data Error", MessageBoxButtons.OK);
                isValid = false;
            }
            return isValid;
        }
        private bool GatherInputs()
        {
            _localServer = txtLocalIP.Text;
            _localDbName = txtLocalDbName.Text;
            _localUsername = txtLocalDbUsername.Text;
            _localPassword = txtLocalDbPassword.Text;

            _mainServer = txtServerIP.Text;
            _mainDbName = txtServerDbName.Text;
            _mainUsername = txtServerUsername.Text;
            _mainPassword = txtServerPassword.Text;

            _deviceCode = txtDeviceCode.Text;

            try
            {
                if (lstBco.SelectedValue != null)
                {
                    _branchCorpOfficeId = lstBco.SelectedValue.ToString();
                }
                else
                {
                    lstBco.Focus();
                    return false;
                }

                if (lstRevenueUnit.SelectedIndex != -1)
                {
                    _deviceRevenueUnitId = lstRevenueUnit.SelectedValue.ToString();
                }
                else
                {
                    lstRevenueUnit.Focus();
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        private void CheckTableState()
        {
            List<SyncHelper.ThreadState> listOfThread = new List<SyncHelper.ThreadState>();
            List<ManualResetEvent> syncEvents = new List<ManualResetEvent>();
            List<ManualResetEvent> syncEvents1 = new List<ManualResetEvent>();

            for (int i = 0; i < _entities.Count - 1; i++)
            {

                SyncHelper.ThreadState _threadState = new SyncHelper.ThreadState();
                _threadState.table = _entities[i];
                listOfThread.Add(_threadState);

                if (i <= 50)
                {
                    syncEvents.Add(_threadState._event);
                }
                else
                {
                    syncEvents1.Add(_threadState._event);
                }

                try
                {
                    Synchronize sync = new Synchronize(_entities[i].TableName, _filter, _threadState._event, new SqlConnection(_localConnectionString), new SqlConnection(_mainConnectionString));
                    ThreadPool.QueueUserWorkItem(new WaitCallback(sync.PerformSync), _threadState);
                }
                catch (Exception ex)
                {

                }
            }

            WaitHandle.WaitAny(syncEvents.ToArray());
            WaitHandle.WaitAny(syncEvents1.ToArray());

        }
        private void StartProvision()
        {
            List<SyncHelper.ThreadState> listOfState = new List<SyncHelper.ThreadState>();
            List<ManualResetEvent> provisionEvents = new List<ManualResetEvent>();
            List<ManualResetEvent> provisionEvents1 = new List<ManualResetEvent>();
            SqlConnection localConnection = new SqlConnection(_localConnectionString);
            SqlConnection mainConnection = new SqlConnection(_mainConnectionString);

            for (int i = 0; i < _entities.Count - 1; i++)
            {
                SyncHelper.ThreadState state = new SyncHelper.ThreadState();
                state.table = _entities[i];
                listOfState.Add(state);
                if (i <= 60)
                {
                    provisionEvents.Add(state._event);
                }
                else
                {
                    provisionEvents1.Add(state._event);
                }

                try
                {
                    Provision provision = new Provision(_entities[i].TableName, localConnection, mainConnection, state._event, _filter, _branchCorpOfficeId);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(provision.Prepare_Database_For_Synchronization), state);

                }
                catch (Exception ex)
                {

                }
            }

            WaitHandle.WaitAny(provisionEvents.ToArray());
            WaitHandle.WaitAny(provisionEvents1.ToArray());
        }
        private void StartDeprovisionClient()
        {
            List<SyncHelper.ThreadState> listOfState = new List<SyncHelper.ThreadState>();
            List<ManualResetEvent> deprovisionEvents = new List<ManualResetEvent>();
            List<ManualResetEvent> deprovisionEvents1 = new List<ManualResetEvent>();
            SqlConnection localConnection = new SqlConnection(_localConnectionString);

            for (int i = 0; i < _entities.Count - 1; i++)
            {
                SyncHelper.ThreadState state = new SyncHelper.ThreadState();
                state.table = _entities[i];
                listOfState.Add(state);

                if (i <= 60)
                {
                    deprovisionEvents.Add(state._event);
                }
                else
                {
                    deprovisionEvents1.Add(state._event);
                }

                try
                {
                    Deprovision deprovision = new Deprovision(localConnection, state._event, _filter, _entities[i].TableName);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(deprovision.PerformDeprovisionTable), state);
                }
                catch (Exception ex)
                {

                }
            }

            WaitHandle.WaitAny(deprovisionEvents.ToArray());
            WaitHandle.WaitAny(deprovisionEvents1.ToArray());
        }
        private void StartDeprovisionLocal()
        {            
            SqlConnection localConnection = new SqlConnection(_localConnectionString);
            ManualResetEvent _event = new ManualResetEvent(false);
            Deprovision deprovision = new Deprovision(localConnection, _event, "", "");
            ThreadPool.QueueUserWorkItem(new WaitCallback( deprovision.PerformDeprovisionDatabase),_event);
            _event.WaitOne();
        }
        private void StartDeprovisionServer()
        {
            List<SyncHelper.ThreadState> listOfState = new List<SyncHelper.ThreadState>();
            List<ManualResetEvent> deprovisionEvents = new List<ManualResetEvent>();
            List<ManualResetEvent> deprovisionEvents1 = new List<ManualResetEvent>();
            SqlConnection localConnection = new SqlConnection(_localConnectionString);
            SqlConnection mainConnection = new SqlConnection(_mainConnectionString);

            for (int i = 0; i < _entities.Count - 1; i++)
            {
                SyncHelper.ThreadState state = new SyncHelper.ThreadState();
                state.table = _entities[i];
                listOfState.Add(state);

                if (i <= 60)
                {
                    deprovisionEvents.Add(state._event);
                }
                else
                {
                    deprovisionEvents1.Add(state._event);
                }

                try
                {
                    Deprovision deprovision = new Deprovision(mainConnection, state._event, _filter, _entities[i].TableName);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(deprovision.PerformDeprovisionTable), state);
                }
                catch (Exception ex)
                {

                }
            }

            WaitHandle.WaitAny(deprovisionEvents.ToArray());
            WaitHandle.WaitAny(deprovisionEvents1.ToArray());
        }
        private void SetChekcBoxes()
        {
            this.chkDeprovisionClient.Checked = isDeprovisionClient;
            this.chkDeprovisionServer.Checked = IsDeprovisionServer;
            this.chkProvision.Checked = isProvision;
        }
        #endregion


    }
}

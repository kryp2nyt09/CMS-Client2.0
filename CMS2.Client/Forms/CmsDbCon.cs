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

namespace CMS2.Client
{
    public partial class CmsDbCon : Telerik.WinControls.UI.RadForm
    {
        private XmlNodeList settings;

        private BranchCorpOfficeBL bcoService;
        private RevenueUnitTypeBL revenutUnitTypeService;
        private RevenueUnitBL revenueUnitService;

        List<BranchCorpOffice> branchCorpOffices;
        private List<RevenueUnitType> revenueUnitTypes;
        private List<RevenueUnit> revenueUnits;

        public CmsDbCon()
        {
            InitializeComponent();
        }
        public bool IsNeedDBSetup
        {
            get; set;
        }

        public bool IsFormClose
        {
            get; set;
        }

        private void CmsDbCon_Load(object sender, EventArgs e)
        {
            bcoService = new BranchCorpOfficeBL();
            revenutUnitTypeService = new RevenueUnitTypeBL();
            revenueUnitService = new RevenueUnitBL();

            branchCorpOffices = bcoService.FilterActive().OrderBy(x => x.BranchCorpOfficeName).ToList();
            revenueUnitTypes = revenutUnitTypeService.FilterActive().OrderBy(x => x.RevenueUnitTypeName).ToList();
            revenueUnits = revenueUnitService.FilterActive().OrderBy(x => x.RevenueUnitName).ToList();

            lstRevenueUnitType.DataSource = revenueUnitTypes;
            lstRevenueUnitType.DisplayMember = "RevenueUnitTypeName";
            lstRevenueUnitType.ValueMember = "RevenueUnitTypeId";

            lstBco.DataSource = branchCorpOffices;
            lstBco.DisplayMember = "BranchCorpOfficeName";
            lstBco.ValueMember = "BranchCorpOfficeId";

            lstBco.SelectedIndex = -1;

            //lstRevenueUnit.DataSource = revenueUnits;
            //lstRevenueUnit.DisplayMember = "RevenueUnitName";
            //lstRevenueUnit.ValueMember = "RevenueUnitId";
            PopulateRevenueUnit(revenueUnits);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //app.config-connectionStrings
            String connStringCms = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connect Timeout=180;", txtServerName.Text, txtDbName.Text, txtDbUsername.Text, txtDbPassword.Text);
          // string connectionStringCentral = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connect Timeout=180;", CentralServerIp.Text, CentralDbName.Text, CentralUserName.Text, CentralPassword.Text);
            //we need to try first if the connection is valid before saving it
            SqlConnection sqlConnection = new SqlConnection(connStringCms);
            //SqlConnection sqlConnection2 = new SqlConnection(connectionStringCentral);
            try
            {
                sqlConnection.Open();
            }
            catch
            {
                MessageBox.Show("Can't connect to specified local server. Make sure connection is correct");
                sqlConnection.Dispose();
                return;
            }

            //try
            //{
            //    sqlConnection2.Open();
            //}
            //catch
            //{
            //    MessageBox.Show("Can't connect to specified central server. Make sure connection is correct");
            //    sqlConnection2.Dispose();
            //    return;
            //}


            Guid bcoId = new Guid();
            if (lstBco.SelectedValue != null)
            {
                bcoId = Guid.Parse(lstBco.SelectedValue.ToString());
            }

            Guid revenueUnitId = new Guid();
            if (lstRevenueUnit.SelectedIndex > 0)
            {
                var runit = revenueUnits.FirstOrDefault(x => x.RevenueUnitName.Equals(lstRevenueUnit.SelectedItem.ToString()));
                if (runit != null)
                    revenueUnitId = runit.RevenueUnitId;
            }

            // AppUSerSettings
            Settings.Default["LocalDbServer"] = txtServerName.Text;
            Settings.Default["LocalDbName"] = txtDbName.Text;
            Settings.Default["LocalDbUsername"] = txtDbUsername.Text;
            Settings.Default["LocalDbPassword"] = txtDbPassword.Text;
            //Settings.Default["CentralServerIp"] = CentralServerIp.Text;
            //Settings.Default["CentralDbName"] = CentralDbName.Text;
            //Settings.Default["CentralUsername"] = CentralUserName.Text;
            //Settings.Default["CentralPassword"] = CentralPassword.Text;
            Settings.Default["DeviceCode"] = txtDeviceCode.Text;
            Settings.Default["DeviceRevenueUnitId"] = revenueUnitId.ToString();
            Settings.Default["DeviceBcoId"] = bcoId.ToString();
            Settings.Default.Save();

            //app.config-appSettings
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["DeviceCode"].Value = txtDeviceCode.Text;
            config.AppSettings.Settings["RUId"].Value = revenueUnitId.ToString();
            config.AppSettings.Settings["BcoId"].Value = bcoId.ToString();
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
                            item.Attributes["connectionString"].Value = connStringCms;
                        }
                        else if (item.Attributes["name"].Value.Equals("CmsCentral"))
                        {
                            //item.Attributes["connectionString"].Value = connectionStringCentral;
                        }
                    }
                }
            }
            appConfigDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            
            Application.Restart();
        }

        private void CmsDbCon_Shown(object sender, EventArgs e)
        {
            txtServerName.Text = Settings.Default.LocalDbServer;
            txtDbName.Text = Settings.Default.LocalDbName;
            txtDbUsername.Text = Settings.Default.LocalDbUsername;
            txtDbPassword.Text = Settings.Default.LocalDbPassword;
            //CentralServerIp.Text = Settings.Default.CentralServerIp;
            //CentralDbName.Text = Settings.Default.CentralDbName;
            //CentralUserName.Text = Settings.Default.CentralUsername;
            //CentralPassword.Text = Settings.Default.CentralPassword;
            txtDeviceCode.Text = Settings.Default.DeviceCode;

            if (!string.IsNullOrEmpty(Settings.Default.DeviceRevenueUnitId))
            {
                string idString = Settings.Default.DeviceRevenueUnitId;
                if (string.IsNullOrEmpty(idString))
                {
                }
                else
                {
                    Guid id;
                    Guid.TryParse(idString, out id);
                    if (id != Guid.Empty)
                    {
                        lstBco.SelectedValue =
                            revenueUnits.Where(x => x.RevenueUnitId == id)
                                .Select(x => x.City.BranchCorpOffice.BranchCorpOfficeId).First();
                        lstRevenueUnitType.SelectedValue =
                            revenueUnits.Where(x => x.RevenueUnitId == id)
                                .Select(x => x.RevenueUnitType.RevenueUnitTypeId).First();
                        var runit = revenueUnits.FirstOrDefault(x => x.RevenueUnitId == id);
                        if (runit != null)
                            lstRevenueUnit.Text = runit.RevenueUnitName;
                    }
                    else
                    {
                        var bcoId = Guid.Parse(Settings.Default.DeviceBcoId);

                        var office = revenueUnits.FirstOrDefault(x => x.City.BranchCorpOffice.BranchCorpOfficeId == bcoId);
                        if (office != null)
                        {
                            lstBco.SelectedValue = office.City.BranchCorpOffice.BranchCorpOfficeId;
                        }
                    }
                }
            }
        }

        private void lstBco_SelectedIndexChanged(object sender, EventArgs e)
        {
            //    if (lstBco.SelectedValue != null)
            //    {
            //        var temp =
            //            revenueUnits.Where(x => x.City.BranchCorpOffice.BranchCorpOfficeId == Guid.Parse(lstBco.SelectedValue.ToString()) && x.RevenueUnitType.RevenueUnitTypeId == Guid.Parse(lstRevenueUnitType.SelectedValue.ToString())).ToList();
            //        lstRevenueUnit.DataSource = temp;
            //        lstRevenueUnit.DisplayMember = "RevenueUnitName";
            //        lstRevenueUnit.ValueMember = "RevenueUnitId";
            //        lstRevenueUnit.Items.Insert(0, new ListItem(String.Empty, new Guid().ToString()));
            //    }
        }

        private void lstRevenueUnitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (lstRevenueUnitType.SelectedValue!=null)
            //{
            //    var temp =
            //             revenueUnits.Where(x => x.Cluster.BranchCorpOffice.BranchCorpOfficeId == Guid.Parse(lstBco.SelectedValue.ToString()) && x.RevenueUnitType.RevenueUnitTypeId == Guid.Parse(lstRevenueUnitType.SelectedValue.ToString())).ToList();

            //    lstRevenueUnit.DataSource = temp;
            //    lstRevenueUnit.DisplayMember = "RevenueUnitName";
            //    lstRevenueUnit.ValueMember = "RevenueUnitId";
            //    lstRevenueUnit.Items.Insert(0, new ListItem(String.Empty, new Guid().ToString()));
            //}
        }

        private void lstRevenueUnit_Enter(object sender, EventArgs e)
        {
            var temp = revenueUnits.Where(x => x.City.BranchCorpOffice.BranchCorpOfficeId == Guid.Parse(lstBco.SelectedValue.ToString()) && x.RevenueUnitType.RevenueUnitTypeId == Guid.Parse(lstRevenueUnitType.SelectedValue.ToString())).ToList();
            PopulateRevenueUnit(temp);
        }

        private void PopulateRevenueUnit(List<RevenueUnit> list)
        {
            lstRevenueUnit.Items.Clear();
            lstRevenueUnit.Items.Insert(0, new Telerik.WinControls.UI.RadListDataItem(String.Empty, new Guid().ToString()));
            int index = 1;
            foreach (var item in list)
            {
                lstRevenueUnit.Items.Insert(index, new Telerik.WinControls.UI.RadListDataItem(item.RevenueUnitName, item.RevenueUnitId.ToString()));
                index++;
            }
        }

        private void CmsDbCon_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.IsFormClose = true;
        }

        private void lstBco_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
            
        {
            
            
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
    }
}

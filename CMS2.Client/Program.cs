using System;
using System.Configuration;
using System.Windows.Forms;
using System.Xml;
using CMS2.Client.Properties;
using Microsoft.Practices.Unity;
using CMS2_Client;

namespace CMS2.Client
{
    internal static class Program
    {
        private static bool RestartApp = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {

            //bool xBool = Convert.ToBoolean(ConfigurationManager.AppSettings["isSync"]);
            //if (!xBool)
            //{
            //    Extract_Database extract = new Extract_Database();
            //    Application.Run(extract);
            //    Application.Exit();
            //}

            //var container = BuildUnityContainer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var cmsMainWindow = new Main();

            GlobalVars.IsSubserver = false;
            SetAppSetting();
            bool isNeedDBSetup = SetLocalDbConnection();
            bool isAborted = false;

            while (isNeedDBSetup && !isAborted)
            {
                if (isNeedDBSetup)
                {
                    var frmDBCon = new CmsDbCon();
                    frmDBCon.IsNeedDBSetup = isNeedDBSetup;
                    frmDBCon.ShowDialog();
                    isNeedDBSetup = frmDBCon.IsNeedDBSetup;
                    isAborted = frmDBCon.IsFormClose;
                }
            }

            if (isAborted)
            {
                MessageBox.Show("Exit.");
                Application.Exit();
            }
            else
            {
                SetCentralDbConnection();
                if (RestartApp)
                    Application.Restart();

                //Application.Run(container.Resolve</*CMSMain*/>());
                Application.Run(cmsMainWindow);
            }

        }

        private static void SetAppSetting()
        {
            string deviceCode = ConfigurationSettings.AppSettings["DeviceCode"];
            Guid revenueUnitId = Guid.Parse(ConfigurationSettings.AppSettings["RUId"]);
            Guid bcoId = Guid.Parse(ConfigurationSettings.AppSettings["BcoId"]);

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (string.IsNullOrEmpty(deviceCode))
            {
                string _deviceCode = Settings.Default.DeviceCode;
                config.AppSettings.Settings["DeviceCode"].Value = _deviceCode;
            }
            else
            {
                if (deviceCode.Substring(0, 1).Equals("S"))
                    GlobalVars.IsSubserver = true;
            }

            if (revenueUnitId == Guid.Empty)
            {
                Guid rid = Guid.Parse(Settings.Default.DeviceRevenueUnitId);
                if (rid != Guid.Empty)
                    config.AppSettings.Settings["RUId"].Value = rid.ToString();
            }

            if (bcoId == Guid.Empty)
            {
                Guid bid = Guid.Parse(Settings.Default.DeviceBcoId);
                if (bid != Guid.Empty)
                    config.AppSettings.Settings["BcoId"].Value = bid.ToString();
            }

            config.Save(ConfigurationSaveMode.Modified);

        }

        private static bool SetLocalDbConnection()
        {
            XmlDocument appConfigDoc = new XmlDocument();
            appConfigDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            String connStringCms = "";
            String connStringTracking = "";
            var isNeedDBSetup = false;

            // verify get connectionstrings
            foreach (XmlElement xElement in appConfigDoc.DocumentElement)
            {
                if (xElement.Name == "connectionStrings")
                {
                    var nodes = xElement.ChildNodes;
                    foreach (XmlElement item in nodes)
                    {
                        if (item.Attributes["name"].Value.Equals("Cms"))
                        {
                            connStringCms = item.Attributes["connectionString"].Value;
                            isNeedDBSetup = string.IsNullOrWhiteSpace(connStringCms) ? true : false;
                        }                        
                    }
                }
            }

            if (string.IsNullOrEmpty(connStringCms) && string.IsNullOrEmpty(connStringTracking))
            {
                string serverName = "";
                string dbName = "";
                string dbUsername = "";
                string dbPassword = "";

                serverName = Settings.Default.LocalDbServer;
                dbName = Settings.Default.LocalDbName;
                dbUsername = Settings.Default.LocalDbUsername;
                dbPassword = Settings.Default.LocalDbPassword;

                connStringCms =
                    string.Format(
                        "Data Source={0};Initial Catalog={1};User ID ={2};Password={3};Connect Timeout=180;",
                        serverName, dbName, dbUsername, dbPassword);
                connStringTracking =
                    string.Format(
                        "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connect Timeout=180;",
                        serverName, "tracking2", dbUsername, dbPassword);

                // configure local connection strings
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
                            else if (item.Attributes["name"].Value.Equals("Tracking"))
                            {
                                item.Attributes["connectionString"].Value = connStringTracking;
                            }
                        }
                    }
                }
                appConfigDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                //Application.Restart();
                RestartApp = true;
            }

            return isNeedDBSetup;
        }

        private static void SetCentralDbConnection()
        {
            if (GlobalVars.IsSubserver)
            {
                XmlDocument appConfigDoc = new XmlDocument();
                appConfigDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                String connStringCmsCentral = "";
                String connStringTrackingCentral = "";

                // verify get connectionstrings
                foreach (XmlElement xElement in appConfigDoc.DocumentElement)
                {
                    if (xElement.Name == "connectionStrings")
                    {
                        var nodes = xElement.ChildNodes;
                        foreach (XmlElement item in nodes)
                        {
                            if (item.Attributes["name"].Value.Equals("CmsCentral"))
                            {
                                connStringCmsCentral = item.Attributes["connectionString"].Value;
                            }                           
                        }
                    }
                }

                if (string.IsNullOrEmpty(connStringCmsCentral) && string.IsNullOrEmpty(connStringTrackingCentral))
                {
                    // configure central(if subserver) connection strings
                    foreach (XmlElement xElement in appConfigDoc.DocumentElement)
                    {
                        if (xElement.Name == "connectionStrings")
                        {
                            var nodes = xElement.ChildNodes;
                            foreach (XmlElement item in nodes)
                            {
                                if (item.Attributes["name"].Value.Equals("CmsCentral") &&
                                    string.IsNullOrEmpty(connStringCmsCentral))
                                {
                                    item.Attributes["connectionString"].Value =
                                        "Data Source = 192.168.0.27; Initial Catalog = cms2; User ID = cmsuser; Password = P9ssW0rd; Connect Timeout = 180; Connection Lifetime = 0; Pooling = true;";
                                }
                            }
                        }
                        appConfigDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                        //Application.Restart();
                        RestartApp = true;
                    }
                }
            }
        }
    }
}

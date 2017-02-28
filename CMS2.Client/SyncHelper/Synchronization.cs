using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Synchronization;
using System.Data.SqlClient;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using System.Configuration;
using System.Data;
using System.Windows.Forms;

namespace CMS2.Client.SyncHelper
{
    public class Synchronization
    {

        private SqlConnection ServerConn;
        private SqlConnection ClientConn;
        private string DeviceRevenueUnitID;
        private string DeviceBranchCorpOfficeID;
        private string DeviceRevenueUnitName;
        private List<string> Entities = new List<string>();

        private DbSyncScopeDescription scopeDesc;
        private DbSyncTableDescription tableDescription;
        private SqlSyncScopeProvisioning serverTemplate;


        public Synchronization()
        {
            // create a connection to the cms2_Beta1 client database
            //ClientConn = new SqlConnection("Data Source = 192.168.5.1; Initial Catalog = cms2_Beta1; User ID = sa; Password = d0me$tic$QL; Connect Timeout = 180; Connection Lifetime = 0; Pooling = true; ");
            ClientConn = new SqlConnection(ConfigurationManager.ConnectionStrings["Cms"].ConnectionString);

            // create a connection to the cms2_Beta1 server database
            ServerConn = new SqlConnection(ConfigurationManager.ConnectionStrings["CmsCentral"].ConnectionString);
            //ServerConn = new SqlConnection("Data Source=192.168.0.58;Initial Catalog=SyncDB ;User ID=sa;Password=z@nixtri2016;Connect Timeout=180;Connection Lifetime=0;Pooling=true;");

            DeviceRevenueUnitID = ConfigurationManager.AppSettings["RUId"];
            DeviceBranchCorpOfficeID = ConfigurationManager.AppSettings["BcoId"];
            DeviceRevenueUnitName = ConfigurationManager.AppSettings["DeviceCode"];
            SetEntitiesToSync();

            DeProvision_Server();
            DeProvision_Client();
            Prepare_Database_For_Synchronization();

        }

        public Synchronization(string BcoID, string RevenueUnitID, string Filter)
        {
            // create a connection to the cms2_Beta1 client database
            //ClientConn = new SqlConnection("Data Source = 192.168.5.1; Initial Catalog = cms2_Beta1; User ID = sa; Password = d0me$tic$QL; Connect Timeout = 180; Connection Lifetime = 0; Pooling = true; ");
            ClientConn = new SqlConnection(ConfigurationManager.ConnectionStrings["Cms"].ConnectionString);

            // create a connection to the cms2_Beta1 server database
            ServerConn = new SqlConnection(ConfigurationManager.ConnectionStrings["CmsCentral"].ConnectionString);
            //ServerConn = new SqlConnection("Data Source=192.168.0.58;Initial Catalog=SyncDB ;User ID=sa;Password=z@nixtri2016;Connect Timeout=180;Connection Lifetime=0;Pooling=true;");

            DeviceRevenueUnitID = RevenueUnitID;
            DeviceBranchCorpOfficeID = BcoID;
            DeviceRevenueUnitName = Filter;
            SetEntitiesToSync();

            //DeProvision_Server();
            //DeProvision_Client();
            Prepare_Database_For_Synchronization();
            //Synchronize();
        }

        public Synchronization(SqlConnection Local, SqlConnection Server, string BcoID, string Filter)
        {
            // create a connection to local database
            ClientConn = Local;

            // create a connection to  server database
            ServerConn = Server;

            Application.DoEvents();
            DeviceBranchCorpOfficeID = BcoID;
            DeviceRevenueUnitName = Filter;
            SetEntitiesToSync();

            DeProvision_Client();
            //DeProvision_Server();            

            Prepare_Database_For_Synchronization();
            Synchronize();
        }

        public void Prepare_Database_For_Synchronization()
        {
            SqlParameter param;
            try
            {
                foreach (string tableName in Entities)
                {
                    Application.DoEvents();
                    string filterColumn = "";
                    string filterClause = "";
                    switch (tableName)
                    {
                        case "Booking":

                            filterColumn = "AssignedToAreaId";
                            filterClause = "[side].[AssignedToAreaId] IN (SELECT book.AssignedToAreaId  FROM Booking as book " +
                                            "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                            "left join City as city on city.CityId = ru.CityId " +
                                            "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;
                        case "Shipment":

                            filterColumn = "ShipmentId";
                            filterClause = "[side].[ShipmentId] In (SELECT ship.ShipmentId FROM Shipment as ship " +
                                            "left join Booking as book on book.BookingId = ship.BookingId " +
                                            "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                            "left join City as city on city.CityId = ru.CityId " +
                                            "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;

                        case "PackageNumber":

                            filterColumn = "ShipmentId";
                            filterClause = "[side].[ShipmentId] In (SELECT pack.ShipmentId FROM PackageNumber as pack " +
                                            "left join Shipment as ship on ship.ShipmentId = pack.ShipmentId " +
                                            "left join Booking as book on book.BookingId = ship.BookingId " +
                                            "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                            "left join City as city on city.CityId = ru.CityId " +
                                            "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;

                        case "PackageDimension":

                            filterColumn = "ShipmentId";
                            filterClause = "[side].[ShipmentId] In (SELECT pack.ShipmentId FROM PackageDimension as pack " +
                                            "left join   Shipment as ship on ship.ShipmentId = pack.ShipmentId " +
                                            "left join Booking as book on book.BookingId = ship.BookingId " +
                                            "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                            "left join City as city on city.CityId = ru.CityId " +
                                            "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;

                        case "StateOfAccountPayment":

                            filterColumn = "StatementOfAccountId";
                            filterClause = "[side].[StatementOfAccountId] In (Select soaPayment.StatementOfAccountId from StatementOfAccountPayment as soaPayment " +
                                            "left join StatementOfAccount as soa on soa.StatementOfAccountId = soaPayment.StatementOfAccountId " +
                                            "left join Company as company on company.CompanyId = soa.CompanyId " +
                                            "left join City as city on city.CityId = company.CityId " +
                                            "left join BranchCorpOffice as bco on bco.BranchCorpOfficeId = city.BranchCorpOfficeId " +
                                            "where bco.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;
                        case "Payment":

                            filterColumn = "ShipmentId";
                            filterClause = "[side].[ShipmentId] In (Select payment.ShipmentId from Payment as payment " +
                                            "left join Shipment as shipment on shipment.ShipmentId = payment.ShipmentId " +
                                            "left join Booking as book on book.BookingId = shipment.BookingId " +
                                            "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                            "left join City as city on city.CityId = ru.CityId " +
                                            "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;
                        case "PaymenTurnOver":

                            filterColumn = "CollectedById";
                            filterClause = "[side].[CollectedById] In (Select turnOver.CollectedById From PaymentTurnOver as turnOver " +
                                            "left join Employee as emp on emp.EmployeeId = turnOver.CollectedById " +
                                            "left join RevenueUnit as ru on ru.RevenueUnitId = emp.AssignedToAreaId " +
                                            "left join City as city on city.CityId = ru.CityId " +
                                            "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;
                        case "ShipmentAdjustment":

                            filterColumn = "ShipmentId";
                            filterClause = "[side].[ShipmentId] In (Select adjustment.ShipmentId from ShipmentAdjustment as adjustment " +
                                            "left join Shipment as shipment on shipment.ShipmentId = adjustment.ShipmentId " +
                                            "left join Booking as book on book.BookingId = shipment.BookingId " +
                                            "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                            "left join City as city on city.CityId = ru.CityId " +
                                            "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);
                            break;
                        case "Delivery":

                            filterColumn = "ShipmentId";
                            filterClause = "[side].[ShipmentId] In (Select delivery.ShipmentId from Delivery as delivery " +
                                                "left join Shipment as shipment on shipment.ShipmentId = delivery.ShipmentId " +
                                                "left join Booking as book on book.BookingId = shipment.BookingId " +
                                                "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                                "left join City as city on city.CityId = ru.CityId " +
                                                "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;
                        case "DeliveryPackage":

                            filterColumn = "DeliveryId";
                            filterClause = "[side].[DeliveryId] In (Select package.DeliveryId from DeliveredPackage as package  " +
                                            "left join Delivery as delivery on delivery.DeliveryId = package.DeliveryId " +
                                            "left join Shipment as shipment on shipment.ShipmentId = delivery.ShipmentId " +
                                            "left join Booking as book on book.BookingId = shipment.BookingId " +
                                            "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                            "left join City as city on city.CityId = ru.CityId " +
                                            "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;
                        case "DeliveryReceipt":

                            filterColumn = "DeliveryId";
                            filterClause = "[side].[DeliveryId] In (Select receipt.DeliveryId from DeliveryReceipt as receipt " +
                                            "left join Delivery as delivery on delivery.DeliveryId = receipt.DeliveryId " +
                                            "left join Shipment as shipment on shipment.ShipmentId = delivery.ShipmentId " +
                                            "left join Booking as book on book.BookingId = shipment.BookingId " +
                                            "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                            "left join City as city on city.CityId = ru.CityId " +
                                            "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                            param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                            Create_Template(tableName, filterColumn, filterClause, param);

                            Provision_Server(tableName, param, DeviceBranchCorpOfficeID);

                            Provision_Client(tableName);

                            break;
                        default:
                            Provision_Server(tableName);
                            Provision_Client(tableName);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void Create_Template(string TableName, string filterColumn, string filterClause, SqlParameter param)
        {
            // Create a template named tableName + _Filter_Template
            scopeDesc = new DbSyncScopeDescription(TableName + "_Filter_Template");

            // Set a friendly description of the template.
            scopeDesc.UserComment = "Filter template for " + TableName + ".";

            // Definition for tables.
            tableDescription = SqlSyncDescriptionBuilder.GetDescriptionForTable(TableName, ServerConn);
            scopeDesc.Tables.Add(tableDescription);

            // Create a provisioning object for "tableName_Filter_template" that can be used to create a template
            // from which filtered synchronization scopes can be created.
            serverTemplate = new SqlSyncScopeProvisioning(ServerConn, scopeDesc, SqlSyncScopeProvisioningType.Template);

            Add_Filter(TableName, filterColumn, filterClause, param);

            // create a new select changes stored proc for this scope
            serverTemplate.SetCreateProceduresForAdditionalScopeDefault(DbSyncCreationOption.Create);

            // Create the tableName_Filter_template" template in the database.
            if (!serverTemplate.TemplateExists(TableName + "_Filter_Template"))
            {
                try
                {
                    serverTemplate.Apply();
                    Console.WriteLine(TableName + " filter template was created.");
                }
                catch (Exception ex)
                {

                }

            }
            else
            {
                Console.WriteLine(TableName + " filter template was already exist.");
            }
        }

        /// <summary>
        /// Add Filter to use for filtering data,
        /// and the filtering clause to use against the tracking table.
        /// "[side]" is an alias for the tracking table.
        /// An actual filter will be specified when the synchronization scope is created.
        /// </summary>
        /// <param name="TableName"> Name of table subjected to synch.</param>
        /// <param name="FilterColumn"> Table Column included in synch.</param>
        /// <param name="FilterClause"> Filte clause to filter data.</param>
        /// <param name="param">Parameter included in filter clause.</param>
        private void Add_Filter(string TableName, string FilterColumn, string FilterClause, SqlParameter param)
        {
            serverTemplate.Tables[TableName].AddFilterColumn(FilterColumn);
            serverTemplate.Tables[TableName].FilterClause = FilterClause;
            serverTemplate.Tables[TableName].FilterParameters.Add(param);
        }

        /// <summary>
        /// Provision the server without filter.
        /// </summary>
        /// <param name="TableName"> Name of table to subjected to synch.</param>
        private void Provision_Server(string TableName)
        {
            // define a new scope named tableNameScope
            DbSyncScopeDescription scopeDesc = new DbSyncScopeDescription(TableName + DeviceRevenueUnitName);
            // get the description of the tableName
            DbSyncTableDescription tableDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable(TableName, ServerConn);

            // add the table description to the sync scope definition
            scopeDesc.Tables.Add(tableDesc);

            // create a server scope provisioning object based on the tableNameScope
            SqlSyncScopeProvisioning serverProvision = new SqlSyncScopeProvisioning(ServerConn, scopeDesc);

            // start the provisioning process
            if (!serverProvision.ScopeExists(scopeDesc.ScopeName))
            {
                try
                {
                    serverProvision.Apply();
                    Console.WriteLine("Server " + TableName + " was provisioned.");
                }
                catch (Exception)
                {

                }

            }
            else
            {
                Console.WriteLine("Server " + TableName + " was already provisioned.");
            }

        }

        /// <summary>
        /// Provision the server with filter.
        /// </summary>
        /// <param name="TableName">Name of table subjected to synch.</param>
        /// <param name="Parameter">Parameter included to synch.</param>
        /// <param name="ParamValue">Value to bind in parameter.</param>
        private void Provision_Server(string TableName, SqlParameter Parameter, string ParamValue)
        {
            try
            {
                // Create a synchronization scope for OriginState=WA.
                SqlSyncScopeProvisioning serverProvision = new SqlSyncScopeProvisioning(ServerConn);

                // populate the scope description using the template
                serverProvision.PopulateFromTemplate(TableName + DeviceRevenueUnitName, TableName + "_Filter_Template");

                // specify the value we want to pass in the filter parameter, in this case we want only orders from WA
                serverProvision.Tables[TableName].FilterParameters[Parameter.ParameterName].Value = Guid.Parse(ParamValue);

                // Set a friendly description of the template.
                serverProvision.UserComment = TableName + " data includes only " + ParamValue;

                // Create the new filtered scope in the database.
                if (!serverProvision.ScopeExists(serverProvision.ScopeName))
                {
                    try
                    {
                        serverProvision.Apply();
                        Console.WriteLine("Server " + TableName + " was provisioned.");
                    }
                    catch (Exception)
                    {

                    }

                }
                else
                {
                    Console.WriteLine("Server " + TableName + " was already provisioned.");
                }
            }
            catch (Exception)
            {

            }

        }

        private void Provision_Client(string TableName)
        {

            DbSyncScopeDescription scopeDescription = SqlSyncDescriptionBuilder.GetDescriptionForScope(TableName + DeviceRevenueUnitName, ServerConn);

            // create a provisioning object 
            SqlSyncScopeProvisioning clientProvision = new SqlSyncScopeProvisioning(ClientConn, scopeDescription);

            if (!clientProvision.ScopeExists(scopeDescription.ScopeName))
            {
                try
                {
                    clientProvision.Apply();
                    Console.WriteLine("Client " + TableName + " was provisioned.");
                }
                catch (Exception)
                {

                }

            }
            else
            {
                Console.WriteLine("Client " + TableName + " was already provisioned.");
            }

        }

        public void Synchronize()
        {
            foreach (string tableName in Entities)
            {                
                // create a sync orchestration object
                SyncOrchestrator syncOrchestrator = new SyncOrchestrator();
                // set the local provider of sync orchestrator to a sync provider that is
                // associated with the OrdersScope-NC scope in the SyncExpressDB database
                syncOrchestrator.LocalProvider = new SqlSyncProvider(tableName + DeviceRevenueUnitName, ClientConn);
                // set the remote provider of sync orchestrator to a server sync provider that is
                // associated with the OrdersScope-NC scope in the SyncDB database
                syncOrchestrator.RemoteProvider = new SqlSyncProvider(tableName + DeviceRevenueUnitName, ServerConn);
                //// set the direction to Upload and Download
                //syncOrchestrator.Direction = SyncDirectionOrder.UploadAndDownload;

                SyncOperationStatistics syncStats;

                switch (tableName)
                {

                    case "Shipment":
                    case "PackageNumber":
                    case "PackageDimension":
                    case "Payment":
                    case "StatementOfAccountPayment":
                    case "PaymentTurnOver":
                    case "ShipmentAdjustment":
                    case "Delivery":
                    case "DeliveryPackage":
                    case "DeliveryReceipt":
                    case "Booking":
                    case "Client":

                        // set the direction to Upload and Download
                        syncOrchestrator.Direction = SyncDirectionOrder.UploadAndDownload;
                        // subscribe for errors that occur when applying changes to the client
                        //((SqlSyncProvider) syncOrchestrator.LocalProvider).ApplyChangeFailed += new EventHandler<DbApplyChangeFailedEventArgs>(Program_ApplyChangeFailed);


                        try
                        {
                            // starts the synchornization session

                            syncStats = syncOrchestrator.Synchronize();
                            // prints statistics from the sync session
                            //Console.WriteLine("Start Time: " + syncStats.SyncStartTime);
                            //Console.Write(tableName + " Total Changes Uploaded: " + syncStats.UploadChangesTotal + " ");
                            //Console.WriteLine(tableName + " Total Changes Downloaded: " + syncStats.DownloadChangesTotal);
                            //Console.WriteLine("Complete Time: " + syncStats.SyncEndTime);
                        }
                        catch (Exception ex)
                        {

                        }

                        break;
                    default:
                        // set the direction to Download only
                        syncOrchestrator.Direction = SyncDirectionOrder.Download;
                        //((SqlSyncProvider) syncOrchestrator.LocalProvider).ApplyChangeFailed += new EventHandler<DbApplyChangeFailedEventArgs>(Program_ApplyChangeFailed);
                        try
                        {
                            syncStats = syncOrchestrator.Synchronize();
                            // prints statistics from the sync session
                            //Console.WriteLine("Start Time: " + syncStats.SyncStartTime);
                            //Console.Write(tableName + " Total Changes Uploaded: " + syncStats.UploadChangesTotal + " ");
                            //Console.WriteLine(tableName + " Total Changes Downloaded: " + syncStats.DownloadChangesTotal);
                            //Console.WriteLine("Complete Time: " + syncStats.SyncEndTime);
                        }
                        catch (Exception ex)
                        {

                        }

                        break;
                }
                Application.DoEvents();
            }
        }

        private void SetEntitiesToSync()
        {
            List<string> transaction = new List<string>() { "StatementOfAccount" };

            List<string> rate = new List<string>() { "TransShipmentRoute", "TransShipmentLeg", "Crating",
                "Packaging", "ShipMode", "ApplicableRate", "ServiceType", "ServiceMode",
                "CommodityType", "ShipmentBasicFee", "FuelSurcharge", "RateMatrix", "ExpressRate", "Commodity" };

            List<string> employeeUser = new List<string>() { "Department", "Position", "Employee", "Role", "User", "RoleUser" };

            List<string> maintenance = new List<string>() { "Group", "Region","Province", "BranchCorpOffice", "Cluster", "City",
                "RevenueUnitType", "RevenueUnit", "AccountStatus", "AccountType", "AdjustmentReason", "ApplicationSetting",
                "BookingRemark", "BookingStatus", "BusinessType", "Industry", "OrganizationType", "BillingPeriod", "PaymentMode",
                "PaymentTerm", "PaymentType", "GoodsDescription", "DeliveryStatus", "DeliveryRemark", "Truck", "TruckAreaMapping" };
            List<string> tracking = new List<string>() { "Batch", "BranchAcceptance", "Bundle", "CargoTransfer", "Distribution", "GatewayInbound", "GatewayOutbound", "GatewayTransmittal", "HoldCargo", "Reason", "Remarks", "Segregation", "Status", "Unbundle" };
            List<string> withFilter = new List<string>() { "Company", "Client", "Booking", "Shipment", "PackageDimension",
                "PackageNumber", "StatementOfAccountPayment", "Payment", "PaymentTurnover", "ShipmentAdjustment", "Delivery",
                "DeliveredPackage", "DeliveryReceipt" };
            Entities.Clear();
            Entities.AddRange(transaction);
            Entities.AddRange(rate);
            Entities.AddRange(employeeUser);
            Entities.AddRange(maintenance);
            Entities.AddRange(transaction);
            Entities.AddRange(withFilter);
            Entities.AddRange(tracking);


        }

        private void DeProvision_Template()
        {
            // Remove the "_Filter_template" template from the server database.
            // This also removes all of the scopes that depend on the template.
            SqlSyncScopeDeprovisioning templateDeprovisioning = new SqlSyncScopeDeprovisioning(ServerConn);

            try
            {
                // Remove the scope.
                templateDeprovisioning.DeprovisionTemplate("Orders_OriginState_Filter_template");
            }
            catch (Exception)
            {

            }

        }

        private void DeProvision_Server()
        {
            try
            {
                SqlSyncScopeDeprovisioning storeServerDeprovision = new SqlSyncScopeDeprovisioning(ServerConn);
                storeServerDeprovision.DeprovisionStore();
                Console.WriteLine("Server Deprovision.");
            }
            catch (Exception ex)
            {

            }
        }

        private void DeProvision_Client()
        {
            try
            {
                SqlSyncScopeDeprovisioning storeClientDeprovision = new SqlSyncScopeDeprovisioning(ClientConn);
                storeClientDeprovision.DeprovisionStore();
                Console.WriteLine("Client Deprovision.");
            }
            catch (Exception)
            {


            }

        }

        private void Program_ApplyChangeFailed(object sender, DbApplyChangeFailedEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.Conflict.Type);
            Console.WriteLine(e.Conflict.ErrorMessage);

        }
    }
}

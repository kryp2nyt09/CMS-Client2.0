
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using System.Data;
using Microsoft.Synchronization;
using System.Threading.Tasks;
using System.Threading;
using AP_CARGO_SERVICE.Properties;

namespace AP_CARGO_SERVICE
{
    public class Synchronization
    {
        SqlConnection _localConnection;
        SqlConnection _serverConnection;
        string BranchCorpOfficeId;
        string Filter;
        bool isProvision;
        bool DeProvisionServer;
        bool DeprovisionClient;
        List<string> Entities = new List<string>();
        
        List<ManualResetEvent> synchronizeEvents = new List<ManualResetEvent>();
        List<ManualResetEvent> provisionEvents = new List<ManualResetEvent>();
        List<ManualResetEvent> deprovisionEvents = new List<ManualResetEvent>();


        public Synchronization()
        {
            if (ValidateConnectionStrings(Settings.Default.LocalConnectionString, Settings.Default.ServerConnectionString))
            {
                this._localConnection = new SqlConnection(Settings.Default.LocalConnectionString);
                this._serverConnection = new SqlConnection(Settings.Default.ServerConnectionString);
                this.SetEntities();
                this.BranchCorpOfficeId = Settings.Default.BranchCorpOfficeId.ToString();
                this.Filter = Settings.Default.Filter;
                this.isProvision = Settings.Default.Provision;
                this.DeProvisionServer = Settings.Default.DeprovisionServer;
                this.DeprovisionClient = Settings.Default.DeprovisionClient;
                this.DeProvision_Server();
                this.DeProvision_Client();
                this.Prepare_Database_For_Synchronization();
            }
        }
        private bool ValidateConnectionStrings(string _local, string _server)
        {
            if (_local == _server)
            {
                return false;
            }
            return true;
        }
        private void SetEntities()
        {
            List<string> transaction = new List<string>() { "StatementOfAccount" };

            List<string> rate = new List<string>() { "TransShipmentRoute", "TransShipmentLeg", "Crating", "Packaging", "ShipMode", "ApplicableRate", "ServiceType", "ServiceMode", "CommodityType", "ShipmentBasicFee", "FuelSurcharge", "RateMatrix", "ExpressRate", "Commodity" };

            List<string> employeeUser = new List<string>() { "Department", "Position", "Employee", "Role", "User", "RoleUser" };

            List<string> maintenance = new List<string>() { "Group", "Region","Province", "BranchCorpOffice", "Cluster", "City",
                "RevenueUnitType", "RevenueUnit", "AccountStatus", "AccountType", "AdjustmentReason", "ApplicationSetting",
                "BookingRemark", "BookingStatus", "BusinessType", "Industry", "OrganizationType", "BillingPeriod", "PaymentMode",
                "PaymentTerm", "PaymentType", "GoodsDescription", "DeliveryStatus", "DeliveryRemark", "Truck", "TruckAreaMapping" };

            List<string> tracking = new List<string>() { "Batch", "BranchAcceptance", "Bundle", "CargoTransfer", "Distribution", "GatewayInbound", "GatewayOutbound", "GatewayTransmittal", "HoldCargo", "Reason", "Remarks", "Segregation", "Status", "Unbundle" };

            List<string> withFilter = new List<string>() { "Company", "Client", "Booking", "Shipment", "PackageDimension",
                "PackageNumber", "StatementOfAccountPayment", "Payment", "PaymentTurnover", "ShipmentAdjustment", "Delivery",
                "DeliveredPackage", "DeliveryReceipt" };

            Entities.AddRange(transaction);
            Entities.AddRange(rate);
            Entities.AddRange(employeeUser);
            Entities.AddRange(maintenance);
            Entities.AddRange(withFilter);
            Entities.AddRange(tracking);


        }
        private void Prepare_Database_For_Synchronization()
        {

            if (!isProvision)
            {
                return;
            }

            foreach (string tableName in Entities)
            {
                ManualResetEvent _newEvent = new ManualResetEvent(false);
                provisionEvents.Add(_newEvent);
                Provision _provision = new Provision(tableName, _localConnection, _serverConnection, _newEvent, Filter, BranchCorpOfficeId);
                ThreadPool.QueueUserWorkItem(_provision.Prepare_Database_For_Synchronization, tableName);
                _newEvent.WaitOne();

            }
        }
        private void DeProvision_Server()
        {
            if (!this.DeProvisionServer)
            {
                return;
            }

            try
            {
                foreach (string item in Entities)
                {
                    ManualResetEvent _newEvent = new ManualResetEvent(false);
                    deprovisionEvents.Add(_newEvent);
                    Deprovision _deprovision = new Deprovision(_serverConnection, _newEvent, Filter, item);
                    ThreadPool.QueueUserWorkItem(_deprovision.PerformDeprovision, item);
                }


            }
            catch (Exception ex)
            {
                Log.WriteErrorLogs(ex);
            }

        }
        private void DeProvision_Client()
        {
            if (!this.DeprovisionClient)
            {
                return;
            }

            this.DeprovisionClient = false;
            try
            {
                SqlSyncScopeDeprovisioning storeClientDeprovision = new SqlSyncScopeDeprovisioning(_localConnection);
                storeClientDeprovision.DeprovisionStore();

                Log.WriteLogs("Client was Deprovisioned.");
            }
            catch (Exception ex)
            {
                Log.WriteErrorLogs(ex);
            }

        }
        public void Synchronized()
        {

            foreach (string tableName in Entities)
            {
                ManualResetEvent _newEvent = new ManualResetEvent(false);
                synchronizeEvents.Add(_newEvent);
                Synchronize sync = new Synchronize(tableName, Filter, _newEvent, _localConnection, _serverConnection);
                ThreadPool.QueueUserWorkItem(sync.PerformSync, tableName);
                _newEvent.WaitOne();
            }
        }
        private void Program_ApplyChangeFailed(object sender, DbApplyChangeFailedEventArgs e)
        {
            // display conflict type
            //Console.WriteLine(e.Conflict.Type);
            //Console.WriteLine(e.Conflict.ErrorMessage);
            //WriteLogs(e.Conflict.Type.ToString());
            //WriteLogs(e.Conflict.ErrorMessage.ToString());

        }

    }

    public class Synchronize
    {

        private ManualResetEvent _currentEvent;
        private string _tableName;
        private SqlConnection _localConnection;
        private SqlConnection _serverConnection;
        private string _filter;

        public Synchronize(string tableName, string filter, ManualResetEvent currentEvent, SqlConnection localConnection, SqlConnection serverConnection)
        {
            this._tableName = tableName;
            this._filter = filter;
            this._currentEvent = currentEvent;
            this._localConnection = localConnection;
            this._serverConnection = serverConnection;
        }
        public void PerformSync(Object obj)
        {
            SyncOrchestrator syncOrchestrator = new SyncOrchestrator();
            syncOrchestrator.LocalProvider = new SqlSyncProvider(_tableName + _filter, _localConnection);
            syncOrchestrator.RemoteProvider = new SqlSyncProvider(_tableName + _filter, _serverConnection);
            SyncOperationStatistics syncStats;

            switch (_tableName)
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

                    syncOrchestrator.Direction = SyncDirectionOrder.UploadAndDownload;

                    try
                    {
                        syncStats = syncOrchestrator.Synchronize();
                        Log.WriteLogs(_tableName + " Total Changes Uploaded: " + syncStats.UploadChangesTotal + " Total Changes Downloaded: " + syncStats.DownloadChangesTotal + " Total Changes applied: " + syncStats.DownloadChangesApplied + " Total Changes failed: " + syncStats.DownloadChangesFailed);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.WriteErrorLogs(ex);
                    }
                    break;

                default:
                    syncOrchestrator.Direction = SyncDirectionOrder.Download;
                    try
                    {
                        syncStats = syncOrchestrator.Synchronize();

                        Log.WriteLogs(_tableName + " Total Changes Uploaded: " + syncStats.UploadChangesTotal + " Total Changes Downloaded: " + syncStats.DownloadChangesTotal + " Total Changes applied: " + syncStats.DownloadChangesApplied + " Total Changes failed: " + syncStats.DownloadChangesFailed);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.WriteErrorLogs(ex);
                    }
                    break;
            }

            _currentEvent.Set();

        }

    }

    public class Provision
    {

        SqlConnection _serverConnection;
        SqlConnection _localConnection;

        private string _tableName;
        private string _filter;
        private string _branchCorpOfficeId;

        private ManualResetEvent _currentEvent;

        static DbSyncScopeDescription scopeDesc;
        static DbSyncTableDescription tableDescription;
        static SqlSyncScopeProvisioning serverTemplate;

        public Provision(string tableName, SqlConnection localConnection, SqlConnection serverConnection, ManualResetEvent currentEvent, string filter, string branchCorpOfficeId)
        {
            this._tableName = tableName;
            this._localConnection = new SqlConnection(localConnection.ConnectionString);
            this._serverConnection = new SqlConnection(serverConnection.ConnectionString);
            this._currentEvent = currentEvent;
            this._filter = filter;
            this._branchCorpOfficeId = branchCorpOfficeId;
        }
        public void Prepare_Database_For_Synchronization(Object obj)
        {

            SqlParameter param;

            string filterColumn = "";
            string filterClause = "";
            switch (_tableName)
            {
                case "Booking":

                    filterColumn = "AssignedToAreaId";
                    filterClause = "[side].[AssignedToAreaId] IN (SELECT book.AssignedToAreaId  FROM Booking as book " +
                                    "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                    "left join City as city on city.CityId = ru.CityId " +
                                    "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                    param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

                    break;
                case "Shipment":

                    filterColumn = "ShipmentId";
                    filterClause = "[side].[ShipmentId] In (SELECT ship.ShipmentId FROM Shipment as ship " +
                                    "left join Booking as book on book.BookingId = ship.BookingId " +
                                    "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                    "left join City as city on city.CityId = ru.CityId " +
                                    "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                    param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

                    break;

                case "PackageNumber":

                    filterColumn = "ShipmentId";
                    filterClause = "[side].[ShipmentId] In (SELECT pack.ShipmentId FROM PackageNumber as pack " +
                                    "left join   Shipment as ship on ship.ShipmentId = pack.ShipmentId " +
                                    "left join Booking as book on book.BookingId = ship.BookingId " +
                                    "left join RevenueUnit as ru on ru.RevenueUnitId = book.AssignedToAreaId " +
                                    "left join City as city on city.CityId = ru.CityId " +
                                    "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                    param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

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

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

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

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

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

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

                    break;
                case "PaymenTurnOver":

                    filterColumn = "CollectedById";
                    filterClause = "[side].[CollectedById] In (Select turnOver.CollectedById From PaymentTurnOver as turnOver " +
                                    "left join Employee as emp on emp.EmployeeId = turnOver.CollectedById " +
                                    "left join RevenueUnit as ru on ru.RevenueUnitId = emp.AssignedToAreaId " +
                                    "left join City as city on city.CityId = ru.CityId " +
                                    "where city.BranchCorpOfficeId = @BranchCorpOfficeId)";
                    param = new SqlParameter("@BranchCorpOfficeId", SqlDbType.UniqueIdentifier);

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

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

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);
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

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

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

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

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

                    CreateTemplate(_tableName, filterColumn, filterClause, param);

                    ProvisionServer(_tableName, param, _branchCorpOfficeId);

                    ProvisionClient(_tableName);

                    break;
                default:
                    ProvisionServer(_tableName);
                    ProvisionClient(_tableName);
                    break;

                    _currentEvent.Set();
            }
        }
        private void ProvisionServer(string TableName)
        {
            try
            {
                // define a new scope named tableNameScope
                DbSyncScopeDescription scopeDesc = new DbSyncScopeDescription(TableName + _filter);
                // get the description of the tableName
                DbSyncTableDescription tableDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable(TableName, _serverConnection);

                // add the table description to the sync scope definition
                scopeDesc.Tables.Add(tableDesc);

                // create a server scope provisioning object based on the tableNameScope
                SqlSyncScopeProvisioning serverProvision = new SqlSyncScopeProvisioning(_serverConnection, scopeDesc);

                // start the provisioning process
                if (!serverProvision.ScopeExists(scopeDesc.ScopeName))
                {
                    serverProvision.Apply();
                    Console.WriteLine("Server " + TableName + " was provisioned.");
                    Log.WriteLogs("Server " + TableName + " was provisioned.");
                }
                else
                {
                    Console.WriteLine("Server " + TableName + " was already provisioned.");
                    Log.WriteLogs("Server " + TableName + " was already provisioned.");
                }
            }
            catch (Exception ex)
            {
                Log.WriteErrorLogs(ex);
            }


        }
        private void ProvisionServer(string TableName, SqlParameter Parameter, string ParamValue)
        {
            try
            {
                // Create a synchronization scope for OriginState=WA.
                SqlSyncScopeProvisioning serverProvision = new SqlSyncScopeProvisioning(_serverConnection);

                // populate the scope description using the template
                serverProvision.PopulateFromTemplate(TableName + _filter, TableName + "_Filter_Template");

                // specify the value we want to pass in the filter parameter, in this case we want only orders from WA
                serverProvision.Tables[TableName].FilterParameters[Parameter.ParameterName].Value = Guid.Parse(ParamValue);

                // Set a friendly description of the template.
                serverProvision.UserComment = TableName + " data includes only " + ParamValue;

                // Create the new filtered scope in the database.
                if (!serverProvision.ScopeExists(serverProvision.ScopeName))
                {
                    serverProvision.Apply();
                    Console.WriteLine("Server " + TableName + " was provisioned.");
                    Log.WriteLogs("Server " + TableName + " was provisioned.");
                }
                else
                {
                    Console.WriteLine("Server " + TableName + " was already provisioned.");
                    Log.WriteLogs("Server " + TableName + " was provisioned.");
                }
            }
            catch (Exception ex)
            {
                Log.WriteErrorLogs(ex);
            }
        }
        private void ProvisionClient(string TableName)
        {
            DbSyncScopeDescription scopeDescription = SqlSyncDescriptionBuilder.GetDescriptionForScope(TableName + _filter, _serverConnection);

            SqlSyncScopeProvisioning clientProvision = new SqlSyncScopeProvisioning(_localConnection, scopeDescription);

            if (!clientProvision.ScopeExists(scopeDescription.ScopeName))
            {
                clientProvision.Apply();
                Console.WriteLine("Client " + TableName + " was provisioned.");
                Log.WriteLogs("Client " + TableName + " was provisioned.");
            }
            else
            {
                Console.WriteLine("Client " + TableName + " was already provisioned.");
                Log.WriteLogs("Client " + TableName + " was already provisioned.");
            }
        }
        private void CreateTemplate(string TableName, string filterColumn, string filterClause, SqlParameter param)
        {
            try
            {
                // Create a template named tableName + _Filter_Template
                scopeDesc = new DbSyncScopeDescription(TableName + "_Filter_Template");

                // Set a friendly description of the template.
                scopeDesc.UserComment = "Filter template for " + TableName + ".";

                // Definition for tables.
                tableDescription = SqlSyncDescriptionBuilder.GetDescriptionForTable(TableName, _serverConnection);
                scopeDesc.Tables.Add(tableDescription);

                // Create a provisioning object for "tableName_Filter_template" that can be used to create a template
                // from which filtered synchronization scopes can be created.
                serverTemplate = new SqlSyncScopeProvisioning(_serverConnection, scopeDesc, SqlSyncScopeProvisioningType.Template);

                AddFilter(TableName, filterColumn, filterClause, param);

                // create a new select changes stored proc for this scope
                serverTemplate.SetCreateProceduresForAdditionalScopeDefault(DbSyncCreationOption.Create);

                // Create the tableName_Filter_template" template in the database.
                if (!serverTemplate.TemplateExists(TableName + "_Filter_Template"))
                {
                    serverTemplate.Apply();
                    Console.WriteLine(TableName + " filter template was created.");
                    Log.WriteLogs(TableName + " filter template was created.");
                }
                else
                {
                    Console.WriteLine(TableName + " filter template was already exist.");
                    Log.WriteLogs(TableName + " filter template was already exist.");
                }
            }
            catch (Exception ex)
            {
                Log.WriteErrorLogs(ex);
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
        private void AddFilter(string TableName, string FilterColumn, string FilterClause, SqlParameter param)
        {
            serverTemplate.Tables[TableName].AddFilterColumn(FilterColumn);
            serverTemplate.Tables[TableName].FilterClause = FilterClause;
            serverTemplate.Tables[TableName].FilterParameters.Add(param);
        }
    }

    public class Deprovision
    {
        private SqlConnection _connection;
        private ManualResetEvent _currentEvent;
        private string _filter;
        private string _tableName;

        public Deprovision(SqlConnection connection, ManualResetEvent currentEvent, string filter, string tableName)
        {
            this._connection = new SqlConnection(connection.ConnectionString);
            this._currentEvent = currentEvent;
            this._filter = filter;
            this._tableName = tableName;
        }

        public void PerformDeprovision(Object obj)
        {
            SqlSyncScopeDeprovisioning storeClientDeprovision = new SqlSyncScopeDeprovisioning(_connection);
            storeClientDeprovision.DeprovisionScope(this._tableName + this._filter);
            Console.WriteLine("Server " + _tableName + " was Deprovision.");
            Log.WriteLogs("Server " + _tableName + " was Deprovision.");
            _currentEvent.Set();
        }

    }

    static class Log
    {
        public static async Task WriteLogs(string Logs)
        {
            string _fileName = "\\Logs\\SyncTransactionLogs" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt";
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + _fileName, Environment.NewLine + DateTime.Now.ToString() + " :: " + Logs);

        }

        public static async Task WriteErrorLogs(Exception ex)
        {
            string _fileName = "\\Logs\\SyncErrorLogs" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt";
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + _fileName, Environment.NewLine + DateTime.Now.ToString() + " :: " + ex.Message.ToString());
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + _fileName, Environment.NewLine + DateTime.Now.ToString() + " :: " + ex.StackTrace.ToString());
        }

        public static async Task WriteErrorLogs(string Location, Exception ex)
        {
            string _fileName = "\\Logs\\SyncErrorLogs" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".txt";
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + _fileName, Environment.NewLine + DateTime.Now.ToString() + " :: " + Location);
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + _fileName, Environment.NewLine + DateTime.Now.ToString() + " :: " + ex.Message.ToString());
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + _fileName, Environment.NewLine + DateTime.Now.ToString() + " :: " + ex.StackTrace.ToString());
        }
    }

}

namespace CMS2.Client.Forms.TrackingReportsView
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for GatewatInboundReportView.
    /// </summary>
    public partial class GatewayInboundReportView : Telerik.Reporting.Report
    {
        public GatewayInboundReportView()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            txtDate.Value = TrackingReportGlobalModel.Date;
            txtGateway.Value = TrackingReportGlobalModel.Gateway;
            txtMAWB.Value = TrackingReportGlobalModel.AirwayBillNo;
            txtFlightNo.Value = TrackingReportGlobalModel.FlightNo;
            txtCommodityType.Value = TrackingReportGlobalModel.CommodityType;
        }
    }
}
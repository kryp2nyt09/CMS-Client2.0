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
    /// Summary description for GatewayOutboundReportView.
    /// </summary>
    public partial class GatewayOutboundReportView : Telerik.Reporting.Report
    {
        public GatewayOutboundReportView()
        {
            InitializeComponent();

            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            txtDate.Value = TrackingReportGlobalModel.Date;
            txtGateway.Value = TrackingReportGlobalModel.Gateway;
            txtDriver.Value = TrackingReportGlobalModel.Driver;
            txtPlateNo.Value = TrackingReportGlobalModel.PlateNo;

            txtScannedBy.Value = TrackingReportGlobalModel.ScannedBy;
            // txtRemarks.Value = TrackingReportGlobalModel.Remarks;
            // txtNotes.Value = TrackingReportGlobalModel.Notes;
        }
    }
}
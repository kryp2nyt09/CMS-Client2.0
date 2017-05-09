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
    /// Summary description for GatewayTransmitalReportView.
    /// </summary>
    public partial class GatewayTransmitalReportView : Telerik.Reporting.Report
    {
        public GatewayTransmitalReportView()
        {
            InitializeComponent();

            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            txtDate.Value = TrackingReportGlobalModel.Date;
            txtDriver.Value = TrackingReportGlobalModel.Driver;
            txtPlateNo.Value = TrackingReportGlobalModel.PlateNo;
            txtMAWB.Value = TrackingReportGlobalModel.AirwayBillNo;
            txtArea.Value = TrackingReportGlobalModel.Area;
            txtGateway.Value = TrackingReportGlobalModel.Gateway;
            txtScannedBy.Value = TrackingReportGlobalModel.ScannedBy;
           // txtRemarks.Value = TrackingReportGlobalModel.Remarks;
           // txtNotes.Value = TrackingReportGlobalModel.Notes;
        }
    }
}
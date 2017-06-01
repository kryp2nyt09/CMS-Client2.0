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
    /// Summary description for CargoTransferReportView.
    /// </summary>
    public partial class CargoTransferReportView : Telerik.Reporting.Report
    {
        public CargoTransferReportView()
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
            txtOrigin.Value = TrackingReportGlobalModel.Origin;
            txtDestination.Value = TrackingReportGlobalModel.Destination;

            txtDriver.Value = TrackingReportGlobalModel.Driver;
            txtChecker.Value = TrackingReportGlobalModel.Checker;
            txtPlateNo.Value = TrackingReportGlobalModel.PlateNo;

            txtScannedBy.Value = TrackingReportGlobalModel.ScannedBy;
            // txtRemarks.Value = TrackingReportGlobalModel.Remarks;
            // txtNotes.Value = TrackingReportGlobalModel.Notes;
        }
    }
}
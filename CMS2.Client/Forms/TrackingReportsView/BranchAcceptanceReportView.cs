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
    /// Summary description for BranchAcceptance.
    /// </summary>
    public partial class BranchAcceptanceReportView : Telerik.Reporting.Report
    {
        public BranchAcceptanceReportView()
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
            txtArea.Value = TrackingReportGlobalModel.Area;
            txtDriver.Value = TrackingReportGlobalModel.Driver;
            txtChecker.Value = TrackingReportGlobalModel.Checker;
            txtBatch.Value = TrackingReportGlobalModel.Batch;
            txtPlateNo.Value = TrackingReportGlobalModel.PlateNo;
        }
    }
}
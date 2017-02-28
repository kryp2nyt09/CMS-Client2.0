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
            InitializeComponent();

            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            txtDate.Value = TrackingReportGlobalModel.Date;
            txtArea.Value = TrackingReportGlobalModel.Branch; //BRANCH
            txtDriver.Value = TrackingReportGlobalModel.Driver;
            txtChecker.Value = TrackingReportGlobalModel.Checker;
            txtBatch.Value = TrackingReportGlobalModel.Batch;
            txtPlateNo.Value = TrackingReportGlobalModel.PlateNo;

            txtScannedBy.Value = TrackingReportGlobalModel.ScannedBy;
        }
    }
}
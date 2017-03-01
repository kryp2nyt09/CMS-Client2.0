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
    /// Summary description for DailyTripReportView.
    /// </summary>
    public partial class DailyTripReportView : Telerik.Reporting.Report
    {
        public DailyTripReportView()
        {
            InitializeComponent();

            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            var objectDataSource2 = new Telerik.Reporting.ObjectDataSource();
            var objectDataSource3 = new Telerik.Reporting.ObjectDataSource();
            var objectDataSource4 = new Telerik.Reporting.ObjectDataSource();

            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            DataTable dataTable2 = TrackingReportGlobalModel.table2;
            objectDataSource2.DataSource = dataTable2;
            table2.DataSource = objectDataSource2;

            DataTable dataTable3 = TrackingReportGlobalModel.table3;
            objectDataSource3.DataSource = dataTable3;
            table3.DataSource = objectDataSource3;

            DataTable dataTable4 = TrackingReportGlobalModel.table4;
            objectDataSource4.DataSource = dataTable4;
            table4.DataSource = objectDataSource4;

            txtDate.Value = TrackingReportGlobalModel.Date;
            txtArea.Value = TrackingReportGlobalModel.Area;
            txtDriver.Value = TrackingReportGlobalModel.Driver;
            txtChecker.Value = TrackingReportGlobalModel.Checker;
            txtPlateNo.Value = TrackingReportGlobalModel.PlateNo;
        }
    }
}
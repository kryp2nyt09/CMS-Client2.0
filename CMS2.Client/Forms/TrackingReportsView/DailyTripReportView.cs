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
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            dataTable = TrackingReportGlobalModel.table2;
            objectDataSource.DataSource = dataTable;
            table2.DataSource = objectDataSource;

            dataTable = TrackingReportGlobalModel.table3;
            objectDataSource.DataSource = dataTable;
            table2.DataSource = objectDataSource;

            dataTable = TrackingReportGlobalModel.table4;
            objectDataSource.DataSource = dataTable;
            table3.DataSource = objectDataSource;

        }
    }
}
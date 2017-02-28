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
    /// Summary description for Bundle.
    /// </summary>
    public partial class BundleReportView : Telerik.Reporting.Report
    {
        public BundleReportView()
        {
            InitializeComponent();

            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            txtDate.Value = TrackingReportGlobalModel.Date;
            txtBundleNo.Value = TrackingReportGlobalModel.SackNo;
            txtDestination.Value = TrackingReportGlobalModel.Destination;
            txtWeight.Value = TrackingReportGlobalModel.Weight;

        }
    }
}
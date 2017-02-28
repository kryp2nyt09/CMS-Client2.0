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
    /// Summary description for UnbundleReportView.
    /// </summary>
    public partial class UnbundleReportView : Telerik.Reporting.Report
    {
        public UnbundleReportView()
        {
            InitializeComponent();
            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            txtDate.Value = TrackingReportGlobalModel.Date;
            txtOrigin.Value = TrackingReportGlobalModel.Origin;
            txtSackNo.Value = TrackingReportGlobalModel.SackNo;

        }
        public void setData()
        {
            
        }
    }
}
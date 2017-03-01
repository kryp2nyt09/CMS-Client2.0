namespace CMS2.Client.Forms.TrackingReportsView
{
    using Entities.ReportModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for SegregationReportView.
    /// </summary>
    public partial class SegregationReportView : Telerik.Reporting.Report
    {
        public SegregationReportView()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            //List<string> x = new List<string>();

            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            txtDate.Value = TrackingReportGlobalModel.Date;
            txtDriver.Value = TrackingReportGlobalModel.Driver;
            txtChecker.Value = TrackingReportGlobalModel.Checker;
            txtPlateNo.Value = TrackingReportGlobalModel.PlateNo;

            //
        }
    }
}
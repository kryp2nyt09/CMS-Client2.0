namespace CMS2.Client.Forms.TrackingReportsView
{
    using TrackingReports;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using Entities;
    using System.Data;

    /// <summary>
    /// Summary description for PickupCargo.
    /// </summary>
    public partial class PickupCargoManifestReportView : Telerik.Reporting.Report
    {
        public PickupCargoManifestReportView()
        {
            InitializeComponent();

            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            DataTable dataTable = TrackingReportGlobalModel.table;
            objectDataSource.DataSource = dataTable;
            table1.DataSource = objectDataSource;

            txtDate.Value = TrackingReportGlobalModel.Date;
            txtArea.Value = TrackingReportGlobalModel.Area;
            txtDriver.Value = TrackingReportGlobalModel.Driver;
            txtChecker.Value = TrackingReportGlobalModel.Checker;

        }
    }
}
using CMS2.BusinessLogic;
using CMS2.Entities;
using CMS2.Entities.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Client.Forms.TrackingReports
{
    public class SegregationReport
    {
        public DataTable getData(DateTime date)
        {
            SegregationBL segregationBL = new SegregationBL();

            List<Segregation> _segregation = segregationBL.GetAll().Where(x => x.RecordStatus == 1 && x.BranchCorpOfficeID == GlobalVars.DeviceBcoId && x.CreatedDate.ToShortDateString() == date.ToShortDateString()).ToList();

            List<SegregationViewModel> modelList = Macth(_segregation);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Branch Corp Office", typeof(string)));
            dt.Columns.Add(new DataColumn("Driver", typeof(string)));
            dt.Columns.Add(new DataColumn("Checker", typeof(string)));
            dt.Columns.Add(new DataColumn("Plate #", typeof(string)));
            dt.Columns.Add(new DataColumn("Batch", typeof(string)));
            dt.Columns.Add(new DataColumn("AWB", typeof(string)));
            dt.Columns.Add(new DataColumn("Qty", typeof(string)));
            dt.Columns.Add(new DataColumn("Area", typeof(string)));
            dt.Columns.Add(new DataColumn("CreatedDate", typeof(string)));
            dt.BeginLoadData();
            int ctr = 1;
            foreach (SegregationViewModel item in modelList)
            {
                DataRow row = dt.NewRow();
                row[0] = ctr++.ToString();
                row[1] = item.BranchCorpOffice;
                row[2] = item.Driver;
                row[3] = item.Checker;
                row[4] = item.PlateNo;
                row[5] = item.Batch;
                row[6] = item.AirwayBillNo;
                row[7] = item.Qty.ToString();
                row[8] = item.Area;
                row[9] = item.CreatedDate.ToShortDateString();
                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }


        public List<int> setWidth()
        {
            List<int> width = new List<int>();
            width.Add(30); 
            width.Add(180);
            width.Add(150);
            width.Add(150);
            width.Add(150);
            width.Add(120);
            width.Add(120);
            width.Add(100);
            width.Add(200);
            width.Add(0);
            return width;
        }

        public List<SegregationViewModel> Macth(List<Segregation> _segregation)
        {
            List<SegregationViewModel> _results = new List<SegregationViewModel>();
            
            PackageNumberBL _packageNumberService = new PackageNumberBL();

            ShipmentBL shipmentService = new ShipmentBL();
           
            foreach (Segregation segregation in _segregation)
            {
                SegregationViewModel model = new SegregationViewModel();
                string _airwaybill = "";
                try {
                    _airwaybill = _packageNumberService.GetAll().Find(x => x.PackageNo == segregation.Cargo).Shipment.AirwayBillNo;
                }
                catch (Exception) { continue; }
                SegregationViewModel isExist = _results.Find(x => x.AirwayBillNo == _airwaybill);

                if (isExist != null)
                {
                    isExist.Qty++;
                }
                else
                {
                    model.BranchCorpOffice = segregation.BranchCorpOffice.BranchCorpOfficeName;
                    model.Driver = segregation.Driver;
                    model.Checker = segregation.Checker;
                    model.PlateNo = segregation.PlateNo;
                    model.Batch = segregation.Batch.BatchName;
                    model.AirwayBillNo = _airwaybill;
                    model.Qty++;
                    model.Area = shipmentService.GetAll().Find(x => x.AirwayBillNo == _airwaybill).DestinationCity.CityName;
                    model.CreatedDate = segregation.CreatedDate;
                    _results.Add(model);
                }
            }

            return _results;
        }

    }
}

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
    public class UnbundleReport
    {
        public DataTable getBundleData(DateTime date)
        {
            UnbundleBL unbundlebl = new UnbundleBL();
            BundleBL bundlebl = new BundleBL();

            List<Unbundle> unbundleList = unbundlebl.GetAll().Where(x => x.RecordStatus == 1 && x.BranchCorpOfficeID == GlobalVars.DeviceBcoId).ToList();
            List<Bundle> bundleList = bundlebl.GetAll().Where(x => x.RecordStatus == 1 && x.BranchCorpOfficeID == GlobalVars.DeviceBcoId && x.CreatedDate.ToShortDateString() == date.ToShortDateString()).ToList();

            List<UnbundleViewModel> modelList = Match(unbundleList , bundleList);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Sack No", typeof(string)));
            dt.Columns.Add(new DataColumn("Total Pieces", typeof(string)));
            dt.Columns.Add(new DataColumn("Scanned Pieces", typeof(string)));
            dt.Columns.Add(new DataColumn("Origin", typeof(string)));
            dt.Columns.Add(new DataColumn("Weight", typeof(string)));
            dt.Columns.Add(new DataColumn("AWB", typeof(string)));

            dt.Columns.Add(new DataColumn("Recieved(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Dicrepency(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Total Qty", typeof(string)));

            dt.Columns.Add(new DataColumn("CreatedDate", typeof(string)));
            dt.Columns.Add(new DataColumn("Branch", typeof(string)));

            dt.BeginLoadData();
            int ctr = 1;
            foreach (UnbundleViewModel item in modelList)
            {
                DataRow row = dt.NewRow();
                row[0] = "" + ctr++;
                row[1] = item.SackNo;
                row[2] = item.TotalPcs;
                row[3] = item.ScannedPcs;
                row[4] = item.Origin;
                row[5] = item.Weight;
                row[6] = item.AirwayBillNo;
                row[7] = item.TotalRecieved;
                row[8] = item.TotalDiscrepency;
                row[9] = item.Total;
                row[10] = item.CreatedDate;
                row[11] = item.Branch;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }
        public List<int> setBundleWidth()
        {
            List<int> width = new List<int>();
            width.Add(30);
            width.Add(130);
            width.Add(110);
            width.Add(110);
            width.Add(210);
            width.Add(110);
            width.Add(110);            
            width.Add(110);
            width.Add(110);
            width.Add(110);
            width.Add(0);
            width.Add(0);
            return width;
        }
        public List<UnbundleViewModel> Match(List<Unbundle> _unbundle , List<Bundle> _bundle)
        {
            PackageNumberBL _packageNumberService = new PackageNumberBL();
            List<UnbundleViewModel> _results = new List<UnbundleViewModel>();

            ShipmentBL shipment = new ShipmentBL();

            foreach (Bundle bundle in _bundle){

                UnbundleViewModel model = new UnbundleViewModel();
                string _airwaybill = _packageNumberService.GetAll().Find(x => x.PackageNo == bundle.Cargo).Shipment.AirwayBillNo;
                UnbundleViewModel isExist = _results.Find(x => x.AirwayBillNo == _airwaybill);

                if (_unbundle.Exists(x => x.Cargo == bundle.Cargo))
                {
                    if (isExist != null)
                    {
                        isExist.TotalRecieved++;
                        model.Total += model.TotalRecieved;
                    }

                    else
                    {
                        model.AirwayBillNo = _airwaybill;
                        model.SackNo = bundle.SackNo;
                        model.TotalPcs = ""+0;
                        model.ScannedPcs = "" + 0;
                        model.Origin = shipment.GetAll().Find(x => x.AirwayBillNo.Equals(_airwaybill)).OriginCity.CityName;
                        model.TotalRecieved++;
                        model.Total += model.TotalRecieved;
                        model.CreatedDate = bundle.CreatedDate;
                        model.Branch = bundle.BranchCorpOffice.BranchCorpOfficeName;
                        _results.Add(model);

                    }
                }
                else
                {

                    if (isExist != null)
                    {
                        isExist.TotalDiscrepency++;
                        model.Total += model.TotalDiscrepency;
                    }

                    else
                    {
                        model.AirwayBillNo = _airwaybill;
                        model.SackNo = bundle.SackNo;
                        model.TotalPcs = "" + 0;
                        model.ScannedPcs = "" + 0;
                        model.Origin = shipment.GetAll().Find(x => x.AirwayBillNo.Equals(_airwaybill)).OriginCity.CityName;
                        model.TotalDiscrepency++;
                        model.Total += model.TotalDiscrepency;
                        model.CreatedDate = bundle.CreatedDate;
                        model.Branch = bundle.BranchCorpOffice.BranchCorpOfficeName;
                        _results.Add(model);
                    }
                }
            }
            return _results;
        }

    }
}


using CMS2.BusinessLogic;
using CMS2.Entities;
using CMS2.Entities.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;

namespace CMS2.Client.Forms.TrackingReports
{
    public class BundleReport
    {
        public DataTable getBundleData(DateTime date)
        {
            BundleBL bundleBl = new BundleBL();

            List<Bundle> list = bundleBl.GetAll().Where(x => x.RecordStatus == 1 && x.BranchCorpOfficeID == GlobalVars.DeviceBcoId && x.CreatedDate.ToShortDateString() == date.ToShortDateString()).ToList();                        
            List<BundleViewModel> bundleList = Match(list);
            
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("AWB", typeof(string)));
            dt.Columns.Add(new DataColumn("Shipper", typeof(string)));
            dt.Columns.Add(new DataColumn("Consignee", typeof(string)));
            dt.Columns.Add(new DataColumn("Address", typeof(string)));
            dt.Columns.Add(new DataColumn("Commodity Type", typeof(string)));
            dt.Columns.Add(new DataColumn("Commodity", typeof(string)));
            dt.Columns.Add(new DataColumn("Qty", typeof(string)));
            dt.Columns.Add(new DataColumn("AGW", typeof(string)));
            dt.Columns.Add(new DataColumn("Service Mode", typeof(string)));
            dt.Columns.Add(new DataColumn("Payment Mode", typeof(string)));
            dt.Columns.Add(new DataColumn("Area", typeof(string)));
            dt.Columns.Add(new DataColumn("SackNo", typeof(string)));

            dt.Columns.Add(new DataColumn("CreatedDate", typeof(string)));
            dt.Columns.Add(new DataColumn("Destination", typeof(string)));

            dt.Columns.Add(new DataColumn("BCO", typeof(string)));
            dt.Columns.Add(new DataColumn("BSO", typeof(string)));

            dt.BeginLoadData();
            int ctr = 1;
            foreach (BundleViewModel item in bundleList)
            {
                DataRow row = dt.NewRow();
                row[0] = "" + ctr++;
                row[1] = item.AirwayBillNo;
                row[2] = item.Shipper;
                row[3] = item.Consignee;
                row[4] = item.Address;
                row[5] = item.CommodityType;
                row[6] = item.Commodity;
                row[7] = item.Qty;
                row[8] = item.AGW;
                row[9] = item.ServiceMode;
                row[10] = item.PaymendMode;
                row[11] = item.Area;
                row[12] = item.SackNo;
                row[13] = item.CreatedDate.ToShortDateString();
                row[14] = item.Destination;

                row[15] = item.BCO;
                row[16] = item.BSO;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        public List<int> setBundleWidth()
        {
            List<int> width = new List<int>();
            width.Add(30);
            width.Add(110);
            width.Add(110);
            width.Add(110);
            width.Add(110);
            width.Add(110);
            width.Add(110);
            width.Add(70);
            width.Add(80);
            width.Add(110);
            width.Add(110);

            width.Add(0);
            width.Add(0);
            width.Add(0);
            width.Add(0);

            width.Add(0);
            width.Add(0);

            return width;
        }

        public List<BundleViewModel> Match(List<Bundle> bundle) {

            PackageNumberBL _packageNumberService = new PackageNumberBL();
            List<BundleViewModel> _results = new List<BundleViewModel>();        
            ShipmentBL shipment = new ShipmentBL();

            foreach (Bundle _bundle in bundle) {

                BundleViewModel model = new BundleViewModel();

                string _airwaybill = _packageNumberService.GetAll().Find(x => x.PackageNo == _bundle.Cargo).Shipment.AirwayBillNo;
              
                BundleViewModel isExist = _results.Find(x => x.AirwayBillNo == _airwaybill);
                
                if (isExist != null)
                {                 
                    _results.Add(isExist);
                }
                else
                {
                    model.AirwayBillNo = _airwaybill;

                    List<Shipment> list = shipment.GetAll().Where(x => x.AirwayBillNo.Equals(_airwaybill)).ToList();                    
                    foreach (Shipment ship in list)
                    {
                        model.Shipper = ship.Shipper.FullName;
                        model.Consignee = ship.Consignee.FullName;
                        model.Address = ship.Consignee.Address1;
                        model.CommodityType = ship.Commodity.CommodityType.CommodityTypeName;
                        model.Commodity = ship.Commodity.CommodityName;
                        model.Qty = bundle.Count.ToString();
                        model.AGW = ship.Weight.ToString();
                        model.ServiceMode = ship.ServiceMode.ServiceModeName;
                        model.PaymendMode = ship.PaymentMode.PaymentModeName;
                        model.Area = ship.OriginCity.CityName;
                        model.CreatedDate = ship.CreatedDate;
                        model.Destination = ship.DestinationCity.BranchCorpOffice.BranchCorpOfficeName;
                        model.BSO = ship.OriginCity.CityName;
                    }
                    model.SackNo = _bundle.SackNo;
                    model.BCO = _bundle.BranchCorpOffice.BranchCorpOfficeName;
                    _results.Add(model);
                }
           }

            return _results;
        }
    }
}

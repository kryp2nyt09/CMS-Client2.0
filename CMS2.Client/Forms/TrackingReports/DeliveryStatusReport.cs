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
    public class DeliveryStatusReport
    {

        public DataTable getData()
        {
            DeliveryBL deliveryStatusBL = new DeliveryBL();
            List<Delivery> list = deliveryStatusBL.GetAll().ToList();

            List<DeliveryStatusViewModel> modelList = Match(list);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("AWB", typeof(string)));
            dt.Columns.Add(new DataColumn("QTY", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("Remarks", typeof(string)));
            dt.Columns.Add(new DataColumn("Area", typeof(string)));
            dt.Columns.Add(new DataColumn("Driver", typeof(string)));
            dt.Columns.Add(new DataColumn("Checker", typeof(string)));
            dt.Columns.Add(new DataColumn("Plate No", typeof(string)));
            dt.Columns.Add(new DataColumn("Batch", typeof(string)));

            dt.BeginLoadData();
            int ctr = 1;
            foreach (DeliveryStatusViewModel item in modelList)
            {
                DataRow row = dt.NewRow();
                row[0] = (ctr++).ToString();
                row[1] = item.AirwayBillNo;
                row[2] = item.QTY.ToString();
                row[3] = item.Status;
                row[4] = item.Remarks;
                row[5] = item.Area;
                row[6] = item.Driver;
                row[7] = item.Checker;
                row[8] = item.PlateNo;
                row[9] = item.Batch;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        public List<int> setWidth()
        {
            List<int> width = new List<int>();
            width.Add(25);
            width.Add(100);
            width.Add(60);
            width.Add(120);
            width.Add(180);
            width.Add(200);
            width.Add(130);
            width.Add(110);
            width.Add(100);
            width.Add(110);
           
            return width;
        }

        public List<DeliveryStatusViewModel> Match(List<Delivery> _deliveryStatus)
        {
            List<DeliveryStatusViewModel> _results = new List<DeliveryStatusViewModel>();
            PackageNumberBL _packageNumberService = new PackageNumberBL();
            
            foreach (Delivery deliveryStatus in _deliveryStatus)
            {
                DeliveryStatusBL status = new DeliveryStatusBL();
                DeliveryRemarkBL remark = new DeliveryRemarkBL();
                DistributionBL distribution = new DistributionBL();
                ShipmentBL shipmentService = new ShipmentBL();

                DeliveryStatusViewModel model = new DeliveryStatusViewModel();

                string _airwaybill = _packageNumberService.GetAll().Find(x => x.ShipmentId == deliveryStatus.ShipmentId).Shipment.AirwayBillNo;
                DeliveryStatusViewModel isExist = _results.Find(x => x.AirwayBillNo == _airwaybill);

                if (isExist != null)
                {
                    isExist.QTY++;
                }
                else
                {
                    model.AirwayBillNo = _airwaybill;
                    model.QTY++;
                    model.Status = status.GetAll().Find(x => x.DeliveryStatusId == deliveryStatus.DeliveryStatusId).DeliveryStatusName;
                    model.Remarks = deliveryStatus.DeliveryRemark.DeliveryRemarkName;
                    //model.Area = distribution.GetAll().Find(x => x.ShipmentId == deliveryStatus.ShipmentId).City.CityName;
                    //model.Driver = distribution.GetAll().Find(x => x.ShipmentId == deliveryStatus.ShipmentId).Driver;
                    //model.Checker = distribution.GetAll().Find(x => x.ShipmentId == deliveryStatus.ShipmentId).Checker;
                    //model.PlateNo = distribution.GetAll().Find(x => x.ShipmentId == deliveryStatus.ShipmentId).PlateNo;
                    //model.Batch = distribution.GetAll().Find(x => x.ShipmentId == deliveryStatus.ShipmentId).Batch.BatchName;

                    _results.Add(model);
                }
            }
            return _results;

        }
    }
}

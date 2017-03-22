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

        public DataTable getData(DateTime date)
        {
            DeliveryBL deliveryStatusBL = new DeliveryBL();
            List<Delivery> list = deliveryStatusBL.GetAll().Where(x => x.RecordStatus == 1 && x.CreatedDate.ToShortDateString() == date.ToShortDateString()).ToList();

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

            dt.Columns.Add(new DataColumn("BCO", typeof(string)));
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
                row[10] = item.BCO;
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
            width.Add(0);
            return width;
        }

        public List<DeliveryStatusViewModel> Match(List<Delivery> _deliveries)
        {
            DeliveryStatusBL status = new DeliveryStatusBL();
            DeliveryRemarkBL remark = new DeliveryRemarkBL();
            DistributionBL distributionService = new DistributionBL();
            ShipmentBL shipmentService = new ShipmentBL();            
            PackageNumberBL _packageNumberService = new PackageNumberBL();

            List<DeliveryStatusViewModel> _results = new List<DeliveryStatusViewModel>();
            List<Distribution> distributions = distributionService.GetAll().ToList();

            foreach (Delivery delivery in _deliveries)
            {               

                DeliveryStatusViewModel model = new DeliveryStatusViewModel();

                DeliveryStatusViewModel isExist = _results.Find(x => x.AirwayBillNo == delivery.Shipment.AirwayBillNo);

                if (isExist != null)
                {
                    isExist.QTY++;
                }
                else
                {
                    model.AirwayBillNo = delivery.Shipment.AirwayBillNo;
                    model.QTY++;
                    model.Status = delivery.DeliveryStatus.DeliveryStatusName;
                    model.Remarks = "NA";
                    if (delivery.DeliveryRemark != null)
                    {
                        model.Remarks = delivery.DeliveryRemark.DeliveryRemarkName;
                    }
                    Distribution dis = distributions.Find(x => x.ShipmentId == delivery.ShipmentId);
                    //List<Distribution> list = distributions.Where( x => x.ShipmentId == delivery.ShipmentId).Distinct().ToList();
                    //foreach(Distribution dis in list)
                    //{
                    //    //model.Area = dis.Area.RevenueUnitName;
                    //    model.Driver = dis.Driver;
                    //    model.Checker = dis.Checker;
                    //    model.Batch = dis.Batch.BatchName;
                    //    model.PlateNo = dis.PlateNo;
                    //    model.BCO = dis.Area.City.BranchCorpOffice.BranchCorpOfficeName;
                    //}
                    model.Area = dis.Area.RevenueUnitName;
                    model.Driver = dis.Driver;
                    model.Checker = dis.Checker;
                    model.Batch = dis.Batch.BatchName;
                    model.PlateNo = dis.PlateNo;
                    model.BCO = dis.Area.City.BranchCorpOffice.BranchCorpOfficeName;
                  
                    _results.Add(model);
                }
            }
            return _results;

        }
    }
}

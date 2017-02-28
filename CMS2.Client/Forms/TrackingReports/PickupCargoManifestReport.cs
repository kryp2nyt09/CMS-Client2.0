using CMS2.BusinessLogic;
using CMS2.Entities;
using CMS2.Entities.ReportModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Client.Forms.TrackingReports
{
    public class PickupCargoManifestReport
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Shipment> getData()
        {
            //List<Shipment> _results = new List<Shipment>();

            ShipmentBL _shipmentService = new ShipmentBL();
            List<Shipment> _shipments = _shipmentService.GetAll().Where(x => x.Booking.BookingStatus.BookingStatusName == "Picked-up" && x.RecordStatus == 1).ToList();

            return _shipments;
        }


        public DataTable getPickUpCargoData(RevenueUnit area , DateTime date)
        {
            //GET LIST 
            ShipmentBL _shipmentService = new ShipmentBL();
            List<Shipment> _shipments;
            if (area == null) {
                //(x.CreatedDate).ToShortDateString() == date.ToShortDateString()
                _shipments = _shipmentService.GetAll().Where(x => x.Booking.BookingStatus.BookingStatusName == "Picked-up" && x.RecordStatus == 1 && (x.CreatedDate).ToShortDateString() == date.ToShortDateString()).ToList();
            }
            else
            {
                _shipments = _shipmentService.GetAll().Where(x => x.Booking.BookingStatus.BookingStatusName == "Picked-up" && x.RecordStatus == 1 && x.Booking.AssignedToAreaId == area.RevenueUnitId && (x.CreatedDate).ToShortDateString() == date.ToShortDateString()).ToList();
            }
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("AWB", typeof(string)));
            dt.Columns.Add(new DataColumn("Shipper", typeof(string)));
            dt.Columns.Add(new DataColumn("Shipper Address", typeof(string)));
            dt.Columns.Add(new DataColumn("Consignee", typeof(string)));
            dt.Columns.Add(new DataColumn("Consignee Address", typeof(string)));
            dt.Columns.Add(new DataColumn("Commodity", typeof(string)));
            dt.Columns.Add(new DataColumn("QTY", typeof(string)));
            dt.Columns.Add(new DataColumn("AGW", typeof(string)));
            dt.Columns.Add(new DataColumn("Service Mode", typeof(string)));
            dt.Columns.Add(new DataColumn("Payment Mode", typeof(string)));
            dt.Columns.Add(new DataColumn("Amount", typeof(decimal)));

            dt.BeginLoadData();
            int ctr = 1;
            foreach (Shipment item in _shipments)
            {
                DataRow row = dt.NewRow();
                row[0] = (ctr++).ToString();
                row[1] = item.AirwayBillNo.ToString();
                row[2] = item.Shipper.FullName.ToString();
                row[3] = item.OriginAddress.ToString();
                row[4] = item.Consignee.FullName.ToString();
                row[5] = item.DestinationAddress.ToString();
                row[6] = item.Commodity.CommodityName.ToString();
                row[7] = item.Quantity.ToString();
                row[8] = item.Weight.ToString();
                row[9] = item.ServiceMode.ServiceModeName.ToString();
                row[10] = item.PaymentMode.PaymentModeName.ToString();
                row[11] = item.TotalAmount;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        public List<int> setPickUpCargoWidth()
        {
            List<int> width = new List<int>();
            width.Add(25); //No
            width.Add(100); //AWB
            width.Add(110); //Shipper
            width.Add(150); //Address
            width.Add(110); //Consignee
            width.Add(150); //Address
            width.Add(110); //Commodity
            width.Add(30); //Qty
            width.Add(50); //AGW
            width.Add(100); //Servicemode
            width.Add(110); //Paymentmode
            width.Add(100); //Amount
            return width;
        }
    }
}

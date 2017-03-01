﻿using CMS2.BusinessLogic;
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
    public class GatewayTransmitalReport
    {
        public DataTable getData(DateTime date)
        {
            GatewayTransmittalBL gatewayBl = new GatewayTransmittalBL();

            List<GatewayTransmittal> list = gatewayBl.GetAll().Where(x => x.RecordStatus == 1 && x.CreatedDate.ToShortDateString() == date.ToShortDateString()).ToList();
            List<GatewayTransmitalViewModel> modelList = Match(list);

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

            dt.Columns.Add(new DataColumn("Gateway", typeof(string)));
            dt.Columns.Add(new DataColumn("Destination", typeof(string)));
            dt.Columns.Add(new DataColumn("Batch", typeof(string)));

            dt.Columns.Add(new DataColumn("CreatedDate", typeof(string)));

            dt.Columns.Add(new DataColumn("Driver", typeof(string)));
            dt.Columns.Add(new DataColumn("PlateNo", typeof(string)));

            dt.BeginLoadData();
            int ctr = 1;
            foreach (GatewayTransmitalViewModel item in modelList)
            {
                DataRow row = dt.NewRow();
                row[0] = (ctr++).ToString();
                row[1] = item.AirwayBillNo;
                row[2] = item.Shipper;
                row[3] = item.Consignee;
                row[4] = item.Address;
                row[5] = item.CommodityType;
                row[6] = item.Commodity;
                row[7] = item.QTY;
                row[8] = item.AGW;
                row[9] = item.ServiceMode;
                row[10] = item.PaymentMode;

                row[11] = item.Gateway;
                row[12] = item.Destination;
                row[13] = item.Batch;

                row[14] = item.CreatedDate.ToShortDateString() ;

                row[15] = item.Driver;
                row[16] = item.PlateNo;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        public List<int> setWidth()
        {
            List<int> width = new List<int>();

            width.Add(25); //No
            width.Add(60);//AWB
            width.Add(110);//Shipper
            width.Add(110);//Consignee
            width.Add(200);//Address
            width.Add(110);//Com Type
            width.Add(100);//Com
            width.Add(40);//Qty
            width.Add(50);//AGW
            width.Add(150);//Service Mode
            width.Add(150);//Payment Mode

            width.Add(0);  //Gateway
            width.Add(0);  //Destination
            width.Add(0);  //Batch
            width.Add(0);  //Createdate
                           
            width.Add(0);  //Driver
            width.Add(0);  //PlateNO

            return width;
        }

        public List<GatewayTransmitalViewModel> Match(List<GatewayTransmittal> _transmital) {

            List<GatewayTransmitalViewModel> _results = new List<GatewayTransmitalViewModel>();
           
            CommodityBL commodityService = new CommodityBL();
            GatewayTransmittalBL transmitalService = new GatewayTransmittalBL();

            foreach(GatewayTransmittal transmital in _transmital)
            {
                ShipmentBL shipmentService = new ShipmentBL();

                GatewayTransmitalViewModel model = new GatewayTransmitalViewModel();
                //List<Shipment> ship = shipmentService.GetAll().Where(x => x.AirwayBillNo == transmital.AirwayBillNo).ToList();

                string _airwaybill = transmitalService.GetAll().Find(x => x.Cargo == transmital.Cargo).AirwayBillNo;
                GatewayTransmitalViewModel isExist = _results.Find(x => x.AirwayBillNo == _airwaybill);

                if (isExist != null) {
                    isExist.QTY++;
                    isExist.AGW += Convert.ToDecimal(shipmentService.GetAll().Find(x => x.AirwayBillNo == transmital.AirwayBillNo).Weight);
                }
                else
                {
                    model.AirwayBillNo = transmital.AirwayBillNo;
                    model.Shipper = shipmentService.GetAll().Find(x => x.AirwayBillNo == transmital.AirwayBillNo).Shipper.FullName;
                    model.Consignee = shipmentService.GetAll().Find(x => x.AirwayBillNo == transmital.AirwayBillNo).Consignee.FullName;
                    model.Address = shipmentService.GetAll().Find(x => x.AirwayBillNo == transmital.AirwayBillNo).Consignee.Address1;
                    model.CommodityType = transmital.CommodityType.CommodityTypeName;
                    model.Commodity = shipmentService.GetAll().Find(x => x.AirwayBillNo == transmital.AirwayBillNo).Commodity.CommodityName;
                    model.QTY++;
                    model.AGW = shipmentService.GetAll().Find(x => x.AirwayBillNo == transmital.AirwayBillNo).Weight;
                    model.ServiceMode = shipmentService.GetAll().Find(x => x.AirwayBillNo == transmital.AirwayBillNo).ServiceMode.ServiceModeName;
                    model.PaymentMode = shipmentService.GetAll().Find(x => x.AirwayBillNo == transmital.AirwayBillNo).PaymentMode.PaymentModeName;

                    model.Gateway = transmital.Gateway;
                    model.Destination = transmital.BranchCorpOffice.BranchCorpOfficeName;
                    model.Batch = transmital.Batch.BatchName;
                    model.CreatedDate = transmital.CreatedDate;
                    model.PlateNo = transmital.PlateNo;
                    _results.Add(model);
                }

                

            }
            return _results;
        }
    }
}

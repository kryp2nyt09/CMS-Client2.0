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
    class GatewayOutboundReport
    {
        public DataTable getData(DateTime date)
        {

            GatewayOutboundBL gatewayOutboundBl = new GatewayOutboundBL();
            GatewayInboundBL gatewayInboundBl = new GatewayInboundBL();

            List<GatewayOutbound> Outboundlist = gatewayOutboundBl.GetAll().Where(x => x.RecordStatus == 1 && x.CreatedDate.ToShortDateString() == date.ToShortDateString()).ToList();
            List<GatewayInbound> Inboundlist = gatewayInboundBl.GetAll().ToList();

            List<GatewayOutboundViewModel> modelList = Match(Inboundlist, Outboundlist);

            //modelList.GroupBy(x => x.AirwayBillNo).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Gateway", typeof(string)));
            dt.Columns.Add(new DataColumn("Driver", typeof(string)));
            dt.Columns.Add(new DataColumn("Plate #", typeof(string)));
            dt.Columns.Add(new DataColumn("Batch", typeof(string)));

            dt.Columns.Add(new DataColumn("AWB", typeof(string)));

            dt.Columns.Add(new DataColumn("Recieved(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Discrepancy(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Total Qty", typeof(string)));

            dt.Columns.Add(new DataColumn("CreatedDate", typeof(string)));
            dt.Columns.Add(new DataColumn("Branch", typeof(string)));

            dt.Columns.Add(new DataColumn("ScannedBy", typeof(string)));

            dt.BeginLoadData();

            int ctr = 1;
            foreach (GatewayOutboundViewModel item in modelList)
            {
                DataRow row = dt.NewRow();
                row[0] = (ctr++).ToString();
                row[1] = item.Gateway.ToString();
                row[2] = item.Driver.ToString();
                row[3] = item.PlateNo.ToString();
                row[4] = item.Batch;
                row[5] = item.AirwayBillNo.ToString();
                row[6] = item.TotalRecieved.ToString();
                row[7] = item.TotalDiscrepency.ToString();
                row[8] = item.Total.ToString();
                row[9] = item.CreatedDate.ToShortDateString();
                row[10] = item.Branch;
                row[11] = item.ScannedBy;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        public DataTable getGODatabyFilter(DateTime date, Guid? bcoid, string driver, string gateway, Guid? batchid, Guid? commodityTypeId, string mawb, int num)
        {

            GatewayOutboundBL gatewayOutboundBl = new GatewayOutboundBL();
            GatewayInboundBL gatewayInboundBl = new GatewayInboundBL();
            CommodityTypeBL comtypeService = new CommodityTypeBL();


            List<GatewayInbound> Inboundlist = gatewayInboundBl.GetAll().ToList();
            List<GatewayOutbound> Outboundlist = new List<GatewayOutbound>();
            List<GatewayOutboundViewModel> modelList = new List<GatewayOutboundViewModel>();
            if (num == 0)
            {
                Outboundlist = gatewayOutboundBl.GetAll().Where
                    (x => x.RecordStatus == 1 
                    && x.CreatedDate.ToShortDateString() == date.ToShortDateString() 
                    && x.MasterAirwayBill == mawb).ToList();

            }
            else if(num == 1)
            {
                Outboundlist = gatewayOutboundBl.GetAll().Where
               (x => x.RecordStatus == 1
               && ((x.PackageNumber.Shipment.DestinationCity.BranchCorpOfficeId == bcoid && x.PackageNumber.Shipment.DestinationCity.BranchCorpOfficeId != null) || (x.PackageNumber.Shipment.DestinationCity.BranchCorpOfficeId == x.PackageNumber.Shipment.DestinationCity.BranchCorpOfficeId && x.PackageNumber.Shipment.DestinationCity.BranchCorpOfficeId == null))
               && ((x.Driver == driver && x.Driver != "All") || (x.Driver == x.Driver && x.Driver == "All"))
               && ((x.Gateway == gateway && x.Gateway != "All") || (x.Gateway == x.Gateway && x.Gateway == "All"))
               && ((x.BatchID == batchid && x.BatchID != null) || (x.BatchID == x.BatchID && x.BatchID == null))
               && x.CreatedDate.ToShortDateString() == date.ToShortDateString()
               ).ToList();
            }

            if(commodityTypeId != null)
            {
                string comType = comtypeService.GetAll().Where(x => x.RecordStatus == 1 && x.CommodityTypeId == commodityTypeId).Select(x => x.CommodityTypeName).ToString();
                modelList = Match(Inboundlist, Outboundlist).FindAll(x => x.CommodityTypeName == comType);
            }
            else
            {
                modelList = Match(Inboundlist, Outboundlist);
            }

            

            //modelList.GroupBy(x => x.AirwayBillNo).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Gateway", typeof(string)));
            dt.Columns.Add(new DataColumn("Driver", typeof(string)));
            dt.Columns.Add(new DataColumn("Plate #", typeof(string)));
            dt.Columns.Add(new DataColumn("Batch", typeof(string)));

            dt.Columns.Add(new DataColumn("AWB", typeof(string)));

            dt.Columns.Add(new DataColumn("Recieved(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Discrepancy(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Total Qty", typeof(string)));

            dt.Columns.Add(new DataColumn("CreatedDate", typeof(string)));
            dt.Columns.Add(new DataColumn("Branch", typeof(string)));

            dt.Columns.Add(new DataColumn("ScannedBy", typeof(string)));

            dt.BeginLoadData();

            int ctr = 1;
            foreach (GatewayOutboundViewModel item in modelList)
            {
                DataRow row = dt.NewRow();
                row[0] = (ctr++).ToString();
                row[1] = item.Gateway.ToString();
                row[2] = item.Driver.ToString();
                row[3] = item.PlateNo.ToString();
                row[4] = item.Batch;
                row[5] = item.AirwayBillNo.ToString();
                row[6] = item.TotalRecieved.ToString();
                row[7] = item.TotalDiscrepency.ToString();
                row[8] = item.Total.ToString();
                row[9] = item.CreatedDate.ToShortDateString();
                row[10] = item.Branch;
                row[11] = item.ScannedBy;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }




        public List<int> setWidth()
        {
            List<int> width = new List<int>();
            width.Add(25);
            width.Add(220);
            width.Add(150);
            width.Add(110);
            width.Add(110);
            width.Add(150);
            width.Add(120);

            width.Add(120);
            width.Add(120);
            width.Add(0);
            width.Add(0);
            width.Add(120);
            return width;
        }

        public List<GatewayOutboundViewModel> Match(List<GatewayInbound> _inbound , List<GatewayOutbound> _outbound) {

            PackageNumberBL _packageNumberService = new PackageNumberBL();
            List<GatewayOutboundViewModel> _results = new List<GatewayOutboundViewModel>();

            foreach (GatewayOutbound outbound in _outbound)
            {

                GatewayOutboundViewModel model = new GatewayOutboundViewModel();
                string _airwaybill = "";
                try {
                    _airwaybill = _packageNumberService.GetAll().Find(x => x.PackageNo == outbound.Cargo).Shipment.AirwayBillNo;
                }
                catch (Exception) { continue;  }
                GatewayOutboundViewModel isExist = _results.Find(x => x.AirwayBillNo == _airwaybill);
                if (_inbound.Exists(x => x.Cargo == outbound.Cargo))
                {
                    if (isExist != null)
                    {
                        isExist.TotalRecieved++;
                        model.Total += model.TotalRecieved;
                        //_results.Add(isExist);
                    }
                    else
                    {
                        model.AirwayBillNo = _airwaybill;
                        model.Gateway = outbound.Gateway;
                        model.Driver = outbound.Driver;
                        model.PlateNo = outbound.PlateNo;
                        model.Batch = outbound.Batch.BatchName;
                        model.TotalRecieved++;
                        model.Total += model.TotalRecieved;
                        model.Branch = outbound.BranchCorpOffice.BranchCorpOfficeName;
                        model.ScannedBy = AppUser.User.Employee.FullName;
                        model.CommodityTypeName = _inbound.Where(x => x.Cargo == outbound.Cargo).Select(x => x.CommodityType.CommodityTypeName).ToString();
                        _results.Add(model);

                    }
                }
                else
                {
                    if (isExist != null)
                    {
                        isExist.TotalDiscrepency++;
                        model.Total += model.TotalDiscrepency;
                        //_results.Add(isExist);
                    }
                    else
                    {
                        model.AirwayBillNo = _airwaybill;
                        model.Gateway = outbound.Gateway;
                        model.Driver = outbound.Driver;
                        model.PlateNo = outbound.PlateNo;
                        model.Batch = outbound.Batch.BatchName;
                        model.TotalDiscrepency++;
                        model.Total += model.TotalDiscrepency;
                        model.Branch = outbound.BranchCorpOffice.BranchCorpOfficeName;
                        model.ScannedBy = AppUser.User.Employee.FullName;
                        model.CommodityTypeName = _inbound.Where(x => x.Cargo == outbound.Cargo).Select(x => x.CommodityType.CommodityTypeName).ToString();
                        _results.Add(model);

                    }
                }

            }


            return _results;
        }
    }
}

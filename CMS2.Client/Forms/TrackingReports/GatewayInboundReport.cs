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
    public class GatewayInboundReport
    {
        public DataTable getData(DateTime date)
        {
            GatewayInboundBL gatewayInboundBl = new GatewayInboundBL();
            List<GatewayInbound> list = list = gatewayInboundBl.GetAll().Where(x => x.BranchCorpOffice.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.RecordStatus == 1 && x.CreatedDate.ToShortDateString() == date.ToShortDateString()).ToList();

            List<GatewayInboundViewModel> modelList = Match(list);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Gateway", typeof(string)));
            dt.Columns.Add(new DataColumn("Origin", typeof(string)));
            dt.Columns.Add(new DataColumn("Pieces", typeof(string)));
            dt.Columns.Add(new DataColumn("MAWB", typeof(string)));
            dt.Columns.Add(new DataColumn("Flight #", typeof(string)));
            dt.Columns.Add(new DataColumn("Commodity Type", typeof(string)));
            dt.Columns.Add(new DataColumn("AWB", typeof(string)));
            dt.Columns.Add(new DataColumn("CreatedDate", typeof(string)));

            dt.Columns.Add(new DataColumn("ScannedBy", typeof(string)));
            dt.BeginLoadData();
            int ctr = 1;
            foreach (GatewayInboundViewModel item in modelList)
            {
                DataRow row = dt.NewRow();
                row[0] = (ctr++).ToString();
                row[1] = item.Gateway;
                row[2] = item.Origin;
                row[3] = item.Pieces;
                row[4] = item.MAWB;
                row[5] = item.FlightNo;
                row[6] = item.CommodityType;
                row[7] = item.AirwayBillNo;
                row[8] = item.CreatedDate.ToShortDateString();
                row[9] = item.ScannedBy;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        public List<int> setWidth()
        {
            List<int> width = new List<int>();
            width.Add(25);
            width.Add(200);
            width.Add(200);
            width.Add(100);
            width.Add(100);
            width.Add(100);
            width.Add(200);
            width.Add(100);
            width.Add(0);
            width.Add(100);
            return width;
        }

        public List<GatewayInboundViewModel> Match(List<GatewayInbound> _inbound)
        {
            
            PackageNumberBL _packageNumberService = new PackageNumberBL();
            List<GatewayInboundViewModel> _results = new List<GatewayInboundViewModel>();

            foreach (GatewayInbound inbound in _inbound)
            {
                GatewayInboundViewModel model = new GatewayInboundViewModel();
                string _airwaybill = "";
                try {
                    _airwaybill = _packageNumberService.GetAll().Find(x => x.PackageNo == inbound.Cargo).Shipment.AirwayBillNo;
                }
                catch (Exception) { continue; }
                GatewayInboundViewModel isExist = _results.Find(x => x.AirwayBillNo == _airwaybill);
                  if (isExist != null)
                  {
                    isExist.Pieces++;
                  }
                  else
                  {
                    model.AirwayBillNo = _airwaybill;
                    model.Gateway = inbound.Gateway;
                    model.Origin = inbound.BranchCorpOffice.BranchCorpOfficeName;
                    model.Pieces++;
                    model.MAWB = inbound.MasterAirwayBill;
                    model.FlightNo = inbound.FlightNumber;
                    model.CommodityType = inbound.CommodityType.CommodityTypeName;
                    model.CreatedDate = inbound.CreatedDate;
                    model.ScannedBy = AppUser.User.Employee.FullName;
                    _results.Add(model);
                  
                  }
            }


            return _results;
        }
    }
}

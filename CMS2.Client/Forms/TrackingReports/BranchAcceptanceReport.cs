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
    public class BranchAcceptanceReport
    {
       
        public DataTable getBranchAcceptanceData(DateTime date)
        {
            BranchAcceptanceBL branchAcceptanceBl = new BranchAcceptanceBL();               
            CargoTransferBL cargoTransferBl = new CargoTransferBL();
            List<CargoTransfer> cargoTransferList = cargoTransferBl.GetAll().Where(x => x.BranchCorpOffice.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.RecordStatus == 1 && x.CreatedDate.ToShortDateString() == date.ToShortDateString()).ToList();
            List<BranchAcceptance> branchAcceptance = branchAcceptanceBl.GetAll().Where(x => x.BranchCorpOffice.BranchCorpOfficeId == GlobalVars.DeviceBcoId ).ToList();

            List <BranchAcceptanceViewModel> list = Match(branchAcceptance, cargoTransferList);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Area/Branch", typeof(string)));
            dt.Columns.Add(new DataColumn("Driver", typeof(string)));
            dt.Columns.Add(new DataColumn("Checker", typeof(string)));
            dt.Columns.Add(new DataColumn("Plate #", typeof(string))); 
            dt.Columns.Add(new DataColumn("Batch", typeof(string)));
            dt.Columns.Add(new DataColumn("AWB", typeof(string)));
            dt.Columns.Add(new DataColumn("Recieved(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Dicrepency(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Total Qty", typeof(string)));
            dt.Columns.Add(new DataColumn("BCO", typeof(string)));
            dt.Columns.Add(new DataColumn("BSO", typeof(string)));


            dt.BeginLoadData();
            int ctr = 1;
            foreach (BranchAcceptanceViewModel item in list)
            {
                DataRow row = dt.NewRow();
                row[0] = ctr++.ToString();
                row[1] = item.Area.ToString();
                row[2] = item.Driver.ToString();
                row[3] = item.Checker.ToString();
                row[4] = item.PlateNo.ToString();
                row[5] = item.Batch.ToString();
                row[6] = item.AirwayBillNo.ToString();
                row[7] = item.TotalRecieved.ToString();
                row[8] = item.TotalDiscrepency.ToString();
                row[9] = item.Total.ToString();

                row[10] = item.BCO;
                row[11] = item.BSO;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        public List<int> setBranchAcceptanceWidth()
        {
            List<int> width = new List<int>();
            width.Add(25); //No
            width.Add(180); //Area/Branch
            width.Add(150); //Driver
            width.Add(150); //Checker
            width.Add(147); //Plate #
            width.Add(100); //Batch

            width.Add(60); //AWB
            width.Add(95); //Recieved
            width.Add(95); //Dis
            width.Add(90); //Total

            width.Add(0); //BCO
            width.Add(0); //BSO 


            return width;
        }

        public List<BranchAcceptanceViewModel> Match(List<BranchAcceptance> _branchAcceptances, List<CargoTransfer> _cargoTransfers)
        {

            PackageNumberBL _packageNumberService = new PackageNumberBL();
            List<BranchAcceptanceViewModel> _results = new List<BranchAcceptanceViewModel>();

            foreach (CargoTransfer cargoTransfer in _cargoTransfers)
            {
                BranchAcceptanceViewModel model = new BranchAcceptanceViewModel();
                string _airwaybill = _packageNumberService.GetAll().Find(x => x.PackageNo == cargoTransfer.Cargo).Shipment.AirwayBillNo;
                BranchAcceptanceViewModel isExist = _results.Find(x => x.AirwayBillNo == _airwaybill);

                if (_branchAcceptances.Exists(x => x.Cargo == cargoTransfer.Cargo))
                {
                    if (isExist != null)
                    {
                        isExist.TotalRecieved++;
                        isExist.Total += model.TotalRecieved;
                    }
                    else
                    {                        
                        model.AirwayBillNo = _airwaybill;
                        model.Area = cargoTransfer.RevenueUnit.RevenueUnitName;
                        model.Driver = cargoTransfer.Driver;
                        model.Checker = cargoTransfer.Checker;
                        model.PlateNo = cargoTransfer.PlateNo;
                        model.Batch = cargoTransfer.Batch.BatchName;
                        model.TotalRecieved++;
                        model.Total += model.TotalRecieved;
                        model.CreatedBy = cargoTransfer.CreatedDate;

                        model.BCO = cargoTransfer.BranchCorpOffice.BranchCorpOfficeName;
                        model.BSO = cargoTransfer.RevenueUnit.RevenueUnitName;

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
                        model.Area = cargoTransfer.RevenueUnit.RevenueUnitName;
                        model.Driver = cargoTransfer.Driver;
                        model.Checker = cargoTransfer.Checker;
                        model.PlateNo = cargoTransfer.PlateNo;
                        model.Batch = cargoTransfer.Batch.BatchName;
                        model.TotalDiscrepency++;
                        model.Total += model.TotalDiscrepency;
                        model.CreatedBy = cargoTransfer.CreatedDate;

                        model.BCO = cargoTransfer.BranchCorpOffice.BranchCorpOfficeName;
                        model.BSO = cargoTransfer.RevenueUnit.RevenueUnitName;

                        _results.Add(model);

                    }
                }
            }

            return _results;



        }
    }
}

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
            ShipmentBL shipmentService = new ShipmentBL();
            List<Shipment> shipments = shipmentService.FilterActive().Where(x => x.AcceptedBy.AssignedToArea.City.BranchCorpOffice.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.RecordStatus == 1 && x.CreatedDate.ToShortDateString() == date.ToShortDateString()).ToList();
            List<BranchAcceptance> branchAcceptance = branchAcceptanceBl.GetAll().Where(x => x.BranchCorpOffice.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.RecordStatus == 1).ToList();

            List<BranchAcceptanceViewModel> list = Match(branchAcceptance, shipments);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Area/Branch", typeof(string)));
            dt.Columns.Add(new DataColumn("Driver", typeof(string)));
            dt.Columns.Add(new DataColumn("Checker", typeof(string)));
            dt.Columns.Add(new DataColumn("Plate #", typeof(string)));
            dt.Columns.Add(new DataColumn("Batch", typeof(string)));
            dt.Columns.Add(new DataColumn("AWB", typeof(string)));
            dt.Columns.Add(new DataColumn("Recieved(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Discrepancy(Qty)", typeof(string)));
            dt.Columns.Add(new DataColumn("Total Qty", typeof(string)));
            dt.Columns.Add(new DataColumn("BCO", typeof(string)));
            dt.Columns.Add(new DataColumn("BSO", typeof(string)));

            dt.Columns.Add(new DataColumn("ScannedBy", typeof(string)));
            dt.Columns.Add(new DataColumn("Remarks", typeof(string)));
            dt.Columns.Add(new DataColumn("Notes", typeof(string)));
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
                row[12] = item.ScannedBy;
                row[13] = item.Remarks;
                row[14] = item.Notes;
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

            width.Add(110); //Scanned by
            width.Add(0); //Remarks
            width.Add(0); //Notes
            return width;
        }

        public List<BranchAcceptanceViewModel> Match(List<BranchAcceptance> _branchAcceptances, List<Shipment> _shipments)
        {

            PackageNumberBL _packageNumberService = new PackageNumberBL();
            BranchAcceptanceBL _branchAcceptanceService = new BranchAcceptanceBL();
            List<BranchAcceptanceViewModel> _results = new List<BranchAcceptanceViewModel>();
           

            foreach (Shipment shipment in _shipments)
            {
                BranchAcceptanceViewModel model = new BranchAcceptanceViewModel();
                BranchAcceptanceViewModel isAirawayBillExist = _results.Find(x => x.AirwayBillNo == shipment.AirwayBillNo);
                List<PackageNumber> _packageNumbers = _packageNumberService.GetAll().Where(x => x.ShipmentId == shipment.ShipmentId).ToList();

                foreach (PackageNumber packagenumber in _packageNumbers)
                {
                    BranchAcceptance _brachAcceptance = _branchAcceptanceService.GetAll().Where(x => x.Cargo == packagenumber.PackageNo).FirstOrDefault();


                    if (_brachAcceptance != null)
                    {
                        if (isAirawayBillExist != null)
                        {
                            isAirawayBillExist.TotalRecieved++;
                            isAirawayBillExist.Total += model.TotalRecieved;
                        }
                        else
                        {
                            model.AirwayBillNo = shipment.AirwayBillNo;

                            model.Area = "N/A";

                            if (shipment.Booking.AssignedToArea != null)
                            {
                                model.Area = shipment.Booking.AssignedToArea.RevenueUnitName;
                            }
                            model.Driver = _brachAcceptance.Driver;
                            model.Checker = _brachAcceptance.Checker;
                            model.PlateNo = "N/A";
                            model.Batch = _brachAcceptance.Batch.BatchName;
                            model.TotalRecieved++;
                            model.Total += model.TotalRecieved;
                             model.CreatedBy = _brachAcceptance.CreatedDate;

                            model.BCO = _brachAcceptance.BranchCorpOffice.BranchCorpOfficeName;
                            model.BSO = shipment.Booking.AssignedToArea.RevenueUnitName;
                            model.ScannedBy = AppUser.User.Employee.FullName;
                            model.Remarks = shipment.Remarks;
                            model.Notes = _brachAcceptance.Notes;
                            _results.Add(model);

                        }
                    }
                    else
                    {
                        if (isAirawayBillExist != null)
                        {
                            isAirawayBillExist.TotalDiscrepency++;
                            isAirawayBillExist.Total += model.TotalDiscrepency;
                        }
                        else
                        {
                            model.AirwayBillNo = shipment.AirwayBillNo;
                            model.Area = "N/A";
                            if (shipment.Booking.AssignedToArea != null)
                            {
                                model.Area = shipment.Booking.AssignedToArea.RevenueUnitName;
                            }
                            model.Driver = "N/A"; //_brachAcceptance.Driver;
                            model.Checker = "N/A"; //_brachAcceptance.Checker;
                            model.PlateNo = "N/A";
                            model.Batch = "N/A"; //_brachAcceptance.Batch.BatchName;
                            model.TotalDiscrepency++;
                            model.Total += model.TotalDiscrepency;

                            model.BCO = "N/A"; //_brachAcceptance.BranchCorpOffice.BranchCorpOfficeName;
                            model.BSO = "N/A"; //shipment.Booking.AssignedToArea.RevenueUnitName;
                            model.ScannedBy = AppUser.User.Employee.FullName;
                            //model.Remarks = shipment.Remarks;
                            //model.Notes = _brachAcceptance.Notes;
                            _results.Add(model);

                        }
                    }
                }
            }

            return _results;



        }
    }
}

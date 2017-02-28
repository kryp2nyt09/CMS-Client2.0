using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Entities.ReportModel
{
    public class HoldCargoViewModel
    {
        public DateTime Date { get; set; }
        public string AirwayBillNo { get; set; }
        public string Shipper { get; set; }
        public string Consignee { get; set; }
        public string Address { get; set; }
        public string PaymentMode { get; set; }
        public string ServiceMode { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string EndorseBy { get; set; }
        public string ScannedBy { get; set; }
        public string PreparedBy { get; set; }
        public double Aging { get; set; }
        public string Branch { get; set; }
    }
}

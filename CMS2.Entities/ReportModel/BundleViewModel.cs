using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Entities.ReportModel
{
    public class BundleViewModel
    {
        public string AirwayBillNo { get; set; }
        public string Shipper { get; set; }
        public string Consignee { get; set; }
        public string Address { get; set; }
        public string CommodityType { get; set; }
        public string Commodity { get; set; }
        public string Qty { get; set; }
        public string AGW { get; set; }
        public string ServiceMode { get; set; }
        public string PaymendMode { get; set; }
        public string Area { get; set; }
        public string SackNo { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Destination { get; set; }
    }
}

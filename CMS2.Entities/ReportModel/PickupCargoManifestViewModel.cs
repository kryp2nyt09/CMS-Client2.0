using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Entities.ReportModel
{
    public class PickupCargoManifestViewModel
    {
        [DisplayName("AWB")]
        public string AirwayBillNo { get; set; }
        public string Shipper { get; set; }
        [DisplayName("Shipper Address")]
        public string ShipperAddress { get; set; }
        public string Consignee { get; set; }
        [DisplayName("Consignee Address")]
        public string ConsigneeAddress { get; set; }
        public string Commodity { get; set; }
        public int QTY { get; set; }
        public string AGW { get; set; }
        [DisplayName("Service Mode")]
        public string ServiceMode { get; set; }
        [DisplayName("Payment Mode")]
        public string PaymentMode { get; set; }
        public string Area { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Entities.ReportModel
{
    public class GatewayOutboundViewModel
    {       
        public string Gateway { get; set; }
        public string Driver { get; set; }
        public string PlateNo { get; set; }
        public string Batch { get; set; }
        public string AirwayBillNo { get; set; }
        public int TotalRecieved { get; set; }
        public int TotalDiscrepency { get; set; }
        public int Total { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Branch { get; set; }

        public string ScannedBy { get; set; }

    }
}

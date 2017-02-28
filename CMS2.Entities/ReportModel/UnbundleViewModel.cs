using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Entities.ReportModel
{
    public class UnbundleViewModel
    {       
        public string SackNo { get; set; }
        public int TotalPcs { get; set; }
        public int ScannedPcs { get; set; }
        public string Origin { get; set; }
        public decimal Weight { get; set; }
        
        public string AirwayBillNo { get; set; }

        public int TotalDiscrepency { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Branch { get; set; }
    }
}

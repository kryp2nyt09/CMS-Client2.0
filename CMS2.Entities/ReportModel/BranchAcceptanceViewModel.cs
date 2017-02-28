using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Entities.ReportModel
{
    public class BranchAcceptanceViewModel
    {
        [DisplayName("area")]
        public string Area { get; set; }
        public string Driver { get; set; }
        public string Checker { get; set; }
        public string PlateNo { get; set; }
        public string  Batch { get; set; }
        public string AirwayBillNo { get; set; }
        public int TotalRecieved { get; set; }
        public int TotalDiscrepency { get; set; }
        public int Total { get; set; }
        public bool Match { get; set; }
        public DateTime CreatedBy { get; set; }

        public string BCO { get; set; }
        public string BSO { get; set; }

    }

    
}

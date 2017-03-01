using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Entities.ReportModel
{
    public class CargoTransferViewModel
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Driver { get; set; }
        public string Checker { get; set; }
        public int Pieces { get; set; }
        public string PlateNo { get; set; }
        public string Batch { get; set; }
        public string AWB { get; set; }
        public int QTY { get; set; }
        public DateTime CreatedDate { get; set; }

        public string BCO { get; set; }
        public string GATEWAY { get; set; }
        public string SATELLITE { get; set; }
    }
}

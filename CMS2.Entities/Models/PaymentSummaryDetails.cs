using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Entities.Models
{
   public class PaymentSummaryDetails
    {
        public string AwbNo { get; set; }
        public string ClientName { get; set; }
        public string PaymentTypeName { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal taxWithheld { get; set; }
        public string OrNo { get; set; }
        public string PrNo { get; set; }
        public string ValidatedBy { get; set; }

        public string PaymentCode { get; set; }

        public string Status { get; set; }
       
    }
}

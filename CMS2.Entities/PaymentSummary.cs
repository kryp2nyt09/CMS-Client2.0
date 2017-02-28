using CMS2.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Entities
{
    public class PaymentSummary: BaseEntity
    {
        public PaymentSummary()
        {
            PaymentSummaryId = Guid.NewGuid();
        }


        [Key]

        public Guid PaymentSummaryId { get; set; }
        
        [DisplayName("Client")]
        public Guid ClientId { get; set; }
        [DisplayName("Client")]

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        [DisplayName("Payment")]
        public Guid PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }
        [DisplayName("Checked By")]
        public Guid? CheckedBy { get; set; }

        [ForeignKey("CheckedBy")]
        public virtual Employee Check { get; set; }



        [DisplayName("Validated By")]
        public Guid ValidatedBy { get; set; }

        [ForeignKey("ValidatedBy")]
        public virtual Employee Validated { get; set; }

        [DisplayName("Remitted By")]
        public Guid RemittedBy { get; set; }

        [ForeignKey("RemittedBy")]
        public virtual Employee Remitted { get; set; }

        [DisplayName("PaymentSummaryStatus")]
        public Guid PaymentSummaryStatusId { get; set; }

        [ForeignKey("PaymentSummaryStatusId")]
        public virtual PaymentSummaryStatus PaymentSummaryStatus { get; set; }

        public DateTime DateAccepted { get; set; }

        public string Remarks { get; set; }

        public byte[] Signature { get; set; }

        public List<PaymentSummary> modelToEntity(List<PaymentSummaryModel> listPaymentSummary)
        {
            List<PaymentSummary> paymentSummaries = new List<PaymentSummary>();
            foreach (PaymentSummaryModel item in listPaymentSummary)
            {
                PaymentSummary paymentSummary = new PaymentSummary();
                paymentSummary.PaymentSummaryId = item.PaymentSummaryId;
                paymentSummary.ClientId = item.ClientId;
                paymentSummary.PaymentId = item.PaymentId;
                //paymentSummary.CheckedBy = null;
                paymentSummary.CheckedBy = item.CheckedBy;
                paymentSummary.ValidatedBy = item.ValidatedBy;
                paymentSummary.RemittedBy = item.RemittedBy;
                paymentSummary.PaymentSummaryStatusId = item.PaymentSummaryStatusId;
                paymentSummary.DateAccepted = item.DateAccepted;
                paymentSummary.Remarks = item.Remarks;
                paymentSummary.Signature = item.Signature;
                paymentSummary.CreatedDate = item.CreatedDate;
                paymentSummary.CreatedBy = item.CreatedBy;
                paymentSummary.ModifiedBy = item.ModifiedBy;
                paymentSummary.ModifiedDate = item.ModifiedDate;
                paymentSummary.RecordStatus = item.RecordStatus;
                paymentSummaries.Add(paymentSummary);
               
            }
            return paymentSummaries;
        }

        
    }
}

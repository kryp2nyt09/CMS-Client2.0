using System;
using System.ComponentModel.DataAnnotations;

namespace CMS2.Entities
{
    public class PaymentSummaryStatus : BaseEntity
    {
        [Key]
        public Guid PaymentSummaryStatusId { get; set; }

        public string PaymentSummaryStatusName { get; set; }
    }
}

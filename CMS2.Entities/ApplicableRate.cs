using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CMS2.Entities
{
    public class ApplicableRate : BaseEntity
    {
        [Key]
        public Guid ApplicableRateId { get; set; }
        [Required]
        [MaxLength(100)]
        [DisplayName("Applicable Rate")]
        public string ApplicableRateName { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
    }
}

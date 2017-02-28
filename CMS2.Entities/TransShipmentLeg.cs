using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS2.Entities
{
    public class TransShipmentLeg:BaseEntity
    {
        [Key]
        public Guid TransShipmentLegId { get; set; }
        [DisplayName("Leg")]
        public Guid LegId { get; set; }
        [ForeignKey("LegId")]
        public City Leg { get; set; }
        [Required]
        [DefaultValue(1)]
        public int LegOrder { get; set; }
        public Guid TransShipmentRouteId { get; set; }
        public virtual TransShipmentRoute TransShipmentRoute { get; set; }
    }
}

namespace CMS2.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Segregation")]
    public partial class Segregation : BaseEntity
    {
        public Guid SegregationID { get; set; }

        public Guid BranchCorpOfficeID { get; set; }

        public Guid UserID { get; set; }

        [Required]
        [StringLength(20)]
        public string Cargo { get; set; }

        [Required]
        [StringLength(20)]
        public string Driver { get; set; }

        [Required]
        [StringLength(20)]
        public string Checker { get; set; }

        public Guid BatchID { get; set; }

        [Required]
        [StringLength(20)]
        public string PlateNo { get; set; }

        public DateTime SegregationDate { get; set; }

        public Guid RemarkID { get; set; }

        [StringLength(100)]
        public string Notes { get; set; }

        public bool Uploaded { get; set; }

        [ForeignKey("BranchCorpOfficeID")]
        public virtual BranchCorpOffice BranchCorpOffice { get; set; }

        [ForeignKey("BatchID")]
        public virtual Batch Batch { get; set; }

        //[ForeignKey("Cargo")]
        //public virtual PackageNumber PackageNumber { get; set; }

    }
}

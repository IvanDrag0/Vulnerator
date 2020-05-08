namespace Vulnerator.Model.Entity
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ControlApplicabilityAssessment")]
    public partial class ControlApplicabilityAssessment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ControlApplicabilityAssessment()
        { NistControlsCAAs = new ObservableCollection<NistControlsCAA>(); }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CAA_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string CAA_Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NistControlsCAA> NistControlsCAAs { get; set; }
    }
}

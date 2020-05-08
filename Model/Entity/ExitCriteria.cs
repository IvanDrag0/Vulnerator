namespace Vulnerator.Model.Entity
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ExitCriteria")]
    public partial class ExitCriteria
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExitCriteria()
        { SAPs = new ObservableCollection<SAP>(); }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ExitCriteria_ID { get; set; }

        [Column("ExitCriteria")]
        [Required]
        [StringLength(100)]
        public string ExitCriteria1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAP> SAPs { get; set; }
    }
}

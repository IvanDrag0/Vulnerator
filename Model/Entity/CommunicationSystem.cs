namespace Vulnerator.Model.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CommunicationSystem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CommunicationSystem_ID { get; set; }

        [Required]
        [StringLength(5)]
        public string VoiceCommunication { get; set; }

        [Required]
        [StringLength(5)]
        public string SatelliteCommunication { get; set; }

        [Required]
        [StringLength(5)]
        public string TacticalCommunication { get; set; }

        [Required]
        [StringLength(5)]
        public string ISDN_VTC_Systems { get; set; }

        [Required]
        [StringLength(5)]
        public string InterrogatorsTransponders { get; set; }
    }
}

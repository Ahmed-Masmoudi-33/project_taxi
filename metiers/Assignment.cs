using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace metiers
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TaxiId { get; set; }
        [ForeignKey("TaxiId")]

        public virtual Taxi Taxi { get; set; } = null!;
        [Required]

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]

        public virtual User Employee { get; set; } = null!;
        [Required]

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}

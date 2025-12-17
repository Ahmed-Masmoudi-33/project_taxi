using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace metiers
{
    public class Ride
    {

        [Key]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public decimal DistanceKm { get; set; }
        [Required]
        public decimal Amount { get; set; } // TND

        //public string Status { get; set; } = RideStatus.COMPLETED;
        
        [Required]
        public int TaxiId { get; set; }
        [ForeignKey("TaxiId")]
        public virtual Taxi Taxi { get; set; } = null!;
        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]

        public virtual User Employee { get; set; } = null!;
    }

}

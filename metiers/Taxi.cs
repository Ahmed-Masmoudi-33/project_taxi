using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace metiers
{
    public class Taxi
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string PlateNumber { get; set; } = string.Empty;
        [Required]
        public string Governorate { get; set; } = string.Empty; // Tunis, Sfax, etc.

        public int BossId { get; set; }
        [ForeignKey("BossId")]


        public virtual User Boss { get; set; } = null!;

        public ICollection<Ride> Rides { get; set; } = new List<Ride>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }

}

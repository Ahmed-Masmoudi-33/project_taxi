using Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace metiers
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; } = ExpenseType.OTHER;
        [Required]
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }

        public string? Description { get; set; }
        public int TaxiId { get; set; }
        [ForeignKey("TaxiId")]
        public  string PlateNumber { get; set; }
        public virtual Taxi Taxi { get; set; } = null!;
    }

}

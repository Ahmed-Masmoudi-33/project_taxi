using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace metiers
{
    public class Commission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public decimal Percentage { get; set; } = 35; // e.g. 35
        [Required]

        public int BossId { get; set; }
        [ForeignKey("BossId")]
        public virtual User Boss { get; set; } = null!;
    }

}

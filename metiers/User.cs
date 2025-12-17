using Enums;
using System.ComponentModel.DataAnnotations;

namespace metiers
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [StringLength(8)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string CIN { get; set; }
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = UserRoles.EMPLOYEE;

        public ICollection<Taxi> OwnedTaxis { get; set; } = new List<Taxi>();
        public ICollection<Ride> Rides { get; set; } = new List<Ride>();

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }

}

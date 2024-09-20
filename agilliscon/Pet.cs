using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesterConsoleApp.Pets
{
    public class Pet
    {
        [Key]
        public int pId { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime Dob { get; set; }

        [ForeignKey("Person")]
        public int OwnerId { get; set; }

        public PetType type { get; set; }

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - Dob.Year;
                if (Dob.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public enum PetType
        {
            Dog,
            Cat,
            Fish,
            Bird,
            Hamster,
            Other
        }
    }
}

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using TesterConsoleApp.Pets;

namespace TesterConsoleApp.People
{
    public class Person
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        public ObservableCollection<Pet> Pets { get; set; } = new ObservableCollection<Pet>();
    }
}

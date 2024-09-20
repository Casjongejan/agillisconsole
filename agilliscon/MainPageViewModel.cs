using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TesterConsoleApp.People;
using TesterConsoleApp.Pets;
using Microsoft.EntityFrameworkCore;
using Bogus;

namespace TesterConsoleApp
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private AppDbContext _dbContext;

        public ObservableCollection<TesterConsoleApp.People.Person> People { get; set; }

        public MainPageViewModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            People = new ObservableCollection<TesterConsoleApp.People.Person>(_dbContext.Persons.Include(p => p.Pets).ToList());
        }
        // Method to generate sample data
        public void GenerateSampleData(int numberOfPeople, int numberOfPets)
        {
            var faker = new Faker();

            // Generate fake people
            var peopleFaker = new Faker<TesterConsoleApp.People.Person>()
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.Surname, f => f.Name.LastName());

            var petFaker = new Faker<Pet>()
                .RuleFor(p => p.Name, f => f.Name.FirstName())
                .RuleFor(p => p.Dob, f => f.Date.Past(10))
                .RuleFor(p => p.type, f => f.PickRandom<Pet.PetType>());

            // Add sample people
            for (int i = 0; i < numberOfPeople; i++)
            {
                var person = peopleFaker.Generate();
                People.Add(person);

                // Add sample pets to the person
                for (int j = 0; j < numberOfPets; j++)
                {
                    var pet = petFaker.Generate();
                    person.Pets.Add(pet);
                }
            }

            // Save to the database
            SaveData();
        }
        private void SaveData()
        {
            foreach (var person in People)
            {
                if (person.ID == 0) // Only add new persons who are not saved yet
                {
                    _dbContext.Persons.Add(person);
                }
            }

            _dbContext.SaveChanges();
        }
        public void AddPerson()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(Surname))
                return;

            var newPerson = new TesterConsoleApp.People.Person
            {
                FirstName = FirstName,
                Surname = Surname,
                Pets = new ObservableCollection<Pet>()
            };

            People.Add(newPerson);
            _dbContext.Persons.Add(newPerson);
            _dbContext.SaveChanges();
        }

        public void AddPet()
        {
            var owner = People.FirstOrDefault(p => p.ID == OwnerIdInput);
            if (owner == null)
                return;

            var newPet = new Pet
            {
                Name = PetName,
                Dob = PetDob,
                OwnerId = OwnerIdInput,
                type = SelectedPetType
            };

            owner.Pets.Add(newPet);
            _dbContext.Pets.Add(newPet);
            _dbContext.SaveChanges();
        }

        public void UpdatePerson(TesterConsoleApp.People.Person person)
        {
            _dbContext.Persons.Update(person);
            _dbContext.SaveChanges();
        }

        public void UpdatePet(Pet pet)
        {
            _dbContext.Pets.Update(pet);
            _dbContext.SaveChanges();
        }

        public void DeletePerson(int personId)
        {
            var person = People.FirstOrDefault(p => p.ID == personId);
            if (person == null) return;

            People.Remove(person);
            _dbContext.Persons.Remove(person);
            _dbContext.SaveChanges();
        }

        public void DeletePet(int petId)
        {
            var pet = _dbContext.Pets.FirstOrDefault(p => p.pId == petId);
            if (pet == null) return;

            var owner = People.FirstOrDefault(p => p.ID == pet.OwnerId);
            owner?.Pets.Remove(pet);

            _dbContext.Pets.Remove(pet);
            _dbContext.SaveChanges();
        }

        public Pet GetPetById(int petId)
        {
            return _dbContext.Pets.FirstOrDefault(p => p.pId == petId);
        }

        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string PetName { get; set; }
        public Pet.PetType SelectedPetType { get; set; }
        public int OwnerIdInput { get; set; }
        public DateTime PetDob { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

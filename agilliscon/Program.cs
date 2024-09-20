using System; // Provides basic system functions like input/output.
using System.Collections.ObjectModel; // Provides the ObservableCollection class, used to store collections of data.
using TesterConsoleApp.Pets; // Imports the Pets namespace containing classes related to pets.
using TesterConsoleApp.People; // Imports the People namespace containing classes related to people.
using Microsoft.EntityFrameworkCore; // Provides classes and methods for Entity Framework Core, which is used to interact with the database.

namespace TesterConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the database context (AppDbContext) and ensure the database is created.
            var dbContext = new AppDbContext();
            dbContext.Database.EnsureCreated();

            // Initialize the ViewModel (MainPageViewModel) which handles data and business logic.
            var viewModel = new MainPageViewModel(dbContext);

            // Infinite loop for the console menu until the user exits.
            while (true)
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Add Person");
                Console.WriteLine("2. Add Pet");
                Console.WriteLine("3. List People and their Pets");
                Console.WriteLine("4. Edit Person");
                Console.WriteLine("5. Edit Pet");
                Console.WriteLine("6. Delete Person");
                Console.WriteLine("7. Delete Pet");
                Console.WriteLine("8. Generate Sample Data");
                Console.WriteLine("9. Exit");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddPerson(viewModel);
                        break;
                    case "2":
                        AddPet(viewModel);
                        break;
                    case "3":
                        ListPeople(viewModel);
                        break;
                    case "4":
                        EditPerson(viewModel);
                        break;
                    case "5":
                        EditPet(viewModel);
                        break;
                    case "6":
                        DeletePerson(viewModel);
                        break;
                    case "7":
                        DeletePet(viewModel);
                        break;
                    case "8":
                        GenerateSampleData(viewModel); // New option for generating sample data
                        break;
                    case "9":
                        return; // Exit the program
                }
            }
        }

        // Function to add a new person.
        private static void AddPerson(MainPageViewModel viewModel)
        {
            Console.Write("Enter first name: "); // Prompt for first name.
            viewModel.FirstName = Console.ReadLine(); // Set the first name in the ViewModel.

            Console.Write("Enter surname: "); // Prompt for surname.
            viewModel.Surname = Console.ReadLine(); // Set the surname in the ViewModel.

            viewModel.AddPerson(); // Call the ViewModel's AddPerson method to add the person.
            Console.WriteLine("Person added."); // Confirm person has been added.
        }

        // Function to add a new pet to a person.
        private static void AddPet(MainPageViewModel viewModel)
        {
            Console.Write("Enter owner ID: "); // Prompt for the owner’s (person's) ID.
            viewModel.OwnerIdInput = int.Parse(Console.ReadLine()); // Set the owner ID in the ViewModel.

            Console.Write("Enter pet name: "); // Prompt for the pet's name.
            viewModel.PetName = Console.ReadLine(); // Set the pet name in the ViewModel.

            // Display pet type options for selection.
            Console.WriteLine("Select pet type:");
            var petTypes = Enum.GetValues(typeof(Pet.PetType)); // Get all available pet types.
            for (int i = 0; i < petTypes.Length; i++)
            {
                Console.WriteLine($"{i}. {petTypes.GetValue(i)}"); // Print each pet type option.
            }
            viewModel.SelectedPetType = (Pet.PetType)int.Parse(Console.ReadLine()); // Set the selected pet type.

            // Prompt for the pet's date of birth.
            Console.Write("Enter pet date of birth (yyyy-mm-dd): ");
            viewModel.PetDob = DateTime.Parse(Console.ReadLine()); // Set the pet date of birth.

            viewModel.AddPet(); // Call the ViewModel's AddPet method to add the pet.
            Console.WriteLine("Pet added."); // Confirm the pet has been added.
        }

        // Function to list all people and their pets.
        private static void ListPeople(MainPageViewModel viewModel)
        {
            // Iterate through each person in the ViewModel's People collection.
            foreach (var person in viewModel.People)
            {
                // Display the person's ID, first name, and surname.
                Console.WriteLine($"Person ID: {person.ID}, Name: {person.FirstName} {person.Surname}");

                // Iterate through the person's pets and display their details.
                foreach (var pet in person.Pets)
                {
                    Console.WriteLine($"    Pet: {pet.Name}, Type: {pet.type}, Age: {pet.Age}");
                }
            }
        }

        // Function to edit an existing person's details.
        private static void EditPerson(MainPageViewModel viewModel)
        {
            Console.Write("Enter person ID to edit: "); // Prompt for the person’s ID.
            int personId = int.Parse(Console.ReadLine()); // Parse the ID.

            var person = viewModel.People.FirstOrDefault(p => p.ID == personId); // Find the person by ID.
            if (person == null)
            {
                Console.WriteLine("Person not found."); // Handle case where the person ID doesn't exist.
                return;
            }

            // Prompt for the new first name and update it if entered.
            Console.Write($"Enter new first name (current: {person.FirstName}): ");
            var newFirstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newFirstName)) person.FirstName = newFirstName;

            // Prompt for the new surname and update it if entered.
            Console.Write($"Enter new surname (current: {person.Surname}): ");
            var newSurname = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newSurname)) person.Surname = newSurname;

            viewModel.UpdatePerson(person); // Update the person's details in the database.
            Console.WriteLine("Person updated."); // Confirm the person has been updated.
        }

        // Function to edit an existing pet's details.
        private static void EditPet(MainPageViewModel viewModel)
        {
            Console.Write("Enter pet ID to edit: "); // Prompt for the pet's ID.
            int petId = int.Parse(Console.ReadLine()); // Parse the ID.

            var pet = viewModel.GetPetById(petId); // Find the pet by ID.
            if (pet == null)
            {
                Console.WriteLine("Pet not found."); // Handle case where the pet ID doesn't exist.
                return;
            }

            // Prompt for the new pet name and update it if entered.
            Console.Write($"Enter new pet name (current: {pet.Name}): ");
            var newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName)) pet.Name = newName;

            // Display the current pet type and allow the user to select a new type.
            Console.WriteLine($"Select new pet type (current: {pet.type}):");
            var petTypes = Enum.GetValues(typeof(Pet.PetType)); // Get all available pet types.
            for (int i = 0; i < petTypes.Length; i++)
            {
                Console.WriteLine($"{i}. {petTypes.GetValue(i)}"); // Print each pet type option.
            }
            pet.type = (Pet.PetType)int.Parse(Console.ReadLine()); // Update the pet type.

            // Prompt for the new pet date of birth and update it.
            Console.Write($"Enter new pet date of birth (current: {pet.Dob:yyyy-MM-dd}): ");
            var newDob = DateTime.Parse(Console.ReadLine());
            pet.Dob = newDob;

            viewModel.UpdatePet(pet); // Update the pet's details in the database.
            Console.WriteLine("Pet updated."); // Confirm the pet has been updated.
        }

        // Function to delete an existing person by ID.
        private static void DeletePerson(MainPageViewModel viewModel)
        {
            Console.Write("Enter person ID to delete: "); // Prompt for the person’s ID.
            int personId = int.Parse(Console.ReadLine()); // Parse the ID.

            viewModel.DeletePerson(personId); // Call the ViewModel's DeletePerson method to remove the person.
            Console.WriteLine("Person deleted."); // Confirm the person has been deleted.
        }

        // Function to delete an existing pet by ID.
        private static void DeletePet(MainPageViewModel viewModel)
        {
            Console.Write("Enter pet ID to delete: "); // Prompt for the pet’s ID.
            int petId = int.Parse(Console.ReadLine()); // Parse the ID.

            viewModel.DeletePet(petId); // Call the ViewModel's DeletePet method to remove the pet.
            Console.WriteLine("Pet deleted."); // Confirm the pet has been deleted.
        }
        private static void GenerateSampleData(MainPageViewModel viewModel)
        {
            Console.Write("Enter number of people to generate: ");
            int numberOfPeople = int.Parse(Console.ReadLine());

            Console.Write("Enter number of pets per person to generate: ");
            int numberOfPets = int.Parse(Console.ReadLine());

            viewModel.GenerateSampleData(numberOfPeople, numberOfPets); // Call the ViewModel method to generate data
            Console.WriteLine("Sample data generated.");
        }
    }
}

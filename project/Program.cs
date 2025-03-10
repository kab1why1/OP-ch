using System;
using System.Collections.Generic;
using System.Linq;

namespace CHAROLIS
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }
        protected BaseEntity(int id)
        {
            Id = id;
        }
    }

    public interface IRepository<T> where T : BaseEntity
    {
        void Create(T item);
        T Read(int id);
        IEnumerable<T> ReadAll();
        void Update(T item);
        void Delete(int id);
        IEnumerable<T> GetAllSortedById();
    }

    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected List<T> items = new List<T>();

        public virtual void Create(T item)
        {
            if (items.Any(x => x.Id == item.Id))
            {
                throw new ArgumentException($"Item with Id {item.Id} already exists.");
            }
            items.Add(item);
        }

        public virtual T Read(int id)
        {
            return items.FirstOrDefault(x => x.Id == id);
        }

        public virtual IEnumerable<T> ReadAll()
        {
            return items;
        }

        public virtual void Update(T item)
        {
            var index = items.FindIndex(x => x.Id == item.Id);
            if (index < 0)
            {
                throw new ArgumentException($"Item with Id {item.Id} not found.");
            }
            items[index] = item;
        }

        public virtual void Delete(int id)
        {
            var item = Read(id);
            if (item != null)
            {
                items.Remove(item);
            }
        }

        public virtual IEnumerable<T> GetAllSortedById()
        {
            return items.OrderBy(x => x.Id);
        }
    }

    public class UserRepository : Repository<User>
    {
        public IEnumerable<User> GetUsersByEmail(string email)
        {
            return items.Where(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class ProductRepository : Repository<Product>
    {
        public IEnumerable<Product> GetAvailableProducts()
        {
            return items.Where(p => p.IsAvailable);
        }
    }

    public abstract class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public User(int id, string name, string email, string phone, string address)
            : base(id)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
        }

        public virtual void Desc()
        {
            Console.WriteLine($"Name: {Name}, Email: {Email}, Phone: {Phone}, Address: {Address}");
        }
    }

    public class Administrator : User
    {
        public Administrator(int id, string name, string email, string phone, string address)
            : base(id, name, email, phone, address) { }

        public override void Desc()
        {
            base.Desc();
            Console.WriteLine($"{Name} is controlling the shop.");
        }
    }

    public class RegisteredUser : User
    {
        public RegisteredUser(int id, string name, string email, string phone, string address)
            : base(id, name, email, phone, address) { }

        public override void Desc()
        {
            base.Desc();
            Console.WriteLine($"Welcome back, {Name}!");
        }
    }

    public class Guest : User
    {
        public Guest(int id, string name)
            : base(id, name, "N/A", "N/A", "N/A") { }

        public override void Desc()
        {
            base.Desc();
            Console.WriteLine($"Welcome, {Name}. Register to gain full access.");
        }
    }

    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool IsAvailable { get; set; }

        public Product(int id, string name, string description, double price, bool isAvailable)
            : base(id)
        {
            Name = name;
            Description = description;
            Price = price;
            IsAvailable = isAvailable;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Name: {Name}\nDescription: {Description}\nPrice: {Price:C}\nAvailability: {(IsAvailable ? "Available" : "Not Available")}");
        }

        public bool CheckAvailability() => IsAvailable;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Створення репозиторіїв
            var userRepo = new UserRepository();
            var productRepo = new ProductRepository();

            // Додавання користувачів
            userRepo.Create(new Administrator(1, "Laura", "alice@charolis.com", "123456789", "St. Magic, 1"));
            userRepo.Create(new RegisteredUser(2, "Tony", "bob@charolis.com", "987654321", "Sq. Some, 2"));
            userRepo.Create(new Guest(3, "Guest"));

            // Відображення всіх користувачів (відсортовано за Id)
            Console.WriteLine("All Users:");
            foreach (var user in userRepo.GetAllSortedById())
            {
                user.Desc();
                Console.WriteLine();
            }

            // Знайти конкретного користувача за Id
            var specificUser = userRepo.Read(2);
            Console.WriteLine("User with Id 2:");
            specificUser?.Desc();
            Console.WriteLine();

            // Додавання товарів
            productRepo.Create(new Product(1, "Magic Wand", "Wand full of magic", 70.00, true));
            productRepo.Create(new Product(2, "Magic Scent", "Magical aroma", 49.99, false));

            // Відображення всіх товарів
            Console.WriteLine("All Products:");
            foreach (var product in productRepo.GetAllSortedById())
            {
                product.DisplayInfo();
                Console.WriteLine();
            }

            // Фільтрація товарів: тільки доступні
            Console.WriteLine("Available Products:");
            foreach (var product in productRepo.GetAvailableProducts())
            {
                product.DisplayInfo();
                Console.WriteLine();
            }

            // Оновлення товару
            var productToUpdate = productRepo.Read(2);
            if (productToUpdate != null)
            {
                // Наприклад, змінюємо доступність
                productToUpdate.IsAvailable = true;
                productRepo.Update(productToUpdate);
            }

            Console.WriteLine("After updating product availability:");
            foreach (var product in productRepo.GetAllSortedById())
            {
                product.DisplayInfo();
                Console.WriteLine();
            }

            // Видалення користувача
            userRepo.Delete(3);
            Console.WriteLine("After deleting user with Id 3:");
            foreach (var user in userRepo.GetAllSortedById())
            {
                user.Desc();
                Console.WriteLine();
            }
        }
    }
}

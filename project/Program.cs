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

        public virtual void desc()
        {
            Console.WriteLine($"name: {Name}, email: {Email}, phone: {Phone}, address: {Address}");
        }
    }

    public class Administrator : User
    {
        public Administrator(int id, string name, string email, string phone, string address)
            : base(id, name, email, phone, address) { }

        public override void desc()
        {
            Console.WriteLine($"name: {Name}, email: {Email}, phone: {Phone}, address: {Address}");
            Console.WriteLine($"{Name} is controll shop");

        }
    }

    public class RegisteredUser : User
    {
        public RegisteredUser(int id, string name, string email, string phone, string address)
            : base(id, name, email, phone, address) { }

        public override void desc()
        {
            Console.WriteLine($"name: {Name}, email: {Email}, phone: {Phone}, address: {Address}");
            Console.WriteLine($"welcome back, {Name}");
        }
    }

    public class Guest : User
    {
        public Guest(int id, string name)
            : base(id, name, "N/A", "N/A", "N/A") { }

        public override void desc()
        {
            Console.WriteLine($"name: {Name}, email: {Email}, phone: {Phone}, address: {Address}");
            Console.WriteLine($"welcome, {Name}. Register to gain full access.");
        }
    }

    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool IsAvailable { get; set; } // Чи є товар в наявності

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
            Console.WriteLine($"name: {Name}\ndesc: {Description}\ntext: {Price:C}\nprice: {(IsAvailable ? "availible" : "not availible")}");
        }

        public bool CheckAvailability()
        {
            return IsAvailable;
        }
    }

    public class OrderItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    public interface INotifiable
    {
        void SendNotification(string message);
    }


    public abstract class Order : BaseEntity, INotifiable
    {
        public List<OrderItem> Items { get; set; }
        public string Status { get; protected set; }

        public Order(int id) : base(id)
        {
            Items = new List<OrderItem>();
            Status = "new";
        }

        public double TotalPrice => Items.Sum(item => item.Product.Price * item.Quantity);
        public int TotalQuantity => Items.Sum(item => item.Quantity);

        public void AddItem(Product product, int quantity)
        {
            if (!product.CheckAvailability())
            {
                Console.WriteLine($"product \"{product.Name}\" is not availible.");
                return;
            }
            Items.Add(new OrderItem(product, quantity));
        }

        public virtual void DisplayOrder()
        {
            Console.WriteLine("request details:");
            foreach (var item in Items)
            {
                Console.WriteLine($"product: {item.Product.Name}, amount: {item.Quantity}, sum: {(item.Product.Price * item.Quantity):C}");
            }
            Console.WriteLine($"general amount: {TotalQuantity}");
            Console.WriteLine($"general price: {TotalPrice:C}");
            Console.WriteLine($"starus of request: {Status}");
        }

        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
            SendNotification($"status of your request has been updated: {Status}");
        }

        public void SendNotification(string message)
        {
            Console.WriteLine($"[email message]: {message}");
        }

    }

    public class StandardOrder : Order
    {
        public StandardOrder(int id) : base(id) { }

    }


    public class Program
    {
        public static void Main(string[] args)
        {
            Product p1 = new Product(1, "magic wand",
                "wand, full of magic", 70.00, true);
            Product p2 = new Product(2, "magic scent",
                "magic desc", 49.99, false);

            p1.DisplayInfo();
            Console.WriteLine();

            Administrator admin = new Administrator(1, "Laura", "alice@charolis.com", "123456789", "st. magic, 1");
            RegisteredUser regUser = new RegisteredUser(2, "Tony", "bob@charolis.com", "987654321", "sq. some, 2");
            Guest guest = new Guest(3, "guest");

            User[] users = { admin, regUser, guest };
            foreach (var user in users)
            {
                user.desc();
                Console.WriteLine();
            }

            StandardOrder order = new StandardOrder(101);
            order.AddItem(p1, 2);
            order.AddItem(p2, 1);

            order.DisplayOrder();
            Console.WriteLine();

            order.DisplayOrder();
            Console.WriteLine();

            StandardOrder order2 = new StandardOrder(102);
            order2.AddItem(p1, 1);
            order2.DisplayOrder();
            Console.WriteLine();
        }
    }
}

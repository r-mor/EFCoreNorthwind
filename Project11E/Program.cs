using Microsoft.Data.SqlClient;
using Project11E.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using static System.Console;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics;

namespace Project11E;

internal class Program
{
    static void Main(string[] args)
    {
        WriteLine($"The database provider is: {ProjectConstants.DatabaseProvider}");
        TestDatabaseConnection();
        //ListAllProducts();
        //ListAllSuppliers();
        //CountAllProductsPerCategory();
        QueryingProductsWithLike();
        GetAllOrders();
        ExplicitLoadingCategories();
        ManipulateData("DELETE");
        ListAllProducts();
        WriteLine("Done.");
    }



    private static void ManipulateData(string task)
    {
        int result;
        switch (task)
        {
            case "ADD":
                result = AddProduct(categoryId: 6, supplierId: 10, productName: "Bob's Burgers", price: 500M);
                break;
            case "DELETE":
                result = DeleteProducts(productNameStartsWith: "Bob's");
                break;
            case "UPDATE":
                result = IncreaseProductPrice(productNameStartsWith: "Bob's", amount: 10.25M);
                break;
            default:
                WriteLine($"No task for: {task}");
                return;
        }

        WriteLine($"Executed task {task}. {result} row(s) affected.");
    }

    private static int AddProduct(int categoryId, int supplierId, string productName, decimal price)
    {
        using (Entity.Northwind NorthwindDb = new())
        {
            Product p = new()
            {
                CategoryId = categoryId,
                SupplierId = supplierId,
                Name = productName,
                UnitPrice = price
            };

            NorthwindDb.Add(p);

            int affected = NorthwindDb.SaveChanges();
            return affected;
        }
    }

    private static int IncreaseProductPrice(string productNameStartsWith, decimal amount)
    {
        using (Entity.Northwind NorthwindDb = new())
        {
            Product updateProduct = NorthwindDb.Products.First(p => p.Name.StartsWith(productNameStartsWith));

            updateProduct.UnitPrice += amount;

            int affected = NorthwindDb.SaveChanges();
            return affected;
        }
    }

    private static int DeleteProducts(string productNameStartsWith)
    {
        using (Entity.Northwind NorthwindDb = new())
        {
            IQueryable<Product>? products = NorthwindDb.Products?.Where(p => p.Name.StartsWith(productNameStartsWith));

            if (products is null)
            {
                WriteLine("No products found to delete");
                return 0;
            }
            NorthwindDb.Products?.RemoveRange(products);

            int affected = NorthwindDb.SaveChanges();
            return affected;
        }
    }

    private static void ExplicitLoadingCategories()
    {
        using (Entity.Northwind NorthwindDb = new())
        {
            IQueryable<Category>? categories;

            Write("Enable eager loading? (Y/N): ");
            bool eagerLoading = (ReadKey().Key == ConsoleKey.Y);
            bool explicitLoading = false;
            WriteLine();

            if (eagerLoading)
            {
                categories = NorthwindDb.Categories?.Include(c => c.Products);
            }
            else
            {
                categories = NorthwindDb.Categories;

                Write("Enable explicit loading? (Y/N): ");
                explicitLoading = (ReadKey().Key == ConsoleKey.Y);
                WriteLine();
            }

            if (categories is null)
            {
                WriteLine("No categories found.");
                return;
            }

            foreach (Category c in categories)
            {
                if (explicitLoading)
                {
                    Write($"Explicitly load products for {c.Name}? (Y/N): ");
                    ConsoleKeyInfo key = ReadKey();
                    WriteLine();
                    if (key.Key == ConsoleKey.Y)
                    {
                        CollectionEntry<Category, Product> products =
                            NorthwindDb.Entry(c).Collection(c2 => c2.Products);
                        if (!products.IsLoaded)
                        {
                            products.Load();
                        }
                    }
                }

                WriteLine($"{c.Name} has {c.Products.Count} products.");
            }
        }
    }

    private static void GetAllOrders()
    {
        using (Entity.Northwind NorthwindDb = new())
        {
            IQueryable<Order>? orders = NorthwindDb.Orders?
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product);

            if (orders is null)
            {
                WriteLine("No orders found");
                return;
            }

            foreach (Order o in orders)
            {
                WriteLine($"Details for Order #{o.Id}:");

                foreach (OrderDetail od in o.OrderDetails)
                {
                    Product p = od.Product;
                    WriteLine($"{p.Id,-3}{p.Name,-40} {p.UnitPrice,10:C}");
                }
            }
        }
    }

    private static void QueryingProductsWithLike()
    {
        using (Entity.Northwind NorthwindDb = new())
        {
            ILoggerFactory loggerFactory = NorthwindDb.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new Entity.ConsoleLoggerProvider());

            Write("Enter a part of a product name: ");
            string? input = ReadLine();

            IQueryable<Product>? products = NorthwindDb.Products?
                .Where(p => EF.Functions.Like(p.Name, $"%{input}%"));

            if (products is null)
            {
                WriteLine("No products found");
                return;
            }

            foreach (Product p in products)
            {
                Write($"{p.Name}");
                if (p.UnitPrice is not null)
                {
                    Write($" is {FormatAsMoney((decimal)p.UnitPrice)}");
                }
                WriteLine();
            }

        }
    }

    private static void CountAllProductsPerCategory()
    {
        using (Entity.Northwind NorthwindDb = new())
        {
            ILoggerFactory loggerFactory = NorthwindDb.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new Entity.ConsoleLoggerProvider());

            WriteLine("Here are all suppliers:");

            IQueryable<Category>? categories = NorthwindDb.Categories?
                .TagWith("List all categories and count how many products")
                .Include(s => s.Products);

            if (categories is null)
            {
                WriteLine("No categories found.");
                return;
            }

            WriteLine($"Query String: {categories.ToQueryString()}");

            foreach (Category c in categories)
            {
                WriteLine($"{c.Name} has {c.Products.Count} products.");
            }

        }
    }

    private static void ListAllSuppliers()
    {
        using (Entity.Northwind NorthwindDb = new())
        {
            WriteLine("Here are all suppliers:");

            IQueryable<Supplier>? suppliers = NorthwindDb.Suppliers?
                .Include(s => s.Products);

            if (suppliers is null)
            {
                WriteLine("No products found");
                return;
            }

            foreach (Supplier s in suppliers)
            {
                WriteLine($"{s.ContactName} is in {s.Address}, {s.City}, {s.Country}, {s.PostalCode}. Supples {s.Products.Count} products");
                WriteLine("Products are:");
                foreach (Product p in s.Products)
                {
                    WriteLine($"{p.UnitPrice:C} - {p.Name}");
                }
            }
        }
    }

    private static void ListAllProducts()
    {
        using (Entity.Northwind NorthwindDb = new())
        {
            WriteLine("Here are all products:");

            IQueryable<Product>? products = NorthwindDb.Products;

            if (products is null)
            {
                WriteLine("No products found");
                return;
            }

            foreach (Product p in products)
            {
                Write($"{p.Name}");
                if (p.UnitPrice is not null)
                {
                    Write($" is {FormatAsMoney((decimal)p.UnitPrice)}");
                }
                WriteLine();
            }
        }
    }

    private static string FormatAsMoney(decimal amount)
    {
        return string.Format("{0:C}", amount);
    }

    private static void TestDatabaseConnection()
    {
        if (ProjectConstants.DatabaseProvider == "SqlServer")
        {
            try
            {
                SqlConnection sc = new(ProjectConstants.SqlServerConnectionString);
                sc.Open();
                WriteLine("Connection successful..");
                sc.Close();
            }
            catch (Exception ex)
            {
                WriteLine($"Cannot connect to the database:\n{ex.Message}");
            }
            return;
        }
    }
}
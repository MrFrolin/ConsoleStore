using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Labb2ProgTemplate.CostumerLevels;

namespace Labb2ProgTemplate
{
    public class Shop
    {
        public bool Inlogged;
        private int SelectedIndex { get; set; }

        public string[] CurrentMenu;

        public Dictionary<string, string[]> Menus;
        private List<Customer> AllCustomers { get; } = new();
        private Customer CurrentCustomer { get; set; }
        private List<Product> Products { get; set; }


        public Shop()
        {
            Menus = new Dictionary<string, string[]>
            {
                { "mainMenu", new[] { "Register", "Login" } },
                { "shopMenu", new[] { "Shop", "View cart", "Checkout", "Select currency" } },
                { "currencyMenu", new[] { "SEK", "USD", "DKK" } },
                { "yesOrNo", new[] { "Yes", "No" } },
                { "numbers", new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" } }
            };

            AllCustomers.AddRange(new Customer[]
            {
                new BronzeLevel("Tjatte", "213"),

                new SilverLevel("Knatte", "123"),

                new GoldLevel("Fnatte", "321")
            });

            Products = new List<Product>();
            Product proteinBar = new Product(0, "Protein bar", 25.0);
            Product proteinPowder = new Product(1, "Protein powder", 249.0);
            Product energyDrink = new Product(2, "Energy drink", 29.0);
            Product vitamins = new Product(3, "Vitamins", 149.0);

            Products.Add(proteinBar);
            Products.Add(proteinPowder);
            Products.Add(energyDrink);
            Products.Add(vitamins);

        }


        public void ReadCustomerFile()
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PhilipsStore");
            var path = Path.Combine(directory, "Costumers.txt");

            if (Path.Exists(path))
            {
                using StreamReader sr = new StreamReader(path);

                string userName = null;
                string passWord = null;

                string? line = "";

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("Username:"))
                    {
                        userName = line.Substring("Username:".Length).Trim();
                    }
                    else if (line.StartsWith("Password:"))
                    {
                        passWord = line.Substring("Password:".Length).Trim();
                    }
                    else if (userName != null && passWord != null && AllCustomers.Any(customer => customer.Name == userName) == false)
                    {
                        AllCustomers.Add(new RegularLevel(userName, passWord));
                        userName = null;
                        passWord = null;
                    }
                }
            }
            else
            {
                CreateCustomerFile();
            }
        }
        public void CreateCustomerFile()
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PhilipsStore");
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "Costumers.txt");

            using StreamWriter sw = new StreamWriter(path);
            foreach (var customer in AllCustomers)
            {
                sw.WriteLine(customer.ToString());
            }
        }
        public void MainMenu()
        {
            Navigate(Menus["mainMenu"], $"Welcome to my store!\n" +
                                                        $"Press the arrows to navigate and enter to select");

            if (SelectedIndex == 0)
            {
                Register();
            }
            else if (SelectedIndex == 1)
            {
                Login();
            }
        }
        private void Login()
        {
            Console.Clear();
            Console.WriteLine("Login:");

            if (AllCustomers.Count > 0)
            {
                Console.WriteLine("Username: ");
                string logInUserName = Console.ReadLine();

                if (AllCustomers.Count > 0)
                {

                    CurrentCustomer = null;
                    for (int i = 0; i < AllCustomers.Count; i++)
                    {
                        if (logInUserName == AllCustomers[i].Name)
                        {
                            var correctPassword = false;
                            do
                            {
                                Console.WriteLine("Password: ");
                                var logInPassWord = Console.ReadLine();

                                correctPassword = AllCustomers[i].CheckPassword(logInPassWord);

                            } while (correctPassword == false);

                            CurrentCustomer = AllCustomers[i];
                            Inlogged = true;
                            ShopMenu();
                        }
                    }
                    if (CurrentCustomer == null)
                    {
                        Console.WriteLine("User does not exist");
                        Navigate(Menus["yesOrNo"], "User does not exist.\nDo you wanna register?");
                        if (SelectedIndex == 0)
                        {
                            Register();
                        }
                        else if (SelectedIndex == 1)
                        {
                            MainMenu();
                        }
                    }
                }
                else
                {
                    Console.Clear();
                    Register();
                }
            }
        }
        private void Register()
        {
            Console.Clear();
            Console.WriteLine("Register");
            Console.WriteLine("Username: ");
            string regUserName = Console.ReadLine();
            Console.WriteLine("Password: ");
            string regPassWord = Console.ReadLine();


            if (String.IsNullOrEmpty(regUserName) || regUserName.Length <= 3)
            {
                Console.WriteLine("Username must be at least three characters");
                Console.ReadKey();
                Register();
            }
            else if (String.IsNullOrEmpty(regPassWord) || regPassWord.Length <= 3)
            {
                Console.WriteLine("Password must be at least three characters");
                Console.ReadKey();
                Register();
            }
            else
            {
                var existingUser = false;
                for (int i = 0; i < AllCustomers.Count; i++)
                {

                    if (regUserName != AllCustomers[i].Name)
                    {
                        existingUser = false;

                    }
                    else if (regUserName == AllCustomers[i].Name)
                    {
                        existingUser = true;
                        break;
                    }

                }

                if (!existingUser)
                {
                    Customer customer = new Customer(regUserName, regPassWord);
                    AllCustomers.Add(customer);
                    Console.WriteLine("The account is now registered!");
                    Console.ReadKey();
                    CreateCustomerFile();
                }
                else
                {
                    Console.WriteLine("The user already exists");
                    Console.ReadKey();
                }
            }
            Login();
        }
        private void ShopMenu()
        {
            Navigate(Menus["shopMenu"], $"Welcome {CurrentCustomer.Name}!");
            var continueShopping = true;

            if (SelectedIndex == 0)
            {
                do
                {
                    ProductMenu();
                    Navigate(CurrentMenu, "Press B to go back\n\nShop:");
                    var productOption = SelectedIndex;

                    Navigate(Menus["numbers"], "How many do you want?");

                    for (int i = 0; i <= SelectedIndex; i++)
                    {
                        CurrentCustomer.AddToCart(Products[productOption]);
                    }

                    Navigate(Menus["yesOrNo"], "Continue shopping?");
                    if (SelectedIndex == 1)
                    {
                        continueShopping = false;
                    }
                } while (continueShopping);

                ShopMenu();
            }
            else if (SelectedIndex == 1)
            {
                Console.Clear();
                ViewCart();
                Navigate(CurrentMenu, "Press B to go back\n\nYour Cart\nSelect to delete product");

                if (SelectedIndex == CurrentMenu.Length - 1)
                {
                    Checkout();
                }
                else
                {
                    var productString = CurrentMenu[SelectedIndex];
                    var articleId = Convert.ToInt32(productString[3].ToString());
                    var deleteProduct = new Product(100000, "none", 0);

                    foreach (var product in CurrentCustomer.Cart)
                    {
                        if (product.Id == articleId)
                        {
                            deleteProduct = product;
                        }
                    }

                    Navigate(Menus["numbers"], "How many do you want to remove?");
                    for (int i = 0; i < SelectedIndex + 1; i++)
                    {
                        CurrentCustomer.RemoveFromCart(deleteProduct);
                    }

                }
                ShopMenu();
            }
            else if (SelectedIndex == 2)
            {
                Checkout();
            }
            else if (SelectedIndex == 3)
            {
                Currency();
                ShopMenu();
            }
        }
        private void ViewCart()
        {
            List<string> controlUniqueItems = new List<string>();
            List<string> stringHolder = new List<string>();
            var counter = 0;
            foreach (var product in CurrentCustomer.Cart)
            {
                var pcsOfArticle = CurrentCustomer.Cart.Count(p => p.Name == product.Name);
                var articleTotalCost = pcsOfArticle * product.Price;
                articleTotalCost = Math.Round(articleTotalCost * CurrentCustomer.CurrentCurrency, 2);

                if (!controlUniqueItems.Contains(product.Name))
                {
                    controlUniqueItems.Add(product.Name);
                    stringHolder.Add($"ID:{product.Id}\t{pcsOfArticle} {product.Name}(s) {Math.Round(product.Price * CurrentCustomer.CurrentCurrency, 2)} " +
                                     $"{CurrentCustomer.CurrentCurrencyText}/pcs: {articleTotalCost} {CurrentCustomer.CurrentCurrencyText} in total");
                    counter++;
                }
            }

            var totalCartCost = CurrentCustomer.CartTotal();
            stringHolder.Add($"\nTotal cost: {totalCartCost} {CurrentCustomer.CurrentCurrencyText}\nCheckout?");
            CurrentMenu = stringHolder.ToArray();

        }
        private void Checkout()
        {
            Console.WriteLine(CurrentCustomer);
            Navigate(Menus["yesOrNo"], $"{CurrentCustomer}\n\nWanna checkout?");
            if (CurrentCustomer.Cart.Count > 0 && SelectedIndex == 0)
            {
                var correctPassword = false;
                Console.Clear();
                var discount = CurrentCustomer.CalculateDiscount();
                Console.WriteLine($"You have {CurrentCustomer.Level} level and will receive {(int)CurrentCustomer.Level}% discount");
                Console.WriteLine($"Discount: {Math.Round(discount, 1)} {CurrentCustomer.CurrentCurrencyText}");
                Console.WriteLine("Confirm with password");
                var checkoutPassword = Console.ReadLine();
                correctPassword = CurrentCustomer.CheckPassword(checkoutPassword);

                if (correctPassword)
                {

                    Console.WriteLine("Thanks for your order!");
                    Console.ReadKey();
                    CurrentCustomer.Cart.Clear();
                    ShopMenu();
                }
                else
                {
                    Console.WriteLine("Wrong password");
                    Console.ReadKey();
                    Checkout();
                }
            }
            else if (CurrentCustomer.Cart.Count <= 0 && SelectedIndex == 0)
            {
                Console.WriteLine("No items in the cart");
                Console.ReadLine();
                ShopMenu();
            }
            else
            {
                ShopMenu();
            }
        }
        public void DisplayOptions()
        {
            for (int i = 0; i < CurrentMenu.Length; i++)
            {

                var currentOption = CurrentMenu[i];

                if (i == SelectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }

                if (CurrentMenu == Menus["numbers"])
                {
                    Console.Write(currentOption);
                }
                else
                {
                    Console.WriteLine(currentOption);
                }

            }
            Console.ResetColor();
            if (Inlogged)
            {
                Console.SetCursorPosition(0, CurrentMenu.Length + 15);
                Console.WriteLine("Press X to logout\n");
            }
        }
        public void Navigate(string[] currentMenu, string header)
        {

            SelectedIndex = 0;
            ConsoleKey keyPressed;
            CurrentMenu = currentMenu;

            do
            {
                Console.Clear();
                Console.WriteLine(header + "\n");
                DisplayOptions();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                keyPressed = keyInfo.Key;
                if (currentMenu == Menus["numbers"])
                {
                    if (keyPressed == ConsoleKey.RightArrow)
                    {
                        if (SelectedIndex! < CurrentMenu.Length - 1)
                        {
                            SelectedIndex++;
                        }
                    }
                    else if (keyPressed == ConsoleKey.LeftArrow)
                    {
                        if (SelectedIndex != 0)
                        {
                            SelectedIndex--;
                        }
                    }
                }
                else
                {
                    if (keyPressed == ConsoleKey.DownArrow)
                    {
                        if (SelectedIndex! < CurrentMenu.Length - 1)
                        {
                            SelectedIndex++;
                        }
                    }
                    else if (keyPressed == ConsoleKey.UpArrow)
                    {
                        if (SelectedIndex != 0)
                        {
                            SelectedIndex--;
                        }
                    }
                }
                if (keyPressed == ConsoleKey.X && Inlogged == true)
                {
                    Inlogged = false;
                    CurrentCustomer = null;
                    MainMenu();
                }
                else if (keyPressed == ConsoleKey.B && Inlogged == true)
                {
                    ShopMenu();
                }

            } while (keyPressed != ConsoleKey.Enter);

        }
        public void ProductMenu()
        {
            CurrentMenu = new string[Products.Count];

            for (int i = 0; i < CurrentMenu.Length; i++)
            {
                CurrentMenu[i] = ($"{Products[i].Name} {Math.Round(Products[i].Price * CurrentCustomer.CurrentCurrency, 2)} {CurrentCustomer.CurrentCurrencyText}");
            }

        }
        public void Currency()
        {
            Navigate(Menus["currencyMenu"], "Press B to go back\n\nSelect currency:");
            var currency = "SEK";

            if (SelectedIndex == 0)
            {
                currency = "SEK";
            }
            else if (SelectedIndex == 1)
            {
                currency = "USD";
            }
            else if (SelectedIndex == 2)
            {
                currency = "DKK";
            }
            CurrentCustomer.CurrentCurrencyText = currency;
            CurrentCustomer.CurrentCurrency = CurrentCustomer.Currencies[currency];
        }
    }
}

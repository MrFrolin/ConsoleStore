using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb2ProgTemplate.CostumerLevels;

namespace Labb2ProgTemplate
{
    public class Customer
    {
        public string Name { get; private set; }

        private string Password { get; set; }

        private CostumerLevels.CostumerLevels _level;

        public CostumerLevels.CostumerLevels Level
        {
            get { return _level; }
            set { _level = value; }
        }

        private List<Product> _cart;
        public List<Product> Cart { get { return _cart; } }

        public double CurrentCurrency { get; set; } = 1;

        public string CurrentCurrencyText { get; set; } = "SEK";

        public Dictionary<string, double> Currencies = new()
        {
            { "USD", 0.092 }, { "DKK", 0.65 }, {"SEK", 1 }
        };

        public Customer(string name, string password)
        {
            Name = name;
            Password = password;
            _cart = new List<Product>();
        }

        public bool CheckPassword(string password)
        {

            if (Password == password)
            {
                return true;
            }

            return false;

        }

        public void AddToCart(Product product)
        {
            Cart.Add(product);
        }

        public void RemoveFromCart(Product product)
        {
            Cart.Remove(product);
        }

        public double CartTotal()
        {
            double total = 0;
            foreach (var product in Cart)
            {
                total += product.Price;
            }

            total = Math.Round(total * CurrentCurrency, 2);
            return total;
        }

        public double CalculateDiscount()
        {
            var total = CartTotal();
            var discountPercent = (100 - (double)Level) /100;
            var discount = total * (1-discountPercent);
            return discount;
        }

        public override string ToString()
        {

            List<string> controlUniqueItems = new List<string>();
            var customerDescription = string.Empty;
            customerDescription += $"Username: {Name} \nPassword: {Password}\nCostumerLevel: {Level}\n\nYour cart contains:\n";
            foreach (var product in Cart)
            {
                var countArticle = Cart.Count(p => p.Name == product.Name);
                var articleTotalCost = countArticle * product.Price;
                articleTotalCost = Math.Round(articleTotalCost * CurrentCurrency, 2);

                if (!controlUniqueItems.Contains(product.Name))
                {
                    controlUniqueItems.Add(product.Name);
                    customerDescription += $"\t{countArticle} {product.Name}(s) {Math.Round(product.Price * CurrentCurrency, 2)} {CurrentCurrencyText}/pcs:" +
                                           $" {articleTotalCost} {CurrentCurrencyText} in total\n";
                }
            }

            var totalCost = CartTotal();
            customerDescription += $"\n\tTotal price: {totalCost} {CurrentCurrencyText}\n";
            return customerDescription;
        }
    }
}

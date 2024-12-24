using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDA2.Models
{
    public class ShoppingCartItem
    {
        public int IDCustomer { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string ProductImg { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int id_chitietkho {  get; set; }
        public int Stock { get; set; }
    }
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; }
        public ShoppingCart()
        {
            this.Items = new List<ShoppingCartItem>();
        }
        public void AddToCart(ShoppingCartItem item, int Quantity)
        {
            var checkExits = Items.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (checkExits != null)
            {
                checkExits.Quantity += Quantity;
                checkExits.TotalPrice = checkExits.Price * checkExits.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void Remove(int id)
        {
            var checkExits = Items.SingleOrDefault(x => x.ProductId == id);
            if (checkExits != null)
            {
                Items.Remove(checkExits);
            }
        }
        public void updateQuantity(int id, int quantity)
        {
            var checkExits=Items.SingleOrDefault(x=> x.ProductId == id);
            if(checkExits != null)
            {
                checkExits.Quantity = quantity;
                checkExits.TotalPrice=checkExits.Price * checkExits.Quantity;
            }
        }
        public decimal GetTotal()
        {
            return Items.Sum(x => x.TotalPrice);
        }
        public decimal GetTotalQuantity()
        {
            return Items.Sum(x => x.Quantity);
        }
        public void clearCart()
        { 
            Items.Clear(); 
        }
    }
}
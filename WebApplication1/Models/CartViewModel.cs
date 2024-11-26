namespace WebApplication1.Models
{
    public class CartViewModel
    {
        public List<Inv_Items> InvItems { get; set; } // List for inventory items
        public List<CartItems> CartItems { get; set; } // List for cart items
        public decimal CartCount { get; set; }

        public decimal CartTotalPrice { get; set; }

        public CartViewModel()
        {
            InvItems = new List<Inv_Items>();  // Initialize to avoid null reference issues
            CartItems = new List<CartItems>();  // Initialize to avoid null reference issues
        }
        

    }
}

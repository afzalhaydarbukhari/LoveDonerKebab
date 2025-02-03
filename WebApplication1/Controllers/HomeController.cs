using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Cryptography;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataDbContext _db;
        private readonly MacAddressHelper _macAddressHelper;

        public HomeController(DataDbContext db, ILogger<HomeController> logger, MacAddressHelper macAddressHelper)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger;
            _macAddressHelper = macAddressHelper;


        }
        
        public IActionResult RemoveFromCart(int Cartid)
        {
            var record = _db.CartItems.Where(c=> c.CartID==Cartid).FirstOrDefault();
            _db.CartItems.Remove(record);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        //GET request API
        [HttpPost]
        public IActionResult AddToCart(int itemId, int quantity,string productName)
        {
            try
            {
                var macAddress = MacAddressHelper.GetMacAddress();

                // Step 1: Fetch ItemId and ItemName from Inv_Items table
                var item = _db.items
                    .Where(i => i.ItemName == productName)
                    .Select(i => new { i.ItemId, i.ItemName,i.RecentUnitPrice,i.ImagePath })
                    .FirstOrDefault();

                if (item == null)
                {
                    return NotFound("Item not found.");
                }
                var cartItem = new CartItems
                {
                    ItemID = item.ItemId,
                    ItemName = item.ItemName,
                    Price = item.RecentUnitPrice,
                    Qty = 1,
                    CartStatus = "Pending",
                    ItemImage = item.ImagePath,
                    ClientID = 1,
                    MacAddress = macAddress,
                    Date = DateTime.UtcNow
                };

                _db.CartItems.Add(cartItem);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Add to Cart successful!";
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.Message}. Inner exception: {innerException}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}. Inner exception: {innerException}";
            }
            

            return RedirectToAction("Index");

        }


        [HttpPost]
        public IActionResult UpdateCartQuantity([FromBody] CartUpdateRequest request)
        {
            if (request == null || request.CartId <= 0 || request.Quantity <= 0)
            {
                return Json(new { success = false, message = "Invalid request data." });
            }

            // Find the cart item by CartID
            var cartItem = _db.CartItems.FirstOrDefault(c => c.CartID == request.CartId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "CartItem not found." });
            }

            // Update the quantity
            cartItem.Qty = request.Quantity;
            _db.SaveChanges();

            // Calculate the updated total for the item
            var updatedTotal = (cartItem.Qty ?? 1) * (cartItem.Price ?? 0);

            return Json(new { success = true, updatedTotal });
        }



        public void RemoveOldCarts()
        {
            var expirationDate = DateTime.UtcNow.AddDays(-7);
            var oldCarts = _db.CartItems.Where(c => c.Date < expirationDate);

            
            if (oldCarts.Any())
            {
                _db.CartItems.RemoveRange(oldCarts);
                _db.SaveChanges();
            }
        }
        [HttpGet]
        public IActionResult Clientreg()
        {
            // Step 3: Fetch CartItems for the client and extract `ItemName`
            var cartItems = _db.CartItems
                               .Where(c => c.MacAddress == MacAddressHelper.GetMacAddress().ToString() && c.CartStatus == "Pending")
                               .ToList();
            return View();
        }

        [HttpPost]      
        public IActionResult Checkout(Client client, decimal TotalPrice)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        client.TotalPrice = TotalPrice;
                        _db.clients.Add(client);
                        _db.SaveChanges();

                        var clientId = client.Clientid;

                        var invSale = new Sales
                        {
                            ClientId = clientId,
                            SaleDate = DateTime.Now,
                            Modifier = "System",
                            LastModified = DateTime.Now,
                            Payment = TotalPrice,
                            SaleId = 0,
                            Status = "Posted"
                        };
                        _db.sales.Add(invSale);
                        _db.SaveChanges();

                        var saleId = invSale.SaleId;
                        var macAddress = MacAddressHelper.GetMacAddress();

                        var cartItems = _db.CartItems
                                           .Where(c => c.MacAddress == macAddress)
                                           .ToList();

                        foreach (var cartItem in cartItems)
                        {
                            var soldItem = new SoldItems
                            {
                                SaleId = saleId,
                                ItemId = cartItem.ItemID ?? 0,
                                ItemName = cartItem.ItemName,
                                Qty = cartItem.Qty ?? 0,
                                UnitPrice = cartItem.Price ?? 0,
                                NetPrice = (cartItem.Price ?? 0) * (cartItem.Qty ?? 1)
                            };

                            _db.soldItems.Add(soldItem);
                        }

                        _db.SaveChanges();

                        _db.CartItems.RemoveRange(cartItems);
                        _db.SaveChanges();

                        transaction.Commit();

                        TempData["Message"] = "Checkout Successful!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        TempData["ErrorMessage"] = $"Checkout failed: {ex.Message}";
                        return View("Cart");
                    }
                }
            }

            TempData["ErrorMessage"] = "Checkout failed. Please try again!";
            return View("Cart");
        }

        //[HttpGet]
        //public IActionResult Index(string? category = null)
        //{
        //    // Fetch all items including their categories
        //    var items = _db.items.Include(i => i.Category).ToList();

        //    // Filter the items if a category is provided
        //    if (!string.IsNullOrEmpty(category) && category.ToLower() != "all")
        //    {
        //        // Ensure the comparison is case-insensitive and accounts for multiple-word categories
        //        items = items.Where(i => i.Category.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }

        //    return View(items);
        //}

        [HttpGet]
        public IActionResult Index(string? category = null)
        {
            try
            {
                //var items = _db.items.ToList();
                var items = _db.items.AsNoTracking().Include(i => i.Category).ToList();
                foreach (var item in items)
                {
                    if (item.Category == null)
                    {
                        item.Category = new Category { CategoryName = "Unknown" };  // Prevent null reference issues
                    }
                }

                if (!string.IsNullOrEmpty(category))
                {
                    string lowerCategory = category.ToLower();
                    items = items
                        .Where(i => i.Category != null &&
                                    !string.IsNullOrEmpty(i.Category.CategoryName) &&
                                    i.Category.CategoryName.ToLower() == lowerCategory)
                        .ToList();
                }

                return View(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching items: " + ex.Message);
                return View(new List<Inv_Items>()); // Return an empty list to prevent crashes
            }
        }



        [HttpPost]
        public IActionResult TestAddSale(string productName)
        {
            decimal slid;
            try
            {
                var newSale = new Sales
                {
                    SaleDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    Payment = 100M,
                    Status = "Pending",
                    Cash_Received = 0,
                    Paid_Back = 0,
                    Modifier = "Admin",
                    TokenNumber = null,
                    Serving = "Test"
                };

                _db.sales.Add(newSale);
                _db.SaveChanges();
                slid = newSale.SaleId;
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.Message}. Inner exception in INV_Saale: {innerException}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}. Inner exception in Inv_Sale: {innerException}";
                return RedirectToAction("Index"); // Redirect to Index
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"newSale.SaleId (expected auto-increment): {slid}");


                // Step 1: Fetch ItemId and ItemName from Inv_Items table
                var item = _db.items
                    .Where(i => i.ItemName == productName)
                    .Select(i => new { i.ItemId, i.ItemName })
                    .FirstOrDefault();

                if (item == null)
                {
                    return NotFound("Item not found."); 
                }
                var newSoldItem = new SoldItems
                {
                    SaleId = slid, // Assuming slid is the SaleId from the Sales table
                    ItemId = item.ItemId, // Use fetched ItemId
                    ItemName = item.ItemName, // Use fetched ItemName
                    UnitPrice = 100M, // Replace with actual price
                    Qty = 1, // Replace with actual quantity
                    NetPrice = 100M // Replace with actual net price
                };

                _db.soldItems.Add(newSoldItem);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Test successful."; // Store success message in TempData
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.Message}. Inner exception: {innerException}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}. Inner exception: {innerException}";
            }

            return RedirectToAction("Index"); // Redirect to Index
        }

        // AboutUs
        public IActionResult AboutUs()
        {
            return View();
        }
        // Typography
        public IActionResult OurMenu()
        {
            return View();
        }
        // ContactUS
        public IActionResult ContactUs()
        {
            return View();
        }
        // Payments
        public IActionResult Checkout()
        {
            return View();
        }
        // Payments
        [HttpPost]
        public IActionResult CreateCheckoutSession(CheckoutFormModel model)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                SuccessUrl = "https://yourwebsite.com/success",
                CancelUrl = "https://yourwebsite.com/cancel",
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = 2000L,
                        Currency = "PKR",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = model.ProductName
                        }
                    },
                    Quantity = 1,
                }
            },
                Mode = "payment",
            };

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
        public override void OnActionExecuting(ActionExecutingContext db)
        {
            base.OnActionExecuting(db);

            var cartCount = _db.CartItems.Sum(c => c.Qty) ?? 0;
            ViewBag.CartCount = cartCount;
        }

    }
}





using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using WebApplication1.Data;
using WebApplication1.Models;
using static WebApplication1.Controllers.HomeController;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataDbContext _db;

        public HomeController(DataDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult ViewCart()
        {
            // Fetch all items in the cart from the database
            var cartItems = _db.CartItems.ToList();

            // Pass the list directly to the view
            return View(cartItems);
        }
        

        [HttpPost]
        public IActionResult AddToCart(int itemId, int quantity,string productName)
        {
            
            try
            {

                // Step 1: Fetch ItemId and ItemName from Inv_Items table
                var item = _db.items
                    .Where(i => i.ItemName == productName)
                    .Select(i => new { i.ItemId, i.ItemName })
                    .FirstOrDefault();

                if (item == null)
                {
                    return NotFound("Item not found.");
                }
                var cartItem = new CartItems
                {

                    ItemID = item.ItemId, 
                    ItemName = item.ItemName, 
                    Price = 1000M,
                    Qty = 1,
                    ItemImage = "/images/big-doner-burger.jpg",
                    ClientID = 1,
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


        [HttpGet]
        public IActionResult Clientreg()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(Client client, decimal TotalPrice)
        {
            if (ModelState.IsValid)
            {

                // Assign the total price from the form to the client object
                client.TotalPrice = TotalPrice;


                _db.clients.Add(client);
                _db.SaveChanges();

                // Get the generated ClientId
                var clientId = client.Clientid;

                // Create a new Inv_Sale record
                var invSale = new Sales
                {
                    ClientId = clientId,
                    SaleDate = DateTime.Now,
                    Modifier = "System",
                    LastModified = DateTime.Now,
                    Payment = TotalPrice,
                    SaleId = 0,
                    Status = "Pending"
                };

                // Save Inv_Sale data
                _db.sales.Add(invSale);
                _db.SaveChanges();

                TempData["Message"] = "Checkout Successful!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Checkout failed. Please try again!";
            return View("Cart");
        }

        [HttpGet]
        public IActionResult Index(string category = null)
        {
            // Fetch all items including their categories
            var items = _db.items.Include(i => i.Category).ToList();

            // Filter the items if a category is provided
            if (!string.IsNullOrEmpty(category) && category.ToLower() != "all")
            {
                // Ensure the comparison is case-insensitive and accounts for multiple-word categories
                items = items.Where(i => i.Category.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            return View(items);
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

        //AboutUs
        public IActionResult AboutUs()
        {
            return View();
        }
        //Typography
        public IActionResult OurMenu()
        {
            return View();
        }
        //ContactUS
        public IActionResult ContactUs()
        {
            return View();
        }
        public override void OnActionExecuting(ActionExecutingContext db)
        {
            base.OnActionExecuting(db);

            var cartCount = _db.CartItems.Sum(c => c.Qty) ?? 0;
            ViewBag.CartCount = cartCount;
        }

    }
}





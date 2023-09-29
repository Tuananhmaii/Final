using RopinStore.DataAccess.Repository.IRepository;
using RopinStore.Models;
using RopinStore.Models.ViewModels;
using RopinStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp;
using Azure;
using System.Net.Http;

namespace RopinStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public int TotalPrice { get; set; }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product"),
                Order = new()
            };
            foreach (var item in ShoppingCartVM.ListCart)
            {
                //item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price,
                //    item.Product.Price50, item.Product.Price100);
                ShoppingCartVM.Order.TotalPrice += (item.Product.Price * item.Count);
            }
            return View(ShoppingCartVM);
        }
        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product"),
                Order = new()
            };

            ShoppingCartVM.Order.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
                u => u.Id == claim.Value);

            ShoppingCartVM.Order.Email = ShoppingCartVM.Order.ApplicationUser.Email;
            ShoppingCartVM.Order.Name = ShoppingCartVM.Order.ApplicationUser.FullName;
            ShoppingCartVM.Order.PhoneNumber = ShoppingCartVM.Order.ApplicationUser.PhoneNumber;
            ShoppingCartVM.Order.City = ShoppingCartVM.Order.ApplicationUser.City;
            ShoppingCartVM.Order.Address = ShoppingCartVM.Order.ApplicationUser.Address;
            foreach (var item in ShoppingCartVM.ListCart)
            {
                //item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price,
                //    item.Product.Price50, item.Product.Price100);
                ShoppingCartVM.Order.TotalPrice += (item.Product.Price * item.Count);
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryAsync(string? name, string? phone, string city, string address, string paymentType)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product");


            ShoppingCartVM.Order.ApplicationUserId = claim.Value;

            ShoppingCartVM.Order.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
                u => u.Id == claim.Value);

            //ShoppingCartVM.Order.Name = ShoppingCartVM.Order.ApplicationUser.FullName;
            if (name == null)
            {
                ShoppingCartVM.Order.Name = ShoppingCartVM.Order.ApplicationUser.FullName;
            }
            else
            {
                ShoppingCartVM.Order.Name = name;
            }
            if (phone == null)
            {
                ShoppingCartVM.Order.PhoneNumber = ShoppingCartVM.Order.ApplicationUser.PhoneNumber;
            }
            else
            {
                ShoppingCartVM.Order.PhoneNumber = phone;
            }
            ShoppingCartVM.Order.Email = ShoppingCartVM.Order.ApplicationUser.Email;
            ShoppingCartVM.Order.City = city;
            ShoppingCartVM.Order.Address = address;
            ShoppingCartVM.Order.OrderStatus = "Delivering";
            ShoppingCartVM.Order.OrderDate = DateTime.Now;
            if (paymentType == "VISA")
            {
                ShoppingCartVM.Order.PaymentType = "VISA";
            }
            else
            {
                ShoppingCartVM.Order.PaymentType = "COD";
            }

            foreach (var item in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.Order.TotalPrice += (item.Product.Price * item.Count);
            }
            user.FullName = name;
            user.PhoneNumber = phone;

            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.Order.Add(ShoppingCartVM.Order);
            _unitOfWork.Save();

            var lineItems = new List<object>();
            string totalPrice = (ShoppingCartVM.Order.TotalPrice + 10.4).ToString("0.00");

            foreach (var item in ShoppingCartVM.ListCart)
            {

                var lineItem = new
                {
                    quantity = item.Count,
                    title = item.Product.Name,
                    total_price = item.Product.Price.ToString("0.00"), // Format the total price as needed
                    currency = "USD",// You can adjust this as needed
                    weight = "0.5",
                    weight_unit = "lb"
                };

                lineItems.Add(lineItem);

                OrderDetail orderDetail = new()
                {
                    OrderId = ShoppingCartVM.Order.Id,
                    ProductId = item.ProductId,
                    Count = item.Count,
                    Price = item.Product.Price,
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            };

            try
            {
                var orderData = new
                {
                    to_address = new
                    {
                        city = "Brooklyn",
                        country = "US",
                        email = "maitrantuananh4802@gmail.com",
                        name = "Tuan Anh",
                        phone = "15553419393",
                        state = "New York",
                        street1 = "301 Legion St."
                    },
                    line_items = lineItems,
                    placed_at = "2023-09-23T01:28:12Z",
                    order_number = "#9509",
                    order_status = "PAID",
                    shipping_cost = "10.40",
                    shipping_cost_currency = "USD",
                    shipping_method = "USPS Priority",
                    subtotal_price = ShoppingCartVM.Order.TotalPrice.ToString("0.00"),
                    total_price = totalPrice,
                    total_tax = "0.00",
                    currency = "USD",
                    weight = "1.5",
                    weight_unit = "lb"
                };

                using (var httpClient = new HttpClient())
                {
                    // Set Shippo API URL
                    var apiUrl = "https://api.goshippo.com/orders/";

                    // Serialize the order data to JSON
                    var json = JsonSerializer.Serialize(orderData);

                    // Create the HTTP request
                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    request.Headers.Add("Authorization", "ShippoToken shippo_test_4225adc69f71381a3274219247e106445ba507a9");

                    // Send the request to Shippo API
                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        // Order placed successfully
                        var responseJson = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseJson);

                        var objectId = responseObject["object_id"].ToString();
                        PlaceOrder(objectId);
                    }
                    else
                    {
                        // Handle API error here
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return Content($"Error placing order: {errorMessage}");
                    }

                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return Content($"An error occurred: {ex.Message}");
            }

            if (paymentType == "VISA")
            {
                var domain = "https://localhost:44340/";
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>(),

                    Mode = "payment",
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.Order.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                };
                foreach (var item in ShoppingCartVM.ListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Product.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            },

                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                };
                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.Order.UpdateStripePaymentId(ShoppingCartVM.Order.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.Order.Id });
            }
        }
        public IActionResult OrderConfirmation(int id)
        {
            Order orderHeader = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");

            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order", "<p> New order created </p>");

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId ==
            orderHeader.ApplicationUserId).ToList();
            HttpContext.Session.Clear();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string objectId)
        {
            var shipmentData = new
            {
                shipment = new
                {
                    address_from = new
                    {
                        name = "Ropin Store",
                        street1 = "301 Legion Str",
                        city = "Brooklyn",
                        state = "New York",
                        zip = "11212",
                        country = "US",
                        phone = "+1 (717) 550-1675",
                        email = "maitrantuananh4802@gmail.com"
                    },
                    address_to = new
                    {
                        name = "Tuan Anh",
                        street1 = "45 Ridgeview St.",
                        city = "San Jose",
                        state = "CA",
                        zip = "95122",
                        country = "US",
                        phone = "+1 555 341 9393",
                        email = "anhmttgcd201452@fpt.edu.vn"
                    },
                    parcels = new[]
                    {
                        new
                            {
                                length = "20",
                                width = "20",
                                height = "12",
                                distance_unit = "in",
                                weight = "2",
                                mass_unit = "lb"
                            }
                    }
                },
                carrier_account = "06684a4ba047461d9ea56f83a7978813",
                servicelevel_token = "usps_priority",
                order = objectId,
            };

            using (var HttpClient = new HttpClient())
            {
                // Set the API URL
                var ApiUrl = "https://api.goshippo.com/transactions/"; // Replace with your actual API endpoint

                // Serialize the shipment data to JSON
                var Json = JsonSerializer.Serialize(shipmentData);

                // Create the HTTP request
                var Request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
                Request.Content = new StringContent(Json, Encoding.UTF8, "application/json");
                Request.Headers.Add("Authorization", "ShippoToken shippo_test_4225adc69f71381a3274219247e106445ba507a9");

                // Send the request to the API
                var Response = await HttpClient.SendAsync(Request);
                if (Response.IsSuccessStatusCode)
                {
                    // Order placed successfully
                    var ResponseJson = await Response.Content.ReadAsStringAsync();
                    var ResponseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(ResponseJson);

                    var trackingUrl = ResponseObject["tracking_url_provider"].ToString();
                    var labelUrl = ResponseObject["label_url"].ToString();
                    return Content($"Place shipment successfully. \nTracking Url:{trackingUrl} \nlabelUrl:{labelUrl}");
                }

                else
                {
                    // Handle API error here
                    var ErrorMessage = await Response.Content.ReadAsStringAsync();
                    return Content($"Error placing order: {ErrorMessage}");
                }
            }
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Count > 1)
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }
            else
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.SessionCart, count);
            return RedirectToAction("Index");
        }
    }
}


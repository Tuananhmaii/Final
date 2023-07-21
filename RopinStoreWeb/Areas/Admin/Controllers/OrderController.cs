using RopinStore.DataAccess.Repository.IRepository;
using RopinStore.Models;
using RopinStore.Models.ViewModels;
using RopinStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Linq;
using System.Security.Claims;

namespace RopinStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            orderVM = new OrderVM()
            {
                Order = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product")
            };
            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetail(OrderVM orderVM)
        {
            var orderHeaderFromDb = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderVM.Order.Id);
            orderHeaderFromDb.Name = orderVM.Order.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.Order.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.Order.StreetAddress;
            orderHeaderFromDb.City = orderVM.Order.City;

            _unitOfWork.Order.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Update successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDb.Id });
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult StartProcessing(OrderVM orderVM)
        //{
        //    _unitOfWork.Order.UpdateStatus(orderVM.Order.Id,SD.StatusInProcess);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Update status successfully";
        //    return RedirectToAction("Details", "Order", new { orderId = orderVM.Order.Id });
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult ShipOrder(OrderVM orderVM)
        //{
        //    var orderHeaderFromDb = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderVM.Order.Id);
        //    orderHeaderFromDb.TrackingNumber = orderVM.Order.TrackingNumber;
        //    orderHeaderFromDb.Carrier = orderVM.Order.Carrier;
        //    orderHeaderFromDb.OrderStatus = SD.StatusShipped;
        //    orderHeaderFromDb.ShippingDate = DateTime.Now;
        //    if(orderHeaderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
        //    {
        //        orderHeaderFromDb.PaymentDueDate = DateTime.Now.AddDays(30);
        //    }
        //    _unitOfWork.Order.Update(orderHeaderFromDb);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Ship order successfully";
        //    return RedirectToAction("Details", "Order", new { orderId = orderVM.Order.Id });
        //}

        //[HttpPost]
        //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        //[ValidateAntiForgeryToken]
        //public IActionResult CancelOrder(OrderVM orderVM)
        //{
        //    var orderHeader = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderVM.Order.Id);
        //    if (orderHeader.PaymentStatus == SD.PaymentStatusAprroved)
        //    {
        //        var options = new RefundCreateOptions
        //        {
        //            Reason = RefundReasons.RequestedByCustomer,
        //            PaymentIntent = orderHeader.PaymentIntentId
        //        };

        //        var service = new RefundService();
        //        Refund refund = service.Create(options);

        //        _unitOfWork.Order.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
        //    }
        //    else
        //    {
        //        _unitOfWork.Order.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
        //    }
        //    _unitOfWork.Save();

        //    TempData["Success"] = "Order Cancelled Successfully.";
        //    return RedirectToAction("Details", "Order", new { orderId = orderVM.Order.Id });
        //}

        //[ActionName("Details")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Details_PAY_NOW(OrderVM orderVM)
        //{
        //    orderVM.Order = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderVM.Order.Id, includeProperties: "ApplicationUser");
        //    orderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderVM.Order.Id, includeProperties: "Product");

        //    //stripe settings 
        //    var domain = "https://localhost:44300/";
        //    var options = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string>
        //        {
        //          "card",
        //        },
        //        LineItems = new List<SessionLineItemOptions>(),
        //        Mode = "payment",
        //        SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderId={orderVM.Order.Id}",
        //        CancelUrl = domain + $"admin/order/details?orderId={orderVM.Order.Id}",
        //    };

        //    foreach (var item in orderVM.OrderDetail)
        //    {

        //        var sessionLineItem = new SessionLineItemOptions
        //        {
        //            PriceData = new SessionLineItemPriceDataOptions
        //            {
        //                UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
        //                Currency = "usd",
        //                ProductData = new SessionLineItemPriceDataProductDataOptions
        //                {
        //                    Name = item.Product.Title
        //                },

        //            },
        //            Quantity = item.Count,
        //        };
        //        options.LineItems.Add(sessionLineItem);

        //    }

        //    var service = new SessionService();
        //    Session session = service.Create(options);
        //    _unitOfWork.Order.UpdateStripePaymentId(orderVM.Order.Id, session.Id, session.PaymentIntentId);
        //    _unitOfWork.Save();
        //    Response.Headers.Add("Location", session.Url);
        //    return new StatusCodeResult(303);
        //}

        //public IActionResult PaymentConfirmation(int orderHeaderid)
        //{
        //    Order orderHeader = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderHeaderid);
        //    if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        //    {
        //        var service = new SessionService();
        //        Session session = service.Get(orderHeader.SessionId);
        //        //check the stripe status
        //        if (session.PaymentStatus.ToLower() == "paid")
        //        {
        //            _unitOfWork.Order.UpdateStatus(orderHeaderid, orderHeader.OrderStatus, SD.PaymentStatusAprroved);
        //            _unitOfWork.Save();
        //        }
        //    }
        //    return View(orderHeaderid);
        //}

        #region API CALL
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Order> orderHeaders;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.Order.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.Order.GetAll(u=>u.ApplicationUserId == claim.Value,includeProperties: "ApplicationUser");
            }

            //switch (status)
            //{
            //    case "pending":
            //        orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
            //        break;
            //    case "inprocess":
            //        orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
            //        break;
            //    case "completed":
            //        orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
            //        break;
            //    case "approved":
            //        orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
            //        break;
            //    case "cancelled":
            //        orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusCancelled);
            //        break;
            //    default:
            //        break;
            //}


            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}

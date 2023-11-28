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
            orderHeaderFromDb.Street = orderVM.Order.Street;
            orderHeaderFromDb.City = orderVM.Order.City;

            _unitOfWork.Order.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Update successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDb.Id });
        }
        
        #region API CALL
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Order> orderHeaders;
            if (User.IsInRole(SD.Role_Admin) /*|| User.IsInRole(SD.Role_Employee*/)
            {
                orderHeaders = _unitOfWork.Order.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.Order.GetAll(u=>u.ApplicationUserId == claim.Value,includeProperties: "ApplicationUser");
            }

            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}

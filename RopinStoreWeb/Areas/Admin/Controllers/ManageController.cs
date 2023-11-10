using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RopinStore.DataAccess.Data;
using RopinStore.DataAccess.Repository.IRepository;

namespace RopinStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        public ManageController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult GetReportByYear(int year)
        {
            var result = _db.Orders
                .Where(o => o.OrderDate.Year == year)
                .GroupBy(o => new { Year = o.OrderDate.Year, Month = o.OrderDate.Month })
                .Select(g => new
                {
                    OrderYear = g.Key.Year,
                    OrderMonth = g.Key.Month,
                    Price = g.Sum(o => o.TotalPrice)
                })
                .ToList();

            return Json(result);
        }

        public ActionResult GetPaymentType()
        {
            var paymentCounts = _db.Orders
                .Where(order => order.PaymentType == "COD" || order.PaymentType == "VISA")
                .GroupBy(order => order.PaymentType)
                .Select(group => new
                {
                    PaymentType = group.Key,
                    PaymentCount = group.Count()
                })
                .ToList();

            return Json(paymentCounts);
        }
    }
}

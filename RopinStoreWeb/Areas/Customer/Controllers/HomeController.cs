using RopinStore.DataAccess.Repository.IRepository;
using RopinStore.Models;
using RopinStore.Models.ViewModels;
using RopinStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using RopinStore.DataAccess.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RopinStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Index()
        {
            ViewBag.productList1 = _unitOfWork.Product.GetAll(includeProperties: "Brand").Where(u => u.CollectionId == 1).Take(4).ToList();
            ViewBag.productList2 = _unitOfWork.Product.GetAll(includeProperties: "Brand").Where(u => u.CollectionId == 2).Take(4).ToList();
            return View();
        }
        public IActionResult Main(int minPrice = 0, int maxPrice = 10000, List<int>? filterBrand = null, List<int>? filterCategory = null, List<string>? filterGender = null, string? page = null)
        {
            var query = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand");
            var brand = _unitOfWork.Brand.GetAll().ToList();
            var category = _unitOfWork.Category.GetAll().ToList();
            var gender = new List<string> { "Men", "Women" };

            if (filterBrand != null && filterBrand.Any())
            {
                query = query.Where(u => filterBrand.Contains(u.BrandId));
                ViewBag.SelectedBrand = _unitOfWork.Brand.GetAll().Where(u => filterBrand.Contains(u.Id)).ToList();
                foreach (var item in ViewBag.SelectedBrand)
                {
                    brand.RemoveAll(brand => brand.Id == item.Id);
                }
            }

            if (filterCategory != null && filterCategory.Any())
            {
                query = query.Where(u => filterCategory.Contains(u.CategoryId));
                ViewBag.SelectedCategory = _unitOfWork.Category.GetAll().Where(u => filterCategory.Contains(u.Id)).ToList();
                foreach (var item in ViewBag.SelectedCategory)
                {
                    category.RemoveAll(category => category.Id == item.Id);
                }
            }

            if (filterGender != null && filterGender.Any())
            {
                query = query.Where(u => filterGender.Contains(u.Gender));
                ViewBag.SelectedGender = filterGender;
                foreach (var item in ViewBag.SelectedGender)
                {
                    gender.RemoveAll(gender => gender == item);
                }
            }
            query = query.Where(u => u.Price >= minPrice && u.Price <= maxPrice);
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            ViewBag.Categories = new SelectList(category, "Id", "Name");
            ViewBag.Brands = new SelectList(brand, "Id", "Name");
            ViewBag.Gender = new SelectList(gender);
            IEnumerable<Product> productList = query.ToList();

            //Paging
            const int pageSize = 20;
            if (page == null)
            {
                page = "1";
            }
            int pg = Int32.Parse(page);
            if (pg < 1) pg = 1;
            int recsCount = productList.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = productList.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        public IActionResult Chat()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return View();
            }
            else
            {
                ApplicationUser user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
                return View(user);
            }
        }
        public IActionResult Details(int productid)
        {
            //var list = _db.ProductGalleries.Where(u => u.ProductId == productid).ToList();
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productid,
                Product = _unitOfWork.Product.GetFirstOrDefault(n => n.Id == productid, includeProperties: "Category,Brand,Collection"),
                Gallery = _unitOfWork.ProductGallery.GetAll(u => u.ProductId == productid).ToList(),
                Review = _unitOfWork.Review.GetAll(u => u.ProductId == productid, includeProperties: "ApplicationUser").ToList(),
            };
            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;
            ShoppingCart cartFromDB = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);
            if (cartFromDB == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);

            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartFromDB, shoppingCart.Count);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Rating(int productid)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Review review = new Review()
            {
                Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productid),
                ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value),
            };
            return View(review);
        }

        [HttpPost]
        public ActionResult Rating(Review review, string Rating, string name)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            review.Date = DateTime.Now;
            review.ApplicationUserId = claim.Value;
            review.Rating = Int32.Parse(Rating);
            review.Verify = "Verified";
            user.FullName = name;

            _unitOfWork.Review.Add(review);
            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.Save();
            TempData["success"] = "Your rating will be review";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
﻿using RopinStore.DataAccess.Repository.IRepository;
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
        public IActionResult Index(List<int>? filterBrand, List<int>? filterCategory)
        {
            var query = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand");
            var brand = _unitOfWork.Brand.GetAll().ToList();
            var category = _unitOfWork.Category.GetAll().ToList();

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

            ViewBag.Categories = new SelectList(category, "Id", "Name");
            ViewBag.Brands = new SelectList(brand, "Id", "Name");
            IEnumerable<Product> productList = query.ToList();
            return View(productList);
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
                Gallery = _db.ProductGalleries.Where(u => u.ProductId == productid).ToList()
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
        //[HttpGet]
        //public ActionResult Rating(int ProductId)
        //{
        //    string redirectUrl = $"https://www.example.com/product/details?ProductId={ProductId}";
        //    return Redirect("https://www.google.com/");
        //}

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
        public ActionResult Rating(Review review, string Rating)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            review.Date = DateTime.Now;
            review.ApplicationUserId = claim.Value;
            review.Rating = Int32.Parse(Rating);
            review.Verify = "Verified";
            _unitOfWork.Review.Add(review);
            _unitOfWork.Save();
            TempData["success"] = "Your rating will be review";
            return RedirectToAction(nameof(Index));
        }
        //[HttpPost]
        //public JsonResult LeaveComment(Review review)
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    JsonResult result = new JsonResult();
        //    try
        //    {
        //        review.Date = DateTime.Now;
        //        review.ApplicationUserId = claim.Value;
        //        var res =
        //    }
        //}

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
using RopinStore.DataAccess;
using RopinStore.DataAccess.Repository;
using RopinStore.DataAccess.Repository.IRepository;
using RopinStore.Models;
using RopinStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RopinStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult GetAll()
        {
            var UserList = _unitOfWork.ApplicationUser.GetAll();
            return Json(new { data = UserList });
        }


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Edit(string? id)
        {
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(s => s.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        ////POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(Brand obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Brand.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Edit successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View(obj);
        //}
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(string id)
        {
            var obj = _unitOfWork.ApplicationUser.GetFirstOrDefault(s => s.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.ApplicationUser.Remove(obj);
            TempData["success"] = "Delete successfully";
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}

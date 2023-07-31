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
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Brand> objCoverTypeList = _unitOfWork.Brand.GetAll();
            return View(objCoverTypeList);
        }

        //GET
        public IActionResult Create()
        {

            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Brand obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Brand.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Create successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var CoverTypeFromDb = _unitOfWork.Brand.GetFirstOrDefault(s => s.Id == id);
            if (CoverTypeFromDb == null)
            {
                return NotFound();
            }
            return View(CoverTypeFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Brand obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Brand.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Edit successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var CoverTypeFromDb = _unitOfWork.Brand.GetFirstOrDefault(s => s.Id == id);
            if (CoverTypeFromDb == null)
            {
                return NotFound();
            }
            return View(CoverTypeFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Brand.GetFirstOrDefault(s => s.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Brand.Remove(obj);
            TempData["success"] = "Delete successfully";
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}

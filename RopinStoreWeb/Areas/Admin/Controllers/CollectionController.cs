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
    public class CollectionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CollectionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Collection> objCollectionList = _unitOfWork.Collection.GetAll();
            return View(objCollectionList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Collection obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Collection.Add(obj);
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
            var CategoryFromDb = _unitOfWork.Collection.GetFirstOrDefault(s => s.Id == id);
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Collection obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Collection.Update(obj);
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
            var CategoryFromDb = _unitOfWork.Collection.GetFirstOrDefault(s => s.Id == id);
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Collection.GetFirstOrDefault(s => s.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Collection.Remove(obj);
            TempData["success"] = "Delete successfully";
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}

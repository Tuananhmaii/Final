using RopinStore.DataAccess;
using RopinStore.DataAccess.Repository;
using RopinStore.DataAccess.Repository.IRepository;
using RopinStore.Models;
using RopinStore.Models.ViewModels;
using RopinStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace RopinStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(n => new SelectListItem
                {
                    Text = n.Name,
                    Value = n.Id.ToString()
                }),
                BrandList = _unitOfWork.Brand.GetAll().Select(n => new SelectListItem
                {
                    Text = n.Name,
                    Value = n.Id.ToString()
                }),
                CollectionList = _unitOfWork.Collection.GetAll().Select(n => new SelectListItem
                {
                    Text = n.Name,
                    Value = n.Id.ToString()
                }),
            };
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(n => n.Id == id);
                return View(productVM);
            }
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file, IFormFile? file2, List<IFormFile>? imageGallery)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);
                    var extension2 = Path.GetExtension(file2.FileName);

                    if (!String.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        // Delete the old image if it exists in Cloudinary
                        DeleteImageFromCloudinary(obj.Product.ImageUrl);
                    }

                    // Upload the new image to Cloudinary
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(fileName + extension, file.OpenReadStream()),
                        Folder = "Products/" // Specify the folder in Cloudinary where you want to store the images
                    };
                    var uploadParams2 = new ImageUploadParams()
                    {
                        File = new FileDescription(fileName + extension2, file2.OpenReadStream()),
                        Folder = "Products/" // Specify the folder in Cloudinary where you want to store the images
                    };

                    var cloudinary = new Cloudinary("cloudinary://651675597367258:fUk4tt7zSPGAl7JwE8cTSxrQ-5Q@dc6xcnpke");
                    var uploadResult = cloudinary.Upload(uploadParams);
                    var uploadResult2 = cloudinary.Upload(uploadParams2);

                    if (uploadResult.Error == null || uploadResult2.Error == null)
                    {
                        // Set the image URL to the Cloudinary URL
                        obj.Product.ImageUrl = uploadResult.SecureUrl.ToString();
                        obj.Product.SecondImage = uploadResult2.SecureUrl.ToString();
                    }
                }
                if (imageGallery != null)
                {

                    obj.ProductGallery = new List<IFormFile>();

                    foreach (var item in imageGallery)
                    {
                        string fileNameGallery = Guid.NewGuid().ToString();
                        var extensionGallery = Path.GetExtension(item.FileName);

                        var uploadParamsGallery = new ImageUploadParams()
                        {
                            File = new FileDescription(fileNameGallery + extensionGallery, item.OpenReadStream()),
                            Folder = "Products/" // Specify the folder in Cloudinary where you want to store the images
                        };

                        var cloudinaryGallery = new Cloudinary("cloudinary://651675597367258:fUk4tt7zSPGAl7JwE8cTSxrQ-5Q@dc6xcnpke");
                        var uploadResultGallery = cloudinaryGallery.Upload(uploadParamsGallery);

                        if (uploadResultGallery.Error == null)
                        {
                            // Set the image URL to the Cloudinary URL
                            //gallery.URL = uploadResultGallery.SecureUrl.ToString();
                            var gallery = new ProductGallery()
                            {
                                URL = uploadResultGallery.SecureUrl.ToString()
                            };
                            obj.Product.Gallery.Add(gallery);
                        }
                    }
                }
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }
                _unitOfWork.Cache.DeleteAll();
                _unitOfWork.Save();
                TempData["success"] = "Edit successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        private void DeleteImageFromCloudinary(string imageUrl)
        {
            var cloudinary = new Cloudinary("cloudinary://651675597367258:fUk4tt7zSPGAl7JwE8cTSxrQ-5Q@dc6xcnpke");
            var publicId = GetPublicIdFromImageUrl(imageUrl);
            var deleteParams = new DeletionParams(publicId);

            cloudinary.Destroy(deleteParams);
        }

        private string GetPublicIdFromImageUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var publicId = uri.Segments.Last().Split('.').First();
            return publicId;
        }

        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand");
            return Json(new { data = productList });
        }
        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(s => s.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Delete failed" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete success" });
        }
        #endregion
    }
}

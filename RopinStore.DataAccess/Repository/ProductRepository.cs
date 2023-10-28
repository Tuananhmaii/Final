using RopinStore.DataAccess.Data;
using RopinStore.DataAccess.Repository.IRepository;
using RopinStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopinStore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            var objFromProduct = _db.Products.FirstOrDefault(s => s.Id.Equals(obj.Id));
            if(objFromProduct != null)
            {
                objFromProduct.Id = obj.Id;
                objFromProduct.Name = obj.Name;
                objFromProduct.Description = obj.Description;
                objFromProduct.Gender = obj.Gender;
                objFromProduct.Color = obj.Color;
                objFromProduct.Specification = obj.Specification;
                objFromProduct.Price = obj.Price;
                objFromProduct.BrandId = obj.BrandId;
                objFromProduct.CollectionId = obj.CollectionId;
                objFromProduct.CategoryId = obj.CategoryId;
                objFromProduct.SecondImage = obj.SecondImage;

                if(obj.ImageUrl != null)
                {
                    objFromProduct.ImageUrl = obj.ImageUrl;
                }
                if (obj.Gallery != null)
                {
                    foreach(var item in obj.Gallery)
                    {
                        objFromProduct.Gallery.Add(new ProductGallery
                        {
                            URL = item.URL
                        });
                    }

                }
            }
        }
    }
}

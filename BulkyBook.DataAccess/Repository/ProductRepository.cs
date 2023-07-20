using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
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
                objFromProduct.Title = obj.Title;
                objFromProduct.Description = obj.Description;
                objFromProduct.ISBN = obj.ISBN;
                objFromProduct.Author = obj.Author;
                objFromProduct.ListPrice = obj.ListPrice;
                objFromProduct.Price = obj.Price;
                objFromProduct.Price50 = obj.Price50;
                objFromProduct.Price100 = obj.Price100;
                if(objFromProduct.ImageUrl != null)
                {
                    objFromProduct.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}

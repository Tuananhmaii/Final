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
    public class ProductGalleryRepository : Repository<ProductGallery>, IProductGalleryRepository
    {
        private ApplicationDbContext _db;
        public ProductGalleryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ProductGallery obj)
        {
            _db.ProductGalleries.Update(obj);
        }
    }
}

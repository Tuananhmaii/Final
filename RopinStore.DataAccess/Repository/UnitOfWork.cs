using RopinStore.DataAccess.Data;
using RopinStore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopinStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public IBrandRepository Brand { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IProductGalleryRepository ProductGallery { get; private set; }
        public IReviewRepository Review { get; private set; }
        public ICollectionRepository Collection { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Brand = new BrandRepository(_db);
            Product = new ProductRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            Order = new OrderRepository(_db);
            ProductGallery = new ProductGalleryRepository(_db);
            Review = new ReviewRepository(_db);
            Collection = new CollectionRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

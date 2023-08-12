using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopinStore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IBrandRepository Brand{ get; }
        ICategoryRepository Category{ get; }
        IProductRepository Product{ get; }
        IApplicationUserRepository ApplicationUser{ get; }
        IShoppingCartRepository ShoppingCart{ get; }
        IOrderDetailRepository OrderDetail{ get; }
        IOrderRepository Order { get; }
        IProductGalleryRepository ProductGallery { get; }
        void Save();
    }
}

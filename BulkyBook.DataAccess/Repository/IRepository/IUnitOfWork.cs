using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICoverTypeRepository CoverType{ get; }
        ICategoryRepository Category{ get; }
        IProductRepository Product{ get; }
        ICompaniesRepository Company{ get; }
        IApplicationUserRepository ApplicationUser{ get; }
        IShoppingCartRepository ShoppingCart{ get; }
        IOrderDetailRepository OrderDetail{ get; }
        IOrderHeaderRepository OrderHeader { get; }
        void Save();
    }
}

using RopinStore.DataAccess.Repository.IRepository;
using RopinStore.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RopinStore.Models;

namespace RopinStoreWeb.ViewComponents
{
    public class ProductList : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductList(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Product> list = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand,Collection").ToList();
            return View(list);
        }
    }
}

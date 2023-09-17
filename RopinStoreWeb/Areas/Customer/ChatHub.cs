using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage;
using RopinStore.DataAccess.Data;
using RopinStore.DataAccess.Repository.IRepository;
using RopinStore.Models;
using RopinStore.Utility;
using RopinStoreWeb.Areas.Customer.Controllers;

namespace RopinStoreWeb.Areas.Customer
{
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChatHub( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task SendMessage(string user, string message)
        {
            var admin = _unitOfWork.ApplicationUser.GetAll(includeProperties:"Role").ToList();
            //foreach(var item in admin)
            //{
            //    if(item.(SD.Role_Admin)) { }
            //}
            //if (Context.User.IsInRole(SD.Role_Admin))
            //{

            //}
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Order obj)
        {
            _db.Orders.Update(obj);
        }

        //public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        //{
        //    var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);
        //    if(orderFromDb != null)
        //    {
        //        orderFromDb.OrderStatus = orderStatus;
        //        if(orderFromDb.PaymentStatus != null)
        //        {
        //            orderFromDb.PaymentStatus = paymentStatus;
        //        }
        //    }
        //}
        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);
            orderFromDb.SessionId = sessionId;
            orderFromDb.PaymentIntentId = paymentIntentId;
        }
    }
}

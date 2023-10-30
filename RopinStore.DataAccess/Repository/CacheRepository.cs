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
    public class CacheRepository : Repository<Cache>, ICacheRepository
    {
        private ApplicationDbContext _db;
        public CacheRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void DeleteAll()
        {
            _db.Caches.RemoveRange(_db.Caches);
        }
    }
}

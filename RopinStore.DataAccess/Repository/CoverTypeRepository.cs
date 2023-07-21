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
    public class CoverTypeRepository : Repository<Brand>, ICoverTypeRepository
    {
        private ApplicationDbContext _db;
        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Brand obj)
        {
            _db.CoverTypes.Update(obj);
        }
    }
}

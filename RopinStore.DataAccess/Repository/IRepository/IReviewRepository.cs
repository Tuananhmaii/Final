﻿using RopinStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopinStore.DataAccess.Repository.IRepository
{
    public interface IReviewRepository : IRepository<Review>
    {
        void Update(Review obj);
    }
}

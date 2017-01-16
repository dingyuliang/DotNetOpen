﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Data
{
    public interface IDomainService<T>
    {
        bool Add(T entity);
        void AddRange(params T[] entities);
        bool Update(T entity);
        void UpdateRange(params T[] entities);
        bool Delete(T entity);
        void DeleteRange(params T[] entities);
        IEnumerable<T> FindAll();
    }
}

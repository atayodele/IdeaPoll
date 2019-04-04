using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdeaSolution.Data.IGeneric
{
    public interface IRepo
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
    }
}

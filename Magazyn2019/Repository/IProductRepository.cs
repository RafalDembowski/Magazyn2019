using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazyn2019.Repository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        IQueryable GetAllActiveProducts();
        IQueryable GetActiveProductByID(int id);
        bool CheckIfExistActiveProductByName(string name);
        bool CheckIfExistActiveProductByCode(int code);

    }
}

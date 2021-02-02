using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazyn2019.Repository
{
    public interface IWarehouseRepository : IGenericRepository<Warehouse> 
    {
        IQueryable GetAllWarehouses();
        IQueryable GetWarehouseByID(int id);
        bool CheckIfExistWarehouseByName(string name);
        bool CheckIfExistWarehouseByCode(int code);

    }
}

using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazyn2019.Repository
{
    public interface IInventoryRepository : IGenericRepository<Inventory>
    {
        IQueryable GetAllInventories();
        IQueryable GetInventoryrByID(int id);
        IQueryable GetAllproductsFromWarehouse(int idWarehouse, int idProduct);
        Inventory GetInventoryByIdProductAndIdWarehouse(int idProduct, int idWarehouse);
    }
}

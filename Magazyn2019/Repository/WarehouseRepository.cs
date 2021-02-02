using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magazyn2019.Repository
{
    public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(Magazyn2019Entities mainContext) : base(mainContext) { }

        public IQueryable GetAllWarehouses()
        {
            var warehouses = _context
                            .Warehouses.Select(x =>
                             new {
                                 x.id_warehouse, 
                                 x.code, x.name, 
                                 x.description, 
                                 x.created, 
                                 x.id_user
                             });
            return warehouses;
        }
        public IQueryable GetWarehouseByID(int id)
        {
            var warehouse = from w in _context.Warehouses
                            where id == w.id_warehouse
                            select new
                            {
                                name = w.name,
                                code = w.code,
                                description = w.description,
                                created = w.created,
                                userName = w.User.fullName,
                            };
            return warehouse;
        }
        public bool CheckIfExistWarehouseByName(string name)
        {
            return _context.Warehouses.Any(x => x.name == name);
        }
        public bool CheckIfExistWarehouseByCode(int code)
        {
            return _context.Warehouses.Any(x => x.code == code);

        }
    }
}
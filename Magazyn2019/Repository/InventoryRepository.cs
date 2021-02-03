using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magazyn2019.Repository
{
    public class InventoryRepository : GenericRepository<Inventory>  , IInventoryRepository
    {
        public InventoryRepository(Magazyn2019Entities mainContext) : base(mainContext) { }

        public IQueryable GetAllInventories()
        {
            var inventories = _context
                            .Inventories
                            .Select(x =>
                            new { 
                                x.id_inventory, 
                                x.id_product, 
                                x.id_warehouse, 
                                x.amount 
                            });
            return inventories;
        }
        public IQueryable GetInventoryrByID(int id)
        {
            var inventory = from i in _context.Inventories
                            where id == i.id_warehouse && i.amount > 0
                            select new
                            {
                                id_inventory = i.id_inventory,
                                id_product = i.id_product,
                                id_warehouse = i.id_warehouse,
                                amount = i.amount,
                                name = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.name),
                                code = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.code),
                                groupName = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.Group.name),
                                description = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.description),
                                unit = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.unit),
                            };

            return inventory;
        }
        public IQueryable GetAllproductsFromWarehouse(int idWarehouse, int idProduct)
        {
            var inventories = from i in _context.Inventories
                              where (idWarehouse == i.id_warehouse)
                              && (idProduct == i.id_product)
                              select new
                              {
                                  id_inventory = i.id_inventory,
                                  id_product = i.id_product,
                                  id_warehouse = i.id_warehouse,
                                  amount = i.amount,
                                  name = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.name),
                                  code = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.code),
                                  groupName = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.Group.name),
                                  description = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.description),
                                  unit = _context.Products.Where(x => x.id_product == i.id_product).Select(x => x.unit),
                              };
            return inventories;
        }
        public Inventory GetInventoryByIdProductAndIdWarehouse(int idProduct, int idWarehouse)
        {
            var inventory = _context
                            .Inventories
                            .SingleOrDefault
                            (x => x.id_product == idProduct && x.id_warehouse == idWarehouse);

            return inventory;
        }
    }
}
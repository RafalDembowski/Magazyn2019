using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magazyn2019.Repository
{
    public class ProductRepository : GenericRepository<Product> , IProductRepository
    {
        public ProductRepository(Magazyn2019Entities mainContext) : base(mainContext) { }

        public IQueryable GetAllActiveProducts()
        {
            var products = _context
                          .Products.Select(x =>
                           new {
                               x.id_product,
                               x.id_group,
                               x.name,
                               x.code,
                               x.description,
                               x.unit,
                               x.created,
                               x.id_user,
                               group_name = x.Group.name,
                               x.is_active })
                          .Where(x => x.is_active == true);

            return products;
        }
        public IQueryable GetActiveProductByID(int id)
        {
            var product = from p in _context.Products
                          where id == p.id_product && p.is_active == true
                          select new
                          {
                              id_group = p.id_group,
                              code = p.code,
                              name = p.name,
                              description = p.description,
                              unit = p.unit,
                              created = p.created,
                              group_name = p.Group.name,
                              userName = p.User.fullName,
                          };
            return product;
        }
        public bool CheckIfExistActiveProductByName(string name)
        {
            return _context.Products.Any(x => x.name == name && x.is_active == true);
        }
        public bool CheckIfExistActiveProductByCode(int code)
        {
            return _context.Products.Any(x => x.code == code && x.is_active == true);
        }
    }
}
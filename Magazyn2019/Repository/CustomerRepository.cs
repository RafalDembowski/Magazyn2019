using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;

namespace Magazyn2019.Repository
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(Magazyn2019Entities mainContext) : base(mainContext) { }
        public IQueryable GetAllActiveCustomers()
        {
            IQueryable customers =  _context
                                    .Customers.Select(x =>
                                    new { 
                                        x.id_customer, 
                                        x.name, x.code, 
                                        x.street, 
                                        x.zipCode, 
                                        x.city, 
                                        x.type, 
                                        x.created, 
                                        x.id_user, 
                                        x.is_active })
                                    .Where(x => x.is_active ==true);          
            return customers;
        }
        public IQueryable GetActiveCustomerByID(int id)
        {
            var customer = from c in _context.Customers
                           where id == c.id_customer
                           && c.is_active == true
                           select new
                           {
                               name = c.name,
                               code = c.code,
                               street = c.street,
                               zipCode = c.zipCode,
                               city = c.city,
                               type = c.type,
                               created = c.created,
                               userName = c.User.fullName,
                           };

            return customer;
        }
        public bool CheckIfExistActiveCustomerByName(string name)
        {
            return _context.Customers.Any(x => x.name == name && x.is_active == true);
        }
        public bool CheckIfExistActiveCustomerByCode(int code)
        {
            return _context.Customers.Any(x => x.code == code && x.is_active == true);
        }
    }
}
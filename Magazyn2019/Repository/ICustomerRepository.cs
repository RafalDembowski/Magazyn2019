using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazyn2019.Repository
{
    public interface ICustomerRepository : IGenericRepository<Customer> 
    {
        IQueryable GetAllActiveCustomers();
        IQueryable GetActiveCustomerByID(int id);
        bool CheckIfExistActiveCustomerByName(string name);
        bool CheckIfExistActiveCustomerByCode(int code);
    }
}

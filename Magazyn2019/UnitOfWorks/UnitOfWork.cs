using Magazyn2019.Models;
using Magazyn2019.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magazyn2019.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Magazyn2019Entities _context;
        public ICustomerRepository CustomerRepository { get; }
        public IUserRepository UserRepository { get; }
        public IWarehouseRepository WarehouseRepository { get; }
        public IGroupRepository GroupRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IMoveRepository MoveRepository { get; }

        public UnitOfWork(Magazyn2019Entities context)
        {
            _context = context;
            CustomerRepository = new CustomerRepository(_context);
            UserRepository = new UserRepository(_context);
            WarehouseRepository = new WarehouseRepository(_context);
            GroupRepository = new GroupRepository(_context);
            ProductRepository = new ProductRepository(_context);
            MoveRepository = new MoveRepository(_context);
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magazyn2019.Repository
{
    public class UserRepository : GenericRepository<User> , IUserRepository
    {
        public UserRepository(Magazyn2019Entities mainContext) : base(mainContext) { }
        public User GetActiveUser(int id)
        {
            User activeUser = _context
                             .Users
                             .Single(x => x.id_user == id);
            return activeUser;
        }
    }
}
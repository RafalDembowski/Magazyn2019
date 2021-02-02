using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazyn2019.Repository
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        IQueryable GetAllActiveGroup();
        IQueryable GetActiveGroupByID(int id);
        bool CheckIfExistActiveGroupByName(string name);
        bool CheckIfExistActiveGroupByCode(int code);
    }
}

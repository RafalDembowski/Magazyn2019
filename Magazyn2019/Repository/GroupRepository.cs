using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magazyn2019.Repository
{
    public class GroupRepository : GenericRepository<Group> , IGroupRepository
    {
        public GroupRepository(Magazyn2019Entities mainContext) : base(mainContext) { }
        public IQueryable GetAllActiveGroup()
        {
            var groups = _context
                        .Groups.Select(x =>
                        new { 
                            x.id_group, 
                            x.name, 
                            x.code,
                            x.description, 
                            x.created, 
                            x.id_user, 
                            x.is_active })
                        .Where(x => x.is_active == true);
            return groups;
        }
        public IQueryable GetActiveGroupByID(int id)
        {
            var group = from g in _context.Groups
                        where id == g.id_group && g.is_active == true
                        select new
                        {
                            name = g.name,
                            code = g.code,
                            created = g.created,
                            description = g.description,
                            userName = g.User.fullName,
                        };
            return group;
        }
        public bool CheckIfExistActiveGroupByName(string name)
        {
            return _context.Groups.Any(x => x.name == name && x.is_active == true);
        }
        public bool CheckIfExistActiveGroupByCode(int code)
        {
            return _context.Groups.Any(x => x.code == code && x.is_active == true);
        }

    }
}
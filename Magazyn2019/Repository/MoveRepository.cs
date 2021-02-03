using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magazyn2019.Repository
{
    public class MoveRepository : GenericRepository<Move>, IMoveRepository
    {
        public MoveRepository(Magazyn2019Entities mainContext) : base(mainContext) { }

        public int GetNumberOfDocuments(int typeOfMove)
        {
            var numberOfDocument = (from m in _context.Moves
                                    where m.type == typeOfMove
                                    select m).Max(m => (int?)m.number) ?? 0;

            if (numberOfDocument == 0)
            {
                numberOfDocument = 1;
            }
            else
            {
                numberOfDocument++;
            }
            return numberOfDocument;
        }
        public IQueryable GetAllDocuments()
        {
            var documents = _context
                            .Moves
                            .Select(x =>
                            new
                            {
                                x.id_move,
                                x.type,
                                x.number,
                                x.time,
                                name_WarehouseOne = _context.Warehouses.Where(w => w.id_warehouse == x.id_warehouse1).Select(w => w.name),
                                name_WarehouseTwo = _context.Warehouses.Where(w => w.id_warehouse == x.id_warehouse2).Select(w => w.name),
                                name_User = _context.Users.Where(u => u.id_user == x.id_user).Select(u => u.fullName),
                                name_Customer = _context.Customers.Where(c => c.id_customer == x.id_custmer).Select(c => c.name),
                            });
            return documents;

        }
    }
}
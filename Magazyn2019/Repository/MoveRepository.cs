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

        public int getNumberOfDocuments(int typeOfMove)
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
    }
}
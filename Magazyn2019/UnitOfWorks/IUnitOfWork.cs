using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazyn2019.UnitOfWorks
{
    interface IUnitOfWork : IDisposable
    {
        int Complete();
    }
}

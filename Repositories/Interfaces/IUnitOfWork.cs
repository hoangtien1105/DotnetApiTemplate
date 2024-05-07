using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAccountRepository AccountRepository { get; }

        Task<int> SaveChangeAsync();
    }
}

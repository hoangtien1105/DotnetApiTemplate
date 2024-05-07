using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TemplateDbContext _templateDbContext;
        private readonly IAccountRepository _accountRepository;

        public UnitOfWork(TemplateDbContext templateDbContext, IAccountRepository accountRepository)
        {
            _templateDbContext = templateDbContext;
            _accountRepository = accountRepository;
        }

        public IAccountRepository AccountRepository { get { return _accountRepository; } }

        public Task<int> SaveChangeAsync()
        {
            return _templateDbContext.SaveChangesAsync();
        }
    }
}

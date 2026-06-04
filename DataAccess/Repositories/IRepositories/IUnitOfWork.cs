using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Employee> EmployeeRepository { get; }
        IRepository<Qualification> QualificationRepository { get; }
        IRepository<Vacation> VacationRepository { get; }

        Task CommitAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}

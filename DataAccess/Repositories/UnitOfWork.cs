using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;

        private IRepository<Employee> _employeeRepository;
        private IRepository<Qualification> _qualificationRepository;
        private IRepository<Vacation> _vacationRepository;
        public IRepository<Employee> EmployeeRepository =>
           _employeeRepository ??= new Repository<Employee>(_context);
        public IRepository<Qualification> QualificationRepository => 
            _qualificationRepository ??=  new Repository<Qualification>(_context);


        public IRepository<Vacation> VacationRepository =>
            _vacationRepository ??= new Repository<Vacation>(_context);

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}

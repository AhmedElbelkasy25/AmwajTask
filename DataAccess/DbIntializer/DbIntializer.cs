using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DbIntializer
{
    public class DbIntializer : IDbIntializer
    {
        private readonly ApplicationDbContext _context;


        public DbIntializer(ApplicationDbContext context)
        {
            _context = context;
           
        }

        public void Initialize()
        {
            try
            {

                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
                if (!_context.Qualifications.Any())
                {
                    _context.Qualifications.AddRange(
                        new Qualification { Title = "دبلوم" },
                        new Qualification { Title = "بكالريوس" },
                        new Qualification { Title = "ماجيستير" },
                        new Qualification { Title = "دكتوراه" }
                    );
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}

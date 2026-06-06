using DataAccess.DbIntializer;
using Microsoft.EntityFrameworkCore;
using Service;
using Service.IServices;

namespace AmwajTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                 .AddCookieTempDataProvider();

            //add db context

            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });




            // add Services
            builder.Services.AddScoped<IRepository<Employee>, Repository<Employee>>();
            builder.Services.AddScoped<IRepository<Qualification>, Repository<Qualification>>();
            builder.Services.AddScoped<IRepository<Vacation>, Repository<Vacation>>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork > ();
            builder.Services.AddScoped<IEmployeeService, EmployeeService> ();
            builder.Services.AddScoped<IDbIntializer, DbIntializer>();
            builder.Services.AddScoped<IVacationService, VacationService>();

            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseSession();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Db Intializer

            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbIntializer>();
                dbInitializer.Initialize();
            }

            app.Run();
        }
    }
}

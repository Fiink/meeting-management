
using MeetingManagementSystem.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure ORM
            builder.Services.AddDbContext<MeetingDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure services

            // Configure REST controllers
            builder.Services.AddControllers();
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                Console.WriteLine("TODO Set up swagger");
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

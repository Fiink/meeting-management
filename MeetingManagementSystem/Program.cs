
using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Interfaces;
using MeetingManagementSystem.Data.Repositories;
using MeetingManagementSystem.Services.Implementations;
using MeetingManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

            // Configure repositories 
            builder.Services.AddScoped<IMeetingRoomRepository, MeetingRoomRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Configure services
            builder.Services.AddScoped<IUserServiceAsync, UserService>();
            builder.Services.AddScoped<IMeetingServiceAsync, MeetingService>();

            // Configure REST controllers
            builder.Services.AddControllers();
            // Configure Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Meeting Management System API", Version = "v1" });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "Meeting Management System API");
                });
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

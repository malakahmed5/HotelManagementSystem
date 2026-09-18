using HMS.API.WebApplicationRegister;
using HMS.Core.Contracts;
using HMS.Infrastructure.Data.DbContexts;
using HMS.Infrastructure.UnirOfWork;
using HMS.Services.Abstraction;
using HMS.Services.Services;
using HMS.Services.Services.AutoMapper;
using HMS.Services.Services.AutoMapper.RoomModule;
using HMS.Services.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HMS.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region DI Container
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<HotelDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddAutoMapper(options => options.AddMaps(typeof(ServiceAssemblyReference).Assembly));
            builder.Services.AddTransient<RoomImageResolver>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddTransient<IAttachmentService, AttachmentService>();

            #endregion

            var app = builder.Build();
            await app.MigrateDatabaseAsync();

            #region Middlewares [Piplines]

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}

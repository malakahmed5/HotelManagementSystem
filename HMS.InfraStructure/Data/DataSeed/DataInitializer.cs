using HMS.Core.Contracts;
using HMS.Core.Entiites;
using HMS.Core.Entiites.AuthModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Data.DataSeed
{
    public class DataInitializer : IDataInitializer
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataInitializer(UserManager<HotelUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task InitializeAsync()
        {
            await SeedAdminAndRoleAsync();
            //await SeedSystemDataAsync();
        }

        private async Task SeedAdminAndRoleAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Staff" });
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Guest" });
            }
            if (!_userManager.Users.Any())
            {
                var admin = new HotelUser()
                {
                    FullName = "Admin.Hotel",
                    Email = "admin.HMS@gmail.com",
                    UserName = "Admin_HMS",
                    PhoneNumber = "01123456789",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                };
                await _userManager.CreateAsync(admin , "P@ssw0rd");
                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        #region Helper Methods
        private static async Task SeedData<Tkey,TEntity>(DbSet<TEntity> entity, string fileName)
           where TEntity : BaseEntity<Tkey>
        {
            var dataLoaded = await LoadDataFromJson<TEntity>(fileName);
            if (dataLoaded != null)
                entity.AddRange(dataLoaded);
        }
        private static async Task<List<TEntity>> LoadDataFromJson<TEntity>(string fileName)
        {
            var filePath = @"..\FleetCarePro.Persistence\Data\DataSeed\JsonFiles\" + fileName;
            if (!File.Exists(filePath)) return [];
            var dataStream = File.OpenRead(filePath);
            if (dataStream is null || dataStream.Length == 0) return [];
            var data = await JsonSerializer.DeserializeAsync<List<TEntity>>(dataStream
                , new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return data ?? [];
        }
        #endregion

    }
}

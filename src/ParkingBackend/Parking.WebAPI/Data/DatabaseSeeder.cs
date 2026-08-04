using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking.WebAPI.Models;

namespace Parking.WebAPI.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // First, ensure the database is created and all migrations are applied
            await context.Database.MigrateAsync();

            bool needsSave = false;

            // 1. Seed 5 Dummy Users
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User { Id = Guid.NewGuid(), Name = "Alice Smith" },
                    new User { Id = Guid.NewGuid(), Name = "Bob Jones" },
                    new User { Id = Guid.NewGuid(), Name = "Charlie Brown" },
                    new User { Id = Guid.NewGuid(), Name = "Diana Prince" },
                    new User { Id = Guid.NewGuid(), Name = "Evan Wright" }
                };

                await context.Users.AddRangeAsync(users);
                needsSave = true;
            }

            // 2. Seed 20 Dummy Parking Slots
            if (!await context.ParkingSlots.AnyAsync())
            {
                var slots = new List<ParkingSlot>();

                // Generating slots A01 through A20
                for (int i = 1; i <= 20; i++)
                {
                    slots.Add(new ParkingSlot
                    {
                        Id = Guid.NewGuid(),
                        Designation = $"A{i:D2}"
                    });
                }

                await context.ParkingSlots.AddRangeAsync(slots);
                needsSave = true;
            }

            // Save changes only if new entities were added
            if (needsSave)
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
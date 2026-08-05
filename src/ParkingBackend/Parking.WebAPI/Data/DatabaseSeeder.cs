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
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { Id = Guid.NewGuid(), Name = "Alice (All)", AuthorizedSlotTypes = new List<SlotType> { SlotType.Regular, SlotType.VIP, SlotType.Electric, SlotType.Accessible } },
                    new User { Id = Guid.NewGuid(), Name = "Bob (Electric)", AuthorizedSlotTypes = new List<SlotType> { SlotType.Regular, SlotType.Electric } },
                    new User { Id = Guid.NewGuid(), Name = "Charlie (Accessible)", AuthorizedSlotTypes = new List<SlotType> { SlotType.Regular, SlotType.Accessible } },
                    new User { Id = Guid.NewGuid(), Name = "Dave (Regular)", AuthorizedSlotTypes = new List<SlotType> { SlotType.Regular } },
                    new User { Id = Guid.NewGuid(), Name = "Eve (VIP)", AuthorizedSlotTypes = new List<SlotType> { SlotType.Regular, SlotType.VIP } }
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

                slots[0].Type = SlotType.Accessible;
                slots[1].Type = SlotType.Electric;
                slots[5].Type = SlotType.Accessible;
                slots[6].Type = SlotType.VIP;
                slots[10].Type = SlotType.Electric;

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
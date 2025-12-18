using CalendarApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CalendarApp.Infrastructure.Database.Seeding
{
	public class UsersInit
	{
		public List<User> GenerateUsers()
		{
			List<User> entities = new List<User>();

			var entity1 = new User()
			{
				Id = -1,
				UserName = "admin",
				Email = "admin@calendarapp.com",
				UpdatedAt = new DateTime(2025, 11, 7, 0, 0, 0)
			};

			var entity2 = new User()
			{
				Id = -2,
				UserName = "alexjohnson",
				Email = "alex.johnson@email.com",
				UpdatedAt = new DateTime(2025, 11, 7, 0, 0, 0)
			};

			var entity3 = new User()
			{
				Id = -3,
				UserName = "sarahchen",
				Email = "sarah.chen@email.com",
				UpdatedAt = new DateTime(2025, 11, 12, 0, 0, 0)
			};

			var entity4 = new User()
			{
				Id = -4,
				UserName = "mikedavis",
				Email = "mike.davis@email.com",
				UpdatedAt = new DateTime(2025, 11, 17, 0, 0, 0)
			};

			entities.Add(entity1);
			entities.Add(entity2);
			entities.Add(entity3);
			entities.Add(entity4);

			return entities;
		}

		public Dictionary<string, string> GetUserRoleMappings()
		{
			return new Dictionary<string, string>
			{
				{ "admin@calendarapp.com", "Admin" },
				{ "alex.johnson@email.com", "User" },
				{ "sarah.chen@email.com", "User" },
				{ "mike.davis@email.com", "User" }
			};
		}

		public async Task SeedIdentityUsersAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
		{
			// Create roles if they don't exist
			if (!await roleManager.RoleExistsAsync("Admin"))
			{
				await roleManager.CreateAsync(new Role { Name = "Admin" });
			}

			if (!await roleManager.RoleExistsAsync("User"))
			{
				await roleManager.CreateAsync(new Role { Name = "User" });
			}

			// Get existing user data
			var users = GenerateUsers();
			var roleMappings = GetUserRoleMappings();

			// Seed admin user
			var adminUser = users.First(u => u.UserName == "admin");
			if (await userManager.FindByNameAsync(adminUser.UserName!) == null)
			{
				adminUser.EmailConfirmed = true;
				var result = await userManager.CreateAsync(adminUser, "Admin123!");
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "Admin");
				}
			}

			// Seed regular users
			var regularUsers = users.Where(u => u.UserName != "admin").ToList();
			foreach (var user in regularUsers)
			{
				if (await userManager.FindByNameAsync(user.UserName!) == null)
				{
					user.EmailConfirmed = true;
					var result = await userManager.CreateAsync(user, "User123!");
					if (result.Succeeded && roleMappings.TryGetValue(user.Email!, out var role))
					{
						await userManager.AddToRoleAsync(user, role);
					}
				}
			}
		}
	}

}

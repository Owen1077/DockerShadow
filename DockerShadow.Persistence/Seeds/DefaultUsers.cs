using DockerShadow.Domain.Constant;
using DockerShadow.Domain.Entities;
using DockerShadow.Domain.Enum;

namespace DockerShadow.Persistence.Seeds
{
    public static class DefaultUsers
    {
        public static List<User> UserList()
        {
            return new List<User>()
            {
                new() {
                    Id = RoleConstants.AdministratorUser,
                    UserName = "shadao",
                    Email = "Oluwatosin.Shada@ACCESSBANKPLC.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    // Password@123
                    PasswordHash = "AQAAAAEAACcQAAAAEBLjouNqaeiVWbN0TbXUS3+ChW3d7aQIk/BQEkWBxlrdRRngp14b0BIH0Rp65qD6mA==",
                    NormalizedEmail= "OLUWATOSIN.SHADA@ACCESSBANKPLC.COM",
                    NormalizedUserName="SHADAO",
                    Name = "Oluwatosin Shada",
                    Status = UserStatus.Active,
                    IsLoggedIn = false,
                    ConcurrencyStamp = "71f781f7-e957-469b-96df-9f2035147e45",
                    SecurityStamp = "71f781f7-e957-469b-96df-9f2035147e93",
                    AccessFailedCount = 0,
                    LockoutEnabled = false,
                    LastLoginTime = DateTime.Parse("2023-10-20"),
                    CreatedAt = DateTime.Parse("2023-10-20"),
                    UpdatedAt = DateTime.Parse("2023-10-20")
                },
                new() {
                    Id = RoleConstants.LogUser,
                    UserName = "ohuet",
                    Email = "Thelma.Ohue@ACCESSBANKPLC.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    // Password@123
                    PasswordHash = "AQAAAAEAACcQAAAAEBLjouNqaeiVWbN0TbXUS3+ChW3d7aQIk/BQEkWBxlrdRRngp14b0BIH0Rp65qD6mA==",
                    NormalizedEmail= "THELMA.OHUE@ACCESSBANKPLC.COM",
                    NormalizedUserName="OHUET",
                    Name = "Thelma Ohue",
                    Status = UserStatus.Active,
                    IsLoggedIn = false,
                    ConcurrencyStamp = "71f781f7-e957-469b-96df-9f2035147e98",
                    SecurityStamp = "71f781f7-e957-469b-96df-9f2035147e37",
                    AccessFailedCount = 0,
                    LockoutEnabled = false,
                    LastLoginTime = DateTime.Parse("2023-10-20"),
                    CreatedAt = DateTime.Parse("2023-10-20"),
                    UpdatedAt = DateTime.Parse("2023-10-20")
                }
            };
        }
    }
}
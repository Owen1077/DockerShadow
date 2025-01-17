using DockerShadow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DockerShadow.Persistence.Seeds
{
    public static class ContextSeed
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            CreateRoles(modelBuilder);

            CreateJwtUsers(modelBuilder);

            MapUserRole(modelBuilder);
        }

        private static void CreateRoles(ModelBuilder modelBuilder)
        {
            List<Role> roles = DefaultRoles.IdentityRoleList();
            modelBuilder.Entity<Role>().HasData(roles);
        }

        private static void CreateJwtUsers(ModelBuilder modelBuilder)
        {
            List<User> users = DefaultUsers.UserList();
            modelBuilder.Entity<User>().HasData(users);
        }

        private static void MapUserRole(ModelBuilder modelBuilder)
        {
            var identityUserRoles = MappingUserRole.IdentityUserRoleList();
            modelBuilder.Entity<UserRole>().HasData(identityUserRoles);
        }
    }
}

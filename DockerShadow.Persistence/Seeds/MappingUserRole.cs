using DockerShadow.Domain.Constant;
using DockerShadow.Domain.Entities;

namespace DockerShadow.Persistence.Seeds
{
    public static class MappingUserRole
    {
        public static List<UserRole> IdentityUserRoleList()
        {
            return new List<UserRole>()
            {
                new UserRole
                {
                    RoleId = RoleConstants.Log,
                    UserId = RoleConstants.LogUser
                },
                new UserRole
                {
                    RoleId = RoleConstants.Administrator,
                    UserId = RoleConstants.AdministratorUser
                }
            };
        }
    }
}

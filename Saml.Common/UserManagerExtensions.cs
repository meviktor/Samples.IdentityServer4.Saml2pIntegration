using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public static class UserManagerExtensions
    {
        public static async Task<bool> ValidateCredentials(this UserManager<IdentityUser> userStore, string username, string password)
        {
            IdentityUser findUser = await userStore.FindByNameAsync(username);
            if (findUser != null)
            {
                return await userStore.CheckPasswordAsync(findUser, password);
            }
            else return false;
        }

        public static IdentityUser FindByExternalProvider(this UserManager<IdentityUser> userStore, string provider, string providerUserId)
        {
            return userStore.Users.Where(x => x);
        }
    }
}
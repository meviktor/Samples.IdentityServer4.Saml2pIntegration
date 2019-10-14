using System.Threading.Tasks;
using sp.Data;
using System.Linq;

namespace Microsoft.AspNetCore.Identity
{
    public static class UserManagerExtensions
    {
        public static async Task<bool> ValidateCredentials(this UserManager<ApplicationUser> userStore, string username, string password)
        {
            ApplicationUser findUser = await userStore.FindByNameAsync(username);
            if (findUser != null)
            {
                return await userStore.CheckPasswordAsync(findUser, password);
            }
            else return false;
        }

        public static ApplicationUser FindByExternalProvider(this UserManager<ApplicationUser> userStore, string providerName, string providerUserId)
        {
            return userStore.Users.Where(x => 
                x.ProviderName == providerName && x.ProviderUserId == providerUserId
            ).FirstOrDefault();
        }
    }
}
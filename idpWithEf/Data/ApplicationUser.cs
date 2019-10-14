using Microsoft.AspNetCore.Identity;

namespace idpWithEf.Data
{
    /// <summary>
    /// Model of an application user.
    /// </summary>
   public class ApplicationUser : IdentityUser
   {
       /// <summary>
       /// Name of the external provider.
       /// </summary>
       public string ProviderName { get; set; }

       /// <summary>
       /// Identifier of the account at the external provider.
       /// </summary>
       public string ProviderUserId { get; set; }

       /// <summary>
       /// Gets or sets if the user is active.
       /// </summary>
       public bool IsActive { get; set; }
   }
}
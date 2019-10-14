using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Saml.EntityFramework.DbContexts;
using IdentityServer4.Saml.EntityFramework.Interfaces;
using IdentityServer4.Saml.EntityFramework.Mappers;
using IdentityServer4.Saml.EntityFramework.Stores;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using idpWithEf.Data;


namespace idpWithEf
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration){
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllersWithViews();

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.AuthenticationDisplayName = "Windows";
            });

            // SAML SP database (DbContext)
            /*services.AddDbContext<SamlConfigurationDbContext>(db => 
                db.UseInMemoryDatabase("ServiceProviders"));*/
            services.AddDbContext<SamlConfigurationDbContext>(db => db.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ISamlConfigurationDbContext, SamlConfigurationDbContext>();

            services.AddDbContext<ApplicationDbContext>(db => db.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddTestUsers(TestUsers.Users)
                .AddSigningCredential(new X509Certificate2("idsrv3test.pfx", "idsrv3test"))
                .AddSamlPlugin(options =>
                {
                    options.Licensee = "DEMO";
                    options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMTktMTEtMDhUMDE6MDA6MDMuNDMyMTYzKzAwOjAwIiwiaWF0IjoiMjAxOS0xMC0wOVQwMDowMDowMyIsIm9yZyI6IkRFTU8iLCJhdWQiOjJ9.oAEHvlhdsHfe8nVsxFxYT0daP+BBjgFImrA41Ge8PAvzr/PzAT9sEuEvEDPLXrJ1w16mzToMnCTXdIMfYAhpWICaeQyknJGSE2bOj9a4u00smzZfWW3+7lH9M5sGiGgppsZOrrV9OsnyaUiBbfUDNh1inB2wDwbHjzee/M7keVW/9uHdSYIY/Z0wcFvECS+/iDHAZx99XC9x3CogqMtPaA1W2AQsjXL97Dao9iYKkBk4XA+dTSMLHXeIbCvii/0/BSwE+waENKbhVe1P1KXNzzvOrHQrV6jbGIk4KeYAXWlxoj12xuQZ0YZTUUVQESqioCz/Kccj4a8Wxnkmun3rr2mtjIFznM+RMPmnL2Qrtc+POqZ3jbXiKQA1tFTmuLpGRb/A/X7SOgFz3cljwc0OAiF+006pUutIKttGoNwbQPxI7A3kFmTBCjY6uSpOFnTwo+1+nzJ7JYShL5wknapdU4kvdial6b2iyhaPLwa0KnpnrNyO+omxDgHGQ32+OZtMxhuaoQg54EbKvob/tP3Ik52rLbIctcgVB7oici+BnezpYN28cNaQgO/Z3pQKSY6t15Vp/kNsllvcyUS+sZPRMoZKpb6j+yLwXil3I9mV3TJmr902LPU3G/arz/p+JfyUX36/ugciHWP9XsIw0bTYA68I6+MngrCl5ZaDp5Oz4Cw=";
                    options.WantAuthenticationRequestsSigned = false;
                })
                // Tell IdentityServer about new SAML SP database
                .AddServiceProviderStore<ServiceProviderStore>();
                //.AddInMemoryServiceProviders(Config.GetServiceProviders());
            
            //Add ASP.NET Core Identity to IdentityServer.
            builder.AddAspNetIdentity<IdentityUser>();

            // in-memory, code config
            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddInMemoryApiResources(Config.GetApis());
            builder.AddInMemoryClients(Config.GetClients());

            // in-memory, json config
            //builder.AddInMemoryIdentityResources(Configuration.GetSection("IdentityResources"));
            //builder.AddInMemoryApiResources(Configuration.GetSection("ApiResources"));
            //builder.AddInMemoryClients(Configuration.GetSection("clients"));

            builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme,
                cookie => { cookie.Cookie.Name = "idsrv.idp"; });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            SeedServiceProviderDatabase(app);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer()
               .UseIdentityServerSamlPlugin();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        private void SeedServiceProviderDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<SamlConfigurationDbContext>();
                if (!context.ServiceProviders.Any())
                {
                    foreach (var serviceProvider in Config.GetServiceProviders())
                    {
                        context.ServiceProviders.Add(serviceProvider.ToEntity());
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}

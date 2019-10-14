using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;

namespace sp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllersWithViews();

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.AuthenticationDisplayName = "Windows";
            });

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddTestUsers(TestUsers.Users)
                .AddSigningCredential(new X509Certificate2("testclient.pfx", "test"));

            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddInMemoryApiResources(Config.GetApis());
            builder.AddInMemoryClients(Config.GetClients())
                .AddSamlPlugin(options =>
                {
                    options.Licensee = "DEMO";
                    options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMTktMTEtMDhUMDE6MDA6MDMuNDMyMTYzKzAwOjAwIiwiaWF0IjoiMjAxOS0xMC0wOVQwMDowMDowMyIsIm9yZyI6IkRFTU8iLCJhdWQiOjJ9.oAEHvlhdsHfe8nVsxFxYT0daP+BBjgFImrA41Ge8PAvzr/PzAT9sEuEvEDPLXrJ1w16mzToMnCTXdIMfYAhpWICaeQyknJGSE2bOj9a4u00smzZfWW3+7lH9M5sGiGgppsZOrrV9OsnyaUiBbfUDNh1inB2wDwbHjzee/M7keVW/9uHdSYIY/Z0wcFvECS+/iDHAZx99XC9x3CogqMtPaA1W2AQsjXL97Dao9iYKkBk4XA+dTSMLHXeIbCvii/0/BSwE+waENKbhVe1P1KXNzzvOrHQrV6jbGIk4KeYAXWlxoj12xuQZ0YZTUUVQESqioCz/Kccj4a8Wxnkmun3rr2mtjIFznM+RMPmnL2Qrtc+POqZ3jbXiKQA1tFTmuLpGRb/A/X7SOgFz3cljwc0OAiF+006pUutIKttGoNwbQPxI7A3kFmTBCjY6uSpOFnTwo+1+nzJ7JYShL5wknapdU4kvdial6b2iyhaPLwa0KnpnrNyO+omxDgHGQ32+OZtMxhuaoQg54EbKvob/tP3Ik52rLbIctcgVB7oici+BnezpYN28cNaQgO/Z3pQKSY6t15Vp/kNsllvcyUS+sZPRMoZKpb6j+yLwXil3I9mV3TJmr902LPU3G/arz/p+JfyUX36/ugciHWP9XsIw0bTYA68I6+MngrCl5ZaDp5Oz4Cw=";
                    options.WantAuthenticationRequestsSigned = false;
                })
                .AddInMemoryServiceProviders(Config.GetServiceProviders());

            services.AddAuthentication()
                .AddSaml2p("saml2p", options => {
                    options.Licensee = "DEMO";
                    options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMTktMTEtMDhUMDE6MDA6MDMuNDMyMTYzKzAwOjAwIiwiaWF0IjoiMjAxOS0xMC0wOVQwMDowMDowMyIsIm9yZyI6IkRFTU8iLCJhdWQiOjJ9.oAEHvlhdsHfe8nVsxFxYT0daP+BBjgFImrA41Ge8PAvzr/PzAT9sEuEvEDPLXrJ1w16mzToMnCTXdIMfYAhpWICaeQyknJGSE2bOj9a4u00smzZfWW3+7lH9M5sGiGgppsZOrrV9OsnyaUiBbfUDNh1inB2wDwbHjzee/M7keVW/9uHdSYIY/Z0wcFvECS+/iDHAZx99XC9x3CogqMtPaA1W2AQsjXL97Dao9iYKkBk4XA+dTSMLHXeIbCvii/0/BSwE+waENKbhVe1P1KXNzzvOrHQrV6jbGIk4KeYAXWlxoj12xuQZ0YZTUUVQESqioCz/Kccj4a8Wxnkmun3rr2mtjIFznM+RMPmnL2Qrtc+POqZ3jbXiKQA1tFTmuLpGRb/A/X7SOgFz3cljwc0OAiF+006pUutIKttGoNwbQPxI7A3kFmTBCjY6uSpOFnTwo+1+nzJ7JYShL5wknapdU4kvdial6b2iyhaPLwa0KnpnrNyO+omxDgHGQ32+OZtMxhuaoQg54EbKvob/tP3Ik52rLbIctcgVB7oici+BnezpYN28cNaQgO/Z3pQKSY6t15Vp/kNsllvcyUS+sZPRMoZKpb6j+yLwXil3I9mV3TJmr902LPU3G/arz/p+JfyUX36/ugciHWP9XsIw0bTYA68I6+MngrCl5ZaDp5Oz4Cw=";

                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "http://localhost:5000",
                        SigningCertificate = new X509Certificate2("idsrv3test.cer"),
                        SingleSignOnEndpoint = new SamlEndpoint("http://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("http://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
                    };

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "http://localhost:5001/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = true,
                        SigningCertificate = new X509Certificate2("testclient.pfx", "test")
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml";
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer()
               .UseIdentityServerSamlPlugin();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}

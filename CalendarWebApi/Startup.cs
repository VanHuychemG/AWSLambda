using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;

namespace CalendarWebApi
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // add our Cognito group authorization requirement, specifying CalendarWriter as the group
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
                options.AddPolicy("InCalendarWriterGroup", policy => policy.Requirements.Add(new CognitoGroupAuthorizationRequirement("CalendarWriter")));
            });

            // add a singleton of our cognito authorization handler

            services.AddSingleton<IAuthorizationHandler, CognitoGroupAuthorizationHandler>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Audience = "<Cognito App Client Id>";
                o.Authority = "https://cognito-idp.eu-west-1.amazonaws.com/<Cognito Pool Id>";
                o.RequireHttpsMetadata = false;
            });

            services.AddControllers();

            // Pull in any SDK configuration from Configuration object
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLambdaLogger(Configuration.GetLambdaLoggerOptions());

            app.UseDefaultFiles(); //needs to be before the app.UseStaticFiles() call below

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

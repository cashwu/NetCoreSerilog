using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Context;
using testSerilog.Filter;

namespace testSerilog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<SerilogLoggingActionFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            // app.UseMiddleware<LogUserNameMiddleware>();
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default",
                                             pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class LogUserNameMiddleware
    {
        private readonly RequestDelegate _next;

        public LogUserNameMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            LogContext.PushProperty("UserName", context.User?.Identity?.Name ?? "Not Login");

            return _next(context);
        }
    }
    
    public static class LogHelper 
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if(request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint != null) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using SgartCore3Angular1Todo.Models;

namespace SgartCoreAngular1Todo
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // leggo la configurazione
      services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

      //https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-3.0
      //services.AddMemoryCache();

      // services.AddSingleton<IDBService, DBService>();
      // services.AddScoped<IServiceManager, ServiceManager>();
      // inject del manager
      services.AddScoped<ServerApp.ServiceManager>();

      services.AddControllersWithViews()
        // uso Newtonsoft anziche il default System.Text.Json perche non ho trovato il DateTimeZoneHandling
        .AddNewtonsoftJson(options =>
        {
          //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
          //options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
          options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}

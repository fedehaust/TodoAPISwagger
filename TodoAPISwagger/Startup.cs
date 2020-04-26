using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApi.Models;

namespace TodoAPISwagger
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
            services.AddDbContext<TodoContext>(opt =>
              opt.UseInMemoryDatabase("TodoList"));
            services.AddControllers();

            services.AddSwaggerGen(setup =>
            {

              setup.SwaggerDoc(
              $"TodoApiSwagger",
              new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                  Title = "Todo API",
                  Version = "v1",
                  Description = "Aplicación de ejemplo para usar en charla de GDG Córdoba Argentina",
                  Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                  {
                      Email = "fedehaust@gmail.com",
                      Name = "Federico Haustein",
                      Url = new Uri("https://www.federicohaustein.com")
                  },
                  License = new Microsoft.OpenApi.Models.OpenApiLicense()
                  {
                      Name = "MIT License",
                      Url = new Uri("https://opensource.org/licenses/MIT")
                  }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UseSwagger(); //https://localhost:5001/swagger/TodoApiSwagger/swagger.json

            app.UseSwaggerUI(setup =>
            {
              setup.SwaggerEndpoint("/swagger/TodoApiSwagger/swagger.json", "Todo API");
              setup.RoutePrefix = "";
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}


using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PRA_WebAPI.Context;

namespace PRA_WebAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container. test test
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options => {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            
            
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);

            services.AddMvc();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddDbContext<PRAQuizContext>(optionsAction => {
            
                optionsAction.UseSqlServer(_configuration.GetConnectionString("PRAConnectionString"));
            });
            //
            // services.AddScoped(typeof(IRepository<>), typeof(SQLRepository<>));
           

       

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

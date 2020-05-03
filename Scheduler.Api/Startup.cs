using AutoMapper;
//using AutoMapper.Extensions.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Scheduler.Api.ViewModels.Mappings;
using Scheduler.Data;
using Scheduler.Data.Repositories;
using Scheduler.Data.Abstract;

namespace Scheduler.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public object SchedulerDbInitializer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string sqlConnection = Configuration.GetConnectionString("SchedulerDb");
            services.AddDbContext<SchedulerContext>(options =>
                options.UseSqlServer(sqlConnection,
                b => b.MigrationsAssembly("Scheduler.Api")));

            // Repositories
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAttendeeRepository, AttendeeRepository>();
          //  services.AddScoped<IBusinessSchedulesRepository, BusinessScheduleRepository>();

            services.AddAutoMapper(typeof(Startup));

            // AutoMapperConfiguration.Configure();

            // Enable Cors

            services.AddCors();

            // Add MVC services to the services container.
            services.AddMvc()
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver =
                    new DefaultContractResolver());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Scheduler.Data.SchedulerDbInitializer.Initialize(app.ApplicationServices);
        }
    }
}



namespace DatingApp.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using DatingApp.API.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using System.Net;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using DatingApp.API.Helpers;
    using AutoMapper;

    public class Startup
    {
        public Startup(IConfiguration configuration) //, IHostingEnvironment env) //,  
        {
            Configuration = configuration;
           // Env = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Env {get;}

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<Datacontext>(x =>x.UseSqlite("ConnectionString"));
            services.AddDbContext<DataContext>(x =>x.UseSqlite(Configuration.GetConnectionString("DefaultConnction")));

            services.AddMvc().AddJsonOptions(opt => {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)

            //get info from localhostt500 and share to localhost4200
            services.AddCors();

            // bind CloudinarySettings properties with appsetings.json values
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            // Mapper.Reset();
             services.AddAutoMapper();

            services.AddTransient<Seed>();  

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DataRepository>();

            //authenfication tiwh tokens - middleware
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                    .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
    
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                Mapper.Reset();
            }
           else
            {
                
                // Setting a global exception handler
                app.UseExceptionHandler(builder => {
                    builder.Run(async context=> {
                         context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                         var error = context.Features.Get<IExceptionHandlerFeature>();
                         if(error != null)
                         {
                             // Add header to response
                             context.Response.AddApplicationError(error.Error.Message);
                             await context.Response.WriteAsync(error.Error.Message);
                         }
                    });
                });
            }
            
            // Populate db Users table when app starts
           // seeder.SeedUsers();

            //get info from localhostt500 and share to localhost4200
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();    

            app.UseMvc();
        }
    }
}

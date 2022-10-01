using System.Text.Json.Serialization;
using authorization_microservice.Database;
using authorization_microservice.DatabaseModels;
using authorization_microservice.Repositories.Implementations;
using authorization_microservice.Repositories.Interfaces;
using authorization_microservice.Services.Implementations;
using authorization_microservice.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace authorization_microservice;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
        
        services.AddDbContext<ApplicationContext>(options => 
                options.UseNpgsql("Host=test.database.ru;Port=5112;Database=authTokenDb;User ID=postgres;Password=testPassword;"));
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSingleton<IActiveDirectoryService, ActiveDirectoryService>();
        services.AddSingleton<ITokenService>(d => new TokenService(new TokenServiceConfiguration
        {
            RsaPrivateKey = "-----BEGIN PRIVATE KEY-----private key here-----END PRIVATE KEY-----",
            RsaPublicKey = "-----BEGIN PUBLIC KEY-----public key here-----END PUBLIC KEY-----",
        }));

        services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
    }
 
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
 
        app.UseHttpsRedirection();
 
        app.UseRouting();
 
        app.UseAuthorization();
 
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
using Microsoft.EntityFrameworkCore;
using MottuMotoRental.Application.UseCases;
using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Infrastructure.Data;
using MottuMotoRental.Infrastructure.FileStorage;
using MottuMotoRental.Infrastructure.Repositories;
using MottuMotoRental.Infrastructure.Messaging;
using MottuMotoRental.Infrastructure.Consumers;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Identity;
using MottuMotoRental.Infrastructure.Data.Seed;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.API.Configuration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MotoRental", Version = "v1" });

    // Define the Bearer authentication scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization", 
        In = ParameterLocation.Header, 
        Type = SecuritySchemeType.ApiKey, 
        Scheme = "Bearer"
    });

    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
});

builder.Services.AddDependencies(builder.Configuration);


builder.Services.AddDbContext<MotoRentalContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();


builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
builder.Services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<GetMotorcyclesUseCase>();
builder.Services.AddScoped<UpdateMotorcycleLicensePlateUseCase>();
builder.Services.AddScoped<DeleteMotorcycleUseCase>();
builder.Services.AddScoped<RegisterMotorcycleUseCase>();
builder.Services.AddScoped<RegisterDeliveryPersonUseCase>();
builder.Services.AddScoped<CreateRentalUseCase>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();


builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var hostname = configuration["RabbitMQ:HostName"];
    var userName = configuration["RabbitMQ:UserName"];
    var password = configuration["RabbitMQ:Password"];
    var virtualHost = configuration["RabbitMQ:VirtualHost"];

    if (string.IsNullOrEmpty(hostname))
    {
        throw new ArgumentNullException(nameof(hostname), "RabbitMQ HostName is not configured.");
    }

    return new ConnectionFactory
    {
        HostName = hostname,
        Port = 5672,
        UserName = userName,
        Password = password,
        VirtualHost = string.IsNullOrEmpty(virtualHost) ? "/" : virtualHost 
    };
});


builder.Services.AddSingleton<IEventPublisher>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var hostname = configuration["RabbitMQ:HostName"];
    var queueName = "motorcycle_events_queue";
    var userName = configuration["RabbitMQ:UserName"];
    var password = configuration["RabbitMQ:Password"];
    var virtualHost = configuration["RabbitMQ:VirtualHost"];

    if (string.IsNullOrEmpty(hostname))
    {
        throw new ArgumentNullException(nameof(hostname), "RabbitMQ HostName is not configured.");
    }

    if (string.IsNullOrEmpty(queueName))
    {
        throw new ArgumentNullException(nameof(queueName), "RabbitMQ QueueName is not configured.");
    }

    return new EventPublisher(
        hostname: hostname,
        queueName: queueName,
        userName: userName ?? "rabbitmq",
        password: password ?? "rabbitmq",
        virtualHost: virtualHost ?? "/"
    );
});

builder.Services.AddHostedService<MotorcycleRegisteredConsumer>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers(); 

builder.Services.AddHttpContextAccessor();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors("AllowAll");

app.UseAuthorization(); 
app.UseAuthentication(); 

using (var scope = app.Services.CreateScope())
{

    var userManager = (UserManager<SystemUser>?)scope.ServiceProvider.GetService(typeof(UserManager<SystemUser>));
    var roleManager = (RoleManager<SystemRole>?)scope.ServiceProvider.GetService(typeof(RoleManager<SystemRole>));
    await DataBaseSeed.SeedAsync(userManager, roleManager);

}


app.MapControllers(); 

app.Run();

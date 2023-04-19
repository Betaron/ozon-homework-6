using FluentValidation.AspNetCore;
using Route256.Week5.Workshop.PriceCalculator.Api.NamingPolicies;
using Route256.Week5.Workshop.PriceCalculator.Bll.Extensions;
using Route256.Week5.Workshop.PriceCalculator.Dal.Extensions;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Extensions;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.HostedServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    });

services.AddEndpointsApiExplorer();

// add swagger
services.AddSwaggerGen(o =>
{
    o.CustomSchemaIds(x => x.FullName);
});

//add validation
services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
    conf.AutomaticValidationEnabled = true;
});

services.AddHostedService<DeliveryPriceCalculatorHostedService>();

//add dependencies
services
    .AddBll()
    .AddDalInfrastructure(builder.Configuration)
    .AddDalRepositories()
    .AddHostedServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MigrateUp();
app.Run();

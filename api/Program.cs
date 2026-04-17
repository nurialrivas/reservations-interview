using System.Data;
using api.Models.Validators;
using api.Utils;
using Controllers;
using Dapper;
using Db;
using FluentValidation;
using Microsoft.Data.Sqlite;
using Repositories;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);


{
    var Services = builder.Services;
    var connectionString =
        builder.Configuration.GetConnectionString("ReservationsDb")
        ?? "Data Source=reservations.db;Cache=Shared";

    SqlMapper.AddTypeHandler(new GuidTypeHandler());
    Services.AddSingleton(_ => new SqliteConnection(connectionString));
    Services.AddSingleton<IDbConnection>(sp => sp.GetRequiredService<SqliteConnection>());
    Services.AddSingleton<GuestRepository>();
    Services.AddSingleton<RoomRepository>();
    Services.AddSingleton<ReservationRepository>();
    Services.AddMvc(opt =>
    {
        opt.EnableEndpointRouting = false;
    });
    Services
        .AddFluentValidationAutoValidation()
        .AddValidatorsFromAssemblyContaining<ReservationValidator>();

    Services.AddCors();
    Services.AddEndpointsApiExplorer();
    Services.AddSwaggerGen();
}

var app = builder.Build();


{
    try
    {
        Setup.EnsureDb(app.Services.CreateScope());
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to setup the database, aborting");
        Console.WriteLine(ex.ToString());
        Environment.Exit(1);
        return;
    }

    app.UsePathBase("/api")
        .UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
        .UseMvc()
        .UseSwagger()
        .UseSwaggerUI();
}

app.Run();

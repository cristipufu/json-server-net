using JsonServer;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var database = new Database("db.json");

builder.Services.AddSingleton(database);
builder.Services.AddHostedService<DatabaseAutoSaver>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

foreach (var table in database.TableNames)
{
    app.MapGet($"/{table}", async (Database db, HttpContext context, ILogger<Program> _) =>
    {
        try
        {
            if (db.Tables.TryGetValue(table, out var rows))
            {
                context.Response.ContentType = "application/json";
                var json = JsonSerializer.Serialize(rows);
                await context.Response.WriteAsync(json);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"Table {table} not found");
            }
        }
        catch (Exception ex)
        {
            await ErrorHandler.HandleAsync(context, ex);
        }
    })
    .WithName($"GetAll{table}")
    .WithDescription($"Get All {table}")
    .WithOpenApi();

    app.MapGet($"/{table}/" + "{id}", async (Database db, HttpContext context, string id, ILogger<Program> logger) =>
    {
        try
        {
            var item = db.GetById(id, table);

            if (item == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"Item with id {id} from table {table} not found");
                return;
            }

            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(item);
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            await ErrorHandler.HandleAsync(context, ex);
        }
    })
    .WithName($"Get{table}ById")
    .WithDescription($"Get a single {table} by Id")
    .WithOpenApi();

    app.MapPost($"/{table}", async (Database db, HttpContext context, [FromBody] JsonElement body, ILogger<Program> logger) =>
    {
        try
        {
            var item = db.Insert(body, table);

            if (item == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"Table {table} not found");
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status201Created;
            var json = JsonSerializer.Serialize(item);
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            await ErrorHandler.HandleAsync(context, ex);
        }
    })
    .WithName($"Create{table}")
    .WithDescription($"Create a new {table}")
    .WithOpenApi();

    app.MapPut($"/{table}/{{id}}", async (Database db, HttpContext context, string id, [FromBody] JsonElement body, ILogger<Program> logger) =>
    {
        try
        {
            var updatedItem = db.Update(id, body, table);
            if (updatedItem == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"Item with id {id} from table {table} not found or table does not exist");
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status200OK;
            var json = JsonSerializer.Serialize(updatedItem);
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            await ErrorHandler.HandleAsync(context, ex);
        }
    })
    .WithName($"Update{table}")
    .WithDescription($"Update an existing item in {table}")
    .WithOpenApi();

    app.MapDelete($"/{table}/{{id}}", async (Database db, HttpContext context, string id, ILogger<Program> logger) =>
    {
        try
        {
            bool success = db.Delete(id, table);
            if (!success)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"Item with id {id} from table {table} not found");
                return;
            }

            context.Response.StatusCode = StatusCodes.Status204NoContent;
        }
        catch (Exception ex)
        {
            await ErrorHandler.HandleAsync(context, ex);
        }
    })
    .WithName($"Delete{table}")
    .WithDescription($"Delete an item from {table}")
    .WithOpenApi();
}

app.Run();
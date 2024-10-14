using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tf1DayOff.Api.StartupHelpers;
using Tf1DayOff.App.Commands;
using Tf1DayOff.Domain.Errors;
using Tf1DayOff.Domain.Services;
using Tf1DayOff.Domain.InfraInterfaces;
using Tf1DayOff.Infra.General;
using Tf1DayOff.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(x => x.Filters.Add<ApiExceptionFilterAttribute>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    x => x.OperationFilter<AddRequiredHeaderParameter>()
);

builder.Services.AddHttpContextAccessor();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<RequestDayOffCommand.Handler>();
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
});
builder.Services.AddValidatorsFromAssemblyContaining<RequestDayOffCommand.Handler>();

builder.Services.AddSingleton<IClock, CustomClock>();
builder.Services.AddSingleton<DayOffRequestsService>();
builder.Services.AddSingleton<IDayOffRequestsRepository, InFileStaticDayOffRequestsRepository>();
builder.Services.AddSingleton(new FileStorageSettings("C:\\_\\r\\t\\day-off\\data.json"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validationException)
        {
            context.Result = new BadRequestObjectResult(string.Join("\n", validationException.Errors.Select(x => x.ErrorMessage)));
            context.ExceptionHandled = true;
        }
        if (context.Exception is InvalidUserActionException invalidActionException)
        {
            context.Result = new BadRequestObjectResult(invalidActionException.Message);
            context.ExceptionHandled = true;
        }
        //
        base.OnException(context);
    }
}


namespace Tf1DayOFf.Api
{
    public class Program { }
}
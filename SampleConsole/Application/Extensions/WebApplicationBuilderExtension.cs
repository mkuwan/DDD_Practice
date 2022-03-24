using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace SampleConsole.Application.Extensions
{
    public static class WebApplicationBuilderExtension
    {

        public static void UseFluentValidationExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(x =>
            {
                x.Run(async Context =>
                {
                    var errorFeature = Context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature?.Error;

                    if (!(exception is ValidationException validationException))
                    {
                        throw exception;
                    }

                    var errors = validationException.Errors.Select(err => new 
                    {
                        err.PropertyName,
                        err.ErrorMessage
                    });

                    var errorText = JsonSerializer.Serialize(errors);
                    Context.Response.StatusCode = 400;
                    Context.Response.ContentType = "application/json";
                    await Context.Response.WriteAsync(errorText, Encoding.UTF8);
                });
            });
        }
    }
}

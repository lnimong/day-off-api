using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tf1DayOff.Api.Constants;
using Tf1DayOff.Api.Dtos;

namespace Tf1DayOff.UnitTests.TestInitTools;

public static class IWebHostBuilderTestExtension{


    public static IWebHostBuilder MockDependency<TService>(this IWebHostBuilder builder, TService mock)
        where TService : class
    {
        return builder.ConfigureTestServices(services =>
        {
            services.Replace(new ServiceDescriptor(typeof(TService), mock));
        });
    }
}

public static class Route
{
    public static string GetDayOff = "/day-off";
    public static string NewRequest = "/day-off/new-request";
    public static Func<Guid, string> ValidateRequest = x => $"/day-off/validate-request/{x}";
}

public static class HttpClientTestsExtension
{
    public static async Task<(HttpResponseMessage, T?)> TestGet<T>(this HttpClient client, string user, string route)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, route);
        request.Headers.Add(ApiConstants.XUser, user);
        var response = await client.SendAsync(request);
        return (response, (await response.Content.ReadFromJsonAsync<T>()));
    }

    public static async Task<HttpResponseMessage> TestPost(this HttpClient client, string user, string route, DayOffRequestDto body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, route);
        request.Headers.Add(ApiConstants.XUser, user);
        request.Content = JsonContent.Create(body);
        var response = await client.SendAsync(request);
        return response;
    }

    public static async Task<HttpResponseMessage> TestPost(this HttpClient client, string user, string route)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, route);
        request.Headers.Add(ApiConstants.XUser, user);
        var response = await client.SendAsync(request);
        return response;
    }


}


public static class HttpClientFactory
{
    public static HttpClient CreateHttpClientFor<TProgram>(Action<IWebHostBuilder>? registerMock = null)  where TProgram : class
    {
        var factory = new WebApplicationFactory<TProgram>()
            .WithWebHostBuilder(builder =>
            {
                //Environment.SetEnvironmentVariable...

                builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    //configurationBuilder.AddJsonFile...
                });


                builder.ConfigureTestServices(services =>
                {
                    //services.AddAuthentication(options => ...);
                });

                registerMock?.Invoke(builder);
            });

        return factory.CreateClient();
    }
}
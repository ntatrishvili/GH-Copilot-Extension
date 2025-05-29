using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

var appName = "CraftConfWorkshop";
var appVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/callback", ()=>"Please go home, take a rest and then restart your github chat maybe.");

app.MapPost("/", async ([FromHeader(Name = "X-Github-Token")] string githubToken, 
    [FromBody] Payload payload) =>
{
    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(appName, appVersion));
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", githubToken);

    payload.Messages.RemoveAll(m => m.isExtension);
    payload.Messages.Add(
        new Message
        {
            Role = "system",
            Content = "In Java always use Lomboks Data annotation",
            isExtension=true

        }
    );
    payload.Messages.Add(
        new Message
        {
            Role = "system",
            Content = "You are helpful cat assistant, you answer messages like a cat, you always end your texts with a cat sound",
            isExtension = true

        }
    );

    var copilotResponse = await httpClient.PostAsJsonAsync("https://api.github.com/chat/cmopletions",payload);
    var responseStream = await copilotResponse.Content.ReadAsStreamAsync();

    return Results.Stream(responseStream, "application/json");

});

app.MapGet("/info", () => "Hello World!");

app.Run();

internal class Message
{
    public required string Role { get; set; }
    public required string Content { get; set; }
    public bool isExtension { get; set; }
}

internal class Payload
{
    public bool Stream { get; set; }
    public required List<Message> Messages { get; set; } = [];
}
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

HttpClient httpClient = new HttpClient();

// Чтение переменных среды
bool gradualMigration = Environment.GetEnvironmentVariable("GRADUAL_MIGRATION")?.ToLower() == "true";
int migrationPercent = int.TryParse(Environment.GetEnvironmentVariable("MOVIES_MIGRATION_PERCENT"), out var percent) ? percent : 0;

// Прокси-адреса
var prodTarget = Environment.GetEnvironmentVariable("MONOLITH_URL")?.ToLower();
var devTarget = Environment.GetEnvironmentVariable("MOVIES_SERVICE_URL")?.ToLower();



// Прокси только для /api/*
app.Map("/api/{**catch-all}", async (HttpContext context) =>
{
    // Определяем, куда перенаправить запрос
    var random = new Random();
    bool useDev = false;

    if (gradualMigration && migrationPercent > 0)
    {
        int randValue = random.Next(0, 100); // 0..99
        useDev = randValue < migrationPercent;
    }
    var targetBaseUrl = useDev ? devTarget : prodTarget;
    Console.WriteLine($"Use {targetBaseUrl} as target");
    
    var requestPath = context.Request.Path + context.Request.QueryString;

    var requestMessage = new HttpRequestMessage
    {
        Method = new HttpMethod(context.Request.Method),
        RequestUri = new Uri(targetBaseUrl + requestPath),
        Content = context.Request.Method switch
        {
            "POST" or "PUT" or "PATCH" => new StreamContent(context.Request.Body),
            _ => null
        }
    };

    foreach (var header in context.Request.Headers)
    {
        if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
        {
            requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    var response = await httpClient.SendAsync(requestMessage);

    context.Response.StatusCode = (int)response.StatusCode;

    foreach (var header in response.Headers)
        context.Response.Headers[header.Key] = header.Value.ToArray();
    foreach (var header in response.Content.Headers)
        context.Response.Headers[header.Key] = header.Value.ToArray();

    context.Response.Headers.Remove("transfer-encoding");
    await response.Content.CopyToAsync(context.Response.Body);
});

app.Run();
using Confluent.Kafka;
using sinemaabyss_events.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var kafkaUrl = Environment.GetEnvironmentVariable("KAFKA_BROKERS")?.ToLower();
var config = new ProducerConfig
{
    BootstrapServers = kafkaUrl
};

app.MapGet("/", async (HttpContext context) =>
{
    return Results.Ok("Hello World!");
});

app.MapPost("/api/events/movie", async (HttpContext context) =>
{
    var result = new EventResponse();
    try
    {
        var model = await context.Request.ReadFromJsonAsync<MovieEvent>();
        if (model is null)
            return Results.BadRequest();
        
        using var producer = new ProducerBuilder<Null, string>(config).Build();
        try
        {
            var resultProduce = await producer.ProduceAsync("movie-events", new Message<Null, string>
            {
                Value = $"Вызван метод '/api/events/movie' с Тайтлом: {model.Title}"
            });

            result.Offset = (int)resultProduce.Offset.Value;
            result.Partition = resultProduce.Partition.Value;
            result.Status = "success";
            
            Console.WriteLine($"{DateTime.Now} Сообщение доставлено в: {resultProduce.TopicPartitionOffset}");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Ошибка при отправке сообщения: {e.Error.Reason}");
        }
        
        result.Event = model;
        return Results.Created($"/api/events/movie/{model.MovieId}", result);
    }
    catch (Exception e)
    {
        Console.WriteLine($"При обработке /api/events/movie возникла ошибка {e.Message}. Параметр: {context.Request.Body.ToString()}");
        return Results.InternalServerError("Внутренняя ошибка сервера");
    }
});

app.MapPost("/api/events/user", async (HttpContext context) =>
{
    var result = new EventResponse();
    try
    {
        var model = await context.Request.ReadFromJsonAsync<UserEvent>();
        if (model is null)
            return Results.BadRequest();
        
        using var producer = new ProducerBuilder<Null, string>(config).Build();
        try
        {
            var resultProduce = await producer.ProduceAsync("user-events", new Message<Null, string>
            {
                Value = $"Вызван метод '/api/events/user' с Username: {model.Username}"
            });

            result.Offset = (int)resultProduce.Offset.Value;
            result.Partition = resultProduce.Partition.Value;
            result.Status = "success";
            
            Console.WriteLine($"{DateTime.Now} Сообщение доставлено в: {resultProduce.TopicPartitionOffset}");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Ошибка при отправке сообщения: {e.Error.Reason}");
        }
        
        result.Event = model;
        return Results.Created($"/api/events/user/{model.UserId}", result);
    }
    catch (Exception e)
    {
        Console.WriteLine($"При обработке /api/events/user возникла ошибка {e.Message}. Параметр: {context.Request.Body.ToString()}");
        return Results.InternalServerError("Внутренняя ошибка сервера");
    }
});

app.MapPost("/api/events/payment", async (HttpContext context) =>
{
    var result = new EventResponse();
    try
    {
        var model = await context.Request.ReadFromJsonAsync<PaymentEvent>();
        if (model is null)
            return Results.BadRequest();
        
        using var producer = new ProducerBuilder<Null, string>(config).Build();
        try
        {
            var resultProduce = await producer.ProduceAsync("user-payments", new Message<Null, string>
            {
                Value = $"Вызван метод '/api/events/payment' с PaymentId: {model.PaymentId}"
            });

            result.Offset = (int)resultProduce.Offset.Value;
            result.Partition = resultProduce.Partition.Value;
            result.Status = "success";
            
            Console.WriteLine($"{DateTime.Now} Сообщение доставлено в: {resultProduce.TopicPartitionOffset}");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Ошибка при отправке сообщения: {e.Error.Reason}");
        }
        
        result.Event = model;
        return Results.Created($"/api/events/payment/{model.PaymentId}", result);
    }
    catch (Exception e)
    {
        Console.WriteLine($"При обработке /api/events/payment возникла ошибка {e.Message}. Параметр: {context.Request.Body.ToString()}");
        return Results.InternalServerError("Внутренняя ошибка сервера");
    }
});

using var cts = new CancellationTokenSource();
// Запуск консьюмера в фоне
var consumerTask = Task.Run(() => StartKafkaConsumer(kafkaUrl, cts.Token));

static void StartKafkaConsumer(string kafkaUrl, CancellationToken cancellationToken)
{
    var config = new ConsumerConfig
    {
        BootstrapServers = kafkaUrl,
        GroupId = "csharp-consumer-group",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    consumer.Subscribe(["movie-events", "user-events", "user-payments"]);

    try
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var cr = consumer.Consume(cancellationToken);
                Console.WriteLine($"{DateTime.Now} [Consumer] Получено сообщение из {cr.Topic}: {cr.Message.Value}");
            }
            catch (ConsumeException ex)
            {
                Console.WriteLine($"[Consumer] Ошибка при чтении: {ex.Error.Reason}");
            }
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("[Consumer] Завершение по запросу отмены...");
    }
    finally
    {
        consumer.Close();
    }
}

app.Run();
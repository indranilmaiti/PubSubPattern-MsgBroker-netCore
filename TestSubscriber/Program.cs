using System.Net.Http;
using System.Net.Http.Json;
using TestSubscriber.Dtos;

Thread.Sleep(1000);// wait for api to start

Console.WriteLine("Press ESC to stop");

HttpClient client = new HttpClient();
var subscriptionId = await client.GetFromJsonAsync<string>("https://localhost:7034/subscriptions/Subscribe/" + 1);

Console.WriteLine(subscriptionId);

do
{
    if (subscriptionId == null)
    {
        Console.WriteLine("subscription failed");
        break;
    }

    Console.WriteLine("Listening...");
    while (!Console.KeyAvailable)
    {
        List<int> ackIds = await GetMessagesAsync(client, subscriptionId);

        Thread.Sleep(2000);

        if (ackIds.Count > 0)
        {
            SubscribeMessage msg= new SubscribeMessage();
            msg.MessageIds = ackIds;
            msg.SubscriptionId = subscriptionId;
            Thread.Sleep(new Random().Next(0,1000));//Rnadom processing time
            await AckMessagesAsync(client, msg);
        }
    }

} while (Console.ReadKey(true).Key != ConsoleKey.Escape);

static async Task<List<int>> GetMessagesAsync(HttpClient httpClient, string subscriptionId)
{
    List<int> ackIds = new List<int>();
    List<MessageReadDto>? newMessages = new List<MessageReadDto>();

    try
    {
        newMessages = await httpClient.GetFromJsonAsync<List<MessageReadDto>>("https://localhost:7034/subscriptions/GetMessages/"+ subscriptionId);
    }
    catch
    {
        return ackIds;
    }

    foreach (MessageReadDto msg in newMessages!)
    {
        Console.WriteLine($"{msg.Id} - {msg.TopicMessage} - {msg.MessageStatus}");
        ackIds.Add(msg.Id);
    }

    return ackIds;
}

static async Task AckMessagesAsync(HttpClient httpClient, SubscribeMessage msg)
{
    var response = await httpClient.PostAsJsonAsync("https://localhost:7034/subscriptions/acknowledgemessages/", msg);
    var returnMessage = await response.Content.ReadAsStringAsync();

    Console.WriteLine(returnMessage);
}
namespace TestSubscriber.Dtos
{
    public class SubscribeMessage
    {
        public string SubscriptionId { get; set; }
        public List<int> MessageIds { get; set; }
    }
}

namespace MessageBroker.Dtos
{
    public class SubscribeMessage
    {
        public int SubscriptionId { get; set; }
        public List<int> MessageIds { get; set; }
    }
}

namespace MessageBroker.Dtos
{
    public class AckMessage
    {
        public string SubscriptionId { get; set; }
        public List<int> MessageIds { get; set; }
    }
}

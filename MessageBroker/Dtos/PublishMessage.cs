namespace MessageBroker.Dtos
{
    public class PublishMessage
    {
        public int TopicId { get; set; }
        public string? MessageBody { get; set; }
    }
}

using MessageBroker.Data;
using MessageBroker.Dtos;
using MessageBroker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessageBroker.Controllers
{

    [ApiController]
    [Route("topic")]
    public class TopicController : Controller
    {
        AppDbContext _context;
        public TopicController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("gettopics")]
        public async Task<List<Topic>> GetTopics()
        {
            var topics = await _context.Topics.ToListAsync();
            return topics;
        }

        [HttpPost]
        [Route("createtopic")]
        public async Task<string> CreateTopic(Topic topic)
        {
            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();
            return topic.Id.ToString();
        }

        [HttpPost]
        [Route("publishtopic")]
        public async Task<IResult> PublishTopic(PublishMessage message)
        {

            bool topics = await _context.Topics.AnyAsync(t => t.Id == message.TopicId);
            if (!topics)
                return Results.NotFound("Topic not found");

            var subs = _context.Subscriptions.Where(s => s.TopicId == message.TopicId);

            if (subs.Count() == 0)
                return Results.NotFound("There are no subscriptions for this topic");

            foreach (var sub in subs)
            {
                Message msg = new Message
                {
                    TopicMessage = message.MessageBody,
                    SubscriptionId = sub.Id,
                    ExpiresAfter = DateTime.Now.AddMinutes(1)
                };
                await _context.Messages.AddAsync(msg);
            }
            await _context.SaveChangesAsync();

            return Results.Ok("Message has been published");
        }

    }
}

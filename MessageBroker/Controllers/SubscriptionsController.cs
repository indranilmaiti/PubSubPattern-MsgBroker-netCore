using MessageBroker.Data;
using MessageBroker.Dtos;
using MessageBroker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MessageBroker.Controllers
{

    [ApiController]
    [Route("subscriptions")]
    public class SubscriptionsController : Controller
    {
        AppDbContext _context;
        public SubscriptionsController(AppDbContext context)
        {
            _context = context;
        }

        object lockObj = new object();



        [HttpGet]
        [Route("subscribe/{topicId}")]
        public async Task<IResult> Subscribe(int topicId)
        {
            bool topics = await _context.Topics.AnyAsync(t => t.Id == topicId);
            if (!topics)
                return Results.NotFound("Topic not found");
            Subscription sub = new Subscription
            {
                TopicId = topicId,
                Name = Guid.NewGuid().ToString(),
            };

            await _context.Subscriptions.AddAsync(sub);
            await _context.SaveChangesAsync();
            return Results.Ok(sub.Name);
        }

        [HttpGet]
        [Route("getmessages/{subscriptionname}")]
        public async Task<IResult> GetMessages(string subscriptionname)
        {
            Subscription? subs = _context.Subscriptions.FirstOrDefault(s => s.Name == subscriptionname);
            if (subs == null)
                return Results.NotFound("Subscription not found");

            var messages = _context.Messages.Where(m => m.SubscriptionId == subs.Id && m.MessageStatus != "SENT");
            if (messages.Count() == 0)
                return Results.NotFound("No new messages");

            foreach (var msg in messages)
            {
                msg.MessageStatus = "REQUESTED";
            }

            await _context.SaveChangesAsync();

            return Results.Ok(messages);

        }


        [HttpPost]
        [Route("acknowledgemessages")]
        public async Task<IResult> AcknowledgeMessages(AckMessage subscribeMessage)
        {
            bool subs = await _context.Subscriptions.AnyAsync(s => s.Name == subscribeMessage.SubscriptionId);
            if (!subs)
                return Results.NotFound("Subscription not found");

            if (subscribeMessage.MessageIds.Count <= 0)
                return Results.BadRequest();

            int count = 0;
            foreach (int i in subscribeMessage.MessageIds)
            {
                var msg = _context.Messages.FirstOrDefault(m => m.Id == i);

                if (msg != null)
                {
                    msg.MessageStatus = "SENT";
                    await _context.SaveChangesAsync();
                    lock (lockObj)
                    {
                        count++;
                    }
                }
            }
            return Results.Ok($"Acknowledged {count}/{subscribeMessage.MessageIds.Count} messages");

        }
    }
}

using Cleipnir.ResilientFunctions;
using Microsoft.AspNetCore.Mvc;

namespace Cleipnir.Flows.Sample.Flows.Ordering.MessageDriven;

[ApiController]
[Route("[controller]")]
public class MessageDrivenOrderController(MessageDrivenOrderFlows orderFlows, ILogger<MessageDrivenOrderController> logger) : Controller
{
    [HttpPost]
    public async Task<ActionResult> Post(Order order)
    {
        logger.LogInformation($"{order.OrderId.ToUpper()}: Order processing started");
        await orderFlows.Schedule(order.OrderId, order).Completion(maxWait: TimeSpan.FromDays(1));
        logger.LogInformation($"{order.OrderId.ToUpper()}: Order processing completed");
        return Ok();
    }
}
using Cleipnir.Flows.Sample.Flows.Ordering.MessageDriven;
using Microsoft.AspNetCore.Mvc;

namespace Cleipnir.Flows.Sample.Flows.Ordering.Rpc;

[ApiController]
[Route("[controller]")]
public class OrderController(OrderFlows orderFlows, ILogger<MessageDrivenOrderController> logger) : Controller
{
    [HttpPost]
    public async Task<ActionResult> Post(Order order)
    {
        logger.LogInformation($"{order.OrderId.ToUpper()}: Order processing started");
        await orderFlows.Run(order.OrderId, order);
        logger.LogInformation($"{order.OrderId.ToUpper()}: Order processing completed");
        return Ok();
    }
}
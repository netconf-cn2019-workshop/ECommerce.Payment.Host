using ECommerce.Common.Commands;
using ECommerce.Common.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ECommerce.Payment.Host.Consumers
{
    public class OrderSubmittedEventConsumer : IConsumer<OrderSubmittedEvent>
    {
        private readonly ILogger<OrderSubmittedEventConsumer> _logger;

        public OrderSubmittedEventConsumer(ILogger<OrderSubmittedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderSubmittedEvent> context)
        {
            _logger.LogInformation($"正在初始化支付信息：由顾客 {context.Message.CustomerId} 提交的订单 {context.Message.OrderId}，总金额 {context.Message.Total}");

            await context.Send(new InitiatePaymentCommand() {
                CorrelationId = context.Message.CorrelationId,
                CustomerId = context.Message.CustomerId,
                OrderId = context.Message.OrderId,
                Total = context.Message.Total
            });
        }
    }
}

using System;
using System.Threading.Tasks;
using ECommerce.Common.Commands;
using ECommerce.Common.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ECommerce.Payment.Host.Consumers
{
    public class InitiatePaymentCommandConsumer : IConsumer<InitiatePaymentCommand>
    {
        private readonly ILogger<InitiatePaymentCommandConsumer> _logger;

        public InitiatePaymentCommandConsumer(ILogger<InitiatePaymentCommandConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<InitiatePaymentCommand> context)
        {
            _logger.LogInformation($"正在处理支付信息：由顾客 {context.Message.CustomerId} 提交的订单 {context.Message.OrderId}，总金额 {context.Message.Total}");

            try
            {
                // Try processing the payment
                await ProcessPayment(context.Message);

                // Payment was accepted
                await context.Publish(new PaymentAcceptedEvent()
                {
                    CorrelationId = context.Message.CorrelationId,
                    OrderId = context.Message.OrderId,
                    CustomerId = context.Message.CustomerId,
                    Total = context.Message.Total
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"在处理订单 {context.Message.OrderId} 的支付操作时发生错误");
                throw;
            }
        }

        private async Task ProcessPayment(InitiatePaymentCommand message)
        {
            await Task.Delay(5000); // simulate payment

            _count++;

            if (message.Total > 500)
            {
                // Sometimes payments over 500 fail :)
                if (_count % 2 == 0)
                {
                    throw new InvalidOperationException("支付请求被支付网关拒绝");
                }
            }
        }

        private static int _count;
    }
}

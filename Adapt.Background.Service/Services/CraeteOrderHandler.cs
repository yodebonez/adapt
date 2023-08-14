using Adapt.Background.Service.Config;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adapt.Background.Service.Services
{
    public class CraeteOrderHandler : BackgroundService
    {
        private readonly AppSettings _appSettings;
        private IQueueClient _orderQueueClient;

        public CraeteOrderHandler(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task Handle(Message message, CancellationToken cancelToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var body = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Create Order Details are: {body}");
            await _orderQueueClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
        }
        public virtual Task HandleFailureMessage(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            if (exceptionReceivedEventArgs == null)
                throw new ArgumentNullException(nameof(exceptionReceivedEventArgs));
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var messageHandlerOptions = new MessageHandlerOptions(HandleFailureMessage)
            {
                MaxConcurrentCalls = 5,
                AutoComplete = false,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(10)
            };
            _orderQueueClient = new QueueClient(_appSettings.QueueConnectionString, _appSettings.QueueName);
            _orderQueueClient.RegisterMessageHandler(Handle, messageHandlerOptions);
            Console.WriteLine($"{nameof(CraeteOrderHandler)} service has started.");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"{nameof(CraeteOrderHandler)} service has stopped.");
            await _orderQueueClient.CloseAsync().ConfigureAwait(false);
        }

    }
}
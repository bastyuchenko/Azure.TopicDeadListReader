using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program3
    {

        static ITopicClient topicClient;
        static ISubscriptionClient subscriptionMain;

        static void Main3(string[] args)
        {
            CommonManager.CreateInfrastructure();
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            Initialization();

            IMessageReceiver subscriptionReceiver = new MessageReceiver(
                Connections.ServiceBusConnectionString,
                Microsoft.ServiceBus.Messaging.SubscriptionClient.FormatDeadLetterPath(
                    Connections.TopicName,
                    Constants.Subscriptions.PaymentSubscription
                    ), ReceiveMode.PeekLock);


            subscriptionMain.RegisterMessageHandler(ProcessMessagesAsync,
                new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                });


            while (true)
            {
                var receivedMessage = await subscriptionReceiver.ReceiveAsync();
                if (receivedMessage != null)
                {
                    try
                    {
                        ProcessMessage(receivedMessage, "dead-letter");
                        await subscriptionReceiver.AbandonAsync(receivedMessage.SystemProperties.LockToken);
                    }
                    catch (Exception)
                    {
                        await subscriptionReceiver.AbandonAsync(receivedMessage.SystemProperties.LockToken);
                    }

                }
            }


            Thread.Sleep(TimeSpan.FromDays(1));
        }

        private static void ProcessMessage(Message receivedMessage, string place)
        {
            Console.WriteLine($"{place}: {receivedMessage.UserProperties[Constants.Properties.Category]}");
        }

        private static void Initialization()
        {
            topicClient = new TopicClient(Connections.ServiceBusConnectionString, Connections.TopicName);

            subscriptionMain = new SubscriptionClient(
                Connections.ServiceBusConnectionString,
                Connections.TopicName,
                Constants.Subscriptions.PaymentSubscription);

        }

        static async Task ProcessMessagesAsync(Message receivedMessage, CancellationToken token)
        {
            try
            {
                ProcessMessage(receivedMessage, "subscr");
                await subscriptionMain.AbandonAsync(receivedMessage.SystemProperties.LockToken);
            }
            catch (Exception)
            {
                await subscriptionMain.AbandonAsync(receivedMessage.SystemProperties.LockToken);

            }

        }


        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            //Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.FromResult(true);
        }

    }
}

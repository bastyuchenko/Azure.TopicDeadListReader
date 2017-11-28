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
    class Program
    {

        static ITopicClient topicClient;
        static ISubscriptionClient subscriptionMain;
        static IMessageReceiver subscriptionDeadLetter;

        static void Main(string[] args)
        {
            CommonManager.CreateInfrastructure();
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            Initialization();






            subscriptionDeadLetter.RegisterMessageHandler(ProcessMessagesAsyncDead,
                new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                });


            subscriptionMain.RegisterMessageHandler(ProcessMessagesAsyncMain,
                new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                });

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

            subscriptionDeadLetter = new MessageReceiver(
                Connections.ServiceBusConnectionString,
                Microsoft.ServiceBus.Messaging.SubscriptionClient.FormatDeadLetterPath(
                    Connections.TopicName,
                    Constants.Subscriptions.PaymentSubscription
                    ), ReceiveMode.PeekLock);

        }

        static async Task ProcessMessagesAsyncMain(Message receivedMessage, CancellationToken token)
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

        static async Task ProcessMessagesAsyncDead(Message receivedMessage, CancellationToken token)
        {


            try
            {
                throw new Exception("bibibibibibi");
                ProcessMessage(receivedMessage, "dead-letter");
                await subscriptionDeadLetter.AbandonAsync(receivedMessage.SystemProperties.LockToken);
            }
            catch (Exception)
            {
                await subscriptionDeadLetter.AbandonAsync(receivedMessage.SystemProperties.LockToken);
                Thread.Sleep(TimeSpan.FromSeconds(15));
            }

        }


        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.FromResult(true);
        }

    }
}

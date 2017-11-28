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
    class Program2
    {

        static ITopicClient topicClient;
        static ISubscriptionClient subscriptionMain;
        static IMessageReceiver subscriptionDeadLetter;

        static void Main2(string[] args)
        {
            CommonManager.CreateInfrastructure();
            MainAsync().Wait();


            //subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync,
            //    new MessageHandlerOptions(ExceptionReceivedHandler)
            //    {
            //        MaxConcurrentCalls = 1,
            //        AutoComplete = false
            //    });

            //await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        private static async Task MainAsync()
        {
            Initialization();

            subscriptionDeadLetter = new MessageReceiver(
                Connections.ServiceBusConnectionString,
                Microsoft.ServiceBus.Messaging.SubscriptionClient.FormatDeadLetterPath(
                    Connections.TopicName,
                    Constants.Subscriptions.PaymentSubscription
                    ), ReceiveMode.PeekLock);

            //var options = new Microsoft.ServiceBus.Messaging.OnMessageOptions();
            //options.AutoComplete = false;

            //subscriptionReceiver.OnMessage(processCalculations, options);

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


            //while (true)
            //{
            //    var receivedMessage = await subscriptionReceiver.ReceiveAsync();
            //    if (receivedMessage != null)
            //    {
            //        try
            //        {
            //            ProcessMessage(receivedMessage, "dead-letter");
            //            await subscriptionReceiver.CompleteAsync(receivedMessage.SystemProperties.LockToken);
            //        }
            //        catch (Exception)
            //        {
            //            await subscriptionReceiver.AbandonAsync(receivedMessage.SystemProperties.LockToken);
            //        }

            //    }
            //}


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

        static async Task ProcessMessagesAsyncMain(Message receivedMessage, CancellationToken token)
        {
            try
            {
                ProcessMessage(receivedMessage, "subscr");
                await subscriptionMain.CompleteAsync(receivedMessage.SystemProperties.LockToken);
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
                ProcessMessage(receivedMessage, "dead-letter");
                await subscriptionDeadLetter.CompleteAsync(receivedMessage.SystemProperties.LockToken);
            }
            catch (Exception)
            {
                await subscriptionDeadLetter.AbandonAsync(receivedMessage.SystemProperties.LockToken);

            }

        }


        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            //Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.FromResult(true);
        }

    }
}

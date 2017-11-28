using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class CommonManager
    {
        public static void CreateInfrastructure()
        {
            Uri uri = ServiceBusEnvironment.CreateServiceUri("sb", Connections.ServiceBusNS, string.Empty);

            TokenProvider tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(Connections.AccessKeyName, Connections.AccessKeyKey);
            NamespaceManager namespaceManager = new NamespaceManager(uri, tokenProvider);

            TopicDescription topic = null;
            try
            {
                topic = namespaceManager.CreateTopic(Connections.TopicName);
            }
            catch (Exception)
            { }

            CreatePaymentSubrscription(namespaceManager);
            CreateCRMSubrscription(namespaceManager);
        }

        private static void CreatePaymentSubrscription(NamespaceManager namespaceManager)
        {
            SubscriptionDescription subscr = null;
            try
            {
                Microsoft.ServiceBus.Messaging.SqlFilter dashboardFilter = new Microsoft.ServiceBus.Messaging.SqlFilter($"{Constants.Properties.Category} = '{Constants.Category.PaymentCategory}'");
                subscr = namespaceManager.CreateSubscription(Connections.TopicName, Constants.Subscriptions.PaymentSubscription, dashboardFilter);
            }
            catch (Exception)
            { }

            //return subscr;
        }

        private static void CreateCRMSubrscription(NamespaceManager namespaceManager)
        {
            SubscriptionDescription subscr = null;
            try
            {
                Microsoft.ServiceBus.Messaging.SqlFilter dashboardFilter = new Microsoft.ServiceBus.Messaging.SqlFilter($"{Constants.Properties.Category} = '{Constants.Category.CRMCategory}'");
                subscr = namespaceManager.CreateSubscription(Connections.TopicName, Constants.Subscriptions.CRMSubscription, dashboardFilter);
            }
            catch (Exception)
            { }

            //return subscr;
        }
    }
}

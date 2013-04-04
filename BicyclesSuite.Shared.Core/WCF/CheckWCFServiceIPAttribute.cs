using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace BicyclesSuite.Shared.WCF
{
    /// <summary>
    /// Class level attribute to add IP check to WCF service host
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CheckWCFServiceIPAttribute : Attribute, IServiceBehavior
    {
        internal class MyInspector : IParameterInspector
        {
            public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
            {
            }

            public object BeforeCall(string operationName, object[] inputs)
            {
                OperationContext context = OperationContext.Current;
                MessageProperties messageProperties = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpointProperty =
                    messageProperties[RemoteEndpointMessageProperty.Name]
                    as RemoteEndpointMessageProperty;

                string ip = endpointProperty.Address.ToLower();

                List<string> ips = new List<string>(
#warning Read from config
                        //Config.WCFServerIP.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        new [] { "::1" }
                    );
                if (!ips.Contains(ip))
                {

                    //this.GetType().Error(string.Format("Access to {1} for IP ({0}) was blocked because WCFServerIP should be ({1})", ip, operationName, Config.WCFServerIP));
#warning ILogger, ExceptionCreate();
                    throw new SecurityException("Permission denied: " + ip + " is unacceptable IP address of client");
                }
                return null;
            }
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher eDispatcher in cDispatcher.Endpoints)
                {
                    foreach (DispatchOperation op in eDispatcher.DispatchRuntime.Operations)
                    {
                        op.ParameterInspectors.Add(new MyInspector());
                    }
                }
            }
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }

}

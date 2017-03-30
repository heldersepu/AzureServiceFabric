using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Client;
using System.Net.Http;
using System.Fabric.Description;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Alphabet.Web
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Web : StatelessService
    {
        private static readonly Uri alphabetServiceUri = new Uri(@"fabric:/AlphabetPartitions/Processing");
        private readonly ServicePartitionResolver servicePartitionResolver = ServicePartitionResolver.GetDefault();
        private readonly HttpClient httpClient = new HttpClient();

        public Web(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(this.CreateInputListener, "Input") };
        }
        private ICommunicationListener CreateInputListener(StatelessServiceContext args)
        {
            EndpointResourceDescription inputEndpoint = args.CodePackageActivationContext.GetEndpoint("WebApiServiceEndpoint");

            string uriPrefix = String.Format("{0}://+:{1}/alphabetpartitions/", inputEndpoint.Protocol, inputEndpoint.Port);

            string uriPublished = uriPrefix.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            return new HttpCommunicationListener(uriPrefix, uriPublished, this.ProcessInputRequest);
        }
        private async Task ProcessInputRequest(HttpListenerContext context, CancellationToken cancelRequest)
        {
            String output = null;

            try
            {
                string lastname = context.Request.QueryString["lastname"];

                char firstLetterOfLastName = lastname.First();
                ServicePartitionKey partitionKey = new ServicePartitionKey(Char.ToUpper(firstLetterOfLastName) - 'A');

                ResolvedServicePartition partition = await this.servicePartitionResolver.ResolveAsync(alphabetServiceUri, partitionKey,
                    cancelRequest);
                ResolvedServiceEndpoint ep = partition.GetEndpoint();

                JObject addresses = JObject.Parse(ep.Address);
                string primaryReplicaAddress =
                    addresses["Endpoints"]["Internal"].Value<string>();

                UriBuilder primaryReplicaUriBuilder = new UriBuilder(primaryReplicaAddress);
                primaryReplicaUriBuilder.Query = "lastname=" + lastname;

                string result = await
                    this.httpClient.GetStringAsync(primaryReplicaUriBuilder.Uri);

                output = String.Format("Result: {0}. Partition key: '{1}' generated from the first letter '{2}' of input value '{3}'.Processing service partition ID: {4}. Processing service replica address: {5}",
                                                                result,
                                                        partitionKey.Value,
                                                        firstLetterOfLastName,
                                                        lastname,
                                                        partition.Info.Id,
                                                        primaryReplicaAddress);
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }

            using (HttpListenerResponse response = context.Response)
            {
                if (output != null)
                {
                    byte[] outBytes = Encoding.UTF8.GetBytes(output);
                    response.OutputStream.Write(outBytes, 0, outBytes.Length);
                }
            }
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}

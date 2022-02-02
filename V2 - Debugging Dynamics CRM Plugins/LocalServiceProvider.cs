/**

Copyright 2022 LIVE SOLUTIONS

MIT License

Permission is hereby granted, free of charge, 
to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the 
Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall 
be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/


using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;


namespace LiveSolutions
{
    /**
		Instantiate this class for use in plugins
	*/
    public class LocalServiceProvider : IServiceProvider
    {
        private string crmUrl = null;

        public IOrganizationService service = null;
        public LocalExecutionContext context { get; set; } = null;




        public LocalServiceProvider() { }

        public LocalServiceProvider(string yourCRMUrl, Entity target) {

            Prepare(yourCRMUrl, target.ToEntityReference());

            context.InputParameters["Target"] = target;
        }

        public LocalServiceProvider(string yourCRMUrl, EntityReference target)
        {
            Prepare(yourCRMUrl, target);

            context.InputParameters["Target"] = target;
        }

        private void Prepare(string yourCRMUrl, EntityReference target)
        {
            service = GetOrganizationService(yourCRMUrl);
            WhoAmIRequest reqWhoAmI = new WhoAmIRequest();
            WhoAmIResponse resp = (WhoAmIResponse)service.Execute(reqWhoAmI);

            context = new LocalExecutionContext()
            {
                OrganizationId = resp.OrganizationId,
                BusinessUnitId = resp.BusinessUnitId,
                UserId = resp.UserId,
                InitiatingUserId = resp.UserId,
                PrimaryEntityId = target.Id,
                PrimaryEntityName = target.LogicalName
            };
        }


        /** 
			This method connects to your Dynamics environment using OAuth
		*/
        public static IOrganizationService GetOrganizationService(string yourCRMURL)
        {
            // Microsoft's generic application
            // https://docs.microsoft.com/en-us/powerapps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect#:~:text=AppId%20or%20ClientId%20%3D-,51f81489%2D12ee%2D4a9e%2Daaae%2Da2591f45987d,-Sample%20RedirectUri%20%3D%20app

            var appId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            var redirectUrl = "app://58145B91-0C36-4500-8554-080854F2AC97";

            var cs = $"Url={yourCRMURL};AppId={appId};RedirectUri={redirectUrl};AuthType=OAuth;";

            CrmServiceClient client = new CrmServiceClient(cs);

            return client;
        }

        /**
			The wrapper method for all required services
		*/
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IPluginExecutionContext))
            {
                return context;
            }
            else if (serviceType == typeof(ITracingService))
            {
                // Log all tract logs to console
                return new ConsoleTracingService();
            }
            else if (serviceType == typeof(IOrganizationServiceFactory))
            {
                return new LocalOrganizationServiceFactory(this.service);
            }

            return null;
        }
    }

    class LocalOrganizationServiceFactory : IOrganizationServiceFactory
    {

        public IOrganizationService service = null;

        public LocalOrganizationServiceFactory(IOrganizationService svc)
        {
            service = svc;
        }

        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            return service;
        }
    }

    /**
		The tracing service which uses the console as the output
	*/
    class ConsoleTracingService : ITracingService
    {
        public void Trace(string format, params object[] args)
        {
            Console.Out.WriteLine(format, args);
        }
    }

    /** The execution context which allows changing all properties */
    public class LocalExecutionContext : IPluginExecutionContext
    {
        public int Stage { get; set; } = 0;

        public IPluginExecutionContext ParentContext { get; set; } = null;

        public int Mode { get; set; } = 0;

        public int IsolationMode { get; set; } = 0;

        public int Depth { get; set; } = 1;

        public string MessageName { get; set; } = "Execute";

        public string PrimaryEntityName { get; set; } = null;

        public Guid? RequestId { get; set; } = null;

        public string SecondaryEntityName { get; set; } = null;

        public ParameterCollection InputParameters { get; set; } = new ParameterCollection();

        public ParameterCollection OutputParameters { get; set; } = new ParameterCollection();

        public ParameterCollection SharedVariables { get; set; } = new ParameterCollection();

        public Guid UserId { get; set; }

        public Guid InitiatingUserId { get; set; }

        public Guid BusinessUnitId { get; set; }

        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public Guid PrimaryEntityId { get; set; }

        public EntityImageCollection PreEntityImages { get; set; } = new EntityImageCollection();

        public EntityImageCollection PostEntityImages { get; set; } = new EntityImageCollection();

        public EntityReference OwningExtension { get; set; } = null;

        public Guid CorrelationId { get; set; } = Guid.NewGuid();

        public bool IsExecutingOffline { get; set; } = false;

        public bool IsOfflinePlayback { get; set; } = true;

        public bool IsInTransaction { get; set; } = false;

        public Guid OperationId { get; set; } = Guid.NewGuid();

        public DateTime OperationCreatedOn { get; set; } = DateTime.Now;
    }
}


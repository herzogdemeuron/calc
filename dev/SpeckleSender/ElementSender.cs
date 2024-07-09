using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Calc.Core.Interfaces;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using Speckle.Core.Transports;

namespace SpeckleSender
{
    public class ElementSender : IElementSender
    {
        private readonly ISpeckleConverter speckleConverter;
        private readonly string revitAppName;
        private readonly Client client;
        private readonly Account account;
        private readonly string streamId = "bb8ff4c65d";
        private readonly Document doc;

        public ElementSender(Document doc)
        {
            this.doc = doc;
            revitAppName = HostApplications.Revit.GetVersion(HostAppVersion.v2023);
            var speckleKit = KitManager.GetDefaultKit();

            // this should be loading the revit converter from nuget Speckle.Objects.Converter.Revit2023 (priotized?)
            // or from the installed speckle app kit.
            // see: https://speckle.community/t/how-to-run-revit-to-speckle-conversions-in-c/1548/6
            speckleConverter = speckleKit.LoadConverter(revitAppName);
            speckleConverter.SetContextDocument(doc);

            // make client
            account = new Account();
            account.token = Environment.GetEnvironmentVariable("SPECKLE_TOKEN");
            account.serverInfo = new ServerInfo { url = "https://herzogdemeuron.speckle.xyz/" };
            client = new Client(account);
        }

        public async Task<string> SendToSpeckle(List<int> elementIds, string modelName)
        {
            //var docProvider = new UIDocumentProvider(ConnectorBindingsRevit.RevitApp);
            var cache = new RevitDocumentAggregateCache(doc);
    
            speckleConverter.SetContextDocument(cache);
            // convert elements to speckle objects
            List<object> elementList = elementIds.Select(id => doc.GetElement(new ElementId(id))).ToList().ConvertAll(e => e as Object);
            var speckleBases = speckleConverter.ConvertToSpeckle(elementList);
            var commitObject = new Base();
            commitObject["@all"] = speckleBases;

            var transport = new ServerTransport(account, streamId);

            var objectId = await Operations.Send(commitObject, transport, true);

            // ensure branch exists
            var branch = await client.BranchGet(streamId, modelName);
            if (branch == null)
            {
                // create a new branch with the model name
                var branchId = await client.BranchCreate(
                    new BranchCreateInput
                    {
                        streamId = streamId,
                        name = modelName
                    });                
            }

            var commitId = await client.CommitCreate(
                                  new CommitCreateInput
                                  {
                                      streamId = streamId,
                                      branchName = modelName,
                                      objectId = objectId,
                                      message = "testing message"
                                  });



            //var commitId = Helpers.Send(streamId, commitObject, "My commit message").Result;

            return commitId;


        }
    }
}

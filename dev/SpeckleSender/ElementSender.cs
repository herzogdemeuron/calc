using Autodesk.Revit.DB;
using Calc.Core;
using Calc.Core.Interfaces;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeckleSender
{
    public class ElementSender : IElementSender
    {
        private readonly ISpeckleConverter speckleConverter;
        private readonly string revitAppName;
        private readonly Client client;
        private readonly Account account;
        private readonly string builderProjectId;
        private readonly Document doc;

        public ElementSender(Document doc, CalcConfig config)
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
            account.token = config.SpeckleToken;
            account.serverInfo = new ServerInfo { url = config.SpeckleServerUrl };
            client = new Client(account);
            builderProjectId = config.SpeckleBuilderProjectId;
        }


        /// <summary>
        /// sends the elements to a speckle project with the model name, return the branch id
        /// stream id hardcoded for now
        /// </summary>
        public async Task<string> SendToSpeckle(List<int> elementIds, string modelCode, string description, Dictionary<string,string> dynamicProperties)
        {
            List<object> elementList = elementIds
                .Select(id => doc.GetElement(new ElementId(id)))
                .ToList()
                .ConvertAll(e => e as Object);

            speckleConverter.SetContextDocument(new RevitDocumentAggregateCache(doc));
            var speckleBases = speckleConverter.ConvertToSpeckle(elementList);
            var commitObject = new Base();
            commitObject["@elements"] = speckleBases;
            commitObject["model_code"] = modelCode;
            commitObject["description"] = description;

            foreach (var prop in dynamicProperties)
            {
                commitObject[prop.Key] = prop.Value;
            }

            var transport = new ServerTransport(account, builderProjectId);
            var objectId = await Operations.Send(commitObject, transport, true);

            string branchId;
            // ensure branch exists
            var branch = await client.BranchGet(builderProjectId, modelCode);

            if (branch == null)
            {
                // create a new branch with the model name
                branchId = await client.BranchCreate(
                    new BranchCreateInput
                    {
                        streamId = builderProjectId,
                        name = modelCode,
                        description = description
                    });                
            }
            else
            {
                if(branch.description != description )
                {
                    branch.description = description;
                    await client.BranchUpdate( 
                        new BranchUpdateInput 
                        {
                            streamId = builderProjectId,
                            id = branch.id,
                            name = modelCode,
                            description = description 
                        });
                }
                branchId = branch.id;
            }

            string revitFilePath = doc.PathName;
            string revitUserName = doc.Application.Username;
            var commitId = await client.CommitCreate(
                                  new CommitCreateInput
                                  {
                                      streamId = builderProjectId,
                                      branchName = modelCode,
                                      objectId = objectId,
                                      message = $"[{revitUserName}]{revitFilePath}",
                                      sourceApplication = "Calc Builder " + revitAppName
                                  });

            //var commitId = Helpers.Send(streamId, commitObject, "My commit message").Result;

            return branchId;


        }
    }
}

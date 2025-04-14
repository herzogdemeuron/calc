using Autodesk.Revit.DB;
using Calc.Core;
using Calc.Core.Objects.Assemblies;
using Speckle.Core.Api;
using Speckle.Core.Api.GraphQL.Inputs;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeckleSender
{
    public class ElementSender : IElementSender
    {
        public bool IsValid { get; }
        private readonly ISpeckleConverter speckleConverter;
        private readonly string revitAppName;
        private readonly Client client;
        private readonly Account account;
        private readonly string builderProjectId;
        private readonly Document doc;

        /// <summary>
        /// Should be using the revit converter from nuget Speckle.Objects.Converter.Revit2023,
        /// or from the installed speckle app kit.
        /// see: https://speckle.community/t/how-to-run-revit-to-speckle-conversions-in-c/1548/6
        /// </summary>
        public ElementSender(Document doc, CalcConfig config, int revitVersion)
        {
            if (!config.IsValid)
            {
                IsValid = false;
                return;
            }
            IsValid = true;
            this.doc = doc;
            //revitAppName = HostApplications.Revit.GetVersion(HostAppVersion.v2023);
            revitAppName = "Revit" + revitVersion;
            var speckleKit = KitManager.GetDefaultKit();
            speckleConverter = speckleKit.LoadConverter(revitAppName);
            speckleConverter.SetContextDocument(doc);
            account = new Account();
            account.token = config.SpeckleToken;
            account.serverInfo = new Speckle.Core.Api.GraphQL.Models.ServerInfo { url = config.SpeckleServerUrl };
            client = new Client(account);
            builderProjectId = config.SpeckleBuilderProjectId;
        }

        /// <summary>
        /// Uses the assembly data to create an speckle base, adding all dynamic properties.
        /// </summary>
        private AssemblyBase CreateAssemblyBase(AssemblyData assemblyData)
        {
            List<object> elementList = assemblyData.ElementIds
                .Select(id => doc.GetElement(new ElementId(id)))
                .ToList()
                .ConvertAll(e => e as Object);
            speckleConverter.SetContextDocument(new RevitDocumentAggregateCache(doc));
            var elementBases = speckleConverter.ConvertToSpeckle(elementList);
            var assemblyBase = new AssemblyBase(assemblyData, elementBases);
            foreach (var dProp in assemblyData.Properties)
            {
                assemblyBase[dProp.Key] = dProp.Value;
            }
            return assemblyBase;
        }


        /// <summary>
        /// Sends the elements to a speckle project with the model name, return the model id, which would be saved back to revit group.
        /// All shared parameters of the group are wrapped as dynamic properties.
        /// </summary>
        public async Task<string> SendAssembly(AssemblyData assemblyData)
        {
            if (!IsValid) return null;
           var assemblyBase = CreateAssemblyBase(assemblyData);
            var transport = new ServerTransport(account, builderProjectId);
            var objectId = await Operations.Send(assemblyBase, transport, true);
            // ensure model exists
            // filter the models by model path (group + code) contains model code
            var filter = new ProjectModelsFilter(null,null,null,null,assemblyData.Code,new List<string> { "Calc Builder","Calc Builder Revit2023"}.AsReadOnly());
            var models = await client.Model.GetModels(builderProjectId, 10000, null, filter);
            // get the model with exactly the same code
            // get the model code by splitting '/' and get the last item
            var found = models.items.Where(m => m.name.Split('/').Last().ToLower() == assemblyData.Code.ToLower()).ToList();
            if (found.Count > 1)
            {
                throw new Exception($"Multiple models with code '{assemblyData.Code}' found");
            }
            var model = found.FirstOrDefault();
            if (model == null)
            {
                // create a new model
                model = await client.Model.Create(
                    new CreateModelInput(assemblyData.ModelPath, assemblyData.Description, builderProjectId)
                    );
            }
            else
            {
                // update the description and name(path) if they don't match
                if(model.description != assemblyData.Description || model.name != assemblyData.ModelPath)
                {
                    model = await client.Model.Update(
                        new UpdateModelInput(model.id, assemblyData.ModelPath, assemblyData.Description, builderProjectId)
                        );
                }
            }
            string revitFilePath = doc.PathName;
            string revitUserName = doc.Application.Username;
            var commitId = await client.Version.Create(
                                  new CommitCreateInput
                                  {
                                      streamId = builderProjectId,
                                      branchName = assemblyData.ModelPath,
                                      objectId = objectId,
                                      message = $"[{revitUserName}]{revitFilePath}",
                                      sourceApplication = "Calc Builder"
                                  });
            return model.id;
        }
    }
}

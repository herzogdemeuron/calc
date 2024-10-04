using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.StorageDrivers;
using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects.Standards;
using Calc.Core.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Calc.Core
{
    /// <summary>
    /// Loads, stores, creates and updates the data from the Directus account.
    /// </summary>
    public class CalcStore
    {
        public event EventHandler<int> ProgressChanged;
        public CalcConfig Config { get; set; }
        public CalcProject ProjectSelected { get; set; }
        public bool AllDataLoaded { get; private set; } = false;
        public List<Unit> UnitsAll { get; set; }
        public List<CalcProject> ProjectsAll { get { return ProjectDriver?.GotManyItems; } }
        public List<LcaStandard> StandardsAll { get { return StandardDriver?.GotManyItems; } }
        public List<AssemblyGroup> AssemblyGroupsAll { get { return AssemblyGroupDriver?.GotManyItems; } }
        public List<Assembly> AssembliesAll { get { return AssemblyDriver?.GotManyItems; } }
        public List<Mapping> MappingsAll { get { return MappingDriver?.GotManyItems; } }
        public List<CustomParamSetting> CustomParamSettingsAll { get { return CustomParamSettingDriver?.GotManyItems; } }
        public List<Material> MaterialsAll { get { return MaterialDriver?.GotManyItems; } }
        public List<MaterialFunction> MaterialFunctionsAll { get { return MaterialFunctionDriver?.GotManyItems; } }
        private Dictionary<LcaStandard, List<Material>> MaterialsOfStandards { get; set; }
        public Mapping MappingSelected { get; set; } // todo: check if created new mapping has the current project assigned
        public List<Mapping> RelatedMappings { get => GetProjectRelated(MappingDriver); }
        public List<QueryTemplate> QueryTemplates { get { return QueryTemplateDriver.GotManyItems; } }
        public QueryTemplate QueryTemplateSelected { get; set; }
        public QueryTemplate BlackQuerySet { get; set; }
        public List<QueryTemplate> RelatedQueryTemplate { get => GetProjectRelated(QueryTemplateDriver); }

        private Directus Directus { get; set; }
        private CalcConfigDriver CalcConfigDriver { get; set; }
        private ProjectStorageDriver ProjectDriver { get; set; }
        private StandardDriver StandardDriver { get; set; }
        private MaterialDriver MaterialDriver { get; set; }
        private MaterialFunctionDriver MaterialFunctionDriver { get; set; }
        private AssemblyGroupDriver AssemblyGroupDriver { get; set; }
        private AssemblyDriver AssemblyDriver { get; set; }
        private MappingDriver MappingDriver { get; set; }
        private QueryTemplateDriver QueryTemplateDriver { get; set; }
        private ProjectResultDriver ResultDriver { get; set; }
        private FolderDriver FolderDriver { get; set; }
        private CustomParamSettingDriver CustomParamSettingDriver { get; set; }

        public CalcStore(Directus directus)
        {
            if (directus.Authenticated == false)
            {
                throw new Exception("Directus not authenticated.");
            }

            DirectusManager.DirectusInstance = directus;
            Directus = directus;
            UnitsAll =   Enum.GetValues(typeof(Unit)).Cast<Unit>().ToList();

            CalcConfigDriver = new CalcConfigDriver();
            ProjectDriver = new ProjectStorageDriver();
            StandardDriver = new StandardDriver();
            MaterialDriver = new MaterialDriver();
            AssemblyDriver = new AssemblyDriver();
            AssemblyGroupDriver = new AssemblyGroupDriver();
            MappingDriver = new MappingDriver();
            QueryTemplateDriver = new QueryTemplateDriver();
            ResultDriver = new ProjectResultDriver();
            FolderDriver = new FolderDriver();
            CustomParamSettingDriver = new CustomParamSettingDriver();
            MaterialFunctionDriver = new MaterialFunctionDriver();
        }


        public async Task<bool> LoadCalcProjectData(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var configsDriver = await DirectusManager.GetSingle<CalcConfigDriver, CalcConfig>(CalcConfigDriver);
                Config = configsDriver.GotItem;
                if (Config == null)
                {
                    throw new Exception("No config found");
                }

                cancellationToken.ThrowIfCancellationRequested();
                ProjectDriver = await DirectusManager.GetMany<ProjectStorageDriver, CalcProject>(ProjectDriver);
                OnProgressChanged(5);

                cancellationToken.ThrowIfCancellationRequested();
                StandardDriver = await DirectusManager.GetMany<StandardDriver, LcaStandard>(StandardDriver);
                OnProgressChanged(10);

                cancellationToken.ThrowIfCancellationRequested();
                CustomParamSettingDriver = await DirectusManager.GetMany<CustomParamSettingDriver, CustomParamSetting>(CustomParamSettingDriver);
                OnProgressChanged(20);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialFunctionDriver = await DirectusManager.GetMany<MaterialFunctionDriver, MaterialFunction>(MaterialFunctionDriver);
                OnProgressChanged(30);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialDriver = await DirectusManager.GetMany<MaterialDriver, Material>(MaterialDriver);
                OnProgressChanged(50);

                cancellationToken.ThrowIfCancellationRequested();
                AssemblyGroupDriver = await DirectusManager.GetMany<AssemblyGroupDriver, AssemblyGroup>(AssemblyGroupDriver);
                OnProgressChanged(60);

                cancellationToken.ThrowIfCancellationRequested();
                AssemblyDriver = await DirectusManager.GetMany<AssemblyDriver, Assembly>(AssemblyDriver);
                OnProgressChanged(70);

                cancellationToken.ThrowIfCancellationRequested();
                MappingDriver = await DirectusManager.GetMany<MappingDriver, Mapping>(MappingDriver);
                OnProgressChanged(80);

                cancellationToken.ThrowIfCancellationRequested();
                QueryTemplateDriver = await DirectusManager.GetMany<QueryTemplateDriver, QueryTemplate>(QueryTemplateDriver);
                OnProgressChanged(90);

                cancellationToken.ThrowIfCancellationRequested();
                FolderDriver = await DirectusManager.GetManySystem<FolderDriver, DirectusFolder>(FolderDriver);
                OnProgressChanged(99);

                LinkMaterials();
                OnProgressChanged(100);
                AllDataLoaded = true;
                return true;
            }
            catch (OperationCanceledException e)
            {
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> LoadCalcBuilderData(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var configsDriver = await DirectusManager.GetSingle<CalcConfigDriver, CalcConfig>(CalcConfigDriver);
                Config = configsDriver.GotItem;
                if (Config == null)
                {
                    throw new Exception("No config found");
                }

                cancellationToken.ThrowIfCancellationRequested();
                StandardDriver = await DirectusManager.GetMany<StandardDriver,LcaStandard>(StandardDriver);
                OnProgressChanged(10);

                cancellationToken.ThrowIfCancellationRequested();
                CustomParamSettingDriver = await DirectusManager.GetMany<CustomParamSettingDriver,CustomParamSetting>(CustomParamSettingDriver);
                OnProgressChanged(20);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialFunctionDriver = await DirectusManager.GetMany<MaterialFunctionDriver, MaterialFunction>(MaterialFunctionDriver);
                OnProgressChanged(30);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialDriver = await DirectusManager.GetMany<MaterialDriver,Material>(MaterialDriver);
                OnProgressChanged(50);

                cancellationToken.ThrowIfCancellationRequested();
                AssemblyGroupDriver = await DirectusManager.GetMany<AssemblyGroupDriver,AssemblyGroup>(AssemblyGroupDriver);
                OnProgressChanged(60);

                cancellationToken.ThrowIfCancellationRequested();
                AssemblyDriver = await DirectusManager.GetMany<AssemblyDriver,Assembly>(AssemblyDriver);
                OnProgressChanged(80);

                cancellationToken.ThrowIfCancellationRequested();
                FolderDriver = await DirectusManager.GetManySystem<FolderDriver,DirectusFolder>(FolderDriver);
                OnProgressChanged(99);

                cancellationToken.ThrowIfCancellationRequested();
                LinkMaterials();
                SortMaterials();
                OnProgressChanged(100);
                AllDataLoaded = true;
                return true;
            }
            catch (OperationCanceledException e)
            {
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }      
        }

        /// <summary>
        /// Sorts all materials to MaterialsOfStandards by their standards.
        /// </summary>
        private void SortMaterials()
        {
            MaterialsOfStandards = new();

            foreach (var standard in StandardsAll)
            {
                MaterialsOfStandards.Add(standard, new List<Material>());
            }
            foreach (var material in MaterialsAll)
            {
                MaterialsOfStandards[material.Standard].Add(material);
            }
        }

        /// <summary>
        /// In the loaded assemblies, the calculation components have only material ids associated,
        /// </summary>
        private void LinkMaterials()
        {
            AssemblyDriver.LinkMaterials(MaterialDriver.GotManyItems);
        }

        /// <summary>
        /// Saves the current mapping state by
        /// Creating a new mapping based on the current query template, uploading to directus.
        /// Broken query set is also taken into consideration with the additional argument.
        /// </summary>
        public async Task<bool> UpdateSelectedMapping(QueryTemplate additional = null)
        {             
            if (MappingSelected == null)
            {
                throw new Exception("Please firstly create a new mapping.");
            }
            // refresh the mapping with the selected query template
            var sendMapping = new Mapping(MappingSelected.Name, QueryTemplateSelected, additional)
            {
                Project = ProjectSelected,
                Id = MappingSelected.Id,
            };

            MappingDriver.SendItem = sendMapping;

            try
            {
                await DirectusManager.UpdateSingle<MappingDriver, Mapping>(MappingDriver);
                MappingSelected.MappingItems = sendMapping.MappingItems;
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }
        }

        /// <summary>
        /// Creates a new mapping on directus, updates the id after creation.
        /// </summary>
        public async Task<bool> CreateMapping(Mapping newMapping)
        {
            newMapping.Project = ProjectSelected;
            MappingDriver.SendItem = newMapping;
            try
            {
                var mappingDriver = await DirectusManager.CreateSingle<MappingDriver, Mapping>(MappingDriver);
                newMapping.Id = mappingDriver.CreatedItem.Id; // todo: check if is working?
                MappingDriver.GotManyItems.Add(newMapping);
                MappingSelected = newMapping;
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }
        }

        /// <summary>
        /// Saves the project result to directus.
        /// </summary>
        public async Task<(bool,string)> SaveProjectResult(ProjectResult snapshot)
        {
            ResultDriver.SendItem = snapshot;
            try
            {
                await DirectusManager.CreateSingle<ProjectResultDriver, ProjectResult>(ResultDriver);
                return (true, null);

            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        /// <summary>
        /// Saves the assembly to directus,
        /// update the assembly id if it is created,
        /// adds back the assembly to assemblies all.
        /// </summary>
        public async Task SaveSingleAssembly(Assembly assembly)
        {
            AssemblyDriver.SendItem = assembly;
            try
            {
                await DirectusManager.CreateSingle<AssemblyDriver, Assembly>(AssemblyDriver);
                var id = AssemblyDriver.CreatedItem?.Id;
                // save the assembly back to assemblies all
                if (id != null)
                {
                    assembly.Id = id.Value;
                    AssemblyDriver.GotManyItems.Add(assembly);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Updates an assembly with an existing id assigned.
        /// </summary>
        public async Task UpdateSingleAssembly(Assembly assembly)
        {
            AssemblyDriver.SendItem = assembly;
            try
            {
                await DirectusManager.UpdateSingle<AssemblyDriver, Assembly>(AssemblyDriver);
                // replace the assembly in assemblies all
                var index = AssemblyDriver.GotManyItems.FindIndex(b => b.Id == assembly.Id);
                if (index != -1)
                {
                    AssemblyDriver.GotManyItems[index] = assembly;
                }
                else
                {
                    throw new Exception("Assembly not found, cannot update.");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private List<T> GetProjectRelated<T>(IDriverGetMany<T> driver) where T : IHasProject
        {
            if (ProjectSelected == null)
            {
                throw new Exception("No project selected");
            }
            return driver.GotManyItems.FindAll(m => m.Project?.Id == ProjectSelected.Id);
        }

        /// <summary>
        /// Uploads an image to directus folder "calc_assembly_images".
        /// </summary>
        public async Task<string> UploadImageAsync(string imagePath, string newFileName)
        {
            if (Directus.Authenticated == false)
            {
                throw new Exception("Directus not authenticated");
            }

            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }
            string folderId = FolderDriver.GetFolderId("calc_assembly_images");
            return await Directus.UploadFileAsync("image", imagePath, folderId, newFileName);
        }

        /// <summary>
        /// Uploads a result json file to directus folder "calc_snapshot_files".
        /// </summary>
        public async Task<string> UploadResultAsync(string jsonPath, string newFileName)
        {
            if (Directus.Authenticated == false)
            {
                throw new Exception("Directus not authenticated");
            }

            if (string.IsNullOrEmpty(jsonPath))
            {
                return null;
            }
            string folderId = FolderDriver.GetFolderId("calc_snapshot_files");
            return await Directus.UploadFileAsync("json", jsonPath, folderId, newFileName);
        }

        /// <summary>
        /// Loads an image from directus with the given id.
        /// </summary>
        public async Task<byte[]> LoadImageAsync(string imageId)
        {
            if (Directus.Authenticated == false)
            {
                return null;
            }
            return await Directus.LoadImageByIdAsync(imageId);
        }

        protected virtual void OnProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }
    }
}

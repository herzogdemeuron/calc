using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.Drivers;
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
        public List<MaterialFunction> MaterialFunctionsAll { get { return MaterialFunctionStorageDriver?.GotManyItems; } }
        private Dictionary<LcaStandard, List<Material>> MaterialsOfStandards { get; set; }


        private Mapping _mappingSelected;
        public Mapping MappingSelected
        {
            get => _mappingSelected;
            set
            {
                _mappingSelected = value;
                _mappingSelected.Project = ProjectSelected; // needed?
            }
            }
        public List<Mapping> MappingsProjectRelated
        {
            get => GetProjectRelated(MappingDriver);
            set => MappingDriver.GotManyItems = value;
        }
        public List<Mapping> MappingsProjectUnrelated
        {
              get => GetProjectUnrelated(MappingDriver);
            set => MappingDriver.GotManyItems = value;
        }

        public List<Forest> Forests { get { return ForestDriver.GotManyItems; } }
        public Forest ForestSelected { get; set; }
        public Forest DarkForestSelected { get; set; }
        public List<Forest> ForestProjectRelated
        {
            get => GetProjectRelated(ForestDriver);
            set => ForestDriver.GotManyItems = value;
        }
        public List<Forest> ForestProjectUnrelated
        {
            get => GetProjectUnrelated(ForestDriver);
            set => ForestDriver.GotManyItems = value;
        }


        private Directus Directus { get; set; }
        private CalcConfigStorageDriver CalcConfigDriver { get; set; }
        private ProjectStorageDriver ProjectDriver { get; set; }
        private StandardStorageDriver StandardDriver { get; set; }
        private MaterialStorageDriver MaterialDriver { get; set; }
        private MaterialFunctionStorageDriver MaterialFunctionStorageDriver { get; set; }
        private AssemblyGroupStorageDriver AssemblyGroupDriver { get; set; }
        private AssemblyStorageDriver AssemblyDriver { get; set; }
        private MappingStorageDriver MappingDriver { get; set; }
        private ForestStorageDriver ForestDriver { get; set; }
        private ProjectResultStorageDriver ResultDriver { get; set; }
        private FolderStorageDriver FolderDriver { get; set; }
        private CustomParamSettingStorageDriver CustomParamSettingDriver { get; set; }

        public CalcStore(Directus directus)
        {
            if (directus.Authenticated == false)
            {
                throw new Exception("CalcStore: Directus not authenticated");
            }

            DirectusDriver.DirectusInstance = directus;
            Directus = directus;

            CalcConfigDriver = new CalcConfigStorageDriver();
            ProjectDriver = new ProjectStorageDriver();
            StandardDriver = new StandardStorageDriver();
            MaterialDriver = new MaterialStorageDriver();
            AssemblyDriver = new AssemblyStorageDriver();
            AssemblyGroupDriver = new AssemblyGroupStorageDriver();
            MappingDriver = new MappingStorageDriver();
            ForestDriver = new ForestStorageDriver();
            ResultDriver = new ProjectResultStorageDriver();
            FolderDriver = new FolderStorageDriver();
            CustomParamSettingDriver = new CustomParamSettingStorageDriver();
            MaterialFunctionStorageDriver = new MaterialFunctionStorageDriver();

            UnitsAll =   Enum.GetValues(typeof(Unit)).Cast<Unit>().ToList();
        }



        public async Task<bool> GetMainData(CancellationToken cancellationToken = default)
        {

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var configsDriver = await DirectusDriver.GetSingle<CalcConfigStorageDriver, CalcConfig>(CalcConfigDriver);
                Config = configsDriver.GotItem;
                if (Config == null)
                {
                    throw new Exception("No config found");
                }

                cancellationToken.ThrowIfCancellationRequested();
                ProjectDriver = await DirectusDriver.GetMany<ProjectStorageDriver, CalcProject>(ProjectDriver);
                OnProgressChanged(5);

                cancellationToken.ThrowIfCancellationRequested();
                StandardDriver = await DirectusDriver.GetMany<StandardStorageDriver, LcaStandard>(StandardDriver);
                OnProgressChanged(10);

                cancellationToken.ThrowIfCancellationRequested();
                CustomParamSettingDriver = await DirectusDriver.GetMany<CustomParamSettingStorageDriver, CustomParamSetting>(CustomParamSettingDriver);
                OnProgressChanged(20);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialFunctionStorageDriver = await DirectusDriver.GetMany<MaterialFunctionStorageDriver, MaterialFunction>(MaterialFunctionStorageDriver);
                OnProgressChanged(30);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialDriver = await DirectusDriver.GetMany<MaterialStorageDriver, Material>(MaterialDriver);
                OnProgressChanged(50);

                cancellationToken.ThrowIfCancellationRequested();
                AssemblyGroupDriver = await DirectusDriver.GetMany<AssemblyGroupStorageDriver, AssemblyGroup>(AssemblyGroupDriver);
                OnProgressChanged(60);

                cancellationToken.ThrowIfCancellationRequested();
                AssemblyDriver = await DirectusDriver.GetMany<AssemblyStorageDriver, Assembly>(AssemblyDriver);
                OnProgressChanged(70);

                cancellationToken.ThrowIfCancellationRequested();
                MappingDriver = await DirectusDriver.GetMany<MappingStorageDriver, Mapping>(MappingDriver);
                OnProgressChanged(80);

                cancellationToken.ThrowIfCancellationRequested();
                ForestDriver = await DirectusDriver.GetMany<ForestStorageDriver, Forest>(ForestDriver);
                OnProgressChanged(90);

                cancellationToken.ThrowIfCancellationRequested();
                FolderDriver = await DirectusDriver.GetManySystem<FolderStorageDriver, DirectusFolder>(FolderDriver);
                OnProgressChanged(99);

                LinkMaterials();
                //SortMaterials();
                //InitStandardSelection();
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

        public async Task<bool> GetBuilderData(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var configsDriver = await DirectusDriver.GetSingle<CalcConfigStorageDriver, CalcConfig>(CalcConfigDriver);
                Config = configsDriver.GotItem;
                if (Config == null)
                {
                    throw new Exception("No config found");
                }

                cancellationToken.ThrowIfCancellationRequested();
                StandardDriver = await DirectusDriver.GetMany<StandardStorageDriver,LcaStandard>(StandardDriver);
                OnProgressChanged(10);

                cancellationToken.ThrowIfCancellationRequested();
                CustomParamSettingDriver = await DirectusDriver.GetMany<CustomParamSettingStorageDriver,CustomParamSetting>(CustomParamSettingDriver);
                OnProgressChanged(20);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialFunctionStorageDriver = await DirectusDriver.GetMany<MaterialFunctionStorageDriver, MaterialFunction>(MaterialFunctionStorageDriver);
                OnProgressChanged(30);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialDriver = await DirectusDriver.GetMany<MaterialStorageDriver,Material>(MaterialDriver);
                OnProgressChanged(50);

                cancellationToken.ThrowIfCancellationRequested();
                AssemblyGroupDriver = await DirectusDriver.GetMany<AssemblyGroupStorageDriver,AssemblyGroup>(AssemblyGroupDriver);
                OnProgressChanged(60);

                cancellationToken.ThrowIfCancellationRequested();
                AssemblyDriver = await DirectusDriver.GetMany<AssemblyStorageDriver,Assembly>(AssemblyDriver);
                OnProgressChanged(80);

                cancellationToken.ThrowIfCancellationRequested();
                FolderDriver = await DirectusDriver.GetManySystem<FolderStorageDriver,DirectusFolder>(FolderDriver);
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
        /// sort all materials to MaterialsOfStandards by the standard
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
        /// the first query got materials, assemblies and some other fields with id separately
        /// link the fields with the right objects with id
        /// </summary>
        private void LinkMaterials()
        {
            AssemblyDriver.LinkMaterials(MaterialDriver.GotManyItems);
        }



        public async Task<bool> UpdateSelectedMapping(Forest additionalForest = null)
        {             
            if (MappingSelected == null)
            {
                throw new Exception("Please firstly create a new mapping.");
            }

            // refresh the mapping with the selected forest
            var sendMapping = new Mapping(MappingSelected.Name, ForestSelected, additionalForest)
            {
                Project = ProjectSelected,
                Id = MappingSelected.Id,
            };

            MappingDriver.SendItem = sendMapping;

            try
            {
                await DirectusDriver.UpdateSingle<MappingStorageDriver, Mapping>(MappingDriver);
                MappingSelected.MappingItems = sendMapping.MappingItems;
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }
        }

        public async Task<bool> CreateMapping(Mapping newMapping)
        {
            newMapping.Project = ProjectSelected;
            MappingDriver.SendItem = newMapping;
            try
            {
                var mappingDriver = await DirectusDriver.CreateSingle<MappingStorageDriver, Mapping>(MappingDriver);
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

        public async Task UpdateSelectedForest()
        {
            if (ForestSelected == null)
            {
                throw new Exception("No forest selected");
            }

            ForestDriver.SendItem = ForestSelected;

            try
            {
                await DirectusDriver.UpdateSingle<ForestStorageDriver, Forest>(ForestDriver);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int?> SaveSelectedForest()
        {
            if (ForestSelected == null)
            {
                throw new Exception("No forest selected");
            }

            ForestDriver.SendItem = ForestSelected;

            try
            {
                await DirectusDriver.CreateSingle<ForestStorageDriver, Forest>(ForestDriver);
                return ForestDriver.CreatedItem?.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<(bool,string)> SaveProjectResult(ProjectResult snapshot)
        {

            ResultDriver.SendItem = snapshot;

            try
            {
                await DirectusDriver.CreateSingle<ProjectResultStorageDriver, ProjectResult>(ResultDriver);
                return (true, null);

            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        public async Task SaveSingleAssembly(Assembly assembly)
        {
            AssemblyDriver.SendItem = assembly;

            try
            {
                await DirectusDriver.CreateSingle<AssemblyStorageDriver, Assembly>(AssemblyDriver);
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
        /// update an assembly with an existing id assigned
        /// </summary>
        public async Task UpdateSingleAssembly(Assembly assembly)
        {
            AssemblyDriver.SendItem = assembly;

            try
            {
                await DirectusDriver.UpdateSingle<AssemblyStorageDriver, Assembly>(AssemblyDriver);
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

        private List<T> GetProjectUnrelated<T>(IDriverGetMany<T> driver) where T : IHasProject
        {
            if (ProjectSelected == null)
            {
                throw new Exception("No project selected");
            }

            return driver.GotManyItems.FindAll(m => m.Project?.Id != ProjectSelected.Id);
        }


        public async Task<string> UploadImageAsync(string imagePath, string newFileName)
        {
            if (Directus.Authenticated == false)
            {
                throw new Exception("CalcStore: Directus not authenticated");
            }

            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }

            string folderId = FolderDriver.GetFolderId("calc_assembly_images");

            return await Directus.UploadFileAsync("image", imagePath, folderId, newFileName);
        }

        public async Task<string> UploadResultAsync(string jsonPath, string newFileName)
        {
            if (Directus.Authenticated == false)
            {
                throw new Exception("CalcStore: Directus not authenticated");
            }

            if (string.IsNullOrEmpty(jsonPath))
            {
                return null;
            }

            string folderId = FolderDriver.GetFolderId("calc_snapshot_files");

            return await Directus.UploadFileAsync("json", jsonPath, folderId, newFileName);
        }

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

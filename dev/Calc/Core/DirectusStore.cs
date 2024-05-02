using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GraphQL.Client.Http;
using Polly;
using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Mappings;
using Calc.Core.Objects.Results;
using Calc.Core.Objects.Materials;
using System.Collections.ObjectModel;
using System.Threading;
using Calc.Core.Objects.Elements;

namespace Calc.Core
{
    public class DirectusStore
    {
        public event EventHandler<int> ProgressChanged;

        public bool AllDataLoaded { get; private set; } = false;
        public List<Unit> UnitsAll { get; set; }
        public List<MaterialFunction> MaterialFunctionsAll { get; set; }
        public List<Project> ProjectsAll { get { return ProjectDriver?.GotManyItems; } }
        public Project ProjectSelected { get; set; } // the current project
        public List<LcaStandard> StandardsAll { get { return StandardDriver?.GotManyItems; } }
        public List<BuildupGroup> BuildupGroupsAll { get { return BuildupGroupDriver?.GotManyItems; } }
        public List<Buildup> BuildupsAll { get { return BuildupDriver?.GotManyItems; } }
        public List<Mapping> MappingsAll { get { return MappingDriver?.GotManyItems; } }
        public List<CustomParamSetting> CustomParamSettingsAll { get { return CustomParamSettingDriver?.GotManyItems; } }
        private List<Material> MaterialsAll { get { return MaterialDriver?.GotManyItems; } }
        private Dictionary<LcaStandard, List<Material>> MaterialsOfStandards { get; set; }
        public List<Material> CurrentMaterials
        {
            get
            {
                if (StandardSelected == null)
                {
                    return new List<Material>();
                }
                return MaterialsOfStandards[StandardSelected];
            }
        }
        public LcaStandard StandardSelected { get; set; }
        public List<Buildup> BuildupsStandardRelated
        {
            get => BuildupsAll.FindAll(b => b.Standard?.Id == StandardSelected?.Id);
        }

        private Mapping _mappingSelected = null;
        public Mapping MappingSelected
        {
            get => _mappingSelected;
            set => SetSelectedMapping(value);
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
        private Forest _forestSelected;
        public Forest ForestSelected
        {
            get => _forestSelected;
            set => SetSelectedForest(value);
        }
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

        public string SnapshotName { get; set; }
        private List<Result> _results;
        public List<Result> Results
        {
            get => _results;
            set => SetResults(value);
        }

        private Directus Directus { get; set; }
        private ProjectStorageDriver ProjectDriver { get; set; }
        private StandardStorageDriver StandardDriver { get; set; }
        private MaterialStorageDriver MaterialDriver { get; set; }
        private BuildupGroupStorageDriver BuildupGroupDriver { get; set; }
        private BuildupStorageDriver BuildupDriver { get; set; }
        private MappingStorageDriver MappingDriver { get; set; }
        private ForestStorageDriver ForestDriver { get; set; }
        private SnapshotStorageDriver SnapshotDriver { get; set; }
        private FolderStorageDriver FolderDriver { get; set; }
        private CustomParamSettingStorageDriver CustomParamSettingDriver { get; set; }

        public DirectusStore(Directus directus)
        {
            if (directus.Authenticated == false)
            {
                throw new Exception("DirectusStore: Directus not authenticated");
            }

            DirectusDriver.DirectusInstance = directus;
            Directus = directus;

            ProjectDriver = new ProjectStorageDriver();
            StandardDriver = new StandardStorageDriver();
            MaterialDriver = new MaterialStorageDriver();
            BuildupDriver = new BuildupStorageDriver();
            BuildupGroupDriver = new BuildupGroupStorageDriver();
            MappingDriver = new MappingStorageDriver();
            ForestDriver = new ForestStorageDriver();
            SnapshotDriver = new SnapshotStorageDriver();
            FolderDriver = new FolderStorageDriver();
            CustomParamSettingDriver = new CustomParamSettingStorageDriver();

            UnitsAll =   Enum.GetValues(typeof(Unit)).Cast<Unit>().ToList();
            MaterialFunctionsAll = Enum.GetValues(typeof(MaterialFunction)).Cast<MaterialFunction>().ToList(); // should this source on directus?
        }


        public async Task GetProjects() // todo: catch exception from here
        {
            ProjectDriver = await DirectusDriver.GetMany<ProjectStorageDriver,Project>(ProjectDriver);
        }

        public async Task<bool> GetModelCheckerData(CancellationToken cancellationToken)
        {
            CheckIfProjectSelected();

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                StandardDriver = await DirectusDriver.GetMany<StandardStorageDriver, LcaStandard>(StandardDriver);
                OnProgressChanged(20);

                cancellationToken.ThrowIfCancellationRequested();
                CustomParamSettingDriver = await DirectusDriver.GetMany<CustomParamSettingStorageDriver, CustomParamSetting>(CustomParamSettingDriver);
                OnProgressChanged(30);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialDriver = await DirectusDriver.GetMany<MaterialStorageDriver, Material>(MaterialDriver);
                OnProgressChanged(40);

                cancellationToken.ThrowIfCancellationRequested();
                BuildupGroupDriver = await DirectusDriver.GetMany<BuildupGroupStorageDriver, BuildupGroup>(BuildupGroupDriver);
                OnProgressChanged(60);

                cancellationToken.ThrowIfCancellationRequested();
                BuildupDriver = await DirectusDriver.GetMany<BuildupStorageDriver, Buildup>(BuildupDriver);
                OnProgressChanged(80);

                cancellationToken.ThrowIfCancellationRequested();
                MappingDriver = await DirectusDriver.GetMany<MappingStorageDriver, Mapping>(MappingDriver);
                OnProgressChanged(90);

                cancellationToken.ThrowIfCancellationRequested();
                ForestDriver = await DirectusDriver.GetMany<ForestStorageDriver, Forest>(ForestDriver);
                OnProgressChanged(99);

                LinkFields();
                SortMaterials();
                InitStandardSelection();
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
                StandardDriver = await DirectusDriver.GetMany<StandardStorageDriver,LcaStandard>(StandardDriver);
                OnProgressChanged(20);

                cancellationToken.ThrowIfCancellationRequested();
                CustomParamSettingDriver = await DirectusDriver.GetMany<CustomParamSettingStorageDriver,CustomParamSetting>(CustomParamSettingDriver);
                OnProgressChanged(30);

                cancellationToken.ThrowIfCancellationRequested();
                MaterialDriver = await DirectusDriver.GetMany<MaterialStorageDriver,Material>(MaterialDriver);
                OnProgressChanged(40);

                cancellationToken.ThrowIfCancellationRequested();
                BuildupGroupDriver = await DirectusDriver.GetMany<BuildupGroupStorageDriver,BuildupGroup>(BuildupGroupDriver);
                OnProgressChanged(60);

                cancellationToken.ThrowIfCancellationRequested();
                BuildupDriver = await DirectusDriver.GetMany<BuildupStorageDriver,Buildup>(BuildupDriver);
                OnProgressChanged(80);

                cancellationToken.ThrowIfCancellationRequested();
                FolderDriver = await DirectusDriver.GetManySystem<FolderStorageDriver,DirectusFolder>(FolderDriver);
                OnProgressChanged(99);

                cancellationToken.ThrowIfCancellationRequested();
                LinkFields();
                SortMaterials();
                InitStandardSelection();
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

        private void InitStandardSelection()
        {
            if (StandardsAll.Count > 0)
            {
                StandardSelected = StandardsAll[0];
            }
        }
        


        /// <summary>
        /// the first query got materials, buildups and some other fields with id separately
        /// link the fields with the right objects with id
        /// </summary>
        private void LinkFields()
        {
            MaterialDriver.LinkStandards(StandardDriver.GotManyItems);
            BuildupDriver.LinkStandards(StandardDriver.GotManyItems);
            BuildupDriver.LinkMaterials(MaterialDriver.GotManyItems);
            BuildupDriver.LinkBuildupGroups(BuildupGroupDriver.GotManyItems);
        }

        private void SetSelectedMapping(Mapping mapping)
        {
            CheckIfProjectSelected();
            mapping.Project = ProjectSelected;
            _mappingSelected = mapping;
        }

        public async Task<bool> UpdateSelectedMapping(Forest additionalForest = null)
        {             
            if (MappingSelected == null)
            {
                throw new Exception("No mapping selected");
            }

            // refresh the mapping with the selected forest
            var sendMapping = new Mapping(MappingSelected.Name, ForestSelected, additionalForest)
            {
                Project = ProjectSelected,
                Id = MappingSelected.Id
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

        public async Task<bool> SaveSelectedMapping()
        {
            if (MappingSelected == null)
            {
                throw new Exception("No mapping selected");
            }

            MappingDriver.SendItem = MappingSelected;

            try
            {
                var mappingDriver = await DirectusDriver.CreateSingle<MappingStorageDriver, Mapping>(MappingDriver);
                MappingSelected.Id = mappingDriver.CreatedItem.Id; // todo: check if is working?
                MappingDriver.GotManyItems.Add(MappingSelected);
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }
        }

        private void SetSelectedForest(Forest forest)
        {
            CheckIfProjectSelected();
            forest.Project = ProjectSelected;
            _forestSelected = forest;
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

        private void SetResults(List<Result> results) // todo: simplify this
        {
            CheckIfProjectSelected();
            if (SnapshotName == null)
            {
                throw new Exception("Set SnapshotName first!");
            }

            _results = results;
        }

        public async Task<bool> SaveSnapshot()
        {
            if (Results == null)
            {
                throw new Exception("Set Results first!");
            }

            SnapshotDriver = new SnapshotStorageDriver
            {
                SendItem = new Snapshot 
                { 
                    Results = this.Results,
                    Name = SnapshotName,
                    Project = ProjectSelected
                }
            };
            
            try
            {
                await DirectusDriver.CreateSingle<SnapshotStorageDriver, Snapshot>(SnapshotDriver);
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }
        }

        public async Task SaveSingleBuildup(Buildup buildup)
        {
            BuildupDriver.SendItem = buildup;

            try
            {
                await DirectusDriver.CreateSingle<BuildupStorageDriver, Buildup>(BuildupDriver);
                var id = BuildupDriver.CreatedItem?.Id;
                // save the buildup back to buildups all
                if (id != null)
                {
                    buildup.Id = id.Value;
                    BuildupDriver.GotManyItems.Add(buildup);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// update a buildup with an existing id assigned
        /// </summary>
        public async Task UpdateSingleBuildup(Buildup buildup)
        {
            BuildupDriver.SendItem = buildup;

            try
            {
                await DirectusDriver.UpdateSingle<BuildupStorageDriver, Buildup>(BuildupDriver);
                // replace the buildup in buildups all
                var index = BuildupDriver.GotManyItems.FindIndex(b => b.Id == buildup.Id);
                if (index != -1)
                {
                    BuildupDriver.GotManyItems[index] = buildup;
                }
                else
                {
                    throw new Exception("Buildup not found, cannot update.");
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

        private void CheckIfProjectSelected()
        {
            if (ProjectSelected == null)
            {
                throw new Exception("No project selected");
            }
        }

        public async Task<string> UploadImageAsync(string imagePath, string newFileName)
        {
            if (Directus.Authenticated == false)
            {
                throw new Exception("DirectusStore: Directus not authenticated");
            }

            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }

            string folderId = FolderDriver.GetFolderId("calc_buildup_images");

            return await Directus.UploadImageAsync(imagePath, folderId, newFileName);
        }

        protected virtual void OnProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }
    }
}

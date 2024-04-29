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
        public List<Project> ProjectsAll { get { return this.ProjectDriver?.GotManyItems; } }
        public Project ProjectSelected { get; set; } // the current project
        public List<LcaStandard> StandardsAll { get { return this.StandardDriver?.GotManyItems; } }
        public List<BuildupGroup> BuildupGroupsAll { get { return this.BuildupGroupDriver?.GotManyItems; } }
        public List<Buildup> BuildupsAll { get { return this.BuildupDriver?.GotManyItems; } }
        public List<Mapping> MappingsAll { get { return this.MappingDriver?.GotManyItems; } }
        public List<CustomParamSetting> CustomParamSettingsAll { get { return this.CustomParamSettingDriver?.GotManyItems; } }
        private List<Material> MaterialsAll { get { return this.MaterialDriver?.GotManyItems; } }
        private Dictionary<LcaStandard, List<Material>> MaterialsOfStandards { get; set; }
        public List<Material> CurrentMaterials
        {
            get
            {
                if (this.StandardSelected == null)
                {
                    return new List<Material>();
                }
                return MaterialsOfStandards[this.StandardSelected];
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

        public List<Forest> Forests { get { return this.ForestDriver.GotManyItems; } }
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
        private DirectusManager<Project> ProjectManager { get; set; }
        private DirectusManager<LcaStandard> StandardManager { get; set; }
        private DirectusManager<Material> MaterialManager { get; set; }
        private DirectusManager<BuildupGroup> BuildupGroupManager { get; set; }
        private DirectusManager<Buildup> BuildupManager { get; set; }
        private DirectusManager<Mapping> MappingManager { get; set; }
        private DirectusManager<Forest> ForestManager { get; set; }
        private DirectusManager<Snapshot> SnapshotManager { get; set; }
        private DirectusManager<DirectusFolder> FolderManager { get; set; }
        private DirectusManager<CustomParamSetting> CustomParamSettingManager { get; set; }

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

        private readonly Polly.Retry.AsyncRetryPolicy _graphqlRetry = Policy.Handle<GraphQLHttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} for {exception.Message}");
                });

        public DirectusStore(Directus directus)
        {
            if (directus.Authenticated == false)
            {
                throw new Exception("DirectusStore: Directus not authenticated");
            }

            this.Directus = directus;

            this.ProjectManager = new DirectusManager<Project>(this.Directus);
            this.StandardManager = new DirectusManager<LcaStandard>(this.Directus);
            this.MaterialManager = new DirectusManager<Material>(this.Directus);
            this.BuildupManager = new DirectusManager<Buildup>(this.Directus);
            this.MappingManager = new DirectusManager<Mapping>(this.Directus);
            this.BuildupGroupManager = new DirectusManager<BuildupGroup>(this.Directus);
            this.ForestManager = new DirectusManager<Forest>(this.Directus);
            this.SnapshotManager = new DirectusManager<Snapshot>(this.Directus);
            this.FolderManager = new DirectusManager<DirectusFolder>(this.Directus);
            this.CustomParamSettingManager = new DirectusManager<CustomParamSetting>(this.Directus);

            this.ProjectDriver = new ProjectStorageDriver();
            this.StandardDriver = new StandardStorageDriver();
            this.MaterialDriver = new MaterialStorageDriver();
            this.BuildupDriver = new BuildupStorageDriver();
            this.BuildupGroupDriver = new BuildupGroupStorageDriver();
            this.MappingDriver = new MappingStorageDriver();
            this.ForestDriver = new ForestStorageDriver();
            this.SnapshotDriver = new SnapshotStorageDriver();
            this.FolderDriver = new FolderStorageDriver();
            this.CustomParamSettingDriver = new CustomParamSettingStorageDriver();

            UnitsAll =   Enum.GetValues(typeof(Unit)).Cast<Unit>().ToList();
            MaterialFunctionsAll = Enum.GetValues(typeof(MaterialFunction)).Cast<MaterialFunction>().ToList();
        }


        public async Task<bool> GetProjects()
        {
            try
            {
                this.ProjectDriver = await _graphqlRetry.ExecuteAsync(() => 
                    this.ProjectManager.GetMany<ProjectStorageDriver>(this.ProjectDriver));
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }
        }
        public async Task<bool> GetModelCheckerData()
        {
            CheckIfProjectSelected();

            try
            {
                this.StandardDriver = await _graphqlRetry.ExecuteAsync(() =>
                    this.StandardManager.GetMany<StandardStorageDriver>(this.StandardDriver));

                this.MaterialDriver = await _graphqlRetry.ExecuteAsync(() =>
                    this.MaterialManager.GetMany<MaterialStorageDriver>(this.MaterialDriver));

                this.BuildupGroupDriver = await _graphqlRetry.ExecuteAsync(() =>
                    this.BuildupGroupManager.GetMany<BuildupGroupStorageDriver>(this.BuildupGroupDriver));

                this.BuildupDriver = await _graphqlRetry.ExecuteAsync(() => 
                    this.BuildupManager.GetMany<BuildupStorageDriver>(this.BuildupDriver));

                this.MappingDriver = await _graphqlRetry.ExecuteAsync(() => 
                    this.MappingManager.GetMany<MappingStorageDriver>(this.MappingDriver));

                this.ForestDriver = await _graphqlRetry.ExecuteAsync(() => 
                    this.ForestManager.GetMany<ForestStorageDriver>(this.ForestDriver));

                this.CustomParamSettingDriver = await _graphqlRetry.ExecuteAsync(() => 
                                   this.CustomParamSettingManager.GetMany<CustomParamSettingStorageDriver>(this.CustomParamSettingDriver));

                LinkFields();
                InitStandardSelection();
                AllDataLoaded = true;
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> GetBuilderData(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.StandardDriver = await _graphqlRetry.ExecuteAsync(() =>
                    this.StandardManager.GetMany<StandardStorageDriver>(this.StandardDriver));
                OnProgressChanged(20);

                cancellationToken.ThrowIfCancellationRequested();
                this.CustomParamSettingDriver = await _graphqlRetry.ExecuteAsync(() =>
                                   this.CustomParamSettingManager.GetMany<CustomParamSettingStorageDriver>(this.CustomParamSettingDriver));
                OnProgressChanged(30);

                cancellationToken.ThrowIfCancellationRequested();
                this.MaterialDriver = await _graphqlRetry.ExecuteAsync(() =>
                    this.MaterialManager.GetMany<MaterialStorageDriver>(this.MaterialDriver));
                OnProgressChanged(40);

                cancellationToken.ThrowIfCancellationRequested();
                this.BuildupGroupDriver = await _graphqlRetry.ExecuteAsync(() =>
                    this.BuildupGroupManager.GetMany<BuildupGroupStorageDriver>(this.BuildupGroupDriver));
                OnProgressChanged(60);

                cancellationToken.ThrowIfCancellationRequested();
                this.BuildupDriver = await _graphqlRetry.ExecuteAsync(() =>
                    this.BuildupManager.GetMany<BuildupStorageDriver>(this.BuildupDriver));
                OnProgressChanged(80);

                cancellationToken.ThrowIfCancellationRequested();
                this.FolderDriver = await _graphqlRetry.ExecuteAsync(() =>
                                 this.FolderManager.GetManySystem<FolderStorageDriver>(this.FolderDriver));
                OnProgressChanged(90);

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
            if (this.StandardsAll.Count > 0)
            {
                this.StandardSelected = this.StandardsAll[0];
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
            mapping.Project = this.ProjectSelected;
            this._mappingSelected = mapping;
        }

        public async Task<bool> UpdateSelectedMapping(Forest additionalForest = null)
        {             
            if (this.MappingSelected == null)
            {
                throw new Exception("No mapping selected");
            }

            // refresh the mapping with the selected forest
            var sendMapping = new Mapping(this.MappingSelected.Name, this.ForestSelected, additionalForest)
            {
                Project = this.ProjectSelected,
                Id = this.MappingSelected.Id
            };

            this.MappingDriver.SendItem = sendMapping;

            try
            {
                await _graphqlRetry.ExecuteAsync(() => 
                        this.MappingManager.UpdateSingle<MappingStorageDriver>(this.MappingDriver));
                this.MappingSelected.MappingItems = sendMapping.MappingItems;
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
            if (this.MappingSelected == null)
            {
                throw new Exception("No mapping selected");
            }

            this.MappingDriver.SendItem = this.MappingSelected;

            try
            {
                var mappingDriver = await _graphqlRetry.ExecuteAsync(() =>
                        this.MappingManager.CreateSingle<MappingStorageDriver>(this.MappingDriver));
                this.MappingSelected.Id = mappingDriver.CreatedItem.Id;
                this.MappingDriver.GotManyItems.Add(this.MappingSelected);
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
            forest.Project = this.ProjectSelected;
            this._forestSelected = forest;
        }

        public async Task UpdateSelectedForest()
        {
            if (this.ForestSelected == null)
            {
                throw new Exception("No forest selected");
            }

            this.ForestDriver.SendItem = this.ForestSelected;

            try
            {
                await _graphqlRetry.ExecuteAsync(() =>
                        this.ForestManager.UpdateSingle<ForestStorageDriver>(this.ForestDriver));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int?> SaveSelectedForest()
        {
            if (this.ForestSelected == null)
            {
                throw new Exception("No forest selected");
            }

            this.ForestDriver.SendItem = this.ForestSelected;

            try
            {
                await _graphqlRetry.ExecuteAsync(() =>
                        this.ForestManager.CreateSingle<ForestStorageDriver>(this.ForestDriver));
                return this.ForestDriver.CreatedItem?.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void SetResults(List<Result> results)
        {
            CheckIfProjectSelected();
            if (this.SnapshotName == null)
            {
                throw new Exception("Set SnapshotName first!");
            }

            this._results = results;
        }

        public async Task<bool> SaveSnapshot()
        {
            if (this.Results == null)
            {
                throw new Exception("Set Results first!");
            }

            this.SnapshotDriver = new SnapshotStorageDriver
            {
                SendItem = new Snapshot 
                { 
                    Results = this.Results,
                    Name = this.SnapshotName,
                    Project = this.ProjectSelected
                }
            };
            
            try
            {
                await this.SnapshotManager.CreateSingle<SnapshotStorageDriver>(this.SnapshotDriver);
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
            this.BuildupDriver.SendItem = buildup;

            try
            {
                await _graphqlRetry.ExecuteAsync(() =>
                                       this.BuildupManager.CreateSingle<BuildupStorageDriver>(this.BuildupDriver));
                var id = this.BuildupDriver.CreatedItem?.Id;
                // save the buildup back to buildups all
                if (id != null)
                {
                    buildup.Id = id.Value;
                    this.BuildupDriver.GotManyItems.Add(buildup);
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
            this.BuildupDriver.SendItem = buildup;

            try
            {
                await _graphqlRetry.ExecuteAsync(() =>
                                       this.BuildupManager.UpdateSingle<BuildupStorageDriver>(this.BuildupDriver));
                // replace the buildup in buildups all
                var index = this.BuildupDriver.GotManyItems.FindIndex(b => b.Id == buildup.Id);
                if (index != -1)
                {
                    this.BuildupDriver.GotManyItems[index] = buildup;
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
            if (this.ProjectSelected == null)
            {
                throw new Exception("No project selected");
            }

            return driver.GotManyItems.FindAll(m => m.Project?.Id == this.ProjectSelected.Id);
        }

        private List<T> GetProjectUnrelated<T>(IDriverGetMany<T> driver) where T : IHasProject
        {
            if (this.ProjectSelected == null)
            {
                throw new Exception("No project selected");
            }

            return driver.GotManyItems.FindAll(m => m.Project?.Id != this.ProjectSelected.Id);
        }

        private void CheckIfProjectSelected()
        {
            if (this.ProjectSelected == null)
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

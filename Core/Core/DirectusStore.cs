using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Polly;
using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.Objects;

namespace Calc.Core
{
    public class DirectusStore
    {
        public List<Project> ProjectsAll { get { return this.ProjectDriver.GotManyItems; } }
        public Project ProjectSelected { get; set; } // the current project

        public List<Buildup> BuildupsAll { get { return this.BuildupDriver.GotManyItems; } }

        public List<Mapping> MappingsAll { get { return this.MappingDriver.GotManyItems; } }
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
        public List<Result> Results
        {
            get => Results;
            set => SetResults(value);
        }

        private Directus Directus { get; set; }
        private DirectusManager<Project> ProjectManager { get; set; }
        private DirectusManager<Buildup> BuildupManager { get; set; }
        private DirectusManager<Mapping> MappingManager { get; set; }
        private DirectusManager<Forest> ForestManager { get; set; }
        private DirectusManager<Result> ResultManager { get; set; }

        private ProjectStorageDriver ProjectDriver { get; set; }
        private BuildupStorageDriver BuildupDriver { get; set; }
        private MappingStorageDriver MappingDriver { get; set; }
        private ForestStorageDriver ForestDriver { get; set; }
        private ResultStorageDriver ResultDriver { get; set; }

        private readonly Polly.Retry.AsyncRetryPolicy _graphqlRetry = Policy.Handle<GraphQLHttpRequestException>()
                .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retryCount, context) =>
                {

                    if (retryCount == 4)
                    {
                        Environment.Exit(1);
                    }
                });

        public DirectusStore(Directus directus)
        {
            if (directus.Authenticated == false)
            {
                throw new Exception("DirectusStore: Directus not authenticated");
            }

            this.Directus = directus;

            this.ProjectManager = new DirectusManager<Project>(this.Directus);
            this.BuildupManager = new DirectusManager<Buildup>(this.Directus);
            this.MappingManager = new DirectusManager<Mapping>(this.Directus);
            this.ForestManager = new DirectusManager<Forest>(this.Directus);

            this.ProjectDriver = new ProjectStorageDriver();
            this.BuildupDriver = new BuildupStorageDriver();
            this.MappingDriver = new MappingStorageDriver();
            this.ForestDriver = new ForestStorageDriver();
        }

        public async Task GetProjects()
        {
            try
            {
            this.ProjectDriver = await _graphqlRetry.ExecuteAsync(() => 
                    this.ProjectManager.GetMany<ProjectStorageDriver>(this.ProjectDriver));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetOtherData()
        {
            CheckIfProjectSelected();

            try
            {
            this.BuildupDriver = await _graphqlRetry.ExecuteAsync(() => 
                    this.BuildupManager.GetMany<BuildupStorageDriver>(this.BuildupDriver));
            this.MappingDriver = await _graphqlRetry.ExecuteAsync(() => 
                    this.MappingManager.GetMany<MappingStorageDriver>(this.MappingDriver));
            this.ForestDriver = await _graphqlRetry.ExecuteAsync(() => 
                    this.ForestManager.GetMany<ForestStorageDriver>(this.ForestDriver));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SetSelectedMapping(Mapping mapping)
        {
            CheckIfProjectSelected();
            mapping.Project = this.ProjectSelected;
            this._mappingSelected = mapping;
        }

        public async Task UpdateSelectedMapping()
        {             
            if (this.MappingSelected == null)
            {
                throw new Exception("No mapping selected");
            }

            this.MappingDriver.SendItem = this.MappingSelected;

            try
            {
                await _graphqlRetry.ExecuteAsync(() => 
                        this.MappingManager.UpdateSingle<MappingStorageDriver>(this.MappingDriver));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int?> SaveSelectedMapping()
        {
            if (this.MappingSelected == null)
            {
                throw new Exception("No mapping selected");
            }

            this.MappingDriver.SendItem = this.MappingSelected;

            try
            {
                await _graphqlRetry.ExecuteAsync(() =>
                        this.MappingManager.CreateSingle<MappingStorageDriver>(this.MappingDriver));
                return this.MappingDriver.CreatedItem?.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SetSelectedForest(Forest forest)
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

        public void SetResults(List<Result> results)
        {
            CheckIfProjectSelected();
            if (this.SnapshotName == null)
            {
                throw new Exception("Set SnapshotName first!");
            }

            foreach (var result in results)
            {
                result.Project = this.ProjectSelected;
                result.SnapshotName = this.SnapshotName;
            }

            this.Results = results;
        }
        public async Task SaveResults()
        {
            if (this.Results == null)
            {
                throw new Exception("Set Results first!");
            }

            this.ResultDriver = new ResultStorageDriver
            {
                SendItems = this.Results
            };
            
            foreach (var result in this.ResultDriver.SendItems)
            {
                result.Project = this.ProjectSelected;
            }

            try
            {
                await _graphqlRetry.ExecuteAsync(() =>
                        this.ResultManager.CreateMany<ResultStorageDriver>(this.ResultDriver));
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
    }
}

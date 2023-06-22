using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Core.DirectusAPI.StorageDrivers;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit
{
    public class BuildupManager
    {
        private BuildupStorageDriver buildupStorageDriver;

        public BuildupManager() 
        {
            buildupStorageDriver = new BuildupStorageDriver(App.Directus);
        }

        public async Task<List<Buildup>> GetAllBuildups()
        {
            return await buildupStorageDriver.GetAllBuildupsFromDirectus();
        }

    }
}

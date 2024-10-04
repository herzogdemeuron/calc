using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get calc folders from directus.
    /// </summary>
    internal class FolderDriver : IDriverGetManySystem<DirectusFolder>
    {
        public string QueryGetManySystem { get; } = @"
                query { 
                    folders {
                    id
                    name
                    }
                }";

        [JsonProperty("folders")]
        public List<DirectusFolder> GotManyItems { get; set; }

        /// <summary>
        /// Provides creation variables.
        /// </summary>
        public string GetFolderId(string folderName)
        {
            if (GotManyItems == null) return null;

            foreach (var folder in GotManyItems)
            {
                if (folder.Name == folderName)
                {
                    return folder.Id;
                }
            }
            return null;
        }
    }
}

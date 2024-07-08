using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class FolderStorageDriver : IDriverGetManySystem<DirectusFolder>
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
        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}

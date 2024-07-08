using Calc.Core;
using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using Newtonsoft.Json;

namespace Calc.MVVM.Services
{
    public class SnapshotSender
    {
        /// <summary>
        /// save the snapshot, returns if saved and the error message
        /// </summary>
        public static async Task<(bool?,string)> SaveSnapshot(DirectusStore store, List<LayerResult> layerResults, string newName)
        {
            if (layerResults == null) return (null, null);
            if (layerResults.Count == 0) return (null, null);
            if (store == null) return (null, null);

            var jsonPath = CreateResultJsonFile(newName, layerResults);
            string ResultUuid = await store.UploadResultAsync(jsonPath, newName);
            int count = layerResults.Count;
            var selectedProject = store.ProjectSelected;
            var projectName = selectedProject?.Name;
            var forestName = store.ForestSelected?.Name;
            var mappingName = store.MappingSelected?.Name;
            var description = $"Project: {projectName}\nForest: {forestName}\nMapping: {mappingName}\nLayer count: {count}";

            var snapshot = new Snapshot()
            {
                Name = newName,
                JsonUuid = ResultUuid,
                Project = store.ProjectSelected,
                Description = description
            };

            return await store.SaveSnapshot(snapshot);
        }

        private static string GetFilePath(string baseName)
        {
            string time = DateTime.Now.ToString("yyMMddHHmmss");
            string path = Path.Combine(Path.GetTempPath(), time + "_" + baseName + ".json");
            return path;
        }

        public static string CreateResultJsonFile(string baseName, List<LayerResult> results)
        {
            string filePath = GetFilePath(baseName);

            var json = JsonConvert.SerializeObject(results, Formatting.Indented);
            // save the json as the path
            File.WriteAllText(filePath, json);
            return filePath;
        }
    }
}

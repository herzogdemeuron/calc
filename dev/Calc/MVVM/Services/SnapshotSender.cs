using Calc.Core;
using Calc.Core.Snapshots;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Calc.MVVM.Services
{
    public class SnapshotSender
    {
        /// <summary>
        /// save the snapshot, returns if saved and the error message
        /// </summary>
        public static async Task<(bool?,string)> SaveProjectSnapshot(CalcStore store, List<AssemblySnapshot> assemblySnapshots, string newName)
        {
            if (assemblySnapshots == null) return (null, null);
            if (assemblySnapshots.Count == 0) return (null, null);
            if (store == null) return (null, null);

            var pSnapshot = MakeProjectSnapshot(store, assemblySnapshots);
            var jsonPath = CreateResultJsonFile(newName, pSnapshot);
            string ResultUuid = await store.UploadResultAsync(jsonPath, newName);
            int count = assemblySnapshots.Count;
            var selectedProject = store.ProjectSelected;
            var projectName = selectedProject?.Name;
            var forestName = store.ForestSelected?.Name;
            var mappingName = store.MappingSelected?.Name;
            var description = $"Project: {projectName}\nForest: {forestName}\nMapping: {mappingName}\nLayer count: {count}";

            var pResult = new ProjectResult()
            {
                Name = newName,
                JsonUuid = ResultUuid,
                Project = store.ProjectSelected,
                Description = description
            };

            return await store.SaveProjectResult(pResult);
        }

        private static ProjectSnapshot MakeProjectSnapshot(CalcStore store, List<AssemblySnapshot> assemblySnapshots)
        {
            var aSnapshots = SnapshotMaker.MergeAssemblySnapshots(assemblySnapshots);
            var snapshot = new ProjectSnapshot()
            {
                ProjectNumber = store.ProjectSelected.Number,
                ProjectName = store.ProjectSelected.Name,
                ClassificationSystem = store.ForestSelected.Name,
                Location = store.ProjectSelected.Location,
                LcaMethod = store.ProjectSelected.LcaMethod,
                LifeSpan = store.ProjectSelected.LifeSpan,
                Stages = store.ProjectSelected.Stages,
                ImpactCategories = store.ProjectSelected.ImpactCategories,
                AssemblySnapshots = aSnapshots
            };
            return snapshot;
        }

        private static string GetFilePath(string baseName)
        {
            string time = DateTime.Now.ToString("yyMMddHHmmss");
            string path = Path.Combine(Path.GetTempPath(), time + "_" + baseName + ".json");
            return path;
        }

        public static string CreateResultJsonFile(string baseName,ProjectSnapshot snapshot)
        {
            string filePath = GetFilePath(baseName);
            var json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
            // save the json as the path
            File.WriteAllText(filePath, json);
            return filePath;
        }
    }
}

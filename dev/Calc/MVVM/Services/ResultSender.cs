using Calc.Core;
using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;

namespace Calc.MVVM.Services
{
    public class ResultSender
    {
        public static async Task<bool?> SaveResults(DirectusStore store, List<LayerResult> layerResults, string newName)
        {
            if (layerResults == null) return null;
            if (layerResults.Count == 0) return null;
            if (store == null) return null;

            store.SnapshotName = newName;
            store.Results = layerResults;
            return await store.SaveSnapshot();
        }

        private static string GetFilePath(string baseName)
        {
            string time = DateTime.Now.ToString("yyMMddHHmmss");
            string path = Path.Combine(Path.GetTempPath(), time + "_" + baseName + ".json");
            return path;
        }

        public string CreateResultJsonFile(string baseName)
        {
            string imagePath = GetFilePath(baseName);

            ImageExportOptions options = new ImageExportOptions()
            {
                FilePath = imagePath,
                ZoomType = ZoomFitType.FitToPage,
                ImageResolution = ImageResolution.DPI_300,
                PixelSize = 2048,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ShadowViewsFileType = ImageFileType.PNG,
                ExportRange = ExportRange.CurrentView,
            };

            try
            {
                doc.ExportImage(options);
                return imagePath;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}

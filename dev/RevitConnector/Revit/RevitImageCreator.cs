using Autodesk.Revit.DB;
using Calc.Core;
using System;
using System.IO;

namespace Calc.RevitConnector.Revit
{
    public class RevitImageCreator : IImageSnapshotCreator
    {
        private readonly Document doc;
        public RevitImageCreator(Document doc)
        {
            this.doc = doc;
        }

        /// <summary>
        /// Creates the temporary file path.
        /// </summary>
        private string GetFilePath(string baseName)
        {
            string path = Path.Combine(Path.GetTempPath(), baseName + ".png");
            int counter = 1;
            while (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    break;
                }
                catch (Exception e)
                {
                    path = Path.Combine(Path.GetTempPath(), $"{baseName}_{counter}.png");
                    counter++;
                }
            }
            return path;
        }

        /// <summary>
        /// Creates the image snapshot to a temporary file path.
        /// </summary>
        public string CreateImageSnapshot(string baseName)
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

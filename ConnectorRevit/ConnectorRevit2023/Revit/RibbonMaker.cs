using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;


namespace Calc.ConnectorRevit.Revit
{
    public class RibbonMaker
    {
        public static void Create(UIControlledApplication uiCtrlApp)
        {
            RibbonPanel panel = uiCtrlApp.CreateRibbonPanel("Calc");
            PushButtonData buttonData = new PushButtonData(
                "CalcButton",
                "Start Calc",
                Assembly.GetExecutingAssembly().Location,
                "Calc.ConnectorRevit.Revit.StartCommand");
            PushButton button = panel.AddItem(buttonData) as PushButton;
            Uri uriImage = new Uri("pack://application:,,,/CalcConnectorRevit2023;component/Resources/icon.png", UriKind.Absolute);
                button.LargeImage = new BitmapImage(uriImage);
        }
    }
}



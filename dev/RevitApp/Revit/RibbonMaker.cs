using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;


namespace Calc.RevitApp.Revit
{
    public class RibbonMaker
    {
        public static void Create(UIControlledApplication uiCtrlApp, string panelName)
        {
            RibbonPanel panel = uiCtrlApp.CreateRibbonPanel(panelName);

            CreateButton(
                panel, 
                "CalcBuilderButton", 
                "Calc Builder", 
                "Calc.RevitApp.Revit.CalcBuilderCommand", 
                new Uri("pack://application:,,,/CalcRevitApp;component/Resources/tab_calc_builder_icon.png", UriKind.Absolute));

            CreateButton(
                panel,
                "CalcProjectButton",
                "Calc Project",
                "Calc.RevitApp.Revit.CalcProjectCommand",
                new Uri("pack://application:,,,/CalcRevitApp;component/Resources/tab_calc_project_icon.png", UriKind.Absolute));
        }

        private static void CreateButton(RibbonPanel panel, string buttonName, string buttonText, string commandName, Uri iconUri)
        {
            PushButtonData buttonData = new PushButtonData(
                buttonName,
                buttonText,                
                Assembly.GetExecutingAssembly().Location,
                commandName);
            PushButton button = panel.AddItem(buttonData) as PushButton;
            button.LargeImage = new BitmapImage(iconUri);
        }
    }
}



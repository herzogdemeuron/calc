using Calc.Core.Objects.Results;
using Calc.Core;
using Calc.MVVM.Services;
using Calc.MVVM.ViewModels;
using Calc.MVVM.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Calc.Core.DirectusAPI;

namespace ui_test
{
    [TestClass]
    public class MvvmTest
    {
        private Directus directusInstance;
        [TestMethod]
        public void RunMvvm()
        {
            try
            {

                Task.Run(() => Authenticate()).Wait();
                if (directusInstance == null) return;

                DirectusStore store = new DirectusStore(directusInstance);
                MockBuildupComponentCreator componentCreator = new MockBuildupComponentCreator();

                //List<BuildupComponent> components = componentCreator.CreateBuildupComponentsFromSelection();
                //TaskDialog.Show("Components", components.Count.ToString());

                BuilderViewModel builderViewModel = new BuilderViewModel(store, componentCreator);
                BuilderView builderView = new BuilderView(builderViewModel);

                builderView.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task Authenticate()
        {
            var authenticator = new DirectusAuthenticator();
            directusInstance = await authenticator.ShowLoginWindowAsync("https://hdm-dt.directus.app", "test@abc.com","123456").ConfigureAwait(false);
        }
    }
}

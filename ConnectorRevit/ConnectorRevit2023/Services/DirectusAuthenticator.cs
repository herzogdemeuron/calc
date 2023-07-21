using Autodesk.Revit.UI;
using Calc.ConnectorRevit.Views;
using Calc.Core.DirectusAPI;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calc.ConnectorRevit.Services
{
    public class DirectusAuthenticator
    {
        public async Task<Directus> ShowLoginWindowAsync()
        {

            var url = Properties.Settings.Default.Config1;
            var email = Properties.Settings.Default.Config2;
            var password = Properties.Settings.Default.Config3;

            var directus = new Directus();

            while (!directus.Authenticated)
            {
                using (var inputDialog = new StringInputDialog(url, email, password))
                {
                    if (inputDialog.ShowDialog() == DialogResult.OK)
                    {
                        url = inputDialog.DirectusUrl;
                        email = inputDialog.Email;
                        password = inputDialog.Password;
                    }
                    if (inputDialog.DialogResult == DialogResult.Cancel)
                    {
                        Debug.WriteLine("Directus login cancelled");
                        return null;
                    }
                }
                await directus.Authenticate(url, email, password);
            }

            if (!directus.Authenticated)
            {
                TaskDialog.Show("Directus Login", "Login failed");
                return null;
            }

            Properties.Settings.Default.Config1 = url;
            Properties.Settings.Default.Config2 = email;
            Properties.Settings.Default.Config3 = password;

            Properties.Settings.Default.Save();

            return directus;
        }

    }

}

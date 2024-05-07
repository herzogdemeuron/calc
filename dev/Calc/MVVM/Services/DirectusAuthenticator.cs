using Calc.Core.DirectusAPI;
using Calc.MVVM.Views;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calc.MVVM.Services
{
    public class DirectusAuthenticator // deprecated
    {
        public async Task<Directus> ShowLoginWindowAsync(string _url = null, string _email = null, string _password = null)
        {

            var url = _url??Properties.Settings.Default.Config1;
            var email = _email??Properties.Settings.Default.Config2;
            var password = _password??Properties.Settings.Default.Config3;

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
                //TaskDialog.Show("Directus Login", "Login failed");
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

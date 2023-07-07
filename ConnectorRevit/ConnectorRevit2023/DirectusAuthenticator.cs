using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Calc.ConnectorRevit.Views;
using Calc.Core.DirectusAPI;

namespace Calc.ConnectorRevit
{
    public class DirectusAuthenticator
    {
        public async Task<Directus> ShowLoginWindowAsync()
        {

            var url = Properties.Settings.Default.DirectusUrl;
            var email = Properties.Settings.Default.DirectusEmail;
            var password = Properties.Settings.Default.DirectusPassword;

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
                return null;
            }

            Properties.Settings.Default.DirectusUrl = url;
            Properties.Settings.Default.DirectusEmail = email;
            Properties.Settings.Default.DirectusPassword = password;

            Properties.Settings.Default.Save();

            return directus;
        }
    }

}

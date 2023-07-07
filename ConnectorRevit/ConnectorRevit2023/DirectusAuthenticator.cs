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
            string url = "";
            string email = "";
            string password = "";

            var directus = new Directus();

            while (!directus.Authenticated)
            {
                using (var inputDialog = new StringInputDialog())
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

            return directus;
        }
    }
}

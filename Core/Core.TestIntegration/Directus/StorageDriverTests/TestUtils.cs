using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Core.DirectusAPI;

namespace Calc.Core.TestIntegration.Drivers
{
    public class TestUtils
    {
        public static string GetDirectusUrl()
        {
            var url = Environment.GetEnvironmentVariable("CALC_DIRECTUS_URL");
            
            if (url == null)
            {
                throw new Exception("CALC_DIRECTUS_URL not found in environment variables");
            } else
            {
                return url;
            }
        }

        public static string GetDirectusEmail()
        {
            var email = Environment.GetEnvironmentVariable("CALC_DIRECTUS_EMAIL");

            if (email == null)
            {
                throw new Exception("CALC_DIRECTUS_EMAIL not found in environment variables");
            }
            else
            {
                return email;
            }
        }

        public static string GetDirectusPassword()
        {
            var password = Environment.GetEnvironmentVariable("CALC_DIRECTUS_PASSWORD");

            if (password == null)
            {
                throw new Exception("CALC_DIRECTUS_PASSWORD not found in environment variables");
            }
            else
            {
                return password;
            }
        }

        public static async Task<Directus> GetAuthenticatedDirectus()
        {
            var directus = new Directus();
            await directus.Authenticate(GetDirectusUrl(), GetDirectusEmail(), GetDirectusPassword());
            return directus;
        }
    }

}

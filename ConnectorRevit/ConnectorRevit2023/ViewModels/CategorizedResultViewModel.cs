using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.ViewModels
{
    public class CategorizedResultViewModel
    {
        public string GroupName { get; set; }
        public decimal Gwp { get; set; }
        public decimal Ge { get; set; }
    }
}

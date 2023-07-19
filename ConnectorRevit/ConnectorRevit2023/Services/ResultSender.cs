using Autodesk.Revit.UI;
using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.ViewModels;
using Calc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Services
{
    public class ResultSender
    {
/*        public ResultSender()
        {
            AsyncMediator.Register("SaveResultRequested", async nodeItem => await SaveResults((NodeViewModel)nodeItem));
        }*/
        public async Task<bool> SaveResults(NodeViewModel nodeToCalculate, string newName)
        {
            DirectusStore store = nodeToCalculate.Store;
            var results = CalculationHelper.Calculate(nodeToCalculate);
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string snapshotName = $"{date}_{newName}";
            store.SnapshotName = snapshotName;
            store.Results = results;
            return await store.SaveResults();
        }
    }
}

using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.ViewModels;
using Calc.Core;
using Calc.Core.Objects.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Services
{
    public class ResultSender
    {
        public async Task<bool?> SaveResults(DirectusStore store, List<Result> results, string newName)
        {
            if (results == null) return null;
            if (results.Count == 0) return null;
            if (store == null) return null;

            store.SnapshotName = newName;
            store.Results = results;
            return await store.SaveSnapshot();
        }
    }
}

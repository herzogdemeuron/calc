using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    internal interface IDriverGetMany<T>
    {
        public string QueryGetMany { get; }
        public List<T> GotManyItems { get; set; }
    }

    internal interface IDriverGetSingle<T>
    {
        string QueryGetSingle { get; }
        public T GotItem { get; set; }
    }

    internal interface IDriverGetManySystem<T>
    {
        public string QueryGetManySystem { get; }
        public List<T> GotManyItems { get; set; }
    }


    internal interface IDriverCreateSingle<T>
    {
        public T SendItem { get; set; }
        public string QueryCreateSingle { get; }
        public T CreatedItem { get; set; }
        public string StorageType { get; set; }
        public Dictionary<string, object> GetVariables();
    }

    internal interface IDriverCreateMany<T>
    {
        public List<T> SendItems { get; set; }
        public string QueryCreateMany { get; }
        public List<T> CreatedManyItems { get; set; }
        public string StorageType { get; set; }
        public Dictionary<string, object> GetVariables();
    }

    internal interface IDriverUpdateSingle<T>
    {
        public T SendItem { get; set; }
        public string QueryUpdateSingle { get; }
        public T UpdatedItem { get; set; }
        public string StorageType { get; set; }
        public Dictionary<string, object> GetVariables();
    }
}

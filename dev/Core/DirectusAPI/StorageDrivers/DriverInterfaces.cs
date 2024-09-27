using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.Drivers
{
    public interface IDriverGetMany<T>
    {
        public string QueryGetMany { get; }
        public List<T> GotManyItems { get; set; }
        public Dictionary<string, object> GetVariables();
    }

    public interface IDriverGetSingle<T>
    {
        public string QueryGetSingle { get; }
        public T GotItem { get; set; }
        public Dictionary<string, object> GetVariables();
    }

    public interface IDriverGetManySystem<T>
    {
        public string QueryGetManySystem { get; }
        public List<T> GotManyItems { get; set; }
        public Dictionary<string, object> GetVariables();
    }


    public interface IDriverCreateSingle<T>
    {
        public T SendItem { get; set; }
        public string QueryCreateSingle { get; }
        public T CreatedItem { get; set; }
        public Dictionary<string, object> GetVariables();
    }

    public interface IDriverCreateMany<T>
    {
        public List<T> SendItems { get; set; }
        public string QueryCreateMany { get; }
        public List<T> CreatedManyItems { get; set; }
        public Dictionary<string, object> GetVariables();
    }

    public interface IDriverUpdateSingle<T>
    {
        public T SendItem { get; set; }
        public string QueryUpdateSingle { get; }
        public T UpdatedItem { get; set; }
        public Dictionary<string, object> GetVariables();
    }
}

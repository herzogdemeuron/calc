using Newtonsoft.Json;
using System.ComponentModel;

namespace Calc.Core.Objects.Assemblies
{
    /// <summary>
    /// Assembly images created by calc builder.
    /// </summary>
    public class AssemblyImage : INotifyPropertyChanged
    {
        [JsonProperty("id")]
        public string Id { get; set; }        
        [JsonIgnore]
        public bool ImageLoaded { get; set; } = false;
        [JsonIgnore]
        public byte[] ImageData { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

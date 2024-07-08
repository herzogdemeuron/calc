using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Calc.Core.Objects.Buildups
{
    public class BuildupImage : INotifyPropertyChanged
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        // default has image is true, when load failed, set to false
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

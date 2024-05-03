using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Calc.MVVM.Models
{
    public class LayerMaterialModel : INotifyPropertyChanged
    {
        public event EventHandler MaterialPropertyChanged; // event to invoke ui change of the buildup creatiion vm

        private readonly LayerComponent layer;
        public string TargetMaterialName { get => layer.TargetMaterialName; }
        public List<MaterialFunction> MaterialFunctionsAll { get; }
        public bool IsEnabled { get => layer.IsValid; } // for ui to disable selection controls
        public string MaterialMatchInfo { get => GetMaterialMatchText(); } // warning message for material unit match bewteen main and sub material
        public List<Material> CurrentMaterials
        {
            get
            {
                var materials = new List<Material>();
                if (layer.HasMainMaterial) materials.Add(MainMaterial);
                if (layer.HasSubMaterial) materials.Add(SubMaterial);
                return materials;
            }
        }

        public MaterialFunction SelectedFunction
        {
            get => layer.Function;
            set
            {
                layer.Function = (MaterialFunction)value;
                OnPropertyChanged(nameof(SelectedFunction));
                NotifyPropertiesChange();
            }
        }

        public Material MainMaterial
        {
            get => layer.MainMaterial;
            set
            {
                layer.SetMainMaterial(value);
                if(value == null) SubMaterial = null; // remove sub material if main material is removed
                ActiveMaterial = value;
                OnPropertyChanged(nameof(MainMaterial));
                NotifyPropertiesChange();
            }
        }

        public Material SubMaterial
        {
            get => layer.SubMaterial;
            set
            {
                layer.SetSubMaterial(value);
                ActiveMaterial = value;
                OnPropertyChanged(nameof(SubMaterial));
                NotifyPropertiesChange();
            }
        }

        public double SubMaterialRatio
        {
            get => layer.SubMaterialRatio;
            set
            {
                layer.SetSubMaterialRatio(value);
                OnPropertyChanged(nameof(SubMaterialRatio));
                NotifyPropertiesChange();
            }
        }

        public bool CanAddSecondMaterial
        {
            get
            {
                if (layer.HasMainMaterial)
                {
                    return true;
                }
                return false;
            }
        }

        private Material activeMaterial;
        public Material ActiveMaterial // for the tab to show the current material
        {
            get => activeMaterial;
            set
            {
                activeMaterial = value;
                OnPropertyChanged(nameof(ActiveMaterial));
            }
        }

        public LayerMaterialModel(LayerComponent layercompo, List<Material> allMaterials, List<MaterialFunction> materialFunctionsAll)
        {
            layer = layercompo;
            MaterialFunctionsAll = materialFunctionsAll;
        }

        public LayerMaterialModel()
        {
            layer = new LayerComponent() { IsValid = false };
        }

        // if property changed due to material learning, the event should not be triggered to prevent deadlock
        public void NotifyPropertiesChange(bool sendEvent = true)
        {
            OnPropertyChanged(nameof(CanAddSecondMaterial));
            OnPropertyChanged(nameof(CurrentMaterials));
            OnPropertyChanged(nameof(MaterialMatchInfo));
            if (sendEvent)
            {
                MaterialPropertyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ResetActiveMaterial() // not implemented yet
        {
            if(layer.HasMainMaterial) ActiveMaterial = MainMaterial;
        }

        public void RemoveMaterial()
        {
            if(layer.HasSubMaterial)
            {
                SubMaterial = null;
                SubMaterialRatio = 0;
                return;
            }
            if(layer.HasMainMaterial)
            {
                MainMaterial = null;
                SubMaterialRatio = 0;
                SelectedFunction = null;
                return;
            }
        }


        private string GetMaterialMatchText()
        {
            if (!layer.CheckUnitsMatch())
            {
                return "Material units don't match.";
            }
            return null;
        }

        /// <summary>
        /// check if the target material is the same, and they are not the identical object
        /// </summary>
        public bool CheckIdenticalTargetMaterial(LayerMaterialModel otherModel)
        {
            if (otherModel == null) return false;
            if (this == otherModel) return false;
            if (this.TargetMaterialName == null || otherModel.TargetMaterialName == null) return false;
            return this.TargetMaterialName == otherModel.TargetMaterialName;
        }

        /// <summary>
        /// copy material setting from another model
        /// without triggering the event
        /// </summary>
        public void LearnMaterialSetting(LayerMaterialModel otherModel)
        {
            this.layer.Function = otherModel.SelectedFunction;
            OnPropertyChanged(nameof(SelectedFunction));

            this.layer.SetMainMaterial(otherModel.MainMaterial);
            OnPropertyChanged(nameof(MainMaterial));

            this.layer.SetSubMaterial(otherModel.SubMaterial);
            OnPropertyChanged(nameof(SubMaterial));

            this.layer.SetSubMaterialRatio(otherModel.SubMaterialRatio);
            OnPropertyChanged(nameof(SubMaterialRatio));
            // prevent deadlock
            NotifyPropertiesChange(false);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}

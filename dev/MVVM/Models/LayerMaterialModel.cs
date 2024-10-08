using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Calc.MVVM.Models
{
    /// <summary>
    /// Used in calc builder.
    /// Each layer component has a layer material model, which defines the material settings for the layer
    /// </summary>
    public class LayerMaterialModel : INotifyPropertyChanged
    {
        private readonly LayerComponent layer;
        public event EventHandler MaterialPropertyChanged; // event to invoke ui change of the assembly creatiion vm
        public string TargetMaterialName { get => layer.TargetMaterialName; }
        public List<MaterialFunction> MaterialFunctionsAll { get; }
        public bool IsEnabled { get => layer.IsValid; } // for ui to disable selection controls
        public string MaterialMatchInfo { get => GetMaterialMatchText(); }
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
        public Material ActiveMaterial // to be shown on the tab
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

        /// <summary>
        /// Notify all the properties changed.
        /// sendEvent: if property changed from 'material learning', 
        /// the MaterialPropertyChanged event should not be invoked to prevent deadlock.
        /// </summary>
        internal void NotifyPropertiesChange(bool sendEvent = true)
        {
            OnPropertyChanged(nameof(CanAddSecondMaterial));
            OnPropertyChanged(nameof(CurrentMaterials));
            OnPropertyChanged(nameof(MaterialMatchInfo));
            if (sendEvent)
            {
                MaterialPropertyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        internal void ResetActiveMaterial()
        {
            if(layer.HasMainMaterial) ActiveMaterial = MainMaterial;
        }

        /// <summary>
        /// Remove one material, sub or main.
        /// </summary>
        internal void RemoveMaterial()
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
        /// Checks if two LayerMaterialModels have the same target material.
        /// Compared with material names.
        /// </summary>
        internal bool CheckIdenticalTargetMaterial(LayerMaterialModel otherModel)
        {
            if (otherModel == null) return false;
            if (this == otherModel) return false;
            if (this.TargetMaterialName == null || otherModel.TargetMaterialName == null) return false;
            return this.TargetMaterialName == otherModel.TargetMaterialName;
        }

        /// <summary>
        /// Copies material settings from another model,
        /// including main material, sub material and sub material ratio.
        /// </summary>
        internal void LearnMaterialSetting(LayerMaterialModel otherModel)
        {
            this.layer.SetMainMaterial(otherModel.MainMaterial);
            OnPropertyChanged(nameof(MainMaterial));
            this.layer.SetSubMaterial(otherModel.SubMaterial);
            OnPropertyChanged(nameof(SubMaterial));
            this.layer.SetSubMaterialRatio(otherModel.SubMaterialRatio);
            OnPropertyChanged(nameof(SubMaterialRatio));            
            NotifyPropertiesChange(false); // prevent deadlock
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}

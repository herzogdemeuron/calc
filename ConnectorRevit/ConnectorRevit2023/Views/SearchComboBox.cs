using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Calc.Core.Objects.Buildups;
using System.Diagnostics;

namespace Calc.ConnectorRevit.Views
{
    public class SearchComboBox : ComboBox
    {
        TextBox _editableTextBox;
        object _lastSelectedItem;
        bool _firstFocus = true;
        bool _ignoreTextChange = false;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _editableTextBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            this.IsTextSearchEnabled = false;
            this.StaysOpenOnEdit = true;

            if (_editableTextBox != null)
            {
                _editableTextBox.TextChanged += EditableTextBox_TextChanged;
                _editableTextBox.GotFocus += EditableTextBox_GotFocus;
            }
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            this.IsEditable = true;
            if (_editableTextBox == null)
            {
                this.ApplyTemplate();
            }
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);

            if (this.Items.Count > 0 && _editableTextBox.Text.Trim() != string.Empty && _firstFocus == false)
            {
                this.SelectedIndex = 0;
            }
            else
            {
                this.SelectedItem = _lastSelectedItem;
            }
            this.IsEditable = false;
            _firstFocus = true;
        }

        private void EditableTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //need to clear selection to prevent auto selection when typing
            _lastSelectedItem = this.SelectedItem;
            string text = _editableTextBox.Text;

            _ignoreTextChange = true;
            this.SelectedIndex = -1;
            _editableTextBox.Text = text;
            _ignoreTextChange = false;

            if (ItemsSource is ICollectionView ICV)
            {
                ICV.Filter = null;
            }
            this.IsDropDownOpen = true;
        }

        private void EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if (!_ignoreTextChange)
            {
                _firstFocus = false;
            }

            if (ItemsSource is ICollectionView ICV)
            {
                if (string.IsNullOrEmpty(_editableTextBox.Text.Trim()))
                {
                        
                    ICV.Filter = null;
                }
                else
                {
                    ICV.Filter = new Predicate<object>(item =>
                    {
                        var buildup = (Buildup)item;
                        var searchText = _editableTextBox.Text.ToLower();
                        var nameContains = buildup.Name.ToLower().Contains(searchText);
                        var groupContains = buildup.Group != null && buildup.Group.Name.ToLower().Contains(searchText);
                        return nameContains || groupContains;
                    });

                    /*if (_lastSelectedItem != null && ICV.Contains(_lastSelectedItem))
                    {
                        SelectedItem = _lastSelectedItem;
                    }*/
                }
            }
            
        }
    }
}

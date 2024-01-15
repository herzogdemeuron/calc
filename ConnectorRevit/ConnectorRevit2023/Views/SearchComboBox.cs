using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Calc.Core.Objects.Buildups;

namespace Calc.ConnectorRevit.Views
{
    public class SearchComboBox : ComboBox
    {
        TextBox _editableTextBox;
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

            if (this.Items.Count > 0 && _editableTextBox.Text.Trim() != string.Empty)
            {
                this.SelectedIndex = 0;
            }
            this.IsEditable = false;
        }
        private void EditableTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //need to clear selection to prevent auto selection when typing
            string text = _editableTextBox.Text;
            this.SelectedIndex = -1;
            _editableTextBox.Text = text;
            this.IsDropDownOpen = true;
        }

        private void EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ItemsSource is ICollectionView ICV)
            {
                if (string.IsNullOrEmpty(_editableTextBox.Text.Trim()))
                {
                    ICV.Filter = null;
                }
                else
                {
                    ICV.Filter = new Predicate<object>(i => 
                    ((Buildup)i).Name.ToLower().Contains(_editableTextBox.Text.ToLower()));
                }
            }
        }
    }
}

using Calc.Core.Objects.Buildups;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calc.ConnectorRevit.Views
{
    public class SearchComboBox : ComboBox
    {
        TextBox _editableTextBox;
        object _lastSelectedItem;
        bool _firstFocus = true;
        bool _ignoreTextChange = false;

        public SearchComboBox()
        {
            this.PreviewKeyDown += SearchComboBox_PreviewKeyDown;
        }

        private void SearchComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Check if a character key was pressed and the dropdown is open
            var c = e.Key.ToString().ToLower();
            string input = c.Length == 1 ? c : string.Empty;
            if (this.IsDropDownOpen && _firstFocus)
            {
                if (_editableTextBox != null)
                {
                    this.IsEditable = true;
                    _editableTextBox.Focusable = true;
                    _editableTextBox.Focus();
                    _editableTextBox.Clear();

                    e.Handled = true;

                    // Simulate the text input
                    _editableTextBox.RaiseEvent(new TextCompositionEventArgs(InputManager.Current.PrimaryKeyboardDevice, new TextComposition(InputManager.Current, _editableTextBox, input))
                    {
                        RoutedEvent = TextCompositionManager.TextInputEvent
                    });

                    _firstFocus = false;
                }
            }
        }

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
            _firstFocus = true;
            _lastSelectedItem = this.SelectedItem;
            _editableTextBox.Focusable = false;

            if (_editableTextBox == null)
            {
                this.ApplyTemplate();
            }
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            /*if (this.Items.Count > 0 && _editableTextBox.Text.Trim() != string.Empty && _firstFocus == false && Items.Count > 0)
            {
                this.SelectedIndex = 0;
            }*/
            if (this.SelectedItem == null && _lastSelectedItem != null)
            {
                this.SelectedItem = _lastSelectedItem;
            }

            this.IsEditable = false;
            _firstFocus = true;
            _editableTextBox.Text= string.Empty;
            ResetFilter();
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

            ResetFilter();
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
                }
            }
        }

        private void ResetFilter()
        {
            if (ItemsSource is ICollectionView ICV)
            {
                ICV.Filter = null;
            }
        }
    }
}

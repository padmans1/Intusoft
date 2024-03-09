using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace INTUSOFT.Desktop.Forms
{
    /// <summary>
    /// Interaction logic for MedicalHistory.xaml
    /// </summary>
    public partial class MedicalHistory_UC : UserControl
    {
        public MedicalHistory_UC(MedicalHistoryViewModel viewModel)
        {
            this.DataContext = viewModel;
            InitializeComponent();
        }
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void TextBox_GotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }


    public class TextBoxBehavior
    {
        public static bool GetSelectAllTextOnFocus(TextBox textBox)
        {
            return (bool)textBox.GetValue(SelectAllTextOnFocusProperty);
        }

        public static void SetSelectAllTextOnFocus(TextBox textBox, bool value)
        {
            textBox.SetValue(SelectAllTextOnFocusProperty, value);
        }

        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllTextOnFocus",
                typeof(bool),
                typeof(TextBoxBehavior),
                new UIPropertyMetadata(false, OnSelectAllTextOnFocusChanged));

        private static void OnSelectAllTextOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            if (textBox == null) return;

            if (e.NewValue is bool == false) return;

            if ((bool)e.NewValue)
            {
                textBox.GotFocus += SelectAll;
                textBox.PreviewMouseDown += IgnoreMouseButton;
            }
            else
            {
                textBox.GotFocus -= SelectAll;
                textBox.PreviewMouseDown -= IgnoreMouseButton;
            }
        }

        private static void SelectAll(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox == null) return;
            textBox.SelectAll();
        }

        private static void IgnoreMouseButton(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || (!textBox.IsReadOnly && textBox.IsKeyboardFocusWithin)) return;

            e.Handled = true;
            textBox.Focus();
        }
    }

    //public class SelectTextOnFocus : DependencyObject
    //{
    //    public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
    //        "Active",
    //        typeof(bool),
    //        typeof(SelectTextOnFocus),
    //        new PropertyMetadata(false, ActivePropertyChanged));

    //    private static void ActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is TextBox)
    //        {
    //            TextBox textBox = d as TextBox;
    //            if ((e.NewValue as bool?).GetValueOrDefault(false))
    //            {
    //                textBox.GotKeyboardFocus += OnKeyboardFocusSelectText;
    //                textBox.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
    //            }
    //            else
    //            {
    //                textBox.GotKeyboardFocus -= OnKeyboardFocusSelectText;
    //                textBox.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
    //            }
    //        }
    //    }

    //    private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //    {
    //        DependencyObject dependencyObject = GetParentFromVisualTree(e.OriginalSource);

    //        if (dependencyObject == null)
    //        {
    //            return;
    //        }

    //        var textBox = (TextBox)dependencyObject;
    //        if (!textBox.IsKeyboardFocusWithin)
    //        {
    //            textBox.Focus();
    //            e.Handled = true;
    //        }
    //    }

    //    private static DependencyObject GetParentFromVisualTree(object source)
    //    {
    //        DependencyObject parent = source as UIElement;
    //        while (parent != null && !(parent is TextBox))
    //        {
    //            parent = VisualTreeHelper.GetParent(parent);
    //        }

    //        return parent;
    //    }

    //    private static void OnKeyboardFocusSelectText(object sender, KeyboardFocusChangedEventArgs e)
    //    {
    //        TextBox textBox = e.OriginalSource as TextBox;
    //        if (textBox != null)
    //        {
    //            textBox.SelectAll();
    //        }
    //    }

    //    [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
    //    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    //    public static bool GetActive(DependencyObject @object)
    //    {
    //        return (bool)@object.GetValue(ActiveProperty);
    //    }

    //    public static void SetActive(DependencyObject @object, bool value)
    //    {
    //        @object.SetValue(ActiveProperty, value);
    //    }
    //}
}

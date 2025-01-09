namespace item_eyez
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class ComboBoxBehavior
    {
        public static readonly DependencyProperty DropDownOpenedCommandProperty =
            DependencyProperty.RegisterAttached(
                "DropDownOpenedCommand",
                typeof(ICommand),
                typeof(ComboBoxBehavior),
                new PropertyMetadata(null, OnDropDownOpenedCommandChanged));

        public static ICommand GetDropDownOpenedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DropDownOpenedCommandProperty);
        }

        public static void SetDropDownOpenedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DropDownOpenedCommandProperty, value);
        }

        private static void OnDropDownOpenedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBox comboBox)
            {
                if (e.OldValue != null)
                {
                    comboBox.DropDownOpened -= ComboBox_DropDownOpened;
                }

                if (e.NewValue != null)
                {
                    comboBox.DropDownOpened += ComboBox_DropDownOpened;
                }
            }
        }

        private static void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            var command = GetDropDownOpenedCommand(comboBox);

            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }

}

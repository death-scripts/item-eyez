using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace item_eyez
{
    public static class EnterKeyBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(EnterKeyBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.KeyDown -= TextBox_KeyDown;

                if (e.NewValue is ICommand)
                {
                    textBox.KeyDown += TextBox_KeyDown;
                }
            }
        }

        private static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox)
            {
                var command = GetCommand(textBox);
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }
        }
    }

}

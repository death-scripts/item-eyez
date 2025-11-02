// ----------------------------------------------------------------------------
// <copyright company="death-scripts">
// Copyright (c) death-scripts. All rights reserved.
// </copyright>
//                   ██████╗ ███████╗ █████╗ ████████╗██╗  ██╗
//                   ██╔══██╗██╔════╝██╔══██╗╚══██╔══╝██║  ██║
//                   ██║  ██║█████╗  ███████║   ██║   ███████║
//                   ██║  ██║██╔══╝  ██╔══██║   ██║   ██╔══██║
//                   ██████╔╝███████╗██║  ██║   ██║   ██║  ██║
//                   ╚═════╝ ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═╝
//
//              ███████╗ ██████╗██████╗ ██╗██████╗ ████████╗███████╗
//              ██╔════╝██╔════╝██╔══██╗██║██╔══██╗╚══██╔══╝██╔════╝
//              ███████╗██║     ██████╔╝██║██████╔╝   ██║   ███████╗
//              ╚════██║██║     ██╔══██╗██║██╔═══╝    ██║   ╚════██║
//              ███████║╚██████╗██║  ██║██║██║        ██║   ███████║
//              ╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝╚═╝        ╚═╝   ╚══════╝
// ----------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Item_eyez.Controls
{
    /// <summary>
    /// The enter key behavior.
    /// </summary>
    public static class EnterKeyBehavior
    {
        /// <summary>
        /// The command property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(EnterKeyBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The i command.
        /// </returns>
        public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);

        /// <summary>
        /// Sets the command.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(CommandProperty, value);

        /// <summary>
        /// Called when [command changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the KeyDown event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox)
            {
                ICommand command = GetCommand(textBox);
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }
        }
    }
}
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
    /// The ComboBox behavior.
    /// </summary>
    public static class ComboBoxBehavior
    {
        /// <summary>
        /// The drop down opened command property.
        /// </summary>
        public static readonly DependencyProperty DropDownOpenedCommandProperty =
            DependencyProperty.RegisterAttached(
                "DropDownOpenedCommand",
                typeof(ICommand),
                typeof(ComboBoxBehavior),
                new PropertyMetadata(null, OnDropDownOpenedCommandChanged));

        /// <summary>
        /// Gets the drop down opened command.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The i command.
        /// </returns>
        public static ICommand GetDropDownOpenedCommand(DependencyObject obj) => (ICommand)obj.GetValue(DropDownOpenedCommandProperty);

        /// <summary>
        /// Sets the drop down opened command.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetDropDownOpenedCommand(DependencyObject obj, ICommand value) => obj.SetValue(DropDownOpenedCommandProperty, value);

        /// <summary>
        /// Called when [drop down opened command changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
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

        /// <summary>
        /// Handles the DropDownOpened event of the ComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (sender is not ComboBox comboBox)
            {
                return;
            }

            ICommand command = GetDropDownOpenedCommand(comboBox);

            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}
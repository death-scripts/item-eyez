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

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Item_eyez.Controls;
using Moq;

namespace Item_eyez.Tests
{
    [TestClass]
    public class BehaviorTests
    {
        [TestMethod]
        public void ComboBoxBehavior_ExecutesCommandOnDropDownOpened()
        {
            ManualResetEvent mre = new(false);
            Thread thread = new(() =>
            {
                try
                {
                    // Arrange
                    ComboBox comboBox = new();
                    Mock<ICommand> commandMock = new();
                    _ = commandMock.Setup(cmd => cmd.CanExecute(It.IsAny<object>())).Returns(true);
                    ComboBoxBehavior.SetDropDownOpenedCommand(comboBox, commandMock.Object);

                    // Act
                    MethodInfo? methodInfo = typeof(ComboBoxBehavior).GetMethod("ComboBox_DropDownOpened", BindingFlags.NonPublic | BindingFlags.Static);
                    _ = methodInfo.Invoke(null, new object[] { comboBox, EventArgs.Empty });

                    // Assert
                    commandMock.Verify(cmd => cmd.Execute(null), Times.Once);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Test failed: {ex.Message}");
                }
                finally
                {
                    _ = mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            _ = mre.WaitOne();
        }

        [TestMethod]
        public void EnterKeyBehavior_ExecutesCommandOnEnter()
        {
            ManualResetEvent mre = new(false);
            Thread thread = new(() =>
            {
                try
                {
                    // Arrange
                    TextBox textBox = new();
                    Mock<ICommand> commandMock = new();
                    _ = commandMock.Setup(cmd => cmd.CanExecute(It.IsAny<object>())).Returns(true);
                    EnterKeyBehavior.SetCommand(textBox, commandMock.Object);

                    // Act
                    KeyEventArgs eventArgs = new(Keyboard.PrimaryDevice, new Mock<PresentationSource>().Object, 0, Key.Enter);
                    MethodInfo? methodInfo = typeof(EnterKeyBehavior).GetMethod("TextBox_KeyDown", BindingFlags.NonPublic | BindingFlags.Static);
                    _ = (methodInfo?.Invoke(null, new object[] { textBox, eventArgs }));

                    // Assert
                    commandMock.Verify(cmd => cmd.Execute(null), Times.Once);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Test failed: {ex.Message}");
                }
                finally
                {
                    _ = mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            _ = mre.WaitOne();
        }

        [TestMethod]
        public void FocusBehavior_SetsFocus()
        {
            ManualResetEvent mre = new(false);
            Thread thread = new(() =>
            {
                try
                {
                    // Arrange
                    TextBox textBox = new();
                    Window window = new() { Content = textBox };
                    window.Show();

                    // Act
                    FocusBehavior.SetIsFocused(textBox, true);

                    // Assert
                    Assert.IsTrue(textBox.IsFocused);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Test failed: {ex.Message}");
                }
                finally
                {
                    _ = mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            _ = mre.WaitOne();
        }
    }
}
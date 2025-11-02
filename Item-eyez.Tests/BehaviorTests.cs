
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using Item_eyez.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Windows;

namespace Item_eyez.Tests
{
    [TestClass]
    public class BehaviorTests
    {
        [TestMethod]
        public void EnterKeyBehavior_ExecutesCommandOnEnter()
        {
            var mre = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                try
                {
                    // Arrange
                    var textBox = new TextBox();
                    var commandMock = new Mock<ICommand>();
                    commandMock.Setup(cmd => cmd.CanExecute(It.IsAny<object>())).Returns(true);
                    EnterKeyBehavior.SetCommand(textBox, commandMock.Object);

                    // Act
                    var eventArgs = new KeyEventArgs(Keyboard.PrimaryDevice, new Mock<PresentationSource>().Object, 0, Key.Enter);
                    var methodInfo = typeof(EnterKeyBehavior).GetMethod("TextBox_KeyDown", BindingFlags.NonPublic | BindingFlags.Static);
                    methodInfo.Invoke(null, new object[] { textBox, eventArgs });

                    // Assert
                    commandMock.Verify(cmd => cmd.Execute(null), Times.Once);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Test failed: {ex.Message}");
                }
                finally
                {
                    mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            mre.WaitOne();
        }
    [TestMethod]
        public void ComboBoxBehavior_ExecutesCommandOnDropDownOpened()
        {
            var mre = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                try
                {
                    // Arrange
                    var comboBox = new ComboBox();
                    var commandMock = new Mock<ICommand>();
                    commandMock.Setup(cmd => cmd.CanExecute(It.IsAny<object>())).Returns(true);
                    ComboBoxBehavior.SetDropDownOpenedCommand(comboBox, commandMock.Object);

                    // Act
                    var methodInfo = typeof(ComboBoxBehavior).GetMethod("ComboBox_DropDownOpened", BindingFlags.NonPublic | BindingFlags.Static);
                    methodInfo.Invoke(null, new object[] { comboBox, EventArgs.Empty });

                    // Assert
                    commandMock.Verify(cmd => cmd.Execute(null), Times.Once);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Test failed: {ex.Message}");
                }
                finally
                {
                    mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            mre.WaitOne();
        }
    [TestMethod]
        public void FocusBehavior_SetsFocus()
        {
            var mre = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                try
                {
                    // Arrange
                    var textBox = new TextBox();
                    var window = new Window { Content = textBox };
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
                    mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            mre.WaitOne();
        }
    }
}

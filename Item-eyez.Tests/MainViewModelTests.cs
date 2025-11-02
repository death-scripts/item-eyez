
using Item_eyez.Viewmodels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Item_eyez.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void ContainsKeyword_ShouldReturnTrue_WhenKeywordExists()
        {
            // Arrange
            var text = "This is a test string";
            var keywords = new string[] { "test" };

            // Act
            var result = MainViewModel.ContainsKeyword(text, keywords);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsKeyword_ShouldReturnFalse_WhenKeywordDoesNotExist()
        {
            // Arrange
            var text = "This is a test string";
            var keywords = new string[] { "apple" };

            // Act
            var result = MainViewModel.ContainsKeyword(text, keywords);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExtractKeyword_ShouldReturnKeyword_WhenKeywordExists()
        {
            // Arrange
            var text = "This is a test string";
            var keywords = new string[] { "test" };

            // Act
            var result = MainViewModel.ExtractKeyword(text, keywords);

            // Assert
            Assert.AreEqual("test", result);
        }

        [TestMethod]
        public void ExtractKeyword_ShouldReturnNull_WhenKeywordDoesNotExist()
        {
            // Arrange
            var text = "This is a test string";
            var keywords = new string[] { "apple" };

            // Act
            var result = MainViewModel.ExtractKeyword(text, keywords);

            // Assert
            Assert.IsNull(result);
        }
    }
}

using System.Collections.ObjectModel;
using Item_eyez.Database;
using Item_eyez.Viewmodels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Item_eyez.Tests
{
    [TestClass]
    public class ItemsViewModelTests
    {
        private Mock<IItemEyezDatabase> dbMock;
        private ItemsViewModel viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            dbMock = new Mock<IItemEyezDatabase>();
            dbMock.Setup(db => db.GetItemsWithRelationships()).Returns(new ObservableCollection<Item>());
            viewModel = new ItemsViewModel(dbMock.Object);
        }

        [TestMethod]
        public void Constructor_LoadsItems()
        {
            // Assert
            dbMock.Verify(db => db.GetItemsWithRelationships(), Times.Once);
            Assert.IsNotNull(viewModel.Items);
        }

        [TestMethod]
        public void Add_CallsAddItemOnDatabase()
        {
            // Arrange
            viewModel.Name = "Test Item";
            viewModel.Description = "Test Description";
            viewModel.Value = "10.00";
            viewModel.Catagories = "Test Category";

            // Act
            viewModel.Add();

            // Assert
            dbMock.Verify(db => db.AddItem("Test Item", "Test Description", 10.00m, "Test Category"), Times.Once);
        }

        [TestMethod]
        public void Add_WithSelectedContainer_AssociatesWithContainer()
        {
            // Arrange
            viewModel.Name = "Test Item";
            var parentContainer = new Container(Guid.NewGuid(), "Parent", "");
            viewModel.SelectedContainer = parentContainer;
            var newItemId = Guid.NewGuid();
            dbMock.Setup(db => db.AddItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>())).Returns(newItemId);

            // Act
            viewModel.Add();

            // Assert
            dbMock.Verify(db => db.AssociateItemWithContainer(newItemId, parentContainer.Id), Times.Once);
        }

        [TestMethod]
        public void DeleteSelectedItem_RemovesItemFromCollection()
        {
            // Arrange
            var item = new Item(Guid.NewGuid(), "Test", "", 0, "");
            viewModel.Items.Add(item);
            viewModel.SelectedItem = item;

            // Act
            viewModel.DeleteItemCommand.Execute(null);

            // Assert
            Assert.IsFalse(viewModel.Items.Contains(item));
        }
    }
}
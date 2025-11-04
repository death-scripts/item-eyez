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
            this.dbMock = new Mock<IItemEyezDatabase>();
            this.dbMock.Setup(db => db.GetItemsWithRelationships()).Returns(new ObservableCollection<Item>());
            this.viewModel = new ItemsViewModel(this.dbMock.Object);
        }

        [TestMethod]
        public void Constructor_LoadsItems()
        {
            // Assert
            this.dbMock.Verify(db => db.GetItemsWithRelationships(), Times.Once);
            Assert.IsNotNull(this.viewModel.Items);
        }

        [TestMethod]
        public void Add_CallsAddItemOnDatabase()
        {
            // Arrange
            this.viewModel.Name = "Test Item";
            this.viewModel.Description = "Test Description";
            this.viewModel.Value = "10.00";
            this.viewModel.Catagories = "Test Category";

            // Act
            this.viewModel.Add();

            // Assert
            this.dbMock.Verify(db => db.AddItem("Test Item", "Test Description", 10.00m, "Test Category"), Times.Once);
        }

        [TestMethod]
        public void Add_WithSelectedContainer_AssociatesWithContainer()
        {
            // Arrange
            this.viewModel.Name = "Test Item";
            var parentContainer = new Container(Guid.NewGuid(), "Parent", "");
            this.viewModel.SelectedContainer = parentContainer;
            var newItemId = Guid.NewGuid();
            this.dbMock.Setup(db => db.AddItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>())).Returns(newItemId);

            // Act
            this.viewModel.Add();

            // Assert
            this.dbMock.Verify(db => db.AssociateItemWithContainer(newItemId, parentContainer.Id), Times.Once);
        }

        [TestMethod]
        public void DeleteSelectedItem_RemovesItemFromCollection()
        {
            // Arrange
            var item = new Item(Guid.NewGuid(), "Test", "", 0, "");
            this.viewModel.Items.Add(item);
            this.viewModel.SelectedItem = item;

            // Act
            this.viewModel.DeleteItemCommand.Execute(null);

            // Assert
            Assert.IsFalse(this.viewModel.Items.Contains(item));
        }
    }
}
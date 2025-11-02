using System.Collections.ObjectModel;
using Item_eyez.Database;
using Item_eyez.Viewmodels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace Item_eyez.Tests
{
    [TestClass]
    public class OrganizeViewModelTests
    {
        private Mock<IItemEyezDatabase> dbMock;
        private OrganizeViewModel viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            dbMock = new Mock<IItemEyezDatabase>();
            dbMock.Setup(db => db.GetRoomsList()).Returns(new ObservableCollection<Room>());
            dbMock.Setup(db => db.GetContainersWithRelationships()).Returns(new ObservableCollection<Container>());
            dbMock.Setup(db => db.GetItemsWithRelationships()).Returns(new ObservableCollection<Item>());
            viewModel = new OrganizeViewModel(dbMock.Object);
        }

        [TestMethod]
        public void Constructor_LoadsData()
        {
            // Assert
            dbMock.Verify(db => db.GetRoomsList(), Times.Once);
            dbMock.Verify(db => db.GetContainersWithRelationships(), Times.Once);
            dbMock.Verify(db => db.GetItemsWithRelationships(), Times.Once);
            Assert.IsNotNull(viewModel.Roots);
        }

        [TestMethod]
        public void SearchText_FiltersNodes()
        {
            // Arrange
            var room = new Room(Guid.NewGuid(), "Test Room", "");
            var container = new Container(Guid.NewGuid(), "Test Container", "");
            var item = new Item(Guid.NewGuid(), "Test Item", "", 0, "");

            dbMock.Setup(db => db.GetRoomsList()).Returns(new ObservableCollection<Room> { room });
            dbMock.Setup(db => db.GetContainersWithRelationships()).Returns(new ObservableCollection<Container> { container });
            dbMock.Setup(db => db.GetItemsWithRelationships()).Returns(new ObservableCollection<Item> { item });

            viewModel.Load();

            // Act
            viewModel.SearchText = "Item";

            // Assert
            Assert.IsTrue(viewModel.Roots.Any(r => r.Name == "Test Item" && r.IsVisible));
            Assert.IsFalse(viewModel.Roots.Any(r => r.Name == "Test Room" && r.IsVisible));
            Assert.IsFalse(viewModel.Roots.Any(r => r.Name == "Test Container" && r.IsVisible));
        }
    }
}
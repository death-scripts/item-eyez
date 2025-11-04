using System.Collections.ObjectModel;
using System.Linq;
using Item_eyez.Database;
using Item_eyez.Viewmodels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            this.dbMock = new Mock<IItemEyezDatabase>();
            this.dbMock.Setup(db => db.GetRoomsList()).Returns(new ObservableCollection<Room>());
            this.dbMock.Setup(db => db.GetContainersWithRelationships()).Returns(new ObservableCollection<Container>());
            this.dbMock.Setup(db => db.GetItemsWithRelationships()).Returns(new ObservableCollection<Item>());
            this.viewModel = new OrganizeViewModel(this.dbMock.Object);
        }

        [TestMethod]
        public void Constructor_LoadsData()
        {
            // Assert
            this.dbMock.Verify(db => db.GetRoomsList(), Times.Once);
            this.dbMock.Verify(db => db.GetContainersWithRelationships(), Times.Once);
            this.dbMock.Verify(db => db.GetItemsWithRelationships(), Times.Once);
            Assert.IsNotNull(this.viewModel.Roots);
        }

        [TestMethod]
        public void SearchText_FiltersNodes()
        {
            // Arrange
            var room = new Room(Guid.NewGuid(), "Test Room", "");
            var container = new Container(Guid.NewGuid(), "Test Container", "");
            var item = new Item(Guid.NewGuid(), "Test Item", "", 0, "");

            this.dbMock.Setup(db => db.GetRoomsList()).Returns(new ObservableCollection<Room> { room });
            this.dbMock.Setup(db => db.GetContainersWithRelationships()).Returns(new ObservableCollection<Container> { container });
            this.dbMock.Setup(db => db.GetItemsWithRelationships()).Returns(new ObservableCollection<Item> { item });

            this.viewModel.Load();

            // Act
            this.viewModel.SearchText = "Item";

            // Assert
            Assert.IsTrue(this.viewModel.Roots.Any(r => r.Name == "Test Item" && r.IsVisible));
            Assert.IsFalse(this.viewModel.Roots.Any(r => r.Name == "Test Room" && r.IsVisible));
            Assert.IsFalse(this.viewModel.Roots.Any(r => r.Name == "Test Container" && r.IsVisible));
        }
    }
}
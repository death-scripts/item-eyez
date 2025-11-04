using System.Collections.ObjectModel;
using Item_eyez.Database;
using Item_eyez.Viewmodels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Item_eyez.Tests
{
    [TestClass]
    public class RoomViewModelTests
    {
        private Mock<IItemEyezDatabase> dbMock;
        private RoomViewModel viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            this.dbMock = new Mock<IItemEyezDatabase>();
            this.dbMock.Setup(db => db.GetRoomsList()).Returns(new ObservableCollection<Room>());
            this.viewModel = new RoomViewModel(this.dbMock.Object);
        }

        [TestMethod]
        public void Constructor_LoadsRooms()
        {
            // Assert
            this.dbMock.Verify(db => db.GetRoomsList(), Times.Once);
            Assert.IsNotNull(this.viewModel.Rooms);
        }

        [TestMethod]
        public void AddRoom_CallsAddRoomOnDatabase()
        {
            // Arrange
            this.viewModel.Name = "Test Room";
            this.viewModel.Description = "Test Description";

            // Act
            this.viewModel.AddRoom();

            // Assert
            this.dbMock.Verify(db => db.AddRoom("Test Room", "Test Description"), Times.Once);
        }

        [TestMethod]
        public void DeleteSelectedRoom_RemovesRoomFromCollection()
        {
            // Arrange
            var room = new Room(Guid.NewGuid(), "Test", "");
            this.viewModel.Rooms.Add(room);
            this.viewModel.SelectedRoomRow = room;

            // Act
            this.viewModel.DeleteRoomCommand.Execute(null);

            // Assert
            Assert.IsFalse(this.viewModel.Rooms.Contains(room));
        }
    }
}
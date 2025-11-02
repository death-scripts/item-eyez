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
            dbMock = new Mock<IItemEyezDatabase>();
            dbMock.Setup(db => db.GetRoomsList()).Returns(new ObservableCollection<Room>());
            viewModel = new RoomViewModel(dbMock.Object);
        }

        [TestMethod]
        public void Constructor_LoadsRooms()
        {
            // Assert
            dbMock.Verify(db => db.GetRoomsList(), Times.Once);
            Assert.IsNotNull(viewModel.Rooms);
        }

        [TestMethod]
        public void AddRoom_CallsAddRoomOnDatabase()
        {
            // Arrange
            viewModel.Name = "Test Room";
            viewModel.Description = "Test Description";

            // Act
            viewModel.AddRoom();

            // Assert
            dbMock.Verify(db => db.AddRoom("Test Room", "Test Description"), Times.Once);
        }

        [TestMethod]
        public void DeleteSelectedRoom_RemovesRoomFromCollection()
        {
            // Arrange
            var room = new Room(Guid.NewGuid(), "Test", "");
            viewModel.Rooms.Add(room);
            viewModel.SelectedRoomRow = room;

            // Act
            viewModel.DeleteRoomCommand.Execute(null);

            // Assert
            Assert.IsFalse(viewModel.Rooms.Contains(room));
        }
    }
}
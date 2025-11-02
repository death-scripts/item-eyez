

using Item_eyez.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Item_eyez.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        private const string TestDbConnectionString = @"Server=localhost\SQLEXPRESS;Database=ITEMEYEZ_TEST;Integrated Security=true;TrustServerCertificate=True;";
        private IItemEyezDatabase db;

        [TestInitialize]
        public void TestInitialize()
        {
            DatabaseHelper.CreateDatabase(TestDbConnectionString);
            db = ItemEyezDatabase.Instance(TestDbConnectionString);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DatabaseHelper.DeleteDatabase(TestDbConnectionString);
        }

        [TestMethod]
        public void AddRoom_AddsRoomToDatabase()
        {
            // Act
            db.AddRoom("Test Room", "Test Description");

            // Assert
            var rooms = db.GetRoomsList();
            Assert.AreEqual(1, rooms.Count);
            Assert.AreEqual("Test Room", rooms.First().Name);
        }

        [TestMethod]
        public void AddContainer_AddsContainerToDatabase()
        {
            // Act
            db.AddContainer("Test Container", "Test Description");

            // Assert
            var containers = db.GetContainersWithRelationships();
            Assert.AreEqual(1, containers.Count);
            Assert.AreEqual("Test Container", containers.First().Name);
        }

        [TestMethod]
        public void AddItem_AddsItemToDatabase()
        {
            // Act
            db.AddItem("Test Item", "Test Description", 10.00m, "Test Category");

            // Assert
            var items = db.GetItemsWithRelationships();
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual("Test Item", items.First().Name);
        }
    }
}


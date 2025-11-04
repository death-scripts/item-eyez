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

using Item_eyez.Database;

namespace Item_eyez.Tests
{
    // [TestClass]
    public class DatabaseTests
    {
        private const string TestDbConnectionString = @"Server=localhost\SQLEXPRESS;Database=ITEMEYEZ_TEST;Integrated Security=true;TrustServerCertificate=True;";
        private IItemEyezDatabase db;

        // [TestMethod]
        public void AddContainer_AddsContainerToDatabase()
        {
            // Act
            _ = this.db.AddContainer("Test Container", "Test Description");

            // Assert
            System.Collections.ObjectModel.ObservableCollection<Viewmodels.Container> containers = this.db.GetContainersWithRelationships();
            Assert.AreEqual(1, containers.Count);
            Assert.AreEqual("Test Container", containers.First().Name);
        }

        // [TestMethod]
        public void AddItem_AddsItemToDatabase()
        {
            // Act
            _ = this.db.AddItem("Test Item", "Test Description", 10.00m, "Test Category");

            // Assert
            System.Collections.ObjectModel.ObservableCollection<Viewmodels.Item> items = this.db.GetItemsWithRelationships();
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual("Test Item", items.First().Name);
        }

        // [TestMethod]
        public void AddRoom_AddsRoomToDatabase()
        {
            // Act
            this.db.AddRoom("Test Room", "Test Description");

            // Assert
            System.Collections.ObjectModel.ObservableCollection<Viewmodels.Room> rooms = this.db.GetRoomsList();
            Assert.AreEqual(1, rooms.Count);
            Assert.AreEqual("Test Room", rooms.First().Name);
        }

        // [TestCleanup]
        public void TestCleanup()
        {
            DatabaseHelper.DeleteDatabase(TestDbConnectionString);
        }

        // [TestInitialize]
        public void TestInitialize()
        {
            DatabaseHelper.CreateDatabase(TestDbConnectionString);
            this.db = ItemEyezDatabase.Instance(TestDbConnectionString);
        }
    }
}
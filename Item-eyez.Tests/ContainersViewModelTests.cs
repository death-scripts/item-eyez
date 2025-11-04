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
using System.Collections.ObjectModel;
using Item_eyez.Database;
using Item_eyez.Viewmodels;
using Moq;

namespace Item_eyez.Tests
{
    [TestClass]
    public class ContainersViewModelTests
    {
        private Mock<IItemEyezDatabase> dbMock;
        private ContainersViewModel viewModel;

        [TestMethod]
        public void Add_CallsAddContainerOnDatabase()
        {
            // Arrange
            this.viewModel.Name = "Test Container";
            this.viewModel.Description = "Test Description";

            // Act
            this.viewModel.Add();

            // Assert
            this.dbMock.Verify(db => db.AddContainer("Test Container", "Test Description"), Times.Once);
        }

        [TestMethod]
        public void Add_WithSelectedContainer_AssociatesWithContainer()
        {
            // Arrange
            this.viewModel.Name = "Test Container";
            this.viewModel.Description = "Test Description";
            Container parentContainer = new(Guid.NewGuid(), "Parent", "");
            this.viewModel.SelectedContainer = parentContainer;
            Guid newContainerId = Guid.NewGuid();
            _ = this.dbMock.Setup(db => db.AddContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(newContainerId);

            // Act
            this.viewModel.Add();

            // Assert
            this.dbMock.Verify(db => db.AssociateItemWithContainer(newContainerId, parentContainer.Id), Times.Once);
        }

        [TestMethod]
        public void Constructor_LoadsContainers()
        {
            // Assert
            this.dbMock.Verify(db => db.GetContainersWithRelationships(), Times.Once);
            Assert.IsNotNull(this.viewModel.Containers);
        }

        [TestMethod]
        public void DeleteSelectedContainer_RemovesContainerFromCollection()
        {
            // Arrange
            Container container = new(Guid.NewGuid(), "Test", "");
            this.viewModel.Containers.Add(container);
            this.viewModel.SelectedContainerRow = container;

            // Act
            this.viewModel.DeleteContainerCommand.Execute(null);

            // Assert
            Assert.IsFalse(this.viewModel.Containers.Contains(container));
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.dbMock = new Mock<IItemEyezDatabase>();
            _ = this.dbMock.Setup(db => db.GetContainersWithRelationships()).Returns(new ObservableCollection<Container>());
            this.viewModel = new ContainersViewModel(this.dbMock.Object);
        }
    }
}
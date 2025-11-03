using System.Collections.ObjectModel;
using Item_eyez.Database;
using Item_eyez.Viewmodels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Item_eyez.Tests
{
    [TestClass]
    public class ContainersViewModelTests
    {
        private Mock<IItemEyezDatabase> dbMock;
        private ContainersViewModel viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            dbMock = new Mock<IItemEyezDatabase>();
            dbMock.Setup(db => db.GetContainersWithRelationships()).Returns(new ObservableCollection<Container>());
            viewModel = new ContainersViewModel(dbMock.Object);
        }

        [TestMethod]
        public void Constructor_LoadsContainers()
        {
            // Assert
            dbMock.Verify(db => db.GetContainersWithRelationships(), Times.Once);
            Assert.IsNotNull(viewModel.Containers);
        }

        [TestMethod]
        public void Add_CallsAddContainerOnDatabase()
        {
            // Arrange
            viewModel.Name = "Test Container";
            viewModel.Description = "Test Description";

            // Act
            viewModel.Add();

            // Assert
            dbMock.Verify(db => db.AddContainer("Test Container", "Test Description"), Times.Once);
        }

        [TestMethod]
        public void Add_WithSelectedContainer_AssociatesWithContainer()
        {
            // Arrange
            viewModel.Name = "Test Container";
            viewModel.Description = "Test Description";
            var parentContainer = new Container(Guid.NewGuid(), "Parent", "");
            viewModel.SelectedContainer = parentContainer;
            var newContainerId = Guid.NewGuid();
            dbMock.Setup(db => db.AddContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(newContainerId);


            // Act
            viewModel.Add();

            // Assert
            dbMock.Verify(db => db.AssociateItemWithContainer(newContainerId, parentContainer.Id), Times.Once);
        }

        [TestMethod]
        public void DeleteSelectedContainer_RemovesContainerFromCollection()
        {
            // Arrange
            var container = new Container(Guid.NewGuid(), "Test", "");
            viewModel.Containers.Add(container);
            viewModel.SelectedContainerRow = container;

            // Act
            viewModel.DeleteContainerCommand.Execute(null);

            // Assert
            Assert.IsFalse(viewModel.Containers.Contains(container));
        }
    }
}
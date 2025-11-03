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

using System;
using System.Collections.ObjectModel;
using System.Data;
using Item_eyez.Viewmodels;

namespace Item_eyez.Database
{
    /// <summary>
    /// Interface for the ItemEyez database.
    /// </summary>
    public interface IItemEyezDatabase
    {
        /// <summary>
        /// Occurs when data in the database has changed.
        /// </summary>
        event ItemEyezDatabase.DataChangedEventHandler? DataChanged;

        /// <summary>
        /// Adds a new container to the database.
        /// </summary>
        /// <param name="name">The name of the container.</param>
        /// <param name="description">The description of the container.</param>
        /// <returns>The ID of the new container.</returns>
        Guid AddContainer(string name, string description);

        /// <summary>
        /// Adds a new item to the database.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="description">The description of the item.</param>
        /// <param name="value">The value of the item.</param>
        /// <param name="categories">The categories of the item.</param>
        /// <returns>The ID of the new item.</returns>
        Guid AddItem(string name, string description, decimal value, string categories);

        /// <summary>
        /// Adds a new room to the database.
        /// </summary>
        /// <param name="name">The name of the room.</param>
        /// <param name="description">The description of the room.</param>
        void AddRoom(string name, string description);

        /// <summary>
        /// Associates an item with a container.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="containerId">The ID of the container.</param>
        void AssociateItemWithContainer(Guid itemId, Guid containerId);

        /// <summary>
        /// Associates an item with a room.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="roomId">The ID of the room.</param>
        void AssociateItemWithRoom(Guid itemId, Guid roomId);

        /// <summary>
        /// Begins a batch operation, suppressing data change notifications.
        /// </summary>
        void BeginBatch();

        /// <summary>
        /// Deletes a container from the database.
        /// </summary>
        /// <param name="containerId">The ID of the container to delete.</param>
        void DeleteContainer(Guid containerId);

        /// <summary>
        /// Deletes an item from the database.
        /// </summary>
        /// <param name="itemId">The ID of the item to delete.</param>
        void DeleteItem(Guid itemId);

        /// <summary>
        /// Deletes a room from the database.
        /// </summary>
        /// <param name="roomId">The ID of the room to delete.</param>
        void DeleteRoom(Guid roomId);

        /// <summary>
        /// Ends a batch operation and raises a data change notification.
        /// </summary>
        void EndBatch();

        /// <summary>
        /// Gets the container ID for a given entity.
        /// </summary>
        /// <param name="entityId">The ID of the entity.</param>
        /// <returns>The container ID, or null if not found.</returns>
        Guid? GetContainerIdForEntity(Guid entityId);

        /// <summary>
        /// Gets a DataTable of all containers.
        /// </summary>
        /// <returns>A DataTable of containers.</returns>
        DataTable GetContainers();

        /// <summary>
        /// Gets an ObservableCollection of containers with their relationships.
        /// </summary>
        /// <returns>An ObservableCollection of containers.</returns>
        ObservableCollection<Container> GetContainersWithRelationships();

        /// <summary>
        /// Gets a DataTable of all items.
        /// </summary>
        /// <returns>A DataTable of items.</returns>
        DataTable GetItems();

        /// <summary>
        /// Gets the container for a given item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <returns>The container of the item.</returns>
        Container GetItemsContainer(Guid itemId);

        /// <summary>
        /// Gets the room for a given item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <returns>The room of the item.</returns>
        Room GetItemsRoom(Guid itemId);

        /// <summary>
        /// Gets an ObservableCollection of items with their relationships.
        /// </summary>
        /// <returns>An ObservableCollection of items.</returns>
        ObservableCollection<Item> GetItemsWithRelationships();

        /// <summary>
        /// Gets the room ID for a given entity.
        /// </summary>
        /// <param name="entityId">The ID of the entity.</param>
        /// <returns>The room ID, or null if not found.</returns>
        Guid? GetRoomIdForEntity(Guid entityId);

        /// <summary>
        /// Gets a DataTable of all rooms.
        /// </summary>
        /// <returns>A DataTable of rooms.</returns>
        DataTable GetRooms();

        /// <summary>
        /// Gets an ObservableCollection of all rooms.
        /// </summary>
        /// <returns>An ObservableCollection of rooms.</returns>
        ObservableCollection<Room> GetRoomsList();

        /// <summary>
        /// Raises the DataChanged event.
        /// </summary>
        void OnDataChanged();

        /// <summary>
        /// Sets the container for an item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="containerId">The ID of the new container.</param>
        void SetItemsContainer(Guid itemId, Guid? containerId);

        /// <summary>
        /// Sets the room for an item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="roomId">The ID of the new room.</param>
        void SetItemsRoom(Guid itemId, Guid? roomId);

        /// <summary>
        /// Unassociates an item from its container.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        void UnassociateItemFromContainer(Guid itemId);

        /// <summary>
        /// Unassociates an item from its room.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        void UnassociateItemFromRoom(Guid itemId);

        /// <summary>
        /// Updates a container in the database.
        /// </summary>
        /// <param name="containerId">The ID of the container to update.</param>
        /// <param name="newName">The new name of the container.</param>
        /// <param name="newDescription">The new description of the container.</param>
        void UpdateContainer(Guid containerId, string newName, string newDescription);

        /// <summary>
        /// Updates an item in the database.
        /// </summary>
        /// <param name="itemId">The ID of the item to update.</param>
        /// <param name="newName">The new name of the item.</param>
        /// <param name="newDescription">The new description of the item.</param>
        /// <param name="newValue">The new value of the item.</param>
        /// <param name="newCategories">The new categories of the item.</param>
        void UpdateItem(Guid itemId, string newName, string newDescription, decimal newValue, string newCategories);

        /// <summary>
        /// Updates a room in the database.
        /// </summary>
        /// <param name="roomId">The ID of the room to update.</param>
        /// <param name="newName">The new name of the room.</param>
        /// <param name="newDescription">The new description of the room.</param>
        void UpdateRoom(Guid roomId, string newName, string newDescription);
    }
}
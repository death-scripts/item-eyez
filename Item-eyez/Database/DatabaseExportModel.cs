// ----------------------------------------------------------------------------
// <copyright company="death-scripts">
// Copyright (c) death-scripts. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Item_eyez.Database
{
    /// <summary>
    /// Snapshot of the ItemEyez database used for import/export.
    /// </summary>
    public class DatabaseExportModel
    {
        /// <summary>
        /// Gets or sets the rooms.
        /// </summary>
        public List<DatabaseExportRoom> Rooms { get; set; } = new();

        /// <summary>
        /// Gets or sets the containers.
        /// </summary>
        public List<DatabaseExportContainer> Containers { get; set; } = new();

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public List<DatabaseExportItem> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets the item-to-container relationships.
        /// </summary>
        public List<DatabaseExportItemContainer> ItemContainers { get; set; } = new();

        /// <summary>
        /// Gets or sets the item-to-room relationships.
        /// </summary>
        public List<DatabaseExportItemRoom> ItemRooms { get; set; } = new();
    }

    /// <summary>
    /// Serializable room representation.
    /// </summary>
    public class DatabaseExportRoom
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Guid? ParentRoomId { get; set; }
    }

    /// <summary>
    /// Serializable container representation.
    /// </summary>
    public class DatabaseExportContainer
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    /// <summary>
    /// Serializable item representation.
    /// </summary>
    public class DatabaseExportItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal? Value { get; set; }

        public string? Categories { get; set; }
    }

    /// <summary>
    /// Serializable item-container link.
    /// </summary>
    public class DatabaseExportItemContainer
    {
        public Guid ItemId { get; set; }

        public Guid ContainerId { get; set; }
    }

    /// <summary>
    /// Serializable item-room link.
    /// </summary>
    public class DatabaseExportItemRoom
    {
        public Guid ItemId { get; set; }

        public Guid RoomId { get; set; }
    }
}


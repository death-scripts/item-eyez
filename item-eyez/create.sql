USE ITEMEYEZ;

-- Table for items
CREATE TABLE item (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    value DECIMAL(10, 2),
    categories TEXT -- JSON or a comma-separated list to store multiple categories
);
GO

-- Table for containers
CREATE TABLE container (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name VARCHAR(255) NOT NULL,
    description TEXT
);
GO

-- Table for rooms
CREATE TABLE room (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name VARCHAR(255) NOT NULL,
    description TEXT
);
GO

-- Relationship: item is contained in a container
CREATE TABLE isContainedIn (
    item_id UNIQUEIDENTIFIER,
    container_id UNIQUEIDENTIFIER,
    PRIMARY KEY (item_id, container_id),
    FOREIGN KEY (item_id) REFERENCES item(id) ON DELETE CASCADE,
    FOREIGN KEY (container_id) REFERENCES container(id) ON DELETE CASCADE
);
GO

-- Relationship: item is stored in a room
CREATE TABLE isStoredIn (
    item_id UNIQUEIDENTIFIER,
    room_id UNIQUEIDENTIFIER,
    PRIMARY KEY (item_id, room_id),
    FOREIGN KEY (item_id) REFERENCES item(id) ON DELETE CASCADE,
    FOREIGN KEY (room_id) REFERENCES room(id) ON DELETE CASCADE
);
GO

CREATE PROCEDURE AddRoom
    @roomName VARCHAR(255),      -- Room name
    @roomDescription TEXT        -- Room description
AS
BEGIN
    INSERT INTO room (name, description)
    VALUES (@roomName, @roomDescription);

    PRINT 'Room added successfully.';
END;
GO

CREATE PROCEDURE AddContainer
    @containerName VARCHAR(255),    
    @containerDescription TEXT        
AS
BEGIN
    INSERT INTO container (name, description)
    VALUES (@containerName, @containerDescription);
END;
GO

CREATE PROCEDURE UpdateContainer
    @containerId UNIQUEIDENTIFIER,
    @newName VARCHAR(255),
    @newDescription TEXT
AS
BEGIN
    UPDATE room
    SET name = @newName,
        description = @newDescription
    WHERE id = @containerId;
END;
GO

CREATE PROCEDURE UpdateRoom
    @roomId UNIQUEIDENTIFIER,
    @newName VARCHAR(255),
    @newDescription TEXT
AS
BEGIN
    UPDATE room
    SET name = @newName,
        description = @newDescription
    WHERE id = @roomId;
END;
GO


CREATE PROCEDURE DeleteContainer
    @containerId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE FROM isContainedIn WHERE container_id = @containerId;
    DELETE FROM container WHERE id = @containerId;
END;
GO

CREATE PROCEDURE DeleteRoom
    @roomId UNIQUEIDENTIFIER
AS
BEGIN
    -- Delete associations in `isStoredIn`
    DELETE FROM isStoredIn WHERE room_id = @roomId;
    -- Delete the item itself
    DELETE FROM room WHERE id = @roomId;
END;
GO

-- Procedure to add an item
CREATE PROCEDURE AddItem
    @itemName VARCHAR(255),
    @itemDescription TEXT,
    @itemValue DECIMAL(10, 2),
    @itemCategories TEXT
AS
BEGIN
    INSERT INTO item (name, description, value, categories)
    VALUES (@itemName, @itemDescription, @itemValue, @itemCategories);
END;
GO

-- Procedure to associate an item with a container
CREATE PROCEDURE AssociateItemWithContainer
    @itemId UNIQUEIDENTIFIER,
    @containerId UNIQUEIDENTIFIER
AS
BEGIN
    INSERT INTO isContainedIn (item_id, container_id)
    VALUES (@itemId, @containerId);
END;
GO

-- Procedure to associate an item with a room
CREATE PROCEDURE AssociateItemWithRoom
    @itemId UNIQUEIDENTIFIER,
    @roomId UNIQUEIDENTIFIER
AS
BEGIN
    INSERT INTO isStoredIn (item_id, room_id)
    VALUES (@itemId, @roomId);
END;
GO

-- Procedure to update an item
CREATE PROCEDURE UpdateItem
    @itemId UNIQUEIDENTIFIER,
    @newName VARCHAR(255),
    @newDescription TEXT,
    @newValue DECIMAL(10, 2),
    @newCategories TEXT
AS
BEGIN
    UPDATE item
    SET name = @newName,
        description = @newDescription,
        value = @newValue,
        categories = @newCategories
    WHERE id = @itemId;
END;
GO

-- Procedure to delete an item
CREATE PROCEDURE DeleteItem
    @itemId UNIQUEIDENTIFIER
AS
BEGIN
    -- Delete associations in `isContainedIn`
    DELETE FROM isContainedIn WHERE item_id = @itemId;
    -- Delete associations in `isStoredIn`
    DELETE FROM isStoredIn WHERE item_id = @itemId;
    -- Delete the item itself
    DELETE FROM item WHERE id = @itemId;
END;
GO

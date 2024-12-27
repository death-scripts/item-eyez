-- Table for items
CREATE TABLE item (
    id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    value DECIMAL(10, 2),
    categories TEXT -- JSON or a comma-separated list to store multiple categories
);
GO

-- Table for containers
CREATE TABLE container (
    id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT
);
GO

-- Table for rooms
CREATE TABLE room (
    id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT
);
GO

-- Relationship: item is contained in a container
CREATE TABLE isContainedIn (
    item_id INT,
    container_id INT,
    PRIMARY KEY (item_id, container_id),
    FOREIGN KEY (item_id) REFERENCES item(id) ON DELETE CASCADE,
    FOREIGN KEY (container_id) REFERENCES container(id) ON DELETE CASCADE
);
GO

-- Relationship: item is stored in a room
CREATE TABLE isStoredIn (
    item_id INT,
    room_id INT,
    PRIMARY KEY (item_id, room_id),
    FOREIGN KEY (item_id) REFERENCES item(id) ON DELETE CASCADE,
    FOREIGN KEY (room_id) REFERENCES room(id) ON DELETE CASCADE
);
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
    @itemId INT,
    @containerId INT
AS
BEGIN
    INSERT INTO isContainedIn (item_id, container_id)
    VALUES (@itemId, @containerId);
END;
GO

-- Procedure to associate an item with a room
CREATE PROCEDURE AssociateItemWithRoom
    @itemId INT,
    @roomId INT
AS
BEGIN
    INSERT INTO isStoredIn (item_id, room_id)
    VALUES (@itemId, @roomId);
END;
GO

-- Procedure to update an item
CREATE PROCEDURE UpdateItem
    @itemId INT,
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
    @itemId INT
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

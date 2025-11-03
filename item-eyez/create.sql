IF DB_ID(N'ITEMEYEZ') IS NULL
BEGIN
    CREATE DATABASE ITEMEYEZ;
END;
GO

USE ITEMEYEZ;
GO

-- Ensure core tables exist.
IF OBJECT_ID(N'dbo.item', N'U') IS NULL
BEGIN
    EXEC(N'
        CREATE TABLE dbo.item (
            id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
            name VARCHAR(255) NOT NULL,
            description TEXT NULL,
            value DECIMAL(10, 2) NULL,
            categories TEXT NULL
        );
    ');
END;
GO

IF OBJECT_ID(N'dbo.container', N'U') IS NULL
BEGIN
    EXEC(N'
        CREATE TABLE dbo.container (
            id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
            name VARCHAR(255) NOT NULL,
            description TEXT NULL
        );
    ');
END;
GO

IF OBJECT_ID(N'dbo.room', N'U') IS NULL
BEGIN
    EXEC(N'
        CREATE TABLE dbo.room (
            id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
            name VARCHAR(255) NOT NULL,
            description TEXT NULL
        );
    ');
END;
GO

IF OBJECT_ID(N'dbo.isContainedIn', N'U') IS NULL
BEGIN
    EXEC(N'
        CREATE TABLE dbo.isContainedIn (
            item_id UNIQUEIDENTIFIER NOT NULL,
            container_id UNIQUEIDENTIFIER NOT NULL,
            PRIMARY KEY (item_id, container_id),
            CONSTRAINT FK_isContainedIn_item FOREIGN KEY (item_id) REFERENCES dbo.item(id) ON DELETE CASCADE,
            CONSTRAINT FK_isContainedIn_container FOREIGN KEY (container_id) REFERENCES dbo.container(id) ON DELETE CASCADE
        );
    ');
END;
GO

IF OBJECT_ID(N'dbo.isStoredIn', N'U') IS NULL
BEGIN
    EXEC(N'
        CREATE TABLE dbo.isStoredIn (
            item_id UNIQUEIDENTIFIER NOT NULL,
            room_id UNIQUEIDENTIFIER NOT NULL,
            PRIMARY KEY (item_id, room_id),
            CONSTRAINT FK_isStoredIn_item FOREIGN KEY (item_id) REFERENCES dbo.item(id) ON DELETE CASCADE,
            CONSTRAINT FK_isStoredIn_room FOREIGN KEY (room_id) REFERENCES dbo.room(id) ON DELETE CASCADE
        );
    ');
END;
GO

-- Stored procedures.
CREATE OR ALTER PROCEDURE dbo.AddRoom
    @roomName VARCHAR(255),
    @roomDescription TEXT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.room (name, description)
    VALUES (@roomName, @roomDescription);
END;
GO

CREATE OR ALTER PROCEDURE dbo.AddContainer
    @containerName VARCHAR(255),
    @containerDescription TEXT,
    @containerId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @containerId = NEWID();

    INSERT INTO dbo.container (id, name, description)
    VALUES (@containerId, @containerName, @containerDescription);
END;
GO

CREATE OR ALTER PROCEDURE dbo.AddItem
    @itemName VARCHAR(255),
    @itemDescription TEXT,
    @itemValue DECIMAL(10, 2),
    @itemCategories TEXT,
    @itemId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @itemId = NEWID();

    INSERT INTO dbo.item (id, name, description, value, categories)
    VALUES (@itemId, @itemName, @itemDescription, @itemValue, @itemCategories);
END;
GO

CREATE OR ALTER PROCEDURE dbo.UpdateItem
    @itemId UNIQUEIDENTIFIER,
    @newName VARCHAR(255),
    @newDescription TEXT,
    @newValue DECIMAL(10, 2),
    @newCategories TEXT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.item
    SET name = @newName,
        description = @newDescription,
        value = @newValue,
        categories = @newCategories
    WHERE id = @itemId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.UpdateContainer
    @containerId UNIQUEIDENTIFIER,
    @newName VARCHAR(255),
    @newDescription TEXT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.container
    SET name = @newName,
        description = @newDescription
    WHERE id = @containerId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.UpdateRoom
    @roomId UNIQUEIDENTIFIER,
    @newName VARCHAR(255),
    @newDescription TEXT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.room
    SET name = @newName,
        description = @newDescription
    WHERE id = @roomId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.DeleteItem
    @itemId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.isContainedIn WHERE item_id = @itemId;
    DELETE FROM dbo.isStoredIn WHERE item_id = @itemId;
    DELETE FROM dbo.item WHERE id = @itemId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.DeleteContainer
    @containerId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.isContainedIn WHERE container_id = @containerId;
    DELETE FROM dbo.container WHERE id = @containerId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.DeleteRoom
    @roomId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.isStoredIn WHERE room_id = @roomId;
    DELETE FROM dbo.room WHERE id = @roomId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.AssociateItemWithContainer
    @itemId UNIQUEIDENTIFIER,
    @containerId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.isContainedIn (item_id, container_id)
    VALUES (@itemId, @containerId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.UnassociateItemFromContainer
    @itemId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.isContainedIn
    WHERE item_id = @itemId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.AssociateItemWithRoom
    @itemId UNIQUEIDENTIFIER,
    @roomId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.isStoredIn (item_id, room_id)
    VALUES (@itemId, @roomId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.UnassociateItemFromRoom
    @itemId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.isStoredIn
    WHERE item_id = @itemId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.GetContainerIdForItem
    @itemId UNIQUEIDENTIFIER,
    @containerId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1) @containerId = container_id
    FROM dbo.isContainedIn
    WHERE item_id = @itemId;

    IF @containerId IS NULL
    BEGIN
        SET @containerId = NULL;
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.GetRoomIdForItem
    @itemId UNIQUEIDENTIFIER,
    @roomId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1) @roomId = room_id
    FROM dbo.isStoredIn
    WHERE item_id = @itemId;

    IF @roomId IS NULL
    BEGIN
        SET @roomId = NULL;
    END;
END;
GO

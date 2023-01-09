USE master
GO

DECLARE @SQL nvarchar(1000);
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'tasklist')
BEGIN
    SET @SQL = N'USE tasklist;

                 ALTER DATABASE tasklist SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                 USE master;

                 DROP DATABASE tasklist;';
    EXEC (@SQL);
END;

CREATE DATABASE tasklist
GO

USE tasklist
GO

CREATE TABLE folders
(
	Id			    int				IDENTITY(1,1)		NOT NULL,
	FolderName	    varchar(100)						NOT NULL,
	IsDeleteable	bit									NOT NULL,
	IsRenameable	bit									NOT NULL,
 
	CONSTRAINT PK_Folders PRIMARY KEY (Id),
	CONSTRAINT UC_FolderName UNIQUE(FolderName)
)
GO

CREATE TABLE tasks(
	Id				int				IDENTITY(1,1)		NOT NULL,
	TaskName		varchar(100)						NOT NULL,
	DueDate			date								NULL,
	RecurrenceId	int									NOT NULL,
	IsImportant		bit									NOT NULL,
	IsComplete		bit									NOT NULL,
	FolderId		int									NOT NULL,
	Note			varchar(255)						NULL,
 
	CONSTRAINT PK_Tasks PRIMARY KEY (Id),
	CONSTRAINT FK_Tasks_Folders FOREIGN KEY(FolderId) REFERENCES folders (Id)
		ON UPDATE CASCADE
		ON DELETE CASCADE
)
GO

INSERT INTO folders (FolderName, IsDeleteable, IsRenameable)
VALUES				('Important',0,0),
					('Completed',0,0),
					('Recurring',0,0),
					('Planned',0,0),
					('Tasks',0,0);
GO

CREATE PROCEDURE GetFolder
	@id int
AS
	SELECT f.Id AS 'FolderId',
		   f.FolderName,
		   f.IsDeleteable,
		   f.IsRenameable
	FROM folders f
	WHERE f.Id = @id;
GO

CREATE PROCEDURE GetFolders
AS
	SELECT f.Id AS 'FolderId',
		   f.FolderName,
		   f.IsDeleteable,
		   f.IsRenameable
	FROM folders f;
GO

CREATE PROCEDURE GetFolderWithTasks
	@id int
AS
	SELECT f.Id AS 'FolderId',
		   f.FolderName,
		   f.IsDeleteable,
		   f.IsRenameable,
		   t.Id AS 'TaskId',
		   t.TaskName,
		   t.DueDate,
		   t.RecurrenceId,
		   t.IsImportant,
		   t.IsComplete,
		   t.FolderId AS 'TaskFolderId',
		   t.Note
	FROM folders f
	LEFT JOIN tasks t ON (f.Id = t.FolderId)
	WHERE f.Id = @id;
GO

CREATE PROCEDURE GetFoldersWithTasks
AS
	SELECT f.Id AS 'FolderId',
		   f.FolderName,
		   f.IsDeleteable,
		   f.IsRenameable,
		   t.Id AS 'TaskId',
		   t.TaskName,
		   t.DueDate,
		   t.RecurrenceId,
		   t.IsImportant,
		   t.IsComplete,
		   t.FolderId AS 'TaskFolderId',
		   t.Note
	FROM folders f
	LEFT JOIN tasks t ON (f.Id = t.FolderId);
GO

CREATE PROCEDURE AddFolder
	@folderName varchar(100),
	@folderId int OUTPUT
AS
	INSERT INTO folders (FolderName)
	VALUES (@folderName);

	SELECT @folderId = SCOPE_IDENTITY();
GO

CREATE PROCEDURE DeleteFolder
	@id int
AS
	DELETE f
	FROM folders f
	WHERE f.Id = @id;
GO

CREATE PROCEDURE UpdateFolder
	@id int,
	@folderName varchar(100)
AS
	UPDATE f
	SET f.FolderName = @folderName
	FROM folders f
	WHERE f.Id = @id;
GO

CREATE PROCEDURE GetTask
	@id int
AS
	SELECT t.Id AS 'TaskId',
		   t.TaskName,
		   t.DueDate,
		   t.RecurrenceId,
		   t.IsImportant,
		   t.IsComplete,
		   t.FolderId AS 'TaskFolderId',
		   t.Note,
		   f.Id AS 'FolderId',
		   f.FolderName
	FROM tasks t
	INNER JOIN folders f ON (t.FolderId = f.Id)
	WHERE t.Id = @id;
GO

CREATE PROCEDURE GetTasks
AS
	SELECT t.Id AS 'TaskId',
		   t.TaskName,
		   t.DueDate,
		   t.RecurrenceId,
		   t.IsImportant,
		   t.IsComplete,
		   t.FolderId AS 'TaskFolderId',
		   t.Note,
		   f.Id AS 'FolderId',
		   f.FolderName
	FROM tasks t
	INNER JOIN folders f ON (t.FolderId = f.Id);
GO

CREATE PROCEDURE GetImportantTasks
AS
	SELECT f.Id AS 'FolderId',
		   f.FolderName,
		   t.Id AS 'TaskId',
		   t.TaskName,
		   t.DueDate,
		   t.RecurrenceId,
		   t.IsImportant,
		   t.IsComplete,
		   t.FolderId AS 'TaskFolderId',
		   t.Note
	FROM tasks t
	INNER JOIN folders f ON (t.FolderId = f.Id)
	WHERE t.IsImportant = 1;
GO

CREATE PROCEDURE GetCompletedTasks
AS
	SELECT f.Id AS 'FolderId',
		   f.FolderName,
		   t.Id AS 'TaskId',
		   t.TaskName,
		   t.DueDate,
		   t.RecurrenceId,
		   t.IsImportant,
		   t.IsComplete,
		   t.FolderId AS 'TaskFolderId',
		   t.Note
	FROM tasks t
	INNER JOIN folders f ON (t.FolderId = f.Id)
	WHERE t.IsComplete = 1;
GO

CREATE PROCEDURE GetRecurringTasks
AS
	SELECT f.Id AS 'FolderId',
		   f.FolderName,
		   t.Id AS 'TaskId',
		   t.TaskName,
		   t.DueDate,
		   t.RecurrenceId,
		   t.IsImportant,
		   t.IsComplete,
		   t.FolderId AS 'TaskFolderId',
		   t.Note
	FROM tasks t
	INNER JOIN folders f ON (t.FolderId = f.Id)
	WHERE t.RecurrenceId <> 0;
GO

CREATE PROCEDURE GetPlannedTasks
AS
	SELECT f.Id AS 'FolderId',
		   f.FolderName,
		   t.Id AS 'TaskId',
		   t.TaskName,
		   t.DueDate,
		   t.RecurrenceId,
		   t.IsImportant,
		   t.IsComplete,
		   t.FolderId AS 'TaskFolderId',
		   t.Note
	FROM tasks t
	INNER JOIN folders f ON (t.FolderId = f.Id)
	WHERE t.DueDate IS NOT NULL;
GO

CREATE PROCEDURE CreateTask
	@taskName varchar(100),
	@dueDate date,
	@recurrenceId int,
	@isImportant bit,
	@isComplete bit,
	@folderId int,
	@note varchar(255),
	@taskId int OUTPUT
AS
	INSERT INTO tasks (TaskName, 
					   DueDate, 
					   RecurrenceId, 
					   IsImportant, 
					   IsComplete, 
					   FolderId, 
					   Note)
	VALUES			  (@taskName, 
	                   @dueDate, 
					   @recurrenceId, 
					   @isImportant, 
					   @isComplete, 
					   @folderId, 
					   @note);

	SELECT @taskId = SCOPE_IDENTITY();
GO

CREATE PROCEDURE DeleteTask
	@id int
AS
	DELETE t 
	FROM tasks t
	WHERE t.Id = @id;
GO

CREATE PROCEDURE UpdateTask
	@id int,
	@taskName varchar(100),
	@dueDate date,
	@recurrenceId int,
	@isImportant bit,
	@isComplete bit,
	@folderId int,
	@note varchar(255)
AS
	UPDATE t
	SET t.TaskName = @taskName,
	    t.DueDate = @dueDate,
		t.RecurrenceId = @recurrenceId,
		t.IsImportant = @isImportant,
		t.IsComplete = @isComplete,
		t.FolderId = @folderId,
		t.Note = @note
	FROM tasks t
	WHERE t.Id = @id;
GO
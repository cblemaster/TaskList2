USE [tasklist]
GO

INSERT INTO [folders] ([FolderName],[IsDeleteable],[IsRenameable])
VALUES				  ('Friends and Family',1,1),
					  ('Home',1,1),
					  ('Pets',1,1),
					  ('Finance',1,1),
					  ('Career',1,1);

INSERT INTO [tasks] ([TaskName], [DueDate], [RecurrenceId], [IsImportant], [IsComplete], [FolderId], [Note])
VALUES              ('Start holiday shopping', NULL, 5, 1, 0, (SELECT Id FROM folders WHERE FolderName = 'Friends and Family'), 'See list'),
				    ('Text Dad', GETDATE(), 3, 0, 1, (SELECT Id FROM folders WHERE FolderName = 'Friends and Family'), NULL),
				    ('Trim bushes', NULL, 5, 1, 0, (SELECT Id FROM folders WHERE FolderName = 'Home'), NULL),
				    ('Order dog food', DATEADD(day, 1, GETDATE()), 4, 0, 1, (SELECT Id FROM folders WHERE FolderName = 'Pets'), NULL),
				    ('Send out party invites', DATEADD(day, 4, GETDATE()), 0, 1, 0, (SELECT Id FROM folders WHERE FolderName = 'Friends and Family'), NULL),
				    ('Change furnace filters', GETDATE(), 5, 0, 1, (SELECT Id FROM folders WHERE FolderName = 'Home'), NULL),
				    ('Pay bills', DATEADD(day, 1, GETDATE()), 4, 1, 0, (SELECT Id FROM folders WHERE FolderName = 'Finance'), NULL),
				    ('Order power bank', DATEADD(day, 3, GETDATE()), 0, 0, 1, (SELECT Id FROM folders WHERE FolderName = 'Tasks'), 'Amazon'),
				    ('Catch up on LinkedIn', NULL, 0, 1, 0, (SELECT Id FROM folders WHERE FolderName = 'Career'), NULL),
				    ('Schedule Dr. appointment', NULL, 0, 0, 1, (SELECT Id FROM folders WHERE FolderName = 'Tasks'), NULL),
				    ('Pick up prescriptions', GETDATE(), 4, 1, 0, (SELECT Id FROM folders WHERE FolderName = 'Tasks'), '3 ready'),
				    ('Schedule vet appointments', DATEADD(day, 1, GETDATE()), 5, 0, 1, (SELECT Id FROM folders WHERE FolderName = 'Pets'), NULL),
				    ('Clean gutters', NULL, 0, 1, 0, (SELECT Id FROM folders WHERE FolderName = 'Home'), NULL),
				    ('Cancel haircut appointment', DATEADD(day, 7, GETDATE()), 0, 0, 1, (SELECT Id FROM folders WHERE FolderName = 'Tasks'), NULL),
				    ('Grocery shopping', GETDATE(), 3, 1, 0, (SELECT Id FROM folders WHERE FolderName = 'Tasks'), NULL),
				    ('Fill out paperwork', NULL, 0, 0, 1, (SELECT Id FROM folders WHERE FolderName = 'Tasks'), NULL);
GO
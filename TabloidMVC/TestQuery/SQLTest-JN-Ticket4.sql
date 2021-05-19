USE [TabloidMVC]
GO

SET IDENTITY_INSERT [UserProfile] ON
INSERT INTO [UserProfile] (
	[Id], [FirstName], [LastName], [DisplayName], [Email], [CreateDateTime], [ImageLocation], [UserTypeId])
VALUES (2, 'Co', 'Admin', 'coadmin', 'coadmin@example.com', SYSDATETIME(), 'https://icon-library.com/images/admin_1246350.png', 1);
SET IDENTITY_INSERT [UserProfile] OFF
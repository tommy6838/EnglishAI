USE [EnglishLearningDB]
GO
SET IDENTITY_INSERT [dbo].[Topics] ON 

INSERT [dbo].[Topics] ([Id], [Name], [Description]) VALUES (1, N'測試主題', NULL)
SET IDENTITY_INSERT [dbo].[Topics] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([SerialNo], [Id], [Email], [PasswordHash], [Level], [UserName], [RegisteredAt]) VALUES (4, N'3c76b01f-ef0a-4759-9a14-d34fe7dd92a1', N'atommy6838@gmail.com', N'AQAAAAIAAYagAAAAENWnKqxG8uhs4OcHR3OwSLWmgFLVFIBtumKIr6+IMcreDJZckT49GK8VqVlE1Dooow==', 1, N'atommy6838', CAST(N'2025-05-31T20:21:31.7033036' AS DateTime2))
INSERT [dbo].[Users] ([SerialNo], [Id], [Email], [PasswordHash], [Level], [UserName], [RegisteredAt]) VALUES (3, N'd4badf61-5181-48ce-86cd-7a99ba604997', N'tommy6838@gmail.com', N'AQAAAAIAAYagAAAAEGEIllcJZYgVrBP+eZcNH/WrdUnJjiq/I7R0ZR7gXAGompbTrzD1lVFS2QwT3ktTSQ==', 1, N'tommy6838', CAST(N'2025-05-31T20:20:31.7033036' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET IDENTITY_INSERT [dbo].[Conversations] ON 

INSERT [dbo].[Conversations] ([Id], [UserId], [TopicId], [Question], [Answer], [CreatedAt]) VALUES (7, N'd4badf61-5181-48ce-86cd-7a99ba604997', 1, N'你好 2025/06/01', N'這是AI回覆的內容（請串接你自己的AI API）', CAST(N'2025-06-01T05:17:09.3831447' AS DateTime2))
INSERT [dbo].[Conversations] ([Id], [UserId], [TopicId], [Question], [Answer], [CreatedAt]) VALUES (8, N'd4badf61-5181-48ce-86cd-7a99ba604997', 1, N'今天星期幾?', N'這是AI回覆的內容（請串接你自己的AI API）', CAST(N'2025-06-01T05:17:38.4617530' AS DateTime2))
INSERT [dbo].[Conversations] ([Id], [UserId], [TopicId], [Question], [Answer], [CreatedAt]) VALUES (9, N'd4badf61-5181-48ce-86cd-7a99ba604997', 1, N'你好', N'我是CHATGPT，你好。', CAST(N'2025-06-01T05:23:58.1582529' AS DateTime2))
INSERT [dbo].[Conversations] ([Id], [UserId], [TopicId], [Question], [Answer], [CreatedAt]) VALUES (10, N'd4badf61-5181-48ce-86cd-7a99ba604997', 1, N'隨機單字', N'apple pie cat bee dog girl crazy.。', CAST(N'2025-06-01T05:26:29.0028035' AS DateTime2))
INSERT [dbo].[Conversations] ([Id], [UserId], [TopicId], [Question], [Answer], [CreatedAt]) VALUES (11, N'd4badf61-5181-48ce-86cd-7a99ba604997', 1, N'太棒了 隨機單字', N'check-in COVID-19 crazy. part-time apple!', CAST(N'2025-06-01T05:38:25.6076052' AS DateTime2))
INSERT [dbo].[Conversations] ([Id], [UserId], [TopicId], [Question], [Answer], [CreatedAt]) VALUES (12, N'd4badf61-5181-48ce-86cd-7a99ba604997', 1, N'你現在是GPT嗎? 請用英文回答', N'Yes, I am GPT, an AI language model developed to assist with various tasks, including answering questions and helping with language learning.', CAST(N'2025-06-01T07:51:18.4873226' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Conversations] OFF
GO
SET IDENTITY_INSERT [dbo].[FavoriteWords] ON 

INSERT [dbo].[FavoriteWords] ([Id], [UserId], [Word], [FavoritedAt], [ClickCount]) VALUES (1, N'd4badf61-5181-48ce-86cd-7a99ba604997', N'girl', CAST(N'2025-06-01T05:29:14.8393793' AS DateTime2), 0)
INSERT [dbo].[FavoriteWords] ([Id], [UserId], [Word], [FavoritedAt], [ClickCount]) VALUES (2, N'd4badf61-5181-48ce-86cd-7a99ba604997', N'crazy.。', CAST(N'2025-06-01T05:29:31.2707479' AS DateTime2), 0)
INSERT [dbo].[FavoriteWords] ([Id], [UserId], [Word], [FavoritedAt], [ClickCount]) VALUES (3, N'd4badf61-5181-48ce-86cd-7a99ba604997', N'apple', CAST(N'2025-06-01T05:45:49.2191546' AS DateTime2), 1)
INSERT [dbo].[FavoriteWords] ([Id], [UserId], [Word], [FavoritedAt], [ClickCount]) VALUES (4, N'd4badf61-5181-48ce-86cd-7a99ba604997', N'crazy', CAST(N'2025-06-01T05:42:08.7727027' AS DateTime2), 0)
SET IDENTITY_INSERT [dbo].[FavoriteWords] OFF
GO

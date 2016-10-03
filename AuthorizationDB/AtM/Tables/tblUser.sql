CREATE TABLE [AtM].[tblUser] (
    [Id]             INT          IDENTITY (1, 1) NOT NULL,
    [UserName]       VARCHAR (50) NULL,
    [IsActive]       BIT          CONSTRAINT [DF_tblUser_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]      VARCHAR (50) CONSTRAINT [DF_tblUser_CreatedBy] DEFAULT (suser_name()) NULL,
    [CreatedOn]      DATETIME     CONSTRAINT [DF_tblUser_CreatedOn] DEFAULT (getdate()) NULL,
    [LastModifiedBy] VARCHAR (50) CONSTRAINT [DF_tblUser_LastModifiedBy] DEFAULT (suser_name()) NULL,
    [LasModifiedOn]  DATETIME     CONSTRAINT [DF_tblUser_LasModifiedOn] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_tblUser] PRIMARY KEY CLUSTERED ([Id] ASC)
);


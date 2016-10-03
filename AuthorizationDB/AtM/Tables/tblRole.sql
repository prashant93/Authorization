CREATE TABLE [AtM].[tblRole] (
    [Id]             INT          IDENTITY (1, 1) NOT NULL,
    [RoleName]       VARCHAR (50) NULL,
    [ApplicatoinId]  INT          NULL,
    [IsActive]       BIT          CONSTRAINT [DF_tblAccess_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]      VARCHAR (50) CONSTRAINT [DF_tblRole_CreatedBy] DEFAULT (suser_name()) NULL,
    [CreatedOn]      DATETIME     CONSTRAINT [DF_tblRole_CreatedOn] DEFAULT (getdate()) NULL,
    [LastModifiedBy] VARCHAR (50) CONSTRAINT [DF_tblAccess_LastModifiedBy] DEFAULT (suser_name()) NULL,
    [LastModifiedOn] DATETIME     CONSTRAINT [DF_tblAccess_LastModifiedOn] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_tblAccess] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblRole_tblApplication] FOREIGN KEY ([ApplicatoinId]) REFERENCES [AtM].[tblApplication] ([Id])
);


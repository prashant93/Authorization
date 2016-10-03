CREATE TABLE [AtM].[tblApplicationPermission] (
    [Id]             INT          IDENTITY (1, 1) NOT NULL,
    [ApplicationId]  INT          NULL,
    [UserId]         INT          NULL,
    [GroupId]        INT          NULL,
    [RoleId]         INT          NULL,
    [IsActive]       BIT          CONSTRAINT [DF_tblPermission_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]      VARCHAR (50) CONSTRAINT [DF_tblApplicationPermission_CreatedBy] DEFAULT (suser_name()) NULL,
    [CreatedOn]      DATETIME     CONSTRAINT [DF_tblApplicationPermission_CreatedOn] DEFAULT (getdate()) NULL,
    [LastModifiedBy] VARCHAR (50) NULL,
    [LastModifiedOn] DATETIME     NULL,
    CONSTRAINT [PK_tblPermission] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblPermission_tblApplication] FOREIGN KEY ([ApplicationId]) REFERENCES [AtM].[tblApplication] ([Id]),
    CONSTRAINT [FK_tblPermission_tblGroup] FOREIGN KEY ([GroupId]) REFERENCES [AtM].[tblGroup] ([Id]),
    CONSTRAINT [FK_tblPermission_tblUser] FOREIGN KEY ([UserId]) REFERENCES [AtM].[tblUser] ([Id])
);


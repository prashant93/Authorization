CREATE TABLE [AtM].[tblGroup] (
    [Id]             INT          IDENTITY (1, 1) NOT NULL,
    [GroupName]      VARCHAR (50) NULL,
    [IsActive]       BIT          CONSTRAINT [DF_tblGroup_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]      VARCHAR (50) CONSTRAINT [DF_tblGroup_CreatedBy] DEFAULT (suser_name()) NULL,
    [CreatedOn]      DATETIME     CONSTRAINT [DF_tblGroup_CreatedOn] DEFAULT (getdate()) NULL,
    [LastModifiedBy] VARCHAR (50) NULL,
    [LastModifiedOn] DATETIME     NULL,
    CONSTRAINT [PK_tblGroup] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_GroupName] UNIQUE NONCLUSTERED ([GroupName] ASC)
);


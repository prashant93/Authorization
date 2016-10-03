CREATE TABLE [AtM].[tblApplication] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [ApplicationName]   VARCHAR (50)  NULL,
    [ApplicationURL]    VARCHAR (150) NULL,
    [ApplicationSecret] VARCHAR (500) NULL,
    [IsActive]          BIT           NOT NULL,
    [CreatedBy]         VARCHAR (50)  CONSTRAINT [DF_tblApplication_CreatedBy] DEFAULT (suser_name()) NULL,
    [CreatedOn]         DATETIME      CONSTRAINT [DF_tblApplication_CreatedOn] DEFAULT (getdate()) NULL,
    [LastModifiedBy]    VARCHAR (50)  NULL,
    [LastModifiedOn]    DATETIME      NULL,
    CONSTRAINT [PK_tblApplication] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_tblApplciation] UNIQUE NONCLUSTERED ([ApplicationName] ASC, [ApplicationURL] ASC, [ApplicationSecret] ASC)
);


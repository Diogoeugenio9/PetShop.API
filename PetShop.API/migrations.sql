IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Clientes] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [Sobrenome] nvarchar(max) NULL,
    [Cpf] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Telefone] nvarchar(max) NULL,
    [Cep] nvarchar(max) NULL,
    [Logradouro] nvarchar(max) NULL,
    [Numero] nvarchar(max) NULL,
    [Bairro] nvarchar(max) NULL,
    [Cidade] nvarchar(max) NULL,
    [Estado] nvarchar(max) NULL,
    [DataCadastro] datetime2 NOT NULL,
    [Ativo] bit NOT NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PetsModelo] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [Especie] nvarchar(max) NULL,
    [Raca] nvarchar(max) NULL,
    [Idade] int NOT NULL,
    [DataCadastro] datetime2 NOT NULL,
    [Ativo] bit NOT NULL,
    [ClienteId] int NOT NULL,
    CONSTRAINT [PK_PetsModelo] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PetsModelo_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_PetsModelo_ClienteId] ON [PetsModelo] ([ClienteId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260222235347_CriarBancoTCC', N'8.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260223011212_CriarTabelaServico', N'8.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Servicos] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [Descricao] nvarchar(max) NULL,
    [Preco] decimal(18,2) NOT NULL,
    [DuracaoMinutos] int NOT NULL,
    [Ativo] bit NOT NULL,
    CONSTRAINT [PK_Servicos] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260223012142_AddServicoTableFix', N'8.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Servicos]') AND [c].[name] = N'Preco');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Servicos] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Servicos] ALTER COLUMN [Preco] decimal(10,2) NOT NULL;
GO

CREATE TABLE [Agendamentos] (
    [Id] int NOT NULL IDENTITY,
    [DataHora] datetime2 NOT NULL,
    [Status] nvarchar(max) NULL,
    [PetId] int NOT NULL,
    [ServicoId] int NOT NULL,
    CONSTRAINT [PK_Agendamentos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Agendamentos_PetsModelo_PetId] FOREIGN KEY ([PetId]) REFERENCES [PetsModelo] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Agendamentos_Servicos_ServicoId] FOREIGN KEY ([ServicoId]) REFERENCES [Servicos] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Agendamentos_PetId] ON [Agendamentos] ([PetId]);
GO

CREATE INDEX [IX_Agendamentos_ServicoId] ON [Agendamentos] ([ServicoId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260223013848_CriarTabelaAgendamento', N'8.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Administradores] (
    [Id] int NOT NULL IDENTITY,
    [NomeProprietario] nvarchar(max) NULL,
    [NomeLoja] nvarchar(max) NULL,
    [Telefone] nvarchar(max) NULL,
    [Cidade] nvarchar(max) NULL,
    [Estado] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [SenhaHash] nvarchar(max) NULL,
    CONSTRAINT [PK_Administradores] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260308205109_InitialCreate', N'8.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260321144833_Initial', N'8.0.3');
GO

COMMIT;
GO


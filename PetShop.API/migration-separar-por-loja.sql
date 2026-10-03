SET XACT_ABORT ON;
GO

IF EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20261002225000_SepararPorLojaESemCascata')
BEGIN
    RAISERROR('Esta migration ja foi aplicada. Nada foi alterado.', 16, 1);
    SET NOEXEC ON;
END
GO

IF NOT EXISTS (SELECT 1 FROM [Administradores])
   AND (EXISTS (SELECT 1 FROM [Clientes]) OR EXISTS (SELECT 1 FROM [Servicos])
        OR EXISTS (SELECT 1 FROM [Produtos]) OR EXISTS (SELECT 1 FROM [Lancamentos]))
BEGIN
    RAISERROR('Existem dados mas nenhum administrador. Cadastre um administrador antes de rodar este script.', 16, 1);
    SET NOEXEC ON;
END
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Clientes] ADD [Excluido] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [PetsModelo] ADD [Excluido] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Servicos] ADD [Excluido] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Produtos] ADD [Excluido] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Clientes] ADD [AdministradorId] int NULL;
GO

ALTER TABLE [Servicos] ADD [AdministradorId] int NULL;
GO

ALTER TABLE [Produtos] ADD [AdministradorId] int NULL;
GO

ALTER TABLE [Lancamentos] ADD [AdministradorId] int NULL;
GO

UPDATE [Clientes] SET [AdministradorId] = (SELECT MIN([Id]) FROM [Administradores]) WHERE [AdministradorId] IS NULL;
GO

UPDATE [Servicos] SET [AdministradorId] = (SELECT MIN([Id]) FROM [Administradores]) WHERE [AdministradorId] IS NULL;
GO

UPDATE [Produtos] SET [AdministradorId] = (SELECT MIN([Id]) FROM [Administradores]) WHERE [AdministradorId] IS NULL;
GO

UPDATE [Lancamentos] SET [AdministradorId] = (SELECT MIN([Id]) FROM [Administradores]) WHERE [AdministradorId] IS NULL;
GO

ALTER TABLE [Clientes] ALTER COLUMN [AdministradorId] int NOT NULL;
GO

CREATE INDEX [IX_Clientes_AdministradorId] ON [Clientes] ([AdministradorId]);
GO

ALTER TABLE [Clientes] ADD CONSTRAINT [FK_Clientes_Administradores_AdministradorId] FOREIGN KEY ([AdministradorId]) REFERENCES [Administradores] ([Id]);
GO

ALTER TABLE [Servicos] ALTER COLUMN [AdministradorId] int NOT NULL;
GO

CREATE INDEX [IX_Servicos_AdministradorId] ON [Servicos] ([AdministradorId]);
GO

ALTER TABLE [Servicos] ADD CONSTRAINT [FK_Servicos_Administradores_AdministradorId] FOREIGN KEY ([AdministradorId]) REFERENCES [Administradores] ([Id]);
GO

ALTER TABLE [Produtos] ALTER COLUMN [AdministradorId] int NOT NULL;
GO

CREATE INDEX [IX_Produtos_AdministradorId] ON [Produtos] ([AdministradorId]);
GO

ALTER TABLE [Produtos] ADD CONSTRAINT [FK_Produtos_Administradores_AdministradorId] FOREIGN KEY ([AdministradorId]) REFERENCES [Administradores] ([Id]);
GO

ALTER TABLE [Lancamentos] ALTER COLUMN [AdministradorId] int NOT NULL;
GO

CREATE INDEX [IX_Lancamentos_AdministradorId] ON [Lancamentos] ([AdministradorId]);
GO

ALTER TABLE [Lancamentos] ADD CONSTRAINT [FK_Lancamentos_Administradores_AdministradorId] FOREIGN KEY ([AdministradorId]) REFERENCES [Administradores] ([Id]);
GO

ALTER TABLE [PetsModelo] DROP CONSTRAINT [FK_PetsModelo_Clientes_ClienteId];
GO

ALTER TABLE [Agendamentos] DROP CONSTRAINT [FK_Agendamentos_PetsModelo_PetId];
GO

ALTER TABLE [Agendamentos] DROP CONSTRAINT [FK_Agendamentos_Servicos_ServicoId];
GO

ALTER TABLE [PetsModelo] ADD CONSTRAINT [FK_PetsModelo_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]);
GO

ALTER TABLE [Agendamentos] ADD CONSTRAINT [FK_Agendamentos_PetsModelo_PetId] FOREIGN KEY ([PetId]) REFERENCES [PetsModelo] ([Id]);
GO

ALTER TABLE [Agendamentos] ADD CONSTRAINT [FK_Agendamentos_Servicos_ServicoId] FOREIGN KEY ([ServicoId]) REFERENCES [Servicos] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261002225000_SepararPorLojaESemCascata', N'8.0.3');
GO

COMMIT;
GO

SET NOEXEC OFF;
GO

-- =====================================================
-- GameStore - Script do banco de dados (SQL Server)
-- Usado no dia da apresentação
-- =====================================================

CREATE DATABASE GAMESTORE
GO
USE GAMESTORE
GO

-- TABELAS

CREATE TABLE Categorias (
    id   int NOT NULL PRIMARY KEY,
    nome varchar(50) NOT NULL
)
GO

CREATE TABLE Plataformas (
    id         int NOT NULL PRIMARY KEY,
    nome       varchar(50) NOT NULL,
    fabricante varchar(50) NULL
)
GO

CREATE TABLE Jogos (
    id             int NOT NULL PRIMARY KEY,
    nome           varchar(100) NOT NULL,
    descricao      varchar(1000) NULL,
    preco          decimal(10,2) NOT NULL,
    estoque        int NOT NULL CHECK (estoque >= 0),
    dataLancamento date NOT NULL,
    categoriaId    int NOT NULL FOREIGN KEY REFERENCES Categorias(id),
    plataformaId   int NOT NULL FOREIGN KEY REFERENCES Plataformas(id),
    imagem         varbinary(max) NULL
)
GO

CREATE TABLE Clientes (
    id         int NOT NULL PRIMARY KEY,
    nome       varchar(100) NOT NULL,
    email      varchar(100) NOT NULL UNIQUE,
    senha      varchar(64) NOT NULL,
    cpf        varchar(14) NULL,
    telefone   varchar(20) NULL,
    cep        varchar(9) NULL,
    logradouro varchar(100) NULL,
    numero     varchar(10) NULL,
    bairro     varchar(50) NULL,
    cidade     varchar(50) NULL,
    uf         varchar(2) NULL
)
GO

CREATE TABLE Administradores (
    id      int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    usuario varchar(30) NOT NULL UNIQUE,
    senha   varchar(64) NOT NULL,
    nome    varchar(100) NOT NULL
)
GO

CREATE TABLE Pedidos (
    id         int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    data       datetime NOT NULL,
    clienteId  int NOT NULL FOREIGN KEY REFERENCES Clientes(id),
    valorTotal decimal(10,2) NOT NULL,
    status     varchar(30) NOT NULL
)
GO

CREATE TABLE PedidoItens (
    id            int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    pedidoId      int NOT NULL FOREIGN KEY REFERENCES Pedidos(id) ON DELETE CASCADE,
    jogoId        int NOT NULL FOREIGN KEY REFERENCES Jogos(id),
    qtde          int NOT NULL,
    valorUnitario decimal(10,2) NOT NULL
)
GO

-- PROCEDURES GENÉRICAS (recebem o nome da tabela)

CREATE PROCEDURE spDelete (@id int, @tabela varchar(max))
AS
BEGIN
    DECLARE @sql varchar(max);
    SET @sql = 'delete ' + @tabela + ' where id = ' + CAST(@id AS varchar(max))
    EXEC(@sql)
END
GO

CREATE PROCEDURE spProximoId (@tabela varchar(max))
AS
BEGIN
    EXEC('select isnull(max(id) + 1, 1) as MAIOR from ' + @tabela)
END
GO

-- CATEGORIAS

CREATE PROCEDURE spInsert_Categorias (@id int, @nome varchar(max))
AS
BEGIN
    INSERT INTO Categorias (id, nome) VALUES (@id, @nome)
END
GO

CREATE PROCEDURE spUpdate_Categorias (@id int, @nome varchar(max))
AS
BEGIN
    UPDATE Categorias SET nome = @nome WHERE id = @id
END
GO

CREATE PROCEDURE spConsulta_Categorias (@id int)
AS
BEGIN
    SELECT * FROM Categorias WHERE id = @id
END
GO

CREATE PROCEDURE spListagem_Categorias
AS
BEGIN
    SELECT * FROM Categorias ORDER BY nome
END
GO

-- PLATAFORMAS

CREATE PROCEDURE spInsert_Plataformas (@id int, @nome varchar(max), @fabricante varchar(max))
AS
BEGIN
    INSERT INTO Plataformas (id, nome, fabricante) VALUES (@id, @nome, @fabricante)
END
GO

CREATE PROCEDURE spUpdate_Plataformas (@id int, @nome varchar(max), @fabricante varchar(max))
AS
BEGIN
    UPDATE Plataformas SET nome = @nome, fabricante = @fabricante WHERE id = @id
END
GO

CREATE PROCEDURE spConsulta_Plataformas (@id int)
AS
BEGIN
    SELECT * FROM Plataformas WHERE id = @id
END
GO

CREATE PROCEDURE spListagem_Plataformas
AS
BEGIN
    SELECT * FROM Plataformas ORDER BY nome
END
GO

-- JOGOS

CREATE PROCEDURE spInsert_Jogos (@id int, @nome varchar(max), @descricao varchar(max), @preco decimal(10,2),
    @estoque int, @dataLancamento datetime, @categoriaId int, @plataformaId int, @imagem varbinary(max))
AS
BEGIN
    INSERT INTO Jogos (id, nome, descricao, preco, estoque, dataLancamento, categoriaId, plataformaId, imagem)
    VALUES (@id, @nome, @descricao, @preco, @estoque, @dataLancamento, @categoriaId, @plataformaId, @imagem)
END
GO

CREATE PROCEDURE spUpdate_Jogos (@id int, @nome varchar(max), @descricao varchar(max), @preco decimal(10,2),
    @estoque int, @dataLancamento datetime, @categoriaId int, @plataformaId int, @imagem varbinary(max))
AS
BEGIN
    UPDATE Jogos SET nome = @nome, descricao = @descricao, preco = @preco, estoque = @estoque,
           dataLancamento = @dataLancamento, categoriaId = @categoriaId, plataformaId = @plataformaId, imagem = @imagem
     WHERE id = @id
END
GO

CREATE PROCEDURE spConsulta_Jogos (@id int)
AS
BEGIN
    SELECT * FROM Jogos WHERE id = @id
END
GO

-- listagem com join de categoria e plataforma
CREATE PROCEDURE spListagem_Jogos
AS
BEGIN
    SELECT j.*, c.nome AS NomeCategoria, p.nome AS NomePlataforma
      FROM Jogos j
     INNER JOIN Categorias c ON c.id = j.categoriaId
     INNER JOIN Plataformas p ON p.id = j.plataformaId
     ORDER BY j.nome
END
GO

-- vitrine da área do cliente (filtros: nome, categoria e plataforma; 0 = todas)
CREATE PROCEDURE spListagemVitrine (@nome varchar(max), @categoriaId int, @plataformaId int)
AS
BEGIN
    SELECT j.*, c.nome AS NomeCategoria, p.nome AS NomePlataforma
      FROM Jogos j
     INNER JOIN Categorias c ON c.id = j.categoriaId
     INNER JOIN Plataformas p ON p.id = j.plataformaId
     WHERE j.nome LIKE '%' + @nome + '%'
       AND (@categoriaId = 0 OR j.categoriaId = @categoriaId)
       AND (@plataformaId = 0 OR j.plataformaId = @plataformaId)
     ORDER BY j.nome
END
GO

-- CLIENTES

CREATE PROCEDURE spInsert_Clientes (@id int, @nome varchar(max), @email varchar(max), @senha varchar(max),
    @cpf varchar(max), @telefone varchar(max), @cep varchar(max), @logradouro varchar(max), @numero varchar(max),
    @bairro varchar(max), @cidade varchar(max), @uf varchar(max))
AS
BEGIN
    INSERT INTO Clientes (id, nome, email, senha, cpf, telefone, cep, logradouro, numero, bairro, cidade, uf)
    VALUES (@id, @nome, @email, @senha, @cpf, @telefone, @cep, @logradouro, @numero, @bairro, @cidade, @uf)
END
GO

CREATE PROCEDURE spUpdate_Clientes (@id int, @nome varchar(max), @email varchar(max), @senha varchar(max),
    @cpf varchar(max), @telefone varchar(max), @cep varchar(max), @logradouro varchar(max), @numero varchar(max),
    @bairro varchar(max), @cidade varchar(max), @uf varchar(max))
AS
BEGIN
    UPDATE Clientes SET nome = @nome, email = @email, senha = @senha, cpf = @cpf, telefone = @telefone,
           cep = @cep, logradouro = @logradouro, numero = @numero, bairro = @bairro, cidade = @cidade, uf = @uf
     WHERE id = @id
END
GO

CREATE PROCEDURE spConsulta_Clientes (@id int)
AS
BEGIN
    SELECT * FROM Clientes WHERE id = @id
END
GO

CREATE PROCEDURE spListagem_Clientes
AS
BEGIN
    SELECT * FROM Clientes ORDER BY nome
END
GO

CREATE PROCEDURE spConsultaClientePorEmail (@email varchar(max))
AS
BEGIN
    SELECT * FROM Clientes WHERE email = @email
END
GO

-- ADMINISTRADORES (login da área administrativa)

CREATE PROCEDURE spLoginAdministrador (@usuario varchar(max), @senha varchar(max))
AS
BEGIN
    SELECT nome FROM Administradores WHERE usuario = @usuario AND senha = @senha
END
GO

-- PEDIDOS (mestre)

-- devolve o id gerado
CREATE PROCEDURE spInsert_Pedidos (@id int, @data datetime, @clienteId int, @valorTotal decimal(10,2), @status varchar(max))
AS
BEGIN
    SET NOCOUNT ON
    INSERT INTO Pedidos (data, clienteId, valorTotal, status) VALUES (@data, @clienteId, @valorTotal, @status)
    SELECT CAST(SCOPE_IDENTITY() AS int)
END
GO

CREATE PROCEDURE spAlteraStatusPedido (@id int, @status varchar(max))
AS
BEGIN
    UPDATE Pedidos SET status = @status WHERE id = @id
END
GO

CREATE PROCEDURE spConsulta_Pedidos (@id int)
AS
BEGIN
    SELECT p.*, c.nome AS NomeCliente,
           (SELECT ISNULL(SUM(i.qtde), 0) FROM PedidoItens i WHERE i.pedidoId = p.id) AS QtdeItens
      FROM Pedidos p
     INNER JOIN Clientes c ON c.id = p.clienteId
     WHERE p.id = @id
END
GO

CREATE PROCEDURE spListagem_Pedidos
AS
BEGIN
    SELECT p.*, c.nome AS NomeCliente,
           (SELECT ISNULL(SUM(i.qtde), 0) FROM PedidoItens i WHERE i.pedidoId = p.id) AS QtdeItens
      FROM Pedidos p
     INNER JOIN Clientes c ON c.id = p.clienteId
     ORDER BY p.data DESC
END
GO

CREATE PROCEDURE spListagemPedidosCliente (@clienteId int)
AS
BEGIN
    SELECT p.*, c.nome AS NomeCliente,
           (SELECT ISNULL(SUM(i.qtde), 0) FROM PedidoItens i WHERE i.pedidoId = p.id) AS QtdeItens
      FROM Pedidos p
     INNER JOIN Clientes c ON c.id = p.clienteId
     WHERE p.clienteId = @clienteId
     ORDER BY p.data DESC
END
GO

-- PEDIDO ITENS (detalhe) - também baixa o estoque do jogo
CREATE PROCEDURE spInsert_PedidoItens (@id int, @pedidoId int, @jogoId int, @qtde int, @valorUnitario decimal(10,2))
AS
BEGIN
    SET NOCOUNT ON
    UPDATE Jogos SET estoque = estoque - @qtde WHERE id = @jogoId
    INSERT INTO PedidoItens (pedidoId, jogoId, qtde, valorUnitario) VALUES (@pedidoId, @jogoId, @qtde, @valorUnitario)
    SELECT CAST(SCOPE_IDENTITY() AS int)
END
GO

CREATE PROCEDURE spListagemItensPedido (@pedidoId int)
AS
BEGIN
    SELECT i.*, j.nome AS NomeJogo, p.nome AS NomePlataforma
      FROM PedidoItens i
     INNER JOIN Jogos j ON j.id = i.jogoId
     INNER JOIN Plataformas p ON p.id = j.plataformaId
     WHERE i.pedidoId = @pedidoId
     ORDER BY i.id
END
GO

-- CONSULTAS AVANÇADAS

-- consulta 1: pedidos (filtros: nome do cliente, status, período)
CREATE PROCEDURE spConsultaAvancadaPedidos (@nomeCliente varchar(max), @status varchar(max),
    @dataInicial datetime, @dataFinal datetime)
AS
BEGIN
    SELECT p.*, c.nome AS NomeCliente,
           (SELECT ISNULL(SUM(i.qtde), 0) FROM PedidoItens i WHERE i.pedidoId = p.id) AS QtdeItens
      FROM Pedidos p
     INNER JOIN Clientes c ON c.id = p.clienteId
     WHERE c.nome LIKE '%' + @nomeCliente + '%'
       AND (@status = '' OR p.status = @status)
       AND p.data BETWEEN @dataInicial AND @dataFinal
     ORDER BY p.data DESC
END
GO

-- consulta 2: jogos vendidos (filtros: nome do jogo, categoria, plataforma, período)
CREATE PROCEDURE spConsultaAvancadaVendas (@nomeJogo varchar(max), @categoriaId int, @plataformaId int,
    @dataInicial datetime, @dataFinal datetime)
AS
BEGIN
    SELECT i.*, j.nome AS NomeJogo, c.nome AS NomeCategoria, pl.nome AS NomePlataforma,
           cl.nome AS NomeCliente, p.data AS DataPedido
      FROM PedidoItens i
     INNER JOIN Pedidos p ON p.id = i.pedidoId
     INNER JOIN Jogos j ON j.id = i.jogoId
     INNER JOIN Categorias c ON c.id = j.categoriaId
     INNER JOIN Plataformas pl ON pl.id = j.plataformaId
     INNER JOIN Clientes cl ON cl.id = p.clienteId
     WHERE j.nome LIKE '%' + @nomeJogo + '%'
       AND (@categoriaId = 0 OR j.categoriaId = @categoriaId)
       AND (@plataformaId = 0 OR j.plataformaId = @plataformaId)
       AND p.data BETWEEN @dataInicial AND @dataFinal
     ORDER BY p.data DESC, j.nome
END
GO

-- APIs (dados consolidados, pedidos cancelados não entram)

CREATE PROCEDURE spVendasPorCategoria (@dataInicial datetime, @dataFinal datetime)
AS
BEGIN
    SELECT c.nome AS categoria, SUM(i.qtde) AS qtdeVendida, SUM(i.qtde * i.valorUnitario) AS valorTotal
      FROM PedidoItens i
     INNER JOIN Pedidos p ON p.id = i.pedidoId
     INNER JOIN Jogos j ON j.id = i.jogoId
     INNER JOIN Categorias c ON c.id = j.categoriaId
     WHERE p.status <> 'Cancelado'
       AND p.data BETWEEN @dataInicial AND @dataFinal
     GROUP BY c.nome
     ORDER BY 3 DESC
END
GO

CREATE PROCEDURE spJogosMaisVendidos (@top int)
AS
BEGIN
    SELECT TOP (@top) j.id AS jogoId, j.nome AS jogo, pl.nome AS plataforma,
           SUM(i.qtde) AS qtdeVendida, SUM(i.qtde * i.valorUnitario) AS valorTotal
      FROM PedidoItens i
     INNER JOIN Pedidos p ON p.id = i.pedidoId
     INNER JOIN Jogos j ON j.id = i.jogoId
     INNER JOIN Plataformas pl ON pl.id = j.plataformaId
     WHERE p.status <> 'Cancelado'
     GROUP BY j.id, j.nome, pl.nome
     ORDER BY 4 DESC, 5 DESC
END
GO

CREATE PROCEDURE spResumoVendas
AS
BEGIN
    SELECT (SELECT COUNT(*) FROM Pedidos WHERE status <> 'Cancelado') AS totalPedidos,
           (SELECT ISNULL(SUM(valorTotal), 0) FROM Pedidos WHERE status <> 'Cancelado') AS faturamento,
           (SELECT ISNULL(CAST(AVG(valorTotal) AS decimal(10,2)), 0) FROM Pedidos WHERE status <> 'Cancelado') AS ticketMedio,
           (SELECT COUNT(*) FROM Clientes) AS totalClientes,
           (SELECT COUNT(*) FROM Jogos) AS totalJogos
END
GO

-- DADOS INICIAIS

-- senha dos usuários de teste: 1234 (gravada em SHA-256)
INSERT INTO Administradores (usuario, senha, nome)
VALUES ('admin', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4', 'Administrador')

INSERT INTO Categorias (id, nome) VALUES
(1, 'Ação'), (2, 'RPG'), (3, 'Corrida'), (4, 'Aventura'), (5, 'Tiro'), (6, 'Esporte')

INSERT INTO Plataformas (id, nome, fabricante) VALUES
(1, 'PC', 'Diversos'), (2, 'PlayStation 5', 'Sony'), (3, 'Xbox Series X|S', 'Microsoft'), (4, 'Nintendo Switch', 'Nintendo')

INSERT INTO Jogos (id, nome, descricao, preco, estoque, dataLancamento, categoriaId, plataformaId) VALUES
(1, 'Elden Ring', 'RPG de ação em mundo aberto.', 229.90, 15, '2022-02-25', 2, 2),
(2, 'Forza Horizon 5', 'Corrida em mundo aberto pelo México.', 249.00, 10, '2021-11-09', 3, 3),
(3, 'The Legend of Zelda: Tears of the Kingdom', 'Aventura em Hyrule.', 349.00, 8, '2023-05-12', 4, 4),
(4, 'Counter-Strike 2', 'Tiro tático competitivo.', 0.00, 99, '2023-09-27', 5, 1),
(5, 'EA Sports FC 25', 'Futebol com clubes licenciados.', 299.90, 20, '2024-09-27', 6, 2),
(6, 'God of War Ragnarök', 'Ação e aventura nórdica.', 249.90, 12, '2022-11-09', 1, 2),
(7, 'Baldur''s Gate 3', 'RPG baseado em D&D.', 199.99, 7, '2023-08-03', 2, 1),
(8, 'Mario Kart 8 Deluxe', 'Corrida para toda a família.', 299.00, 18, '2017-04-28', 3, 4)

INSERT INTO Clientes (id, nome, email, senha, cpf, telefone, cep, logradouro, numero, bairro, cidade, uf) VALUES
(1, 'Cliente Teste', 'cliente@teste.com', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4',
 '123.456.789-00', '(11) 99999-0000', '09850-901', 'Av. Humberto de Alencar Castelo Branco', '3972', 'Assunção', 'São Bernardo do Campo', 'SP'),
(2, 'Maria Gamer', 'maria@teste.com', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4',
 '987.654.321-00', '(11) 98888-1111', '01001-000', 'Praça da Sé', '100', 'Sé', 'São Paulo', 'SP')

-- alguns pedidos para as consultas e APIs terem dados
INSERT INTO Pedidos (data, clienteId, valorTotal, status) VALUES
('2026-10-05T14:30:00', 1, 479.80, 'Entregue'),
('2026-10-20T10:15:00', 2, 548.90, 'Enviado'),
('2026-11-10T19:45:00', 1, 299.00, 'Pago'),
('2026-11-15T21:00:00', 2, 199.99, 'Cancelado')

INSERT INTO PedidoItens (pedidoId, jogoId, qtde, valorUnitario) VALUES
(1, 1, 1, 229.90), (1, 6, 1, 249.90),
(2, 5, 1, 299.90), (2, 2, 1, 249.00),
(3, 8, 1, 299.00),
(4, 7, 1, 199.99)
GO

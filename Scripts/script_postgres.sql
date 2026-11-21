-- =====================================================
-- GameStore - Script do banco de dados (PostgreSQL)
-- Antes de rodar: CREATE DATABASE gamestore;
-- Depois conecte no banco gamestore e execute este script
-- (no pgAdmin, clique no banco gamestore antes de abrir o Query Tool)
-- =====================================================

-- CASCADE permite rodar o script de novo (as functions dependem das views)
DROP VIEW IF EXISTS vwpedidoitens, vwpedidos, vwjogos CASCADE;
DROP TABLE IF EXISTS pedidoitens, pedidos, administradores, clientes, jogos, plataformas, categorias CASCADE;

-- TABELAS

CREATE TABLE categorias (
    id   int PRIMARY KEY,
    nome varchar(50) NOT NULL
);

CREATE TABLE plataformas (
    id         int PRIMARY KEY,
    nome       varchar(50) NOT NULL,
    fabricante varchar(50) NULL
);

CREATE TABLE jogos (
    id             int PRIMARY KEY,
    nome           varchar(100) NOT NULL,
    descricao      varchar(1000) NULL,
    preco          numeric(10,2) NOT NULL,
    estoque        int NOT NULL CHECK (estoque >= 0),
    datalancamento date NOT NULL,
    categoriaid    int NOT NULL REFERENCES categorias(id),
    plataformaid   int NOT NULL REFERENCES plataformas(id),
    imagem         bytea NULL
);

CREATE TABLE clientes (
    id         int PRIMARY KEY,
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
);

CREATE TABLE administradores (
    id      serial PRIMARY KEY,
    usuario varchar(30) NOT NULL UNIQUE,
    senha   varchar(64) NOT NULL,
    nome    varchar(100) NOT NULL
);

CREATE TABLE pedidos (
    id         serial PRIMARY KEY,
    data       timestamp NOT NULL,
    clienteid  int NOT NULL REFERENCES clientes(id),
    valortotal numeric(10,2) NOT NULL,
    status     varchar(30) NOT NULL
);

CREATE TABLE pedidoitens (
    id            serial PRIMARY KEY,
    pedidoid      int NOT NULL REFERENCES pedidos(id) ON DELETE CASCADE,
    jogoid        int NOT NULL REFERENCES jogos(id),
    qtde          int NOT NULL,
    valorunitario numeric(10,2) NOT NULL
);

-- VIEWS (usadas nas listagens com join)

CREATE VIEW vwjogos AS
    SELECT j.*, c.nome AS nomecategoria, p.nome AS nomeplataforma
      FROM jogos j
     INNER JOIN categorias c ON c.id = j.categoriaid
     INNER JOIN plataformas p ON p.id = j.plataformaid;

CREATE VIEW vwpedidos AS
    SELECT p.*, c.nome AS nomecliente,
           (SELECT COALESCE(SUM(i.qtde), 0)::int FROM pedidoitens i WHERE i.pedidoid = p.id) AS qtdeitens
      FROM pedidos p
     INNER JOIN clientes c ON c.id = p.clienteid;

CREATE VIEW vwpedidoitens AS
    SELECT i.*, j.nome AS nomejogo, c.nome AS nomecategoria, pl.nome AS nomeplataforma,
           j.categoriaid, j.plataformaid, cl.nome AS nomecliente, p.data AS datapedido, p.status
      FROM pedidoitens i
     INNER JOIN pedidos p ON p.id = i.pedidoid
     INNER JOIN jogos j ON j.id = i.jogoid
     INNER JOIN categorias c ON c.id = j.categoriaid
     INNER JOIN plataformas pl ON pl.id = j.plataformaid
     INNER JOIN clientes cl ON cl.id = p.clienteid;

-- PROCEDURES GENÉRICAS (recebem o nome da tabela)

CREATE OR REPLACE FUNCTION spDelete(id int, tabela text) RETURNS void AS $$
BEGIN
    EXECUTE format('DELETE FROM %I WHERE id = $1', lower(tabela)) USING id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION spProximoId(tabela text) RETURNS int AS $$
DECLARE
    maior int;
BEGIN
    EXECUTE format('SELECT COALESCE(MAX(id) + 1, 1) FROM %I', lower(tabela)) INTO maior;
    RETURN maior;
END;
$$ LANGUAGE plpgsql;

-- CATEGORIAS

CREATE OR REPLACE FUNCTION spInsert_Categorias(id int, nome text) RETURNS void AS $$
    INSERT INTO categorias (id, nome) VALUES ($1, $2);
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spUpdate_Categorias(id int, nome text) RETURNS void AS $$
    UPDATE categorias SET nome = $2 WHERE categorias.id = $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spConsulta_Categorias(id int) RETURNS SETOF categorias AS $$
    SELECT * FROM categorias WHERE categorias.id = $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spListagem_Categorias() RETURNS SETOF categorias AS $$
    SELECT * FROM categorias ORDER BY nome;
$$ LANGUAGE sql;

-- PLATAFORMAS

CREATE OR REPLACE FUNCTION spInsert_Plataformas(id int, nome text, fabricante text) RETURNS void AS $$
    INSERT INTO plataformas (id, nome, fabricante) VALUES ($1, $2, $3);
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spUpdate_Plataformas(id int, nome text, fabricante text) RETURNS void AS $$
    UPDATE plataformas SET nome = $2, fabricante = $3 WHERE plataformas.id = $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spConsulta_Plataformas(id int) RETURNS SETOF plataformas AS $$
    SELECT * FROM plataformas WHERE plataformas.id = $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spListagem_Plataformas() RETURNS SETOF plataformas AS $$
    SELECT * FROM plataformas ORDER BY nome;
$$ LANGUAGE sql;

-- JOGOS

CREATE OR REPLACE FUNCTION spInsert_Jogos(id int, nome text, descricao text, preco numeric, estoque int,
    datalancamento timestamp, categoriaid int, plataformaid int, imagem bytea) RETURNS void AS $$
    INSERT INTO jogos (id, nome, descricao, preco, estoque, datalancamento, categoriaid, plataformaid, imagem)
    VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9);
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spUpdate_Jogos(id int, nome text, descricao text, preco numeric, estoque int,
    datalancamento timestamp, categoriaid int, plataformaid int, imagem bytea) RETURNS void AS $$
    UPDATE jogos SET nome = $2, descricao = $3, preco = $4, estoque = $5, datalancamento = $6,
           categoriaid = $7, plataformaid = $8, imagem = $9
     WHERE jogos.id = $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spConsulta_Jogos(id int) RETURNS SETOF jogos AS $$
    SELECT * FROM jogos WHERE jogos.id = $1;
$$ LANGUAGE sql;

-- listagem com join de categoria e plataforma
CREATE OR REPLACE FUNCTION spListagem_Jogos() RETURNS SETOF vwjogos AS $$
    SELECT * FROM vwjogos ORDER BY nome;
$$ LANGUAGE sql;

-- vitrine da área do cliente (filtros: nome, categoria e plataforma; 0 = todas)
CREATE OR REPLACE FUNCTION spListagemVitrine(nome text, categoriaid int, plataformaid int) RETURNS SETOF vwjogos AS $$
    SELECT * FROM vwjogos v
     WHERE v.nome ILIKE '%' || $1 || '%'
       AND ($2 = 0 OR v.categoriaid = $2)
       AND ($3 = 0 OR v.plataformaid = $3)
     ORDER BY v.nome;
$$ LANGUAGE sql;

-- CLIENTES

CREATE OR REPLACE FUNCTION spInsert_Clientes(id int, nome text, email text, senha text, cpf text, telefone text,
    cep text, logradouro text, numero text, bairro text, cidade text, uf text) RETURNS void AS $$
    INSERT INTO clientes (id, nome, email, senha, cpf, telefone, cep, logradouro, numero, bairro, cidade, uf)
    VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12);
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spUpdate_Clientes(id int, nome text, email text, senha text, cpf text, telefone text,
    cep text, logradouro text, numero text, bairro text, cidade text, uf text) RETURNS void AS $$
    UPDATE clientes SET nome = $2, email = $3, senha = $4, cpf = $5, telefone = $6, cep = $7,
           logradouro = $8, numero = $9, bairro = $10, cidade = $11, uf = $12
     WHERE clientes.id = $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spConsulta_Clientes(id int) RETURNS SETOF clientes AS $$
    SELECT * FROM clientes WHERE clientes.id = $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spListagem_Clientes() RETURNS SETOF clientes AS $$
    SELECT * FROM clientes ORDER BY nome;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spConsultaClientePorEmail(email text) RETURNS SETOF clientes AS $$
    SELECT * FROM clientes WHERE lower(clientes.email) = lower($1);
$$ LANGUAGE sql;

-- ADMINISTRADORES (login da área administrativa)

CREATE OR REPLACE FUNCTION spLoginAdministrador(usuario text, senha text) RETURNS TABLE (nome varchar) AS $$
    SELECT a.nome FROM administradores a WHERE a.usuario = $1 AND a.senha = $2;
$$ LANGUAGE sql;

-- PEDIDOS (mestre)

-- devolve o id gerado (equivalente ao @@IDENTITY da apostila)
CREATE OR REPLACE FUNCTION spInsert_Pedidos(id int, data timestamp, clienteid int, valortotal numeric, status text)
RETURNS int AS $$
    INSERT INTO pedidos (data, clienteid, valortotal, status) VALUES ($2, $3, $4, $5) RETURNING pedidos.id;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spAlteraStatusPedido(id int, status text) RETURNS void AS $$
    UPDATE pedidos SET status = $2 WHERE pedidos.id = $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spConsulta_Pedidos(id int) RETURNS SETOF vwpedidos AS $$
    SELECT * FROM vwpedidos v WHERE v.id = $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spListagem_Pedidos() RETURNS SETOF vwpedidos AS $$
    SELECT * FROM vwpedidos ORDER BY data DESC;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spListagemPedidosCliente(clienteid int) RETURNS SETOF vwpedidos AS $$
    SELECT * FROM vwpedidos v WHERE v.clienteid = $1 ORDER BY v.data DESC;
$$ LANGUAGE sql;

-- PEDIDO ITENS (detalhe) - também baixa o estoque do jogo
CREATE OR REPLACE FUNCTION spInsert_PedidoItens(id int, pedidoid int, jogoid int, qtde int, valorunitario numeric)
RETURNS int AS $$
    UPDATE jogos SET estoque = estoque - $4 WHERE jogos.id = $3;
    INSERT INTO pedidoitens (pedidoid, jogoid, qtde, valorunitario) VALUES ($2, $3, $4, $5) RETURNING pedidoitens.id;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spListagemItensPedido(pedidoid int) RETURNS SETOF vwpedidoitens AS $$
    SELECT * FROM vwpedidoitens v WHERE v.pedidoid = $1 ORDER BY v.id;
$$ LANGUAGE sql;

-- CONSULTAS AVANÇADAS

-- consulta 1: pedidos (filtros: nome do cliente, status, período)
CREATE OR REPLACE FUNCTION spConsultaAvancadaPedidos(nomecliente text, status text, datainicial timestamp, datafinal timestamp)
RETURNS SETOF vwpedidos AS $$
    SELECT * FROM vwpedidos v
     WHERE v.nomecliente ILIKE '%' || $1 || '%'
       AND ($2 = '' OR v.status = $2)
       AND v.data BETWEEN $3 AND $4
     ORDER BY v.data DESC;
$$ LANGUAGE sql;

-- consulta 2: jogos vendidos (filtros: nome do jogo, categoria, plataforma, período)
CREATE OR REPLACE FUNCTION spConsultaAvancadaVendas(nomejogo text, categoriaid int, plataformaid int, datainicial timestamp, datafinal timestamp)
RETURNS SETOF vwpedidoitens AS $$
    SELECT * FROM vwpedidoitens v
     WHERE v.nomejogo ILIKE '%' || $1 || '%'
       AND ($2 = 0 OR v.categoriaid = $2)
       AND ($3 = 0 OR v.plataformaid = $3)
       AND v.datapedido BETWEEN $4 AND $5
     ORDER BY v.datapedido DESC, v.nomejogo;
$$ LANGUAGE sql;

-- APIs (dados consolidados, pedidos cancelados não entram)

CREATE OR REPLACE FUNCTION spVendasPorCategoria(datainicial timestamp, datafinal timestamp)
RETURNS TABLE (categoria varchar, qtdevendida int, valortotal numeric) AS $$
    SELECT c.nome, SUM(i.qtde)::int, SUM(i.qtde * i.valorunitario)
      FROM pedidoitens i
     INNER JOIN pedidos p ON p.id = i.pedidoid
     INNER JOIN jogos j ON j.id = i.jogoid
     INNER JOIN categorias c ON c.id = j.categoriaid
     WHERE p.status <> 'Cancelado'
       AND p.data BETWEEN $1 AND $2
     GROUP BY c.nome
     ORDER BY 3 DESC;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spJogosMaisVendidos(top int)
RETURNS TABLE (jogoid int, jogo varchar, plataforma varchar, qtdevendida int, valortotal numeric) AS $$
    SELECT j.id, j.nome, pl.nome, SUM(i.qtde)::int, SUM(i.qtde * i.valorunitario)
      FROM pedidoitens i
     INNER JOIN pedidos p ON p.id = i.pedidoid
     INNER JOIN jogos j ON j.id = i.jogoid
     INNER JOIN plataformas pl ON pl.id = j.plataformaid
     WHERE p.status <> 'Cancelado'
     GROUP BY j.id, j.nome, pl.nome
     ORDER BY 4 DESC, 5 DESC
     LIMIT $1;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION spResumoVendas()
RETURNS TABLE (totalpedidos int, faturamento numeric, ticketmedio numeric, totalclientes int, totaljogos int) AS $$
    SELECT (SELECT COUNT(*)::int FROM pedidos WHERE status <> 'Cancelado'),
           (SELECT COALESCE(SUM(valortotal), 0) FROM pedidos WHERE status <> 'Cancelado'),
           (SELECT COALESCE(ROUND(AVG(valortotal), 2), 0) FROM pedidos WHERE status <> 'Cancelado'),
           (SELECT COUNT(*)::int FROM clientes),
           (SELECT COUNT(*)::int FROM jogos);
$$ LANGUAGE sql;

-- DADOS INICIAIS

-- senha dos usuários de teste: 1234 (gravada em SHA-256)
INSERT INTO administradores (usuario, senha, nome)
VALUES ('admin', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4', 'Administrador');

INSERT INTO categorias (id, nome) VALUES
(1, 'Ação'), (2, 'RPG'), (3, 'Corrida'), (4, 'Aventura'), (5, 'Tiro'), (6, 'Esporte');

INSERT INTO plataformas (id, nome, fabricante) VALUES
(1, 'PC', 'Diversos'), (2, 'PlayStation 5', 'Sony'), (3, 'Xbox Series X|S', 'Microsoft'), (4, 'Nintendo Switch', 'Nintendo');

INSERT INTO jogos (id, nome, descricao, preco, estoque, datalancamento, categoriaid, plataformaid) VALUES
(1, 'Elden Ring', 'RPG de ação em mundo aberto.', 229.90, 15, '2022-02-25', 2, 2),
(2, 'Forza Horizon 5', 'Corrida em mundo aberto pelo México.', 249.00, 10, '2021-11-09', 3, 3),
(3, 'The Legend of Zelda: Tears of the Kingdom', 'Aventura em Hyrule.', 349.00, 8, '2023-05-12', 4, 4),
(4, 'Counter-Strike 2', 'Tiro tático competitivo.', 0.00, 99, '2023-09-27', 5, 1),
(5, 'EA Sports FC 25', 'Futebol com clubes licenciados.', 299.90, 20, '2024-09-27', 6, 2),
(6, 'God of War Ragnarök', 'Ação e aventura nórdica.', 249.90, 12, '2022-11-09', 1, 2),
(7, 'Baldur''s Gate 3', 'RPG baseado em D&D.', 199.99, 7, '2023-08-03', 2, 1),
(8, 'Mario Kart 8 Deluxe', 'Corrida para toda a família.', 299.00, 18, '2017-04-28', 3, 4);

INSERT INTO clientes (id, nome, email, senha, cpf, telefone, cep, logradouro, numero, bairro, cidade, uf) VALUES
(1, 'Cliente Teste', 'cliente@teste.com', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4',
 '123.456.789-00', '(11) 99999-0000', '09850-901', 'Av. Humberto de Alencar Castelo Branco', '3972', 'Assunção', 'São Bernardo do Campo', 'SP'),
(2, 'Maria Gamer', 'maria@teste.com', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4',
 '987.654.321-00', '(11) 98888-1111', '01001-000', 'Praça da Sé', '100', 'Sé', 'São Paulo', 'SP');

-- alguns pedidos para as consultas e APIs terem dados
INSERT INTO pedidos (data, clienteid, valortotal, status) VALUES
('2026-10-05 14:30', 1, 479.80, 'Entregue'),
('2026-10-20 10:15', 2, 548.90, 'Enviado'),
('2026-11-10 19:45', 1, 299.00, 'Pago'),
('2026-11-15 21:00', 2, 199.99, 'Cancelado');

INSERT INTO pedidoitens (pedidoid, jogoid, qtde, valorunitario) VALUES
(1, 1, 1, 229.90), (1, 6, 1, 249.90),
(2, 5, 1, 299.90), (2, 2, 1, 249.00),
(3, 8, 1, 299.00),
(4, 7, 1, 199.99);

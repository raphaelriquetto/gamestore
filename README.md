# GameStore - E-commerce de Games (ASP.NET Core MVC 3.1)

## Como rodar em casa (PostgreSQL)
1. No pgAdmin (ou psql) crie o banco: `CREATE DATABASE gamestore;`
2. Conectado no banco `gamestore`, execute `Scripts/script_postgres.sql`
3. Em `GameStoreMVC/DAO/ConexaoBD.cs` ajuste a senha do postgres na string de conexão
4. Abra `GameStore.sln` no Visual Studio e rode (F5)

## Dia da apresentação (SQL Server)
1. Execute `Scripts/script_sqlserver.sql` no SQL Server Management Studio
2. Em `GameStoreMVC/DAO/ConexaoBD.cs` comente as 2 linhas do Postgres e descomente as 2 do SQL Server
   (e também o `using System.Data.SqlClient;` no topo do arquivo)

## Usuários de teste (senha 1234)
- Admin: usuário `admin` -> `/login`
- Cliente: `cliente@teste.com` -> `/conta/login`

## Endereços úteis
- Loja (home do cliente): `/`
- Área administrativa (home do admin): `/admin`
- Swagger das APIs: `/swagger`
- APIs (Postman): `GET /api/relatorios/vendas-por-categoria?dataInicial=2026-01-01&dataFinal=2026-12-31`,
  `GET /api/relatorios/jogos-mais-vendidos?top=5`, `GET /api/relatorios/resumo`

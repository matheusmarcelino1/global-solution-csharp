# OZClima — Monitoramento Climático Urbano

**Global Solution 2026.2 — FIAP**

## 1. Sobre o projeto

O **OZClima** é uma solução CitizenTech desenvolvida para apoiar o monitoramento das condições térmicas urbanas no município de Osasco, São Paulo.

A plataforma permite cadastrar áreas monitoradas, registrar leituras de temperatura e umidade relativa, calcular o índice Humidex, classificar condições térmicas e gerenciar alertas e pontos de apoio.

O objetivo é disponibilizar informações ambientais organizadas que possam apoiar análises e iniciativas relacionadas ao conforto térmico urbano.

> O OZClima é um protótipo acadêmico. Suas classificações não substituem alertas meteorológicos ou orientações oficiais das autoridades públicas.

## 2. Integrantes

| Integrante | RM |
|---|---|
| Douglas dos Santos Melo | RM556439 |
| Henrique Borges de Castro Sanches | RM557959 |
| João Pedro Kraide Máximo | RM563166 |
| Matheus Marcelino Dantas da Silva | RM556332 |
| Nicolas Caciolato Reis | RM556506 |

**Instituição:** FIAP  
**Curso:** Engenharia de Software  
**Turma:** 3ESPY
**Disciplina:** Desenvolvimento com C#

## 3. Tecnologias utilizadas

- C# e ASP.NET Core (.NET 10)
- Entity Framework Core
- SQLite
- LINQ
- Async/Await
- Swagger / OpenAPI
- xUnit e Moq
- Data Annotations
- Injeção de Dependência
- Tratamento global de exceções com `IExceptionHandler`

## 4. Funcionalidades

### Áreas monitoradas
Cadastro, consulta, atualização e desativação lógica de regiões urbanas, incluindo identificação, bairro, coordenadas geográficas e nível de arborização.

### Leituras climáticas
Registro de temperatura e umidade relativa, consulta ao histórico e geração de resumos climáticos.

### Classificação térmica
Cálculo do índice Humidex a partir de temperatura e umidade relativa, com classificação interna em quatro níveis: Baixo, Moderado, Alto e Crítico.

### Alertas de calor
Criação automática de alertas quando uma leitura apresenta nível Alto ou Crítico. O sistema também acompanha alterações de classificação e preserva o histórico dos alertas encerrados.

### Pontos de apoio
Cadastro e consulta de pontos de hidratação, áreas verdes, abrigos climáticos e unidades de saúde associados às áreas monitoradas.

## 5. Arquitetura

O projeto está organizado em:

- **Controllers:** exposição dos endpoints HTTP.
- **DTOs:** contratos de entrada e saída de dados.
- **Models:** entidades e regras de domínio.
- **Interfaces:** contratos dos serviços.
- **Services:** processamento das regras de negócio.
- **Data:** configuração do Entity Framework Core.
- **Exceptions:** tratamento centralizado de exceções.
- **OzClima.Tests:** testes automatizados das entidades, regras e persistência.

O projeto utiliza injeção de dependência para reduzir o acoplamento entre os componentes.

## 6. Como executar

### Pré-requisitos

- .NET SDK 10
- Visual Studio 2026 ou IDE compatível
- Ferramenta `dotnet-ef` compatível com o EF Core utilizado

### Instalação

Clone o repositório e acesse a pasta da solução:

```bash
git clone <URL_DO_REPOSITORIO>
cd OzClima
```

Restaure as dependências:

```bash
dotnet restore
```

Aplique as migrations:

```bash
cd OzClima.Api
dotnet ef database update
```

Execute a API:

```bash
dotnet run
```

Abra o endereço `/swagger` na URL local exibida pelo terminal, em ambiente de desenvolvimento.

## 7. Principais endpoints

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/areas` | Cadastrar área |
| GET | `/api/areas` | Listar áreas |
| GET | `/api/areas/{id}` | Consultar área |
| PUT | `/api/areas/{id}` | Atualizar área |
| DELETE | `/api/areas/{id}` | Desativar área |
| POST | `/api/leituras` | Registrar medição |
| GET | `/api/leituras` | Consultar leituras |
| GET | `/api/alertas` | Consultar histórico de alertas |
| GET | `/api/alertas/ativos` | Consultar alertas ativos |
| POST | `/api/pontos-apoio` | Cadastrar ponto de apoio |
| GET | `/api/pontos-apoio` | Listar pontos de apoio |

Os demais endpoints estão documentados no Swagger.

## 8. Classificação térmica

O OZClima utiliza uma aproximação do índice meteorológico Humidex, calculada a partir da temperatura do ar e da umidade relativa.

O índice considera a pressão de vapor estimada e é utilizado para classificar condições térmicas conforme as seguintes faixas:

| Humidex arredondado | Classificação OZClima |
|---|---|
| Abaixo de 30 | Baixo |
| 30 a 39 | Moderado |
| 40 a 45 | Alto |
| 46 ou mais | Crítico |

As nomenclaturas são internas ao projeto e representam uma adaptação acadêmica das faixas de interpretação do Humidex.

**Referências:**
- Environment and Climate Change Canada — Humidex.
- Environment and Climate Change Canada — interpretação do índice Humidex.

## 9. Regras de alertas

- Leituras classificadas como Alto ou Crítico podem iniciar alertas.
- Uma elevação da classificação provoca atualização imediata do alerta.
- Reduções exigem duas leituras consecutivas abaixo do nível do alerta ativo.
- Alertas encerrados permanecem no histórico.
- O processamento da leitura e a atualização dos alertas utilizam uma transação de banco de dados.

A confirmação por duas leituras é uma simplificação acadêmica; não considera, por si só, intervalo temporal, calibração dos sensores ou validação por fontes oficiais.

## 10. Testes automatizados

A solução utiliza xUnit para testes e Moq para simulação de dependências.

Os testes cobrem:

- Validações e regras das entidades.
- Classificação de risco térmico e limites do Humidex.
- Persistência e consultas com SQLite em memória.
- Criação, atualização e desativação lógica de registros.
- Geração e encerramento automático de alertas.
- Comportamento de transações em cenários de falha.

Para executar os testes, utilize na raiz da solução:

```bash
dotnet test
```

**Resultado da última execução local informada:** 75 testes aprovados.

## 11. Limitações do protótipo

- O sistema não representa um serviço meteorológico oficial.
- O Humidex não mede diretamente as ilhas de calor urbano.
- O algoritmo não considera radiação solar ou velocidade do vento.
- Os dados demonstrativos podem ser simulados e não devem ser confundidos com medições reais.
- O protótipo não implementa autenticação para publicação pública.
- O controle de duplicidade de alertas não fornece garantia de unicidade sob concorrência.
- Alertas históricos anteriores à alteração do algoritmo podem refletir classificações antigas.

## 12. Objetivos de Desenvolvimento Sustentável

**ODS 11 — Cidades e Comunidades Sustentáveis**

O OZClima busca contribuir para o acompanhamento de condições térmicas urbanas e disponibilização de informações relevantes para o planejamento e a conscientização ambiental.

**ODS 13 — Ação Contra a Mudança Global do Clima**

O projeto busca apoiar a conscientização e o monitoramento de condições relacionadas ao calor urbano.

## 13. Natureza acadêmica

Projeto desenvolvido como parte da Global Solution da FIAP, com finalidade educacional e demonstrativa.

---

## 14. Briefing — Global Solution 2026.2

**Status:** Pendente de preenchimento pelo responsável do grupo.

### Documento do briefing

[Acesse o briefing do OZClima](docs/briefing.pdf)

> O arquivo será anexado após a finalização do briefing.

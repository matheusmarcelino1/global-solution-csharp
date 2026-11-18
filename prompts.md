# Registro de utilização de IA — OZClima

**Global Solution 2026.2 — FIAP**

## Ferramenta utilizada
ChatGPT — OpenAI.

## Objetivo
Utilizar inteligência artificial como apoio ao planejamento, implementação, compreensão e revisão técnica da API OZClima, desenvolvida em C# e ASP.NET Core.

## 1. Planejamento e arquitetura
A IA auxiliou na organização das entidades, interfaces, serviços, DTOs e Controllers, considerando os requisitos da disciplina.

## 2. Implementação da API
A IA foi utilizada como apoio na implementação dos endpoints REST, configuração do Entity Framework Core, persistência SQLite e injeção de dependência.

## 3. Testes automatizados
A IA auxiliou na elaboração e revisão de testes com xUnit, Moq e SQLite em memória, incluindo testes de regras de negócio, consultas e transações.

## 4. Classificação térmica
A IA apoiou a análise e implementação do cálculo do índice Humidex, incluindo sua utilização nas classificações de risco térmico.

## 5. Revisão de qualidade
A IA foi utilizada para revisar validações dos DTOs, tratamento global de exceções e comportamento dos alertas automáticos.

## 6. Validação
As sugestões foram implementadas e verificadas durante o desenvolvimento, com testes automatizados e verificações manuais pelo Swagger.

**Observação:** este documento apresenta um resumo das atividades realizadas com apoio de IA. Caso seja necessário registrar os prompts exatos, eles deverão ser transcritos do histórico original das conversas.


---

## 7. Exemplos de prompts utilizados

Os exemplos abaixo foram adaptados das solicitações realizadas
durante o desenvolvimento do OZClima. Não representam
necessariamente transcrições literais das mensagens originais.

### Prompt 1 — Arquitetura da API
"Me ajude a estruturar uma API REST em C# com .NET 10
para monitoramento climático urbano, utilizando
Programação Orientada a Objetos, interfaces,
injeção de dependência e Entity Framework Core."

### Prompt 2 — Persistência
"Como configurar o Entity Framework Core com SQLite,
criar o DbContext, definir os relacionamentos entre
as entidades e executar as migrations?"

### Prompt 3 — Regras de negócio
"Como implementar um serviço responsável por classificar
as condições térmicas a partir dos dados coletados
e gerar alertas automaticamente?"

### Prompt 4 — Testes automatizados
"Me ajude a criar testes automatizados com xUnit,
Moq e SQLite em memória para validar os Services,
as regras de negócio e o rollback de transações."

### Prompt 5 — Humidex
"Como implementar o cálculo do índice Humidex em C#,
considerando temperatura e umidade relativa,
e criar testes para validar as classificações?"

### Prompt 6 — Qualidade de código
"Vamos revisar a API OZClima, adicionando validações
nos DTOs, tratamento global de exceções e testes
para garantir que as alterações não quebrem
as funcionalidades existentes."

### Prompt 7 — Documentação
"Vamos auditar o projeto considerando os critérios
da Global Solution da FIAP e preparar o README,
as evidências de execução e a publicação no GitHub."

## 8. Considerações finais

A inteligência artificial foi utilizada como ferramenta
de apoio técnico e educacional.

As implementações foram acompanhadas de verificações
no Visual Studio, testes automatizados e validações
dos endpoints pelo Swagger.

O projeto foi concluído com 75 testes automatizados
aprovados.

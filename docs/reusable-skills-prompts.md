# Prompts Para Usar Skills Reutilizaveis

Use estes exemplos em outros projetos novos ou existentes. Ajuste caminhos, stack e objetivo conforme o contexto.

## project-context-bootstrap

```text
Use $project-context-bootstrap neste repositório antes de qualquer alteração.

Objetivo:
- mapear a estrutura do projeto;
- identificar comandos de build, teste e execução;
- localizar instruções locais como AGENTS.md, README, docs e manifests;
- separar fatos, inferências e dúvidas bloqueadoras;
- sugerir o próximo passo seguro.

Não altere arquivos ainda.
Responda em PT-BR.
```

## engineering-review-plan

```text
Use $engineering-review-plan para revisar a mudança que pretendo fazer.

Contexto:
- quero implementar <descreva a mudança>;
- antes de codar, avalie impacto, riscos, alternativas e o menor plano seguro.

Formato desejado:
- Problema;
- Impacto;
- Correção mínima;
- Prioridade;
- Plano incremental;
- Validação.

Não implemente ainda.
Responda em PT-BR.
```

## resilience-audit

```text
Use $resilience-audit para avaliar este projeto.

Procure pontos problemáticos relacionados a:
- chamadas externas;
- banco de dados;
- filas/jobs;
- cache;
- concorrência;
- timeout/cancelamento;
- rate limiting;
- health checks;
- logs e tratamento de erro.

Para cada ponto, informe:
- problema;
- impacto;
- correção mínima;
- quando não aplicar para evitar overengineering;
- prioridade.

Não altere código ainda.
Responda em PT-BR.
```

## mutation-testing-adoption

```text
Use $mutation-testing-adoption para planejar a inclusão de testes de mutação neste projeto.

Primeiro:
- identifique a stack e ferramenta adequada;
- escolha o primeiro alvo de menor ruído e maior valor;
- proponha configuração inicial;
- defina comandos para rodar localmente;
- explique como documentar o baseline;
- não configure gate de CI na primeira etapa.

Não implemente ainda; gere um plano incremental.
Responda em PT-BR.
```

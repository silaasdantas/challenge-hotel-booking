# Mutation Testing

Este projeto usa Stryker.NET como ferramenta local para testes de mutacao.

## Restaurar ferramenta

```powershell
dotnet tool restore
```

## Rodar baseline de testes

```powershell
rtk dotnet test Hotel.Booking.sln
```

## Rodar mutation testing

Execute a partir da raiz do repositorio:

```powershell
dotnet tool run dotnet-stryker -- --config-file tests/Hotel.Booking.UnitTest/stryker-config.json
```

O primeiro alvo e `Hotel.Booking.Core`, porque concentra regras de dominio e reduz ruido inicial.

## Baseline atual

Primeira execucao validada em 2026-07-14:

- Mutation score: `90.07%`
- Mutantes criados: `192`
- Mutantes testados: `147`
- Killed: `136`
- Survived: `11`
- Timeout: `0`
- Errors: `0`
- No coverage: `4`

## Relatorio

O Stryker gera relatorio HTML em `StrykerOutput`. Abra o arquivo `mutation-report.html` da execucao mais recente para analisar sobreviventes.

## Regra de trabalho

Mutantes sobreviventes devem virar testes novos ou receber justificativa explicita quando forem equivalentes ou sem valor de negocio. O primeiro baseline nao quebra CI porque `thresholds.break` esta em `0`.

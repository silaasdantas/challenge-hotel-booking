---
name: mutation-testing-adoption
description: Introduce mutation testing incrementally and safely. Use when the user asks to add, configure, run, interpret, or document mutation testing, mutation score, surviving mutants, test quality gates, Stryker.NET, StrykerJS, PIT/Pitest, or equivalent tooling across .NET, Java, Node, frontend, or mixed-stack projects.
---

# Mutation Testing Adoption

Use this skill to add mutation testing without creating noisy gates or tool churn.

## Workflow

1. Inspect current test structure, tooling, manifests, and CI before choosing a tool.
2. Pick the first mutation target with high business value and low noise:
   - domain/core/use cases before controllers, generated code, DTOs, adapters, or framework glue;
   - one module first, not the whole repository.
3. Prefer local/versioned tooling:
   - .NET: `dotnet-stryker` local tool.
   - Java: PIT/Pitest via Maven/Gradle config.
   - Node/TypeScript: StrykerJS as dev dependency.
4. Create config with conservative defaults:
   - generate HTML plus terminal report;
   - exclude generated/trivial files;
   - start with no CI-breaking threshold.
5. Run normal tests first, then mutation testing.
6. Record baseline score and counts: killed, survived, timeout, errors, no coverage.
7. Treat surviving mutants as work items:
   - add meaningful tests for real gaps;
   - ignore only equivalent/no-value mutants with explicit justification.
8. Raise `break` threshold only after the first baseline is understood.

## Output Format

```text
Plano:
- alvo inicial: <module/project>
- ferramenta: <tool and install mode>
- config: <path and key settings>
- comando: <exact command>

Baseline:
- score:
- killed:
- survived:
- timeout:
- no coverage:

Proximos passos:
- <tests to add or mutants to review>
```

## Stack Defaults

- .NET: use `stryker-config.json`, mutate one project under test, run from a stable root or test project path.
- Java: configure PIT/Pitest for one module first; avoid mutating generated code, DTO-only packages, and framework bootstrap.
- Node/TypeScript: configure StrykerJS for source modules with existing unit tests; avoid broad mutation of build config and generated clients.

## Guardrails

- Do not make mutation score fail CI on day one.
- Do not lower target framework/runtime just to satisfy a tool without explicit approval.
- Do not add production dependencies for mutation testing.
- Do not treat high line coverage as equivalent to mutation quality.

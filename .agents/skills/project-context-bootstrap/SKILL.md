---
name: project-context-bootstrap
description: Map repository context before coding. Use when entering a new repo, resuming work after context loss, preparing to modify unfamiliar code, or when the user asks to read project instructions, understand structure, identify commands, inspect conventions, or bootstrap implementation context across .NET, Java, Node, frontend, infra, or mixed-stack projects.
---

# Project Context Bootstrap

Use this skill to build a compact, evidence-based project map before making changes.

## Workflow

1. Read local agent instructions first: `AGENTS.md`, `CLAUDE.md`, `.codex/`, `.agents/`, or equivalent repo guidance.
2. Inspect manifests and entrypoints before source details:
   - .NET: `*.sln`, `*.csproj`, `Program.cs`, app settings.
   - Java: `pom.xml`, `build.gradle`, `settings.gradle`, application entrypoints.
   - Node: `package.json`, lockfiles, framework config, scripts.
   - Frontend: router/app entrypoints, build config, component conventions.
3. Identify project shape: layers/modules, runtime, test projects, generated folders, docs, local tooling, and likely ownership boundaries.
4. Search only relevant files first. Avoid broad scans of build outputs, dependencies, and generated artifacts.
5. Separate findings into:
   - Fatos: directly observed in files or command output.
   - Inferencias: likely conclusions from structure or naming.
   - Duvidas: missing facts that could change implementation.
6. Detect validation commands and prefer repo-native commands over generic guesses.
7. Before any code change, state the smallest safe next step and any blocker.

## Output Shape

Use concise Portuguese by default:

```text
Resumo:
- <project shape and current task-relevant context>

Fatos:
- <observed fact with path when useful>

Inferencias:
- <reasonable hypothesis and why>

Duvidas bloqueadoras:
- <only if needed>

Comandos detectados:
- <build/test/run commands>

Proximo passo seguro:
- <smallest action>
```

## Guardrails

- Do not invent architecture, business rules, commands, or dependencies.
- Do not use internet instead of reading the repo.
- Do not change files during bootstrap unless the user explicitly asks for implementation.
- If instructions conflict, follow the closest repo/user instruction and surface the conflict.

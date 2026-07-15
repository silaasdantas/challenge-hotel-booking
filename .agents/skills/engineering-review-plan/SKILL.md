---
name: engineering-review-plan
description: Review code, architecture, or proposed changes and produce an actionable implementation plan. Use when the user asks for review, analysis before coding, risk assessment, technical decision support, or a plan in problem/impact/minimal-fix format across .NET, Java, Node, frontend, infrastructure, or mixed-stack projects.
---

# Engineering Review Plan

Use this skill to turn project evidence into a practical, minimal-risk plan.

## Workflow

1. Read task-relevant instructions and files before judging.
2. Identify the user goal and distinguish it from possible implementation ideas.
3. Inspect existing patterns, contracts, tests, and ownership boundaries.
4. List only problems that have concrete impact. Do not report style preferences as risks unless they affect maintainability, correctness, security, performance, or operability.
5. Prefer incremental and reversible fixes.
6. Avoid new dependencies, architecture changes, public contract changes, migrations, or infrastructure changes unless the evidence justifies them.
7. If a missing fact could change architecture, public API, data model, security, or cost, ask before planning implementation.

## Review Format

Use this compact format for each finding:

```text
N. Problema: <short title>
Impacto: <concrete effect, with file/line references when useful>
Correcao minima: <smallest practical fix>
Prioridade: <alta|media|baixa>
```

Then add:

```text
Plano incremental:
1. <small step>
2. <small step>

Validacao:
- <specific test/build/lint/manual checks>

Riscos restantes:
- <only meaningful residual risks>
```

## Decision Rules

- Recommend one option, but mention serious alternatives when trade-offs matter.
- Treat simplicity, security, resilience, observability, low coupling, high cohesion, and testability as design criteria.
- Keep behavior changes separate from broad refactors unless the refactor is required.
- Make the plan decision-complete: another engineer should not need to choose files, sequence, or validation strategy.

---
name: resilience-audit
description: Audit a project for practical resilience gaps and propose minimal fixes. Use when the user mentions resilience, retry, circuit breaker, timeout, cancellation, abort signal, rate limiting, health checks, idempotency, transient failures, concurrency races, external APIs, databases, queues, caches, or operational robustness in .NET, Java, Node, frontend, infrastructure, or mixed-stack projects.
---

# Resilience Audit

Use this skill to decide where resilience belongs and where it would be overengineering.

## Workflow

1. Map dependency edges:
   - inbound HTTP/API/UI requests;
   - outbound HTTP/gRPC clients;
   - database and transactions;
   - queues, topics, jobs, schedulers;
   - cache, filesystem, cloud services, third-party APIs.
2. Classify failure modes:
   - transient external failure;
   - permanent validation/domain error;
   - timeout or cancellation;
   - concurrency race;
   - overload/backpressure;
   - partial failure or degraded dependency.
3. Check idempotency before recommending retry. Do not retry non-idempotent writes unless idempotency keys, constraints, or safe deduplication exist.
4. Prefer built-in/runtime-native mechanisms first:
   - .NET: cancellation tokens, rate limiter, health checks, HttpClient resilience/Polly when justified.
   - Java: timeouts, Resilience4j/Spring Retry when justified, transaction/constraint controls.
   - Node: AbortController, request timeouts, p-limit/Bottleneck/opossum when justified.
5. Prioritize fixes that reduce real risk with low complexity.

## Output Format

```text
Resumo:
- <main resilience posture>

N. Problema: <risk>
Impacto: <specific failure mode and path/reference>
Correcao minima: <smallest fix>
Quando nao aplicar: <overengineering warning if relevant>
Prioridade: <alta|media|baixa>
```

## Common Recommendations

- Timeout every external call.
- Propagate cancellation/abort signals from request boundary to I/O.
- Add rate limiting/backpressure at expensive or write-heavy endpoints.
- Add health checks for real dependencies, not only process liveness.
- Use database constraints or transactions for consistency; local locks only fit single-process scenarios.
- Use circuit breakers only with monitoring and only for unstable external dependencies.
- Log expected domain failures differently from unexpected infrastructure failures.

## Guardrails

- Do not recommend retry as a default.
- Do not hide failures with silent fallback unless degraded behavior is explicitly acceptable.
- Do not add queues, caches, distributed locks, tracing, or circuit breakers without evidence.
- Do not expose sensitive data in logs or error responses.

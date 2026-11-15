# 0005. Anthropic API (Claude Haiku) for content moderation

- Status: Accepted
- Date: 2026-11-15

## Context
Every post and comment must be checked for music relevance before it goes live. Options: Anthropic API, OpenAI API, AWS Bedrock.

## Decision
Call the Anthropic API (a Haiku model) from the backend, behind an `IModerationService` interface. A fake implementation is used in tests and optionally in local dev.

## Consequences
- Cheap, fast, one API key, works locally without AWS.
- Swappable to Bedrock or another provider via configuration.
- External call on the write path: needs a timeout, retries and a "pending review" fallback if the provider is down.

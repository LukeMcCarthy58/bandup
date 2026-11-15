# 0004. Terraform for infrastructure as code

- Status: Accepted
- Date: 2026-11-15

## Context
Options: Terraform, AWS CDK.

## Decision
Use Terraform, in `infra/`. Written when hosting is chosen (ADR 0002).

## Consequences
- Widely used and transferable; one tool can manage AWS, Vercel and Clerk config.
- HCL rather than a general-purpose language; remote state (S3 + locking) needs setting up.

# FlowCore Specification

## Purpose

FlowCore demonstrates a realistic internal workflow system for public agencies, universities, tribunals, and administrative departments. The MVP focuses on protocol lifecycle management rather than broad CRUD.

## Core Entities

- Department: organizational unit that owns or receives protocol work.
- AppUser: internal user with a role and department.
- Protocol: business case with number, subject, status, owner department, assignee, timestamps, movements, attachments, and audit logs.
- ProtocolMovement: immutable history entry for creation, status changes, and forwarding.
- AttachmentMetadata: file metadata without binary storage.
- AuditLog: record of important actions and actor identity.

## Workflow Statuses

- Draft
- Open
- InAnalysis
- WaitingForDocuments
- Approved
- Rejected
- Closed

## Allowed Transitions

```text
Draft -> Open, Closed
Open -> InAnalysis, WaitingForDocuments, Closed
InAnalysis -> WaitingForDocuments, Approved, Rejected
WaitingForDocuments -> InAnalysis, Closed
Approved -> Closed
Rejected -> Closed
Closed -> none
```

## MVP Scope

- Create protocol records.
- List and inspect protocols.
- Transition protocol status with validation.
- Forward protocols between departments.
- Store attachment metadata.
- Record movement history and audit logs.
- Cover workflow rules with automated tests.

## Out Of Scope For MVP

- Authentication provider integration.
- Binary file storage.
- Full role-based authorization policy.
- Notifications and SLA timers.
- Advanced search.


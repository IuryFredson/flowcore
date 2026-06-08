# API Notes

FlowCore exposes a compact minimal API for the MVP.

## Create Protocol

`POST /protocols`

```json
{
  "subject": "Vehicle registration appeal",
  "description": "Citizen requested administrative review.",
  "createdById": "00000000-0000-0000-0000-000000000001",
  "currentDepartmentId": "00000000-0000-0000-0000-000000000010",
  "assignedUserId": null
}
```

## Transition Status

`POST /protocols/{id}/transitions`

```json
{
  "status": "InAnalysis",
  "actorUserId": "00000000-0000-0000-0000-000000000001",
  "note": "Initial review started."
}
```

## Forward Protocol

`POST /protocols/{id}/forward`

```json
{
  "actorUserId": "00000000-0000-0000-0000-000000000001",
  "destinationDepartmentId": "00000000-0000-0000-0000-000000000020",
  "assignedUserId": null,
  "note": "Forwarded for technical analysis."
}
```


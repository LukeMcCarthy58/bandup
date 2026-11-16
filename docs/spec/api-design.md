# BandUp API design (MVP 1)

The REST API is shared by the web app and, later, the mobile app. The OpenAPI document is generated from the code (`/openapi/v1.json`), and `packages/api-client` is generated from it.

## Conventions

| Topic | Convention |
| --- | --- |
| Base path | `/api/v1` (health endpoints stay at the root) |
| Auth | `Authorization: Bearer <Clerk JWT>`. Every endpoint requires auth unless marked public |
| Format | JSON with camelCase field names. Dates in ISO 8601 UTC |
| Errors | RFC 7807 problem details (`application/problem+json`), with field errors under `errors` |
| Pagination | Cursor-based: `?cursor=…&limit=20`; responses include `nextCursor` (null at the end) |
| Async writes | Moderated content returns `202 Accepted` with `moderationStatus: "Pending"` |
| Rate limits | `429 Too Many Requests` with a `Retry-After` header |
| IDs | UUIDv7 strings |
| Location | Distances are returned rounded ("~8 miles"); coordinates are never returned |

## Endpoints

### Me and profiles
| Method | Path | Notes |
| --- | --- | --- |
| GET | `/me` | Current user; created on first call |
| GET | `/me/profile` | Your profile (empty until set up) |
| PUT | `/me/profile` | Create or update your profile |
| DELETE | `/me` | Delete your account (F12) |
| GET | `/users/{id}` | Public profile |
| POST | `/me/photo/upload-url` | Pre-signed upload URL |

### Reference data (public)
| GET | `/instruments`, `/genres`, `/towns?query=` |
| --- | --- |

### Bands and membership
| Method | Path | Notes |
| --- | --- | --- |
| POST | `/bands` | Creator becomes Admin |
| GET | `/bands/{slug}` | Includes current and former members |
| PATCH | `/bands/{id}` | Band admins only |
| GET | `/me/bands` | Bands I'm in or was in |
| PATCH | `/bands/{id}/members/{userId}` | Change role (band admins only) |
| DELETE | `/bands/{id}/members/{userId}` | Remove a member, or leave if it's me; the membership becomes Former |

### Adverts
| Method | Path | Notes |
| --- | --- | --- |
| GET | `/adverts?townId=&radiusMiles=&type=&instrument=&genre=&cursor=` | Sorted by distance |
| POST | `/adverts` | Band adverts need band admin rights |
| GET | `/adverts/{id}` | |
| PATCH | `/adverts/{id}` | Edit, close or reopen |

### Join requests
| Method | Path | Notes |
| --- | --- | --- |
| POST | `/adverts/{id}/join-requests` | Creates a thread |
| GET | `/me/join-requests` | Mine, with status |
| GET | `/bands/{id}/join-requests?status=Pending` | Band admins only |
| POST | `/join-requests/{id}/accept`, `/decline`, `/withdraw` | Idempotent |

### Messaging
| Method | Path | Notes |
| --- | --- | --- |
| GET | `/threads` | Inbox, with unread counts |
| GET | `/threads/{id}/messages?cursor=` | Participants only |
| POST | `/threads/{id}/messages` | Rate limited |
| POST | `/threads/{id}/read` | Marks the thread as read |
| POST | `/adverts/{id}/reply` | Starts or reuses a thread for a musician advert |
| POST/DELETE | `/blocks/{userId}` | Block or unblock |

### Posts and feed
| Method | Path | Notes |
| --- | --- | --- |
| GET | `/feed?tab=local\|following\|recommended&cursor=` | Approved posts only |
| POST | `/posts` | `postedAsBandId` optional (band admins only) |
| PATCH/DELETE | `/posts/{id}` | Edits are re-moderated |
| POST | `/posts/{id}/images/upload-url` | |
| POST/DELETE | `/posts/{id}/like` | Idempotent |
| GET/POST | `/posts/{id}/comments` | Comments are moderated |
| DELETE | `/comments/{id}` | The author or the post owner |

### Follows
| POST/DELETE | `/users/{id}/follow`, `/bands/{id}/follow` |
| --- | --- |

### Moderation
| Method | Path | Notes |
| --- | --- | --- |
| POST | `/reports` | Report a post, comment, advert or user |
| GET | `/admin/moderation-queue` | Site admins only |
| POST | `/admin/moderation-queue/{id}/decision` | Approve, remove or suspend; audit-logged |

### Health (public)
| GET | `/health/live`, `/health` |
| --- | --- |

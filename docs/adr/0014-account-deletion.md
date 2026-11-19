# 0014. Account deletion

- Status: Accepted
- Date: 2026-11-19

## Context
People must be able to delete their account and data (UK GDPR right to erasure, and Requirements F12). A BandUp account has two halves: Clerk holds the sign-in (ADR 0001), and our database holds everything else (ADR 0012). Only the web server has Clerk's secret key.

## Decision
- **One "Delete account" action** on `/me`, behind a confirmation step (type `DELETE`). A Server Action does the work, so another site can't trigger it.
- **BandUp's data first, then Clerk.** The action calls `DELETE /api/v1/me`, then deletes the Clerk user with the web server's secret key, then the browser signs out.
  - If the Clerk step fails, the person is told and can try again: the API delete is safe to repeat (`204` even when there's nothing left).
  - If they sign in before trying again, they get a new, empty account (ADR 0012), which they can delete in the same way.
- **The API never holds Clerk's secret key.** It only deletes the account matching the caller's validated token; there's no "delete user X" endpoint.
- **Hard delete.** The `Users` row goes in one statement and the database cascades to rows that belong only to that user (`UserInstruments`, `UserGenres`). Nothing is kept "just in case".
- **Rate limited** with the `sensitive` policy.

## Rule for later features
Every feature that stores data linked to a user must say, in its PR, what happens to that data when the user deletes their account, and implement it in `UserAccounts.DeleteAsync` with a test. Expected answers:
- **Their own content** (posts, comments, likes, adverts they posted as themselves, join requests, follows, blocks): deleted.
- **Messages**: removed from the deleted user's side; the other person keeps the thread with the sender shown as "Deleted user".
- **Band memberships**: removed. A band where they were the only admin either hands admin to the longest-standing current member or, if there are no other members, is deleted.
- **Reports they filed**: kept for moderation but no longer linked to them.
- **Photos** (M8): deleted from S3 as well as the database.

## Consequences
- Simple and complete for MVP 1: nothing personal is left once both steps succeed.
- A rare half-done state (BandUp data gone, Clerk user still there) is recoverable by the person retrying.
- Deleting the user in the Clerk dashboard doesn't remove their BandUp data. That needs a Clerk webhook, which ADR 0012 deferred; until then, admins delete through the app.

# API Documentation
## Basic information
* **Base URL:** ```http://localhost:5000/api```
* **Authentication:** No authentication implemented, but some actions require the ```X-User-Id``` HTTP header on request, for authorization.

## Endpoints
### Get parking slot list
* **Endpoint:** `GET /parkingslots`
* **Description:** Returns a list of parking slots and their corresponding reservations.
* **Response (200 OK):** `[{ id, designation, type, reservations: [] }]`

### Get parking slot by id
* **Endpoint:** `GET /parkingslots/{id}`
* **Description:** Returns a list of parking slots and their corresponding reservations.
* **Response:** `200 OK, { id, designation, type, reservations: [] }` (success), `404 Not Found` (non-existing guid)

#### Create new reservation
* **Endpoint:** `POST /reservations`
* **Payload:** `{ parkingSlotId, userId, startTime, endTime }`
* **Description:** Creates a new reservation. Checks for overlaps, special parking slot constraints and that the reservation's duration is a minimum of 30 minutes.
* **Response:** `201 Created` (success), `400 Bad Request` (invalid time or overlap).

#### Cancel reservation
* **Endpoint:** `DELETE /reservations/{id}`
* **Header:** `X-User-Id: <user_guid>`
* **Description:** Deletes a future reservation. Deletion is only available for the owner user of the reservation.
* **Response:** `204 No Content` (success), `403 Forbidden` (unauthorized), `400 Bad Request` (ongoing reservation).

#### Get user list
* **Endpoint:** `GET /users`
* **Description:** Returns a list of users.
* **Response (200 OK):** `[{ id, name, authorizedSlotTypes: [] }]`

#### Get user list
* **Endpoint:** `GET /users/{userId}/reservations`
* **Description:** Returns the list of reservations of the given user.
* **Response:** `200 OK, [{ id, parkingSlotId, parkingSlotDesignation, userId, startTime, endTime }]` (success), `404 Not Found` (non-existing user)
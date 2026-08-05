# System design
## Architecture
The system follows a basic decoupled client-server architecture:
* **Frontend (Client):** A single page application responsible for the UI, state management, client-side validation and connecting to the API endpoints.
* **Backend (API):** A RESTful WebAPI responsible for enforcing business rules, handling data processing and securing endpoints.
* **Database:** A relational database user for persistent data storage, communicating with the backend via an Object-Relational Mapper. 

## Technology Stack 
* **Backend Framework:** ASP.NET Core WebAPI (.NET 10)
* **Backend Architecture:** Model-Service-Controller
* **Database:** SQLite
* **ORM:** Entity Framework Core
* **Frontend Framework:** React, vite
* **Frontend Language:** TypeScript
* **Frontend State & Data Fetching:** TanStack Query and Axios
* **Routing:** React Router
* **Testing:** xUnit, FluentAssertions, WebApplicationFactory
* **Containerization:** Docker, Docker Compose

## Component Design and Layers
### Backend Layers

The backend is strictly divided into three main layers to enforce the Separation of Concerns:
* **Controllers:**
* * Handle incoming HTTP requests and outgoing HTTP responses.
* * Map internal domain models to Data Transfer Objects to prevent over-posting and data leaks.
* * Delegate commands to the Service layer.
* **Services:**
* * Contain the core business logic and rules.
* * Validate time constraints, check for overlaps and enforce authorization.
* * Operate asynchronously to maintain thread availability.
* **Data, Models:**
* * Manage the Database connection.
* * Domain entities represent the database tables.
* * Seed the database that runs on application startup to ensure the database is populated for testability.

### Frontend Architecture
* **Context API:** A ```UserContext``` simulates an authenticated session globally across the application.
* **Axios:** Handle outgoing API requests and inject the ```X-User-Id``` header.
* **React query:** Handle server state caching, automatic refetching and loading, error states.
* **Client-side validation:** Form inputs are validated in the browser before sending them to the server, reducing unnecessary server calculations and load.

## Database Data Schema
### Entities
* **User**
* * ```Id``` (Guid, Primary key)
* * ```Name``` (String)
* * ```AuthorizedSlotTypes``` (List of SlotType) *stored in the database as a JSON string*
* * **Relationships:** 1-* with Reservations
* **ParkingSlot**
* * ```Id``` (Guid, Primary key)
* * ```Designation``` (String)
* * ```Type``` (SlotType) *determine if the slot needs special authorization*
* * **Relationships:** 1-* with Reservations
* **Reservation**
* * ```Id``` (Guid, Primary key)
* * ```StartTime``` (DateTime, UTC)
* * ```EndTime``` (DateTime, UTC)
* * ```UserId``` (Guid, Foreign key)
* * **Relationships:** Belongs to one User, belongs to one ParkingSlot. Cascade delete.
### Enumerators
* **SlotType:** ```Regular = 0```, ```VIP = 1```, ```Electric = 2```, ```Accessible = 3```

## Core Business logic and Rules
### Time management
* **UTC standardization:** All dates are converted to and stored in UTC. The frontend converts and displays to local timezone.
* **Inclusive-Exclusive boundaries:** Overlaps are calculated via the formula: ```(Existing.StartTime < New.EndTime AND Existing.EndTime > New.StartTime)```. This allows booking from the same time the previous one has just ended.
* **Minimum duration:** Every reservation is required to be at least 30 minutes.
* **Future only:** Reservations cannot be made starting from a past time.

## Special Slot authorization
* Certain parking slots are marked as VIP, Electric, or Accessible.
* Before reservation is saved, the backend verifies if the requesting User has the right to make a reservation for the given slot. If not, ```400 Bad Request``` is returned.

## Cancellation
* **Future only:** Cancellation is only available for reservations where ```StartTime``` is in the future.
* **Ownership:** A user can only cancel their own reservations.

## Security and simulated authentication
* The active user is selected from a dropdown in the UI.
* The frontend automatically attaches ```X-User-Id: <Guid>``` to all mutation HTTP requests.
* The Backend verifies whether the action is allowed for the user. If not, ```403 Forbidden``` is returned.

## Performance and concurrency
* For read-only endpoints (GET), the usage of ```.AsNoTracing()``` method when reading data from the database, the EF Change Tracker is bypassed, reducing memory consumption and improving response times.
* Database indexes improve query times, even for way larger databases.
* Overlap checks and reservation insertions are handled sequentially within a scoped dependency injection. This would be wrapped in a serializable transaction level to prevent race conditions in high-load environments.
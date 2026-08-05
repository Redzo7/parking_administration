# Decision logs

| # | Decision point | What you chose | Why? | What alternative idea have you drafted? |
|---|---|---|---|---|
| 1 | Architecture and Database | Backend: ASP.NET Core WebAPI (.NET 10), EF Core, SQLite. Frontend: React + Vite. | Well scalable, and SQLite makes it a portable, file-based selection based on the requirements in the task. React simplifies manual testing. | More complex relational database driver (e.g PostgreSQL), which would require more configuration in the dockerization. |
| 2 | Time zones and overlaps | Strict UTC timestamps on the backend, the frontend converts for the User's local timezone. Inclusive-exclusive boundaries, so that a new reservation can start right after the previous has just ended. | The boundaries and overlap situations need to be specified exactly to avoid unexpected errors.. | Accepting client-side data timestamps which would bring integrity issues. |
| 3 | User authentication simulation | Users seeded in the database, selectable from the UI header. On every mutational request, the `X-User-Id` HTTP header is sent to the server. | The task did not specify the requirement for authentication, but for security reasons, a basic simulation is required. | JWT based authentication. |
| 4 | Special Parking slot types | `SlotType` enum (Regular, VIP, Electric, Accessible). User permissions are stored as a list in a JSON string in the database. | Easy to scale, and easy implementation with a little more security measurements. | Junction tables, and a setting table for allowing dynamic setup for the special slot types. The task specification did not require the dynamically scalable type restrictions. |


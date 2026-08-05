# Reflection
During the development, the largest technical issue was the design of the special type parking slots. When setting up the ```List<SlotType>``` field, I saved it as a JSON string in the database. Upon running the tests, all failed due to a deserialization issue in the initialization phase for null values. For solving, I simply reinitialized the whole database and extended the seeding to avoid null issues.

For making the application more usable, and ensure the possibility for easy manual testing, I have developed a React application which serves as a client for the WebAPI project. This works as a stable and lightweight application, displaying every functionality in a user-friendly manner.

I have used Google Gemini 3.1 Pro as a personal assistant, mainly for iterative planning, development, setting the business logic issues and rules, and the creation of the integration tests. The AI prompt history can be found uploaded to this same folder.
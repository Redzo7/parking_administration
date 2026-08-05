# Folder structure for Parking Administration

/docs                       Documentation files
/tests                      Backend API tests
    /Parking.WebAPI.IntegrationTests
/src
    /ParkingBackend         The Backend project
        /Parking.WebAPI
            /Controllers    Api controllers for mutations and queries.
            /Converters     Data type converter files.
            /Data           Database schema files and seeder.
            /DTOs           Data transfer objects for API communication.
            /Migrations     Migration files.
            /Models         Model files used for ORM with the database.
            /Properties     Setting files.
            /Services       Services for data serving.
    /ParkingFrontend - The Frontend project
        /public
        /src
            /api            Axios setup.
            /components     Components used by the pages for display.
            /context        Context files for global settings handling.
            /models         Model classes to match API response schemas.
            /pages          Page components.
            /query          Hooks for querying API endpoints and cache management.
            /utils          Utility functions and classes.
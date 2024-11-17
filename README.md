# SofomoWeatherForecastAPI

SofomoWeatherForecastAPI is a RESTful API application that provides weather forecasts based on location. It uses Entity Framework for database management and integrates with external weather services.

## Requirements

To run the application, ensure you have the following software installed:

- **.NET 8.0 SDK** (for building the application)
- **MS SQL Server** 
- **Docker** (optional, if you want to run the application in a container)

## Installation

1. **Clone the repository**

    Clone the repository to your local machine:
    ```bash
    git clone https://github.com/wojciechsadlon/SofomoWeatherForecastAPI
    ```

2. **Install dependencies**

    Navigate to the project folder:
    ```bash
    cd SofomoWeatherForecastAPI
    ```

    Then run the following command to install the required dependencies:
    ```bash
    dotnet restore
    ```

3. **Configure the database**

    - The application uses Entity Framework to manage the database.
    - Create the database by applying migrations:
    ```bash
    dotnet ef database update
    ```

    If you want to reset the database (e.g., when creating a new migration), use the following commands:
    ```bash
    dotnet ef database drop --force
    dotnet ef database update
    ```

## Running the Application

1. **Run the application locally**

    To run the application on your local machine, use this command:
    ```bash
    dotnet run
    ```

    The application will be available at: `http://localhost:8080`

2. **Run the application in Docker** (optional)

    If you want to run the application in a Docker container, build the image using the Dockerfile:
    ```bash
    docker-compose up --build
    ```

    The application will be available at: `http://localhost:8080`

## API

The API provides the following endpoints:

- `GET /forecast?latitude={latitude}&longitude={longitude}` – Fetches weather forecast based on latitude and longitude.
- `GET /my-weather` – Fetches the weather forecast based on the user's IP address.
- `POST /sync-weather` – Synchronised actuall weather forecast for all saved locations.
- `GET /locations` – Returns a list of saved locations.
- `GET /weather-data` – Returns weather data from the database.
- `GET /locations/{id}/weather` – Returns weather data for a specific location by its ID.
- `DELETE /locations/{id}` – Deletes a location from the database.

## Testing

Unit tests can be run using xUnit:

```bash
dotnet test

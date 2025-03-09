# Terminal-Style Journal API

A secure RESTful API for a terminal-style logger/journal that stores encrypted entries on the host machine. This API allows you to save text entries, retrieve them with various filtering options, and ensures privacy through AES-256 encryption.

## Features

- Create journal entries with timestamp tracking
- Retrieve entries with pagination
- Get entries for a specific date
- Secure storage with AES-256 encryption
- Persistent storage between server restarts

## Installation

1. Clone the repository
2. Run `dotnet run` to start the server
3. Configure appsettings.json with your encryption key
4. Access the API at http://localhost:5000

## API Endpoints

### POST /api/entries
Create a new journal entry

Request body:
json
{
"content": "Your journal entry text here"
}

Response:
json
{
"id": 1,
"content": "Your journal entry text here",
"timestamp": "2025-03-09T10:14:28.948118Z"
}

### GET /api/entries
Get all journal entries

Query parameters:
- `date`: Filter entries by date (YYYY-MM-DD)
- `limit`: Number of entries per page (default: 10)
- `page`: Page number (default: 1)

Response:
json
[
{
"id": 1,
"content": "Your journal entry text here",
"timestamp": "2025-03-09T10:14:28.948118Z"
}
]

### GET /api/entries/date
Get entries for a specific date

Query parameters:
- `date`: Date to filter entries (YYYY-MM-DD)

Response:
json
[
{
"id": 1,
"content": "Your journal entry text here",
"timestamp": "2025-03-09T10:14:28.948118Z"
}
]




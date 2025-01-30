# Note-Taking API

A simple Note-Taking API built with .NET 6+ using minimal APIs, SQLite (or in-memory) database, and OpenAI integration to automatically generate summaries for notes.

## Features
- **CRUD Operations**: Create, Read, Update, Delete notes.
- **AI Integration**: Automatically generate summaries for notes using OpenAI API (GPT-3 or GPT-4).
- **Persistent Storage**: SQLite database (can be switched to In-Memory for testing).

## Prerequisites
- [.NET 6 SDK or later](https://dotnet.microsoft.com/download) installed.
- An **OpenAI API key** for the AI summarization feature.
- A text editor or IDE like [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/).

## Setup Instructions

### 1. Clone the Repository

Clone the project to your local machine:

```bash
git clone https://github.com/Paulo-Moraes05/NoteTakingAPI.git
cd NoteTakingAPI
```
## 2. Install Dependencies

Make sure you have the required dependencies installed:

```bash
dotnet restore
```

## 3. Configure OpenAI API Key

- Go to the [OpenAI API keys page](https://platform.openai.com/api-keys)
- Create a new API key if you don't have one

Add an environment variable to your PATH with your own OpenAI API key in an open terminal:

```bash
$env:OpenAI__ApiKey="your_openai_api_key_here"
```

Make sure to replace `your_openai_api_key_here` with your own.

## 4. Build and Run the Application

After configuring everything, run:

```bash
dotnet run
```

This will start the server and you'll see output like this:
```mathematica
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5226
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

In this case, access the application using `http://localhost:5226/notes`. However, the address can change depending on your machine.

## 5. Testing the Application

The base URL for the API is: `http://localhost:5226/notes

Use a tool like Postman or `curl` to interact with the API and manage notes. Make sure to use the correct HTTP method and the appropriate URL for each operation.

### 1. Create a Note (POST)

To create a new note, send a `POST` request to the `/notes` endpoint. The request body should contain the `title` and `content` of the note. The API will automatically generate a summary for the note using OpenAI.

### Request:

**URL:** `http://localhost:5226/notes`

**Method:** POST

**Request Body (JSON):**

```json
{
  "title": "My First Note",
  "content": "This is the body of the note. It contains some useful information."
}
```

### 2. Get all Notes (GET)

### Request:

**URL:** `http://localhost:5226/notes`

**Method:** GET

### 3. Get Note by ID (GET)

**URL:** http://localhost:5226/notes/{id}

**Method:** GET

For example, to get the note with ID 1, the request would be:

**URL:** http://localhost:5226/notes/1

### 4. Update a Note (PUT)

To update an existing note, send a PUT request to the /notes/{id} endpoint, where {id} is the ID of the note you want to update. The request body should contain the updated title and content.

### Request:
**URL:** http://localhost:5226/notes/{id}

**Method:** PUT

For example, to update the note with ID 1, the request would be:

**URL:** http://localhost:5226/notes/1

**Request body (json)**

```json
{
  "title": "Updated Title",
  "content": "Updated content of the note."
}
```

### 5. Delete a Note (DELETE)

### Request:
**URL:** http://localhost:5226/notes/{id}

**Method:** DELETE

For example, to delete the note with ID 1, the request would be:

**URL:** http://localhost:5226/notes/1


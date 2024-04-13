# JSON Server Minimal APIs
This repository contains the source code for a dynamic JSON Server API built with ASP.NET Core Minimal APIs. The API provides RESTful services to interact with data stored in a JSON file, allowing CRUD operations on dynamically defined tables.

## Getting Started

### clone 
```bash
cd json-server-api\src\JsonServer
```
### seed

Paste the contents of your database in the `db.json` file:
```json
{
  "posts": [
    { "id": "1", "title": "a title", "views": 100 },
    { "id": "2", "title": "another title", "views": 200 }
  ],
  "comments": [
    { "id": "1", "text": "a comment about post 1", "postId": "1" },
    { "id": "2", "text": "another comment about post 1", "postId": "1" }
  ]
}
```
### run

```bash
dotnet run
```
### browse

Navigate to `http://localhost:5023/swagger/index.html` to view and interact with the API documentation.

## API Endpoints
Each table in the JSON database can be accessed via its name through the following endpoints:

- `GET /{tableName}`: retrieves all entries from the specified table.
- `GET /{tableName}/{id}`: retrieves a single entry by Id from the specified table.
- `POST /{tableName}`: creates a new entry in the specified table.
- `PUT /{tableName}/{id}`: updates an existing entry by Id in the specified table.
- `DELETE /{tableName}/{id}`: deletes an entry by Id from the specified table.

# Contributing
Contributions are welcome! Please feel free to submit a pull request or open an issue for any bugs or feature requests.

# License
This project is licensed under the MIT License.

# Support
If you encounter any issues or have questions, please file an issue on this repository.

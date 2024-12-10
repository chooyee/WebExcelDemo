Here’s a comprehensive `README.md` file for your GitHub repository:

```markdown
# WebExcelDemo

WebExcelDemo is a demo project showcasing how to integrate the **ClosedXML** library with an ASP.NET Core Web API. This project provides various APIs for interacting with Excel files, including reading, writing, and modifying Excel data.

## Features

- Fetch all Excel file entries stored in the database.
- Retrieve sheet names from a specified Excel file.
- Search for specific values within a sheet.
- Write data to a specific cell in an Excel sheet.
- Insert rows or columns into a sheet at specified positions.
- Download an Excel file directly from the server.

## Technologies Used

- **ASP.NET Core**: Web API framework.
- **ClosedXML**: Library for working with Excel files.
- **Newtonsoft.Json**: For JSON serialization.
- **Factory.DB.Model**: Mocked database model classes for demonstration purposes.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- System.Data.Sqlite
- ClosedXML 0.102.3
### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/WebExcelDemo.git
   cd WebExcelDemo
   ```

2. Install dependencies:
   Restore NuGet packages using the following command:
   ```bash
   dotnet restore
   ```

3. Configure the project:
   - Ensure your database settings in the `Factory.DB.Model` library are configured.
   - Update paths or database IDs as needed for testing Excel files.

4. Build and run the project:
   ```bash
   dotnet run
   ```

### API Endpoints

#### Test Endpoint
- **GET** `/api/v1/xls/test`
  - Returns a simple string response for testing purposes.

#### Get All Excel Records
- **GET** `/api/v1/xls/all`
  - Fetches all Excel file records from the database.

#### Get Excel Sheet Names
- **GET** `/api/v1/xls/{id}`
  - Fetches all sheet names from an Excel file with the specified `id`.

#### Search Excel Sheet
- **POST** `/api/v1/xls/search`
  - Parameters:
    - `id` (int): Excel file ID.
    - `sheet` (string): Sheet name.
    - `searchText` (string): Text to search for.
    - `colEndText` (string, optional): End column delimiter.
  - Returns matching cells as a list of objects.

#### Write to Excel Cell
- **POST** `/api/v1/xls/write`
  - Parameters:
    - `id` (int): Excel file ID.
    - `sheet` (string): Sheet name.
    - `row` (int): Row number.
    - `col` (int): Column number.
    - `text` (string): Text to write.
  - Returns `true` if successful.

#### Insert Columns
- **POST** `/api/v1/xls/column`
  - Parameters:
    - `id` (int): Excel file ID.
    - `sheet` (string): Sheet name.
    - `targetCol` (int): Target column index.
    - `numOfCol` (int): Number of columns to insert.
    - `insertPosition` (Enum): Position to insert (`Before` or `After`).

#### Insert Rows
- **POST** `/api/v1/xls/row`
  - Parameters:
    - `id` (int): Excel file ID.
    - `sheet` (string): Sheet name.
    - `targetRow` (int): Target row index.
    - `numOfRow` (int): Number of rows to insert.
    - `insertPosition` (Enum): Position to insert (`Before` or `After`).

#### Download Excel File
- **GET** `/api/v1/xls/download/{id}`
  - Downloads the Excel file with the specified `id`.


## Contributing

Contributions are welcome! If you find a bug or want to add a feature:
1. Fork the repository.
2. Create a new branch (`git checkout -b feature-name`).
3. Commit your changes (`git commit -m "Add feature-name"`).
4. Push to the branch (`git push origin feature-name`).
5. Open a Pull Request.

## License

This project is licensed under the [MIT License](LICENSE).

---

Feel free to reach out with any questions or suggestions!
```

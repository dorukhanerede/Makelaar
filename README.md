# Makelaar API
 
The Makelaar API provides functionality to interact with the Funda API to retrieve property listings, group the results by real estate agents, and determine the top-performing agents based on their property listings.

## Features

- **Azure Functions:** Serverless architecture to handle HTTP requests for property and real estate agent data.
- **Polly:** Implements rate-limiting and retry policies using the Polly library to ensure the API can handle rate limits and transient errors effectively.
- **Unit Testing:** Tests are written using Moq, FluentAssertions, and AutoFixture.

## Design Decisions
1) Why Azure Function?
    - Scalability and ease of maintenance 
2) FundaClient:
    - The `FundaClient` was created to encapsulate HTTP communication logic and make it easier to mock and test independently, improving maintainability and testability.
3) Polly Rate Limit and Retry:
    - Combining rate-limiting and retry policies ensures that the API doesn’t exceed Funda’s rate limits and handles transient errors.
4) Result object:
    - `Result` was used to ensure that both success and failure responses are handled consistently and provide detailed error messages for logging and client feedback.
5) Why Differentiate ApiResult from Result?
    - The `ApiResult` was differentiated from `Result` to provide a more structured, flexible response object specific to the API layer. This separation ensures that API-specific concerns, such as handling different status codes, formatting, or additional metadata, can be easily managed without polluting the core `Result<T>` used within the service logic.

## Future Improvements

1) Add Swagger Documentation
2) Handle Pagination Efficiently
3) Implement Azure Key Vault for secure data storage
4) AutoMapper can be used to simplify mappings if the project grows
5) Authorization if needed
6) ApiResult and Result can be extended even more to cover more scenarios where additional information is needed.
7) Implement a RestClient Factory for Multiple API Clients (if the plan is to integrate with more APIs)
8) And of course MORE UNIT TESTS!


## How to Run

Clone the repo and create local.settings.json file with:
```json
﻿{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_INPROC_NET8_ENABLED": "1",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "FundaApiBaseUrl": "super_secret_url",
    "FundaApiKey": "super_secret_key"
  }
}
```
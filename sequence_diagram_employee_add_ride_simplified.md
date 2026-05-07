# Sequence Diagram: Employee Adds a New Ride (Simplified)

```mermaid
sequenceDiagram
    participant Employee
    participant UI as Rides.razor<br/>(UI)
    participant Service as RideService<br/>(Client)
    participant Controller as RideController<br/>(API)
    participant TaxiRepo as TaxiRepository
    participant AssignmentRepo as AssignmentRepository
    participant RideRepo as RideRepository
    participant Database

    Employee->>UI: Fill ride form & submit
    UI->>Service: AddRideAsync(rideData)
    Service->>Controller: POST /api/rides<br/>(with JWT token)
    
    Controller->>Controller: Extract userId & role<br/>from JWT token
    
    Controller->>TaxiRepo: GetByIdAsync(taxiId)
    TaxiRepo->>Database: Query Taxi
    Database-->>TaxiRepo: Taxi entity
    TaxiRepo-->>Controller: Taxi object
    
    Controller->>AssignmentRepo: IsEmployeeAssignedToTaxiAsync<br/>(userId, taxiId)
    AssignmentRepo->>Database: Check Assignment
    Database-->>AssignmentRepo: Assignment result
    AssignmentRepo-->>Controller: true/false
    
    Controller->>Controller: Create Ride object
    
    Controller->>RideRepo: AddAsync(ride)
    RideRepo->>Database: Save Ride
    Database-->>RideRepo: Success
    RideRepo-->>Controller: Completed
    
    Controller-->>Service: Ok(RideResponseDTO)
    Service-->>UI: Ride object
    UI-->>Employee: Show success message
```

## Key Actors & Objects

- **Employee**: The user adding the ride
- **UI (Rides.razor)**: Blazor component for ride form
- **RideService**: Client-side service handling HTTP requests
- **RideController**: API endpoint handling ride creation
- **TaxiRepository**: Validates taxi exists
- **AssignmentRepository**: Validates employee assignment
- **RideRepository**: Saves the ride to database
- **Database**: Data persistence layer










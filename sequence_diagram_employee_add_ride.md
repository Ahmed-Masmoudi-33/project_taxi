# Sequence Diagram: Employee Adds a New Ride

```mermaid
sequenceDiagram
    participant Employee
    participant RidesPage as Rides.razor<br/>(Blazor UI)
    participant RideService as RideService<br/>(Client)
    participant AuthMiddleware as Authentication<br/>Middleware
    participant RideController as RideController<br/>(API)
    participant TaxiRepository as TaxiRepository
    participant AssignmentRepository as AssignmentRepository
    participant RideRepository as RideRepository
    participant Database as ApplicationContext<br/>(Database)

    Employee->>RidesPage: Fills ride form<br/>(TaxiId, StartDate, EndDate,<br/>DistanceKm, Amount)
    Employee->>RidesPage: Clicks "Save" button
    RidesPage->>RidesPage: SaveRide() method<br/>(Validates dates/times)
    RidesPage->>RideService: AddRideAsync(rideForm)
    
    RideService->>RideService: SetAuthHeaderAsync()<br/>(Gets token from localStorage)
    RideService->>RideController: POST /api/rides<br/>(with JWT token + RideCreateDTO)
    
    RideController->>AuthMiddleware: Validate JWT token
    AuthMiddleware-->>RideController: Token validated
    
    RideController->>RideController: Extract userId and userRole<br/>from Claims
    
    RideController->>TaxiRepository: GetByIdAsync(dto.TaxiId)
    TaxiRepository->>Database: Query Taxis table
    Database-->>TaxiRepository: Taxi entity
    TaxiRepository-->>RideController: Taxi object
    
    alt Taxi not found
        RideController-->>RideService: BadRequest("Taxi not found")
        RideService-->>RidesPage: Exception
        RidesPage-->>Employee: Show error message
    else Taxi found
        RideController->>AssignmentRepository: IsEmployeeAssignedToTaxiAsync<br/>(userId, dto.TaxiId)
        AssignmentRepository->>Database: Query Assignments table<br/>(check active assignment)
        Database-->>AssignmentRepository: Assignment exists?
        AssignmentRepository-->>RideController: true/false
        
        alt Employee not assigned
            RideController-->>RideService: Forbid("You are not assigned to this taxi")
            RideService-->>RidesPage: Exception
            RidesPage-->>Employee: Show error message
        else Employee assigned
            RideController->>RideController: Create Ride object<br/>(StartDate, EndDate, DistanceKm,<br/>Amount, TaxiId, EmployeeId)
            
            RideController->>RideRepository: AddAsync(ride)
            RideRepository->>Database: INSERT INTO Rides
            Database-->>RideRepository: Ride saved
            RideRepository-->>RideController: Task completed
            
            RideController->>RideController: Create RideResponseDTO
            RideController-->>RideService: Ok(RideResponseDTO)
            
            RideService->>RidesPage: Return Ride object
            RidesPage->>RidesPage: LoadRides()<br/>(Refresh ride list)
            RidesPage->>RidesPage: CloseModal()
            RidesPage-->>Employee: Show success<br/>(Modal closed, list updated)
        end
    end
```

## Components Involved

1. **Rides.razor**: Blazor UI component where employee fills the form
2. **RideService**: Client-side service that handles HTTP requests
3. **Authentication Middleware**: Validates JWT token
4. **RideController**: API controller handling the POST request
5. **TaxiRepository**: Repository for taxi data access
6. **AssignmentRepository**: Repository for assignment validation
7. **RideRepository**: Repository for ride data access
8. **ApplicationContext**: Entity Framework database context

## Key Validations

- Employee must be authenticated (JWT token required)
- Taxi must exist in the database
- Employee must be assigned to the taxi (active assignment with EndDate = null)
- All ride data must be provided (StartDate, EndDate, DistanceKm, Amount, TaxiId)


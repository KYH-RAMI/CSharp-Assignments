# Prague Parking V2 – Klassdiagram

## Aktuell domänmodell

```mermaid
classDiagram

    Vehicle <|-- Car
    Vehicle <|-- MC

    ParkingGarage "1" --> "1..*" ParkingSpot : contains
    ParkingSpot "1" --> "0..*" Vehicle : contains

    class Vehicle {
        <<abstract>>
        +string Identifier
        +DateTime ArrivalTime
    }

    class Car {
        +Car(string identifier)
    }

    class MC {
        +MC(string identifier)
    }

    class ParkingSpot {
        +int SpotNumber
        -List~Vehicle~ _parkedVehicles
        +ParkingSpot(int spotNumber)
        +bool CanPark(Vehicle vehicle, int maxVehiclesPerSpot)
        +void ParkVehicle(Vehicle vehicle, int maxVehiclesPerSpot)
        +Vehicle? FindVehicle(string identifier)
        +Vehicle? RemoveVehicle(string identifier)
    }

    class ParkingGarage {
        -List~ParkingSpot~ _parkingSpots
        +IReadOnlyList~ParkingSpot~ ParkingSpots
        +ParkingGarage(int numberOfParkingSpots)
        +ParkingSpot? FindAvailableSpot(Vehicle vehicle, int maxVehiclesPerSpot)
        +Vehicle? FindVehicle(string identifier)
        +ParkingSpot ParkVehicle(Vehicle vehicle, int maxVehiclesPerSpot)
    }
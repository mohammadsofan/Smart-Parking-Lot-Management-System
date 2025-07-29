# Smart Parking System

A simple yet extensible C# console application simulating a smart parking lot management system. It supports vehicle check-in/check-out, data persistence, filtering, and event-driven notifications.

---

## 🚗 Features

- ✅ Check-In / Check-Out vehicles
- 📂 Persistent storage using JSON files (checked-in and checked-out vehicles stored separately)
- 🔎 Filter parked vehicles by type (Car, Truck, Motorcycle)
- 🧠 Polymorphic deserialization using `System.Text.Json`
- 📑 Strong validation using `{CustomeValidation}`
- ❗ Custom exception handling via `ParkingLotException`
- 🛎️ Event triggered on full parking lot
- 📃 Well-structured DTOs and services
- 🧪 Easily testable and extensible architecture

---

## 📋 Menu Options

```
===================== Menu =======================
Please Choose one of the following options:
1. Check-In Vehicle
2. Check-Out Vehicle
3. View Parked Vehicles
4. View Checked Out Vehicles
5. Filter by Vehicle Type
6. Exit
==================================================
```

---

## 🗃️ Project Structure

```
SmartParkingSystem/
│
├── Models/
│   ├── Vehicle.cs             # Base abstract class
│   ├── Car.cs, Truck.cs, Motorcycle.cs
│
├── Enums/
│   ├── ConsoleOperationEnum.cs          
│   ├── VehicleTypeEnum.cs
│
├── Dtos/
│   ├── GetVehiclesResultDto.cs          
│   ├── ParkingLotResultDto.cs
│ 
├── Interfaces/
|   ├── IFileService  # Interface for file storage
│   ├── IBillable.cs           # Interface for fee calculation
│   ├── IVehicleStorageService.cs
│   ├── INotifiable.cs         # For event subscribers
│
├── Services/
│   ├── ParkingLotManager.cs   # Main business logic
│   ├── FullLotNofificationService.cs # Notification service for triggered events
│   ├── FileService.cs  # file storage service
│   ├── VehicleFileStorageService.cs
│
├── Events/
│   └── FullLotEventArgs.cs
│
├── Validators/
│   └── VehicleValidator.cs    # CustomValidation implementation
│
├── Exceptions/
│   └── ParkingLotException.cs
│
├── Utils/
│   ├── ConsoleInputHelper.cs
│   ├── ConsoleOutputHelper.cs
│   ├── SmartParkingAppMenu.css
│
└── Program.cs                 # Entry point
```

---

## 🛠 Technologies

- **C# .NET**
- **System.Text.Json** (with polymorphism)
- **Event Handling**
- **LINQ and Expressions**
- **Console UI**
- ...
---

## 🧪 Validation

Vehicle validation is enforced before check-in:
- License plate must be at least **5 characters**.
- Duplicates are rejected based on license plate, with a raised `ParkingLotException`.
- Capacity limit enforced, with a raised `ParkingLotException` if full.

---

## ⚠️ Event Handling

When the last available slot is filled, an event (`FullLotEvent`) is triggered.
Subscribers can be notified with a custom message (e.g., `"⚠️ The parking lot is now full."`).

---

## 📦 Storage

All vehicle data is persisted in separate JSON files:
- `checkedin.json` → active vehicles in the lot.
- `checkedout.json` → vehicles that already exited.

Files are updated on every operation.

---

## 📞 Contact

Project by: **Mohammad Sofan**

---

## 📄 License

This project is for educational purposes only. Use freely and modify as needed.

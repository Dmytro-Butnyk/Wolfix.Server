# Wolfix.Server 🐺

**Enterprise-grade Modular Monolith E-commerce Backend** *Powering modern digital commerce with .NET 9, Clean Architecture, and Domain-Driven Design.*

---

## 🌍 Language Navigation
- [English Documentation](#-english-documentation)
- [Українська документація](#-українська-документація)

---

## 🇺🇸 English Documentation

### 📌 Project Concept
**Wolfix.Server** is a robust, scalable backend system designed for complex e-commerce ecosystems. By utilizing a **Modular Monolith** architecture, the project achieves high maintainability and logical isolation of business domains, making it ready for a future transition to microservices if needed.

### 🏗 Architectural Excellence
The system implements a multi-layered **Clean Architecture** within each independent module:
- **Domain Layer:** Pure business logic, Aggregates, Entities, and Value Objects.
- **Application Layer:** Orchestrates use cases, handles DTO mapping, and defines port interfaces.
- **Infrastructure Layer:** Concrete implementations of persistence (EF Core), Messaging, and external integrations (Stripe, Azure).
- **Endpoints (API) Layer:** Lightweight **Minimal APIs** following the REPR (Request-Endpoint-Response) pattern for maximum performance.

#### Key Design Decisions:
- **Result Pattern:** Explicit failure/success handling via `Result<T>` and `VoidResult` to avoid exception-driven flow control.
- **DDD Compliance:** Strong encapsulation of business rules within Aggregates and validation of domain concepts through **Value Objects** (`Address`, `Email`, `PhoneNumber`).
- **Guid V7:** High-performance, time-sortable identifiers for database primary keys.
- **In-Memory Event Bus:** Decoupled inter-module communication using Integration Events, allowing modules like `Customer` to react to `Identity` events without direct coupling.

### 📦 Core Modules
- **Identity:** Security core with JWT, Role-Based Access Control (RBAC), and Google OAuth integration.
- **Catalog:** Advanced product management with dynamic attributes, variants, and a **Toxicity API** for content moderation.
- **Order:** Complex order lifecycle management, delivery logic, and **Stripe** payment gateway integration.
- **Customer:** Personalized user experience, including advanced Cart and Wishlist management.
- **Seller:** Onboarding flow and shop management tools.
- **Media:** Centralized, high-availability storage using **Azure Blob Storage**.
- **Support:** Internal ticketing system for customer-seller dispute resolution.

### 🛠 Technology Stack
- **Backend:** .NET 9 (C# 13)
- **Database:** PostgreSQL (Separate schemas/Contexts per module)
- **Orchestration:** .NET Aspire (Cloud-native development inner loop)
- **Cloud Integration:** Azure Blob Storage
- **Payments:** Stripe SDK
- **Caching:** ASP.NET Core In-Memory Cache with `IAppCache` abstraction
- **Communication:** MediatR-based internal messaging

### 🔗 Frontend Repository
The corresponding client application can be found here:  
👉 [Wolfix.Client Repository](https://github.com/SannidoOrg/Wolfix.Client/tree/main)

### 🚀 Setup & Execution
1. **Prerequisites:**
   - [.NET 9 SDK](https://dotnet.microsoft.com/download)
   - [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Required for automatic DB provisioning)
2. **Local Environment:**
   - Clone the repo: `git clone https://github.com/dmytro-butnyk/wolfix.server.git`
   - Open the solution in VS 2022 (17.12+) or JetBrains Rider.
3. **Running the System:**
   - Set **`Wolfix.AppHost`** as the Startup Project.
   - Run (**F5**). 
   - .NET Aspire will automatically spin up PostgreSQL and provide a real-time dashboard at `localhost:17170`.

---

## 🇺🇦 Українська документація

### 📌 Концепція проекту
**Wolfix.Server** — це високонавантажена бекенд-система для сучасного E-commerce. Завдяки архітектурі **модульного моноліту**, проект поєднує простоту розгортання з гнучкістю мікросервісів.

### 🏗 Технічні переваги
- **Clean Architecture:** Повна незалежність бізнес-логіки від зовнішніх фреймворків.
- **DDD:** Кожен домен (Catalog, Order, Identity) має власну базу даних та чіткі межі (Bounded Contexts).
- **Result Pattern:** Професійний підхід до обробки результатів операцій без використання важких системних винятків.

### 🧩 Огляд модулів
- **Catalog:** Гнучкі атрибути товарів та інтегрований сервіс перевірки контенту на токсичність.
- **Order:** Управління замовленнями та інтеграція з платіжною системою **Stripe**.
- **Identity:** Надійна автентифікація через JWT та Google OAuth.
- **Media:** Робота з медіа-файлами через **Azure Blob Storage**.

### 🚀 Як запустити
1. Встановіть **.NET 9 SDK** та **Docker**.
2. Встановіть проект **`Wolfix.AppHost`** як стартовий.
3. Запустіть додаток. .NET Aspire автоматично налаштує PostgreSQL та запустить API з інтегрованим дашбордом.

---

### 🔗 Клиентская часть
Репозиторий фронтенда: [Wolfix.Client](https://github.com/SannidoOrg/Wolfix.Client/tree/main)

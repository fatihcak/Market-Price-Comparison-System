# AI Integrated Market Price Comparison System

## Project Overview
An award-winning, AI-powered system architecture built for dynamic price comparison and data processing. This platform empowers consumers by aggregating real-time product data across multiple markets and utilizing Large Language Models (LLMs) combined with RAG (Retrieval-Augmented Generation) to deliver the most cost-effective shopping routes.

## 👥 Team

| Name | Role | GitHub |
|------|------|--------|
| **Fatih Çakır** | **Team Leader - Backend & AI Developer** | [@fatihcak](https://github.com/fatihcak) |
| Doğukan Örs | Frontend Developer | [@DogukanOrs](https://github.com/DogukanOrs) |
| Doğukan Güler | Backend Developer | [@gulerdogukan](https://github.com/gulerdogukan) |
| Aleyna Yılmaz | Database Engineer | [@aleynayilmz](https://github.com/aleynayilmz) |

## System Architecture & AI Integration
The backend infrastructure is built strictly on **Clean (Onion) Architecture** to ensure high scalability, testability, and separation of concerns.

* **Core Backend Architecture:** Developed using **.NET 8.0** and **ASP.NET Core Web API**, following RESTful principles for stateless, high-performance endpoints.
* **Database & ORM:** Powered by **SQL Server (hosted on AWS RDS)** and managed via **Entity Framework Core 8.0** using the Repository Pattern.
* **Enterprise AI & RAG Engine (Google Gemini 2.5 Flash):** Integrated with the Gemini API using a **Retrieval-Augmented Generation (RAG)** pipeline. The system retrieves real-time market data from the SQL database and feeds it to the LLM to power the natural language Chatbot and intent analysis, ensuring highly accurate and context-aware shopping recommendations.
* **Smart Basket Optimization Algorithm:** A custom C# algorithm that processes shopping lists across multiple markets, identifying missing products and calculating the most cost-effective market combinations asynchronously.
* **Fuzzy Matching Engine:** Implemented the **Levenshtein Distance** algorithm to handle user typos and ensure accurate product mapping across massive datasets.

## Project Structure

```
Market-Price-Comparison-System/
├── Frontend/                 # React application
│   ├── src/
│   │   ├── components/       # React components
│   │   ├── services/         # API services
│   │   ├── types/            # TypeScript types
│   │   └── constants/        # App constants
│   └── package.json
│
├── BACKEND/                  # ASP.NET Core API
│   └── src/
│       ├── API/              # Controllers & Configuration
│       ├── Domain/           # Business Logic & Services
│       ├── DataAccess/       # Database & Repositories
│       └── DTOs/             # Data Transfer Objects
│
└── README.md
```

### Key Backend Contributions (My Role)
As the **Team Leader and Backend/AI Engineer**, my primary focus was architecting the backend infrastructure and designing the AI decision engine:

* Designed and implemented the **Onion Architecture**, strictly decoupling domain logic from external data access and API layers.
* Engineered the **RAG Pipeline and AI Intent Parsing mechanism**, bridging the Google Gemini API with our internal SQL Server database to provide real-time, accurate shopping decisions.
* Developed the **Smart Basket Optimization** logic, allowing the system to process arrays of products and return the cheapest market route in milliseconds.
* Optimized database queries using **Entity Framework Core (LINQ)** and Eager Loading mechanisms to ensure the API handles high-traffic search and comparison requests efficiently.

## Technology Stack

### Frontend
| Technology | Purpose |
|------------|---------|
| React (Vite) | Framework |
| TypeScript | Language |
| TailwindCSS | Styling |
| Lucide React | Icons |
| React Router DOM | Routing |
| React Hooks | State Management |

### Backend
| Technology | Purpose |
|------------|---------|
| ASP.NET Core Web API | Framework |
| C# | Language |
| Entity Framework Core | ORM |
| SQL Server | Database |
| Google Gemini API 2.5 Flash | NLP & RAG Engine |

### Prerequisites
- Node.js (v18 or higher)
- .NET SDK (v8.0 or higher)
- SQL Server

### Installation & Running

#### 1. Backend Setup

```bash
cd BACKEND/src/API
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5000`

#### 2. Frontend Setup

```bash
cd Frontend
npm install
npm run dev
```

The application will be available at `http://localhost:5173`

---

## 📸 Screenshots

> <img width="1903" height="908" alt="image" src="https://github.com/user-attachments/assets/b18930b3-0e99-49a9-bb66-fe05fe3917c1" />


---

## 📄 License

An award-winning, AI-powered system architecture built for dynamic price comparison and data processing.

# AI Integrated Market Price Comparison System

## Project Overview
[cite_start]An award-winning, AI-powered enterprise system architecture built for dynamic price comparison, large-scale data processing, and smart basket optimization[cite: 918, 921, 1063]. [cite_start]This platform empowers consumers by aggregating real-time product data across multiple markets and utilizing Large Language Models (LLMs) to deliver the most cost-effective shopping routes[cite: 1057, 1063].

## 👥 Team

| Name | Role | GitHub |
|------|------|--------|
| **Fatih Çakır** | **Team Leader - Backend & AI Developer** | [@fatihcak](https://github.com/fatihcak) |
| Doğukan Örs | Frontend Developer | [@DogukanOrs](https://github.com/DogukanOrs) |
| Doğukan Güler | Backend Developer | [@gulerdogukan](https://github.com/gulerdogukan) |
| Aleyna Yılmaz | Database Engineer | [@aleynayilmz](https://github.com/aleynayilmz) |

## System Architecture & AI Integration
[cite_start]The backend infrastructure is built strictly on **Clean (Onion) Architecture** to ensure high scalability, testability, and separation of concerns.

* [cite_start]**Core Backend Architecture:** Developed using **.NET 8.0** and **ASP.NET Core Web API**, following RESTful principles for stateless, high-performance endpoints[cite: 1380, 1381, 1443].
* [cite_start]**Database & ORM:** Powered by **SQL Server (hosted on AWS RDS)** and managed via **Entity Framework Core 8.0** using the Repository Pattern[cite: 1382, 1399, 1475].
* [cite_start]**Enterprise AI Engine (Google Gemini 2.5):** Integrated with the Gemini API to power the natural language Chatbot and intent analysis[cite: 1403, 1433]. [cite_start]The AI engine parses user queries into actionable JSON data for dynamic routing and contextual responses[cite: 1433, 1434].
* [cite_start]**Smart Basket Optimization Algorithm:** A custom C# algorithm that processes shopping lists across multiple markets, identifying missing products and calculating the most cost-effective market combinations asynchronously[cite: 1427, 1428, 1430].
* [cite_start]**Fuzzy Matching Engine:** Implemented the **Levenshtein Distance** algorithm to handle user typos and ensure accurate product mapping across massive datasets[cite: 1421, 1422, 1424].

## Key Backend Contributions (My Role)
[cite_start]As the **Team Leader and Backend/AI Engineer**[cite: 1075], my primary focus was architecting the backend infrastructure and designing the AI decision engine:
* [cite_start]Designed and implemented the **Onion Architecture**, strictly decoupling domain logic from external data access and API layers[cite: 1441, 1470].
* [cite_start]Engineered the **AI Intent Parsing mechanism**, bridging the Google Gemini API with our internal SQL Server database to provide real-time, context-aware shopping recommendations[cite: 1403, 1433].
* [cite_start]Developed the **Smart Basket Optimization** logic, allowing the system to process arrays of products and return the cheapest market route in milliseconds[cite: 1427, 1430].
* [cite_start]Optimized database queries using **Entity Framework Core (LINQ)** and Eager Loading mechanisms to ensure the API handles high-traffic search and comparison requests efficiently[cite: 1475, 1477].

## Technology Stack

### Backend
| Technology | Purpose |
|------------|---------|
| **ASP.NET Core 8.0** | [cite_start]High-performance RESTful Web API Framework [cite: 1380, 1381] |
| **C# (v12.0)** | [cite_start]Primary Backend Language [cite: 1379] |
| **Entity Framework Core 8.0** | [cite_start]Object-Relational Mapping (ORM) [cite: 1382] |
| **SQL Server (AWS RDS)** | [cite_start]Relational Database Management [cite: 1399, 1401] |
| **Google Gemini API** | [cite_start]Natural Language Processing & Intent Analysis [cite: 1403] |

### Frontend
| Technology | Purpose |
|------------|---------|
| **React 18.2.0 (Vite)** | [cite_start]UI Framework & Build Tool [cite: 1392, 1394] |
| **TypeScript** | [cite_start]Strongly Typed Client-Side Logic [cite: 1393] |
| **TailwindCSS 3.3.0** | [cite_start]Utility-first Styling [cite: 1395] |

## Getting Started

### Prerequisites
* Node.js (v18 or higher)
* [cite_start].NET SDK (v8.0 or higher) [cite: 1674]
* [cite_start]SQL Server [cite: 1675]

### Installation & Running

#### 1. Database Setup
[cite_start]Update the `DefaultConnection` string in `appsettings.json`[cite: 1680].
```bash
cd BACKEND/src/API
dotnet ef database update

#### 2. Backend Setup
cd BACKEND/src/API
dotnet build
dotnet run
The API will be available at http://localhost:5000

#### 1. Frontend Setup
cd Frontend
npm install
npm run dev
The application will be available at http://localhost:5173

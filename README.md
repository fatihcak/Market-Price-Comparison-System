# Market Price Comparison System

A full-stack application designed to help users compare product prices across different markets, manage shopping lists, and find the best deals.

## 🚀 Features

- **Price Comparison**: Compare prices of products across various markets to find the cheapest options.
- **Smart Shopping List**: Create and manage shopping lists with automatic price calculation.
- **AI Chatbot**: Intelligent assistant to help with product searches and recommendations.
- **Basket Comparison**: Compare the total cost of your entire shopping basket across different markets.
- **Real-time Data**: Up-to-date pricing information (simulated or scraped).

## 🛠️ Technology Stack

### Frontend
- **Framework**: React (Vite)
- **Language**: TypeScript
- **Styling**: TailwindCSS
- **Icons**: Lucide React
- **Routing**: React Router DOM
- **State Management**: React Hooks

### Backend
- **Framework**: ASP.NET Core Web API
- **Database**: SQLite (Entity Framework Core)
- **Language**: C#

## 📦 Project Structure

- **Frontend/**: Contains the React application code.
- **BACKEND/**: Contains the ASP.NET Core Web API project.
- **compsys/**: System component documentation or assets.

## 🏁 Getting Started

### Prerequisites
- Node.js (v18 or higher)
- .NET SDK (v8.0 or higher)

### Installation & Running

#### 1. Backend Setup
Navigate to the API directory and run the application:

```bash
cd BACKEND/src/API
dotnet restore
dotnet run

cd Frontend
npm install
npm run dev

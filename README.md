#  Market Price Comparison System

A full-stack application designed to help users compare product prices across different markets, manage shopping lists, and find the best deals.

---

##  Features

- **Price Comparison** - Compare prices of products across various markets to find the cheapest options
- **Smart Shopping List** - Create and manage shopping lists with automatic price calculation
- **AI Chatbot** - Intelligent assistant to help with product searches and recommendations
- **Basket Comparison** - Compare the total cost of your entire shopping basket across different markets
- **Favorites** - Save your favorite products for quick access
- **Market Locations** - Find nearby markets on an interactive map
- **Real-time Data** - Up-to-date pricing information

---

##  Technology Stack

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

---

##  Project Structure

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

---

##  Getting Started

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

> Add screenshots here

---

## 👥 Team

| Name | Role | GitHub |
|------|------|--------|
| Doğukan Örs | Frontend Developer | [@DogukanOrs](https://github.com/DogukanOrs) |
| Doğukan Güler | Backend Developer | [@gulerdogukan](https://github.com/gulerdogukan) |
| Fatih Çakır | Backend Developer | [@fatihcak](https://github.com/fatihcak) |
| Aleyna Yılmaz | Database Engineer | [@aleynayilmz](https://github.com/aleynayilmz) |

---

## 📄 License

This project is created as a graduation project.

---

## 🔗 Links

- [GitHub Repository](https://github.com/DogukanOrs/Market-Price-Comparison-System)
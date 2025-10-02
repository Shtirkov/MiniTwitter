MiniTwitter 🐦

An educational project inspired by Twitter.
The goal is to build basic social media features while practicing ASP.NET Core Web API, Entity Framework Core, SQL Server, and a React frontend.

🚀 Features
👤 Authentication

User registration & login (JWT token-based)

Protected routes on the frontend

Logout (clears token)

📝 Posts

Create / Read / Delete posts

Like / Unlike posts (real-time toggle)

Comments under posts (add / delete if you are the author)

Feed populated with your posts + friends’ posts

🤝 Friendships

Send friend requests

Accept / Reject requests

View a list of friends

Pending requests panel with actions

👤 Profile

View your own posts

Manage friends list

See and manage pending requests

Delete your own posts

🛠️ Tech Stack

Backend

.NET 9

ASP.NET Core Web API

Entity Framework Core

SQL Server

Frontend

React
 + Vite

Chakra UI

React Router

⚡ Getting Started
Backend

Clone the repository

git clone https://github.com/Shtirkov/MiniTwitter.git
cd MiniTwitter


Configure the database

Ensure you have SQL Server running locally

Update ConnectionStrings:DefaultConnection in appsettings.json

Apply migrations

dotnet ef database update


Run the backend

dotnet run --project MiniTwitter


The API will be available at: https://localhost:5064/api

Frontend

Navigate to the frontend folder

cd frontend


Install dependencies

npm install


Run the frontend

npm run dev


The app will be available at: http://localhost:5173

🎯 Roadmap

✅ JWT Authentication

✅ Posts with likes & comments

✅ Friendships (send, accept/reject, list)

✅ React frontend with protected routes

⏳ Edit posts & comments

⏳ Profile customization (avatar, bio)

⏳ Deployment to cloud (Docker + Azure / Render / Vercel)

📖 Note
This project is for educational purposes only and is not production-ready.
It is being developed step by step to practice full-stack development skills.

👨‍💻 Author
Developed by @Shtirkov

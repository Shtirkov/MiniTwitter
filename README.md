# MiniTwitter 🐦

An educational project inspired by Twitter.  
The goal is to build **basic social media features** while practicing **ASP.NET Core Web API**, **Entity Framework Core**, and **SQL Server**.

---

## 🚀 Features

- 👤 **Authentication**
  - User registration  
  - Login with email and password  
  - (Planned) – JWT token authentication  

- 📝 **Posts**
  - Create a new post  
  - View posts from friends  
  - Delete your own posts  

- 🤝 **Friendships**
  - Send friend requests  
  - Accept / Reject requests  
  - View a list of friends  

---

## 🛠️ Tech Stack

- [.NET 9](https://dotnet.microsoft.com/)  
- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0)  
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/)  
- (Planned) React for frontend  

---

## ⚡ Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/Shtirkov/MiniTwitter.git
   cd MiniTwitter
   ```
2. **Configure the database**
  - Make sure you have SQL Server running locally
  - Update ConnectionStrings:DefaultConnection in appsettings.json

3. **Apply migrations and update the database**
  ```bash
  dotnet ef database update
  ```
4. **Run the project**
  ```bash
  dotnet run
  ```
---

## 🎯 Roadmap:
  - ✅ Basic backend with ASP.NET Core
  - ✅ User authentication and posts CRUD
  - ✅ Friendships (requests, accept/reject)
  - ⏳ JWT Authentication
  - ⏳ Comments and reactions
  - ⏳ React frontend
    
---

📖 Note

- This project is for educational purposes only and is not production-ready.
- It is being developed to practice back-end and later front-end skills.

👨‍💻 Author
Developed by @Shtirkov

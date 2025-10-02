# MiniTwitter 🐦

An educational project inspired by Twitter.  
The goal is to build **basic social media features** while practicing **ASP.NET Core Web API**, **Entity Framework Core**, **SQL Server**, and a **React frontend**.

---

## 🚀 Features

### 👤 Authentication
- User registration & login (JWT token-based)  
- Protected routes on the frontend  
- Logout (clears token)

### 📝 Posts
- Create / Read / Delete posts  
- Like / Unlike posts
- Comments under posts (add / delete if you are the author)  
- Feed populated with your posts + friends’ posts

### 🤝 Friendships
- Send friend requests  
- Accept / Reject requests  
- View a list of friends  
- Pending requests panel with actions  

### 👤 Profile
- View your own posts  
- Manage friends list  
- See and manage pending requests  
- Delete your own posts  

---

## 🛠️ Tech Stack

- **Backend**  
  - [.NET 9](https://dotnet.microsoft.com/)  
  - [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core)  
  - [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)  
  - [SQL Server](https://www.microsoft.com/en-us/sql-server/)  

- **Frontend**  
  - [React](https://react.dev/) + [Vite](https://vitejs.dev/)  
  - [Chakra UI](https://chakra-ui.com/)  
  - [React Router](https://reactrouter.com/)  

---

## ⚡ Getting Started

### Backend
1. **Clone the repository**
```bash
git clone https://github.com/Shtirkov/MiniTwitter.git
cd MiniTwitter
```

2. **Configure the database**
Ensure you have SQL Server running locally
Update ConnectionStrings:DefaultConnection in appsettings.json

3.**Apply migrations**
```bash
dotnet ef database update
```
4. **Run the backend**
```bash
dotnet run --project MiniTwitter
```

The API will be available at: https://localhost:5064/api

###Frontend

1. **Navigate to the frontend folder**
```bash
cd frontend
```

2. **Install dependencies**
```bash
npm install
```

3. **Run the frontend**
```bash
npm run dev
 ```

The app will be available at: http://localhost:5173

🎯 Roadmap

- ✅ JWT Authentication
- ✅ Posts with likes & comments
- ✅ Friendships (send, accept/reject, list)
- ✅ React frontend with protected routes

📖 Note
This project is for educational purposes only and is not production-ready.
It is being developed step by step to practice full-stack development skills.

👨‍💻 Author
Developed by @Shtirkov

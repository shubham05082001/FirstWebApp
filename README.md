# FirstWebApp 🚀

ASP.NET Core MVC Web Application with MySQL & Docker Deployment.

---

## 📌 Features

- User Login Authentication
- Session Management
- Role Based Access
- Multiple Dashboard Pages
- MySQL Database Integration
- Docker Container Deployment
- GitHub Version Control

---

## 🛠️ Technologies Used

- ASP.NET Core MVC (.NET 8)
- MySQL
- Entity Framework Core
- Docker & Docker Compose
- Bootstrap / ArchitectUI
- jQuery

---

## ⚙️ How to Run Project (Docker)

### 1️⃣ Clone Repository

git clone https://github.com/shubham05082001/FirstWebApp.git

### 2️⃣ Go to Project Folder

cd FirstWebApp

### 3️⃣ Run Docker

docker compose up --build

### 4️⃣ Open Browser

http://localhost:8080

---


## 👨‍💻 Developer

**Shubham Sinha**  
Embedded & Software Developer  

GitHub: https://github.com/shubham05082001

## 👨‍💻 How to Access MySQL Database Inside Docker Container

This project uses **MySQL running inside a Docker container** instead of the system MySQL server.

Follow the steps below to access the database.

---

### ✅ Step 1 — Check Running Containers

Run the following command to see active containers:

```
docker ps
```

Look for the MySQL container name.
Example:

```
mysql_db
```

---

### ✅ Step 2 — Connect to MySQL Inside Docker

Use the following command to open the MySQL shell:

```
docker exec -it mysql_db mysql -u root -p
```

Enter the MySQL root password when prompted.

Example:

```
Password: root@123
```

---

### ✅ Step 3 — Run Database Commands

After login, you can run MySQL queries.

#### Show Databases

```
SHOW DATABASES;
```

#### Select Database

```
USE firstwebdb;
```

#### Show Tables

```
SHOW TABLES;
```

#### View Table Structure

```
DESCRIBE users;
```

#### View Data

```
SELECT * FROM users;
```

---

### ⭐ Important Note

* Docker MySQL database is **separate from system MySQL server**
* Application connects to Docker MySQL using container networking
* Data is stored in Docker volume (`mysql_data`) for persistence

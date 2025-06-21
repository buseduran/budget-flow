# 💼 BudgetFlow Backend

This project is the backend of **BudgetFlow**, a personal finance management application. It provides an API service where users can track their income, expenses, and investments, as well as manage their wallets.

## 🔍 Purpose

To help users gain financial awareness and make budget management easier. The system enables control over personal financial data with features such as income-expense tracking, category-based reporting, and investment monitoring.

## ⚙️ Architecture

The project is built with .NET Core and follows a layered architecture:

- **Api**: Handles incoming HTTP requests
- **Application**: Contains application logic (command-query structure)
- **Domain**: Business rules and core models
- **Infrastructure**: External services and database operations
- **Persistence**: Data access layer with Entity Framework

## 📌 Key Features

- User registration and authentication
- JWT-based secure access
- Wallet and category management
- Income, expense, and investment tracking
- Synchronization of stock, commodity, and currency data
- Invitation system for user collaboration

## 🏗️ Technologies

- .NET 8
- PostgreSQL
- Entity Framework Core
- MediatR & FluentValidation
- MailKit for SMTP-based email service

---

The project is under active development and will continue to grow with new features as needed.

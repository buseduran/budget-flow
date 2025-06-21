# 💼 BudgetFlow Backend

## 📚 Available Languages

- [🇺🇸 English](#english)
- [🇹🇷 Türkçe](#türkçe)

---

## 🇺🇸 English

### 📌 Description

This project is the backend of **BudgetFlow**, a personal finance management application. It provides an API service where users can track their income, expenses, and investments, as well as manage their wallets.

### 🔍 Purpose

To help users gain financial awareness and make budget management easier. The system enables control over personal financial data with features such as income-expense tracking, category-based reporting, and investment monitoring.

### ⚙️ Architecture

The project is built with .NET Core and follows a layered architecture:

- **Api**: Handles incoming HTTP requests  
- **Application**: Contains application logic (command-query structure)  
- **Domain**: Business rules and core models  
- **Infrastructure**: External services and database operations  
- **Persistence**: Data access layer with Entity Framework

### 📌 Key Features

- User registration and authentication  
- JWT-based secure access  
- Wallet and category management  
- Income, expense, and investment tracking  
- Synchronization of stock, commodity, and currency data  
- Invitation system for user collaboration

### 🏗️ Technologies

- .NET 8  
- PostgreSQL  
- Entity Framework Core  
- MediatR & FluentValidation  
- MailKit for SMTP-based email service

---

The project is under active development and will continue to grow with new features as needed.

---

## 🇹🇷 Türkçe

### 📌 Açıklama

Bu proje, kişisel finans yönetimi uygulaması olan **BudgetFlow**'un backend kısmını oluşturur. Kullanıcıların gelir, gider ve yatırım işlemlerini takip edebileceği; cüzdanlarını yönetebileceği bir API servisi sunar.

### 🔍 Amaç

Kullanıcılara finansal farkındalık kazandırmak ve bütçe yönetimini kolaylaştırmak. Gelir-gider takibi, kategori bazlı raporlama ve yatırım izleme gibi özelliklerle kişisel mali verilerin kontrolünü sağlar.

### ⚙️ Genel Yapı

Proje, .NET Core ile geliştirilmiştir ve katmanlı mimari prensipleri kullanır:

- **Api**: HTTP isteklerinin karşılandığı dış katman  
- **Application**: Uygulama mantığını barındırır (command-query yapısı)  
- **Domain**: İş kuralları ve temel modeller  
- **Infrastructure**: Harici servisler ve veritabanı işlemleri  
- **Persistence**: Entity Framework ile veri erişimi

### 📌 Temel Özellikler

- Kullanıcı kayıt ve giriş işlemleri  
- JWT tabanlı kimlik doğrulama  
- Cüzdan ve kategori oluşturma  
- Gelir/gider/yatırım işlemleri  
- Hisse senedi, emtia ve döviz verisi senkronizasyonu  
- Kullanıcılar arası davet sistemi

### 🏗️ Teknolojiler

- .NET 8  
- PostgreSQL  
- Entity Framework Core  
- MediatR & FluentValidation  
- MailKit ile SMTP e-posta desteği

---

Proje geliştirme aşamasındadır ve ihtiyaçlara göre yeni özelliklerle zenginleştirilmektedir.


## 📄 License

This project is licensed under the MIT License.

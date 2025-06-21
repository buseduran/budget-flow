# 💼 BudgetFlow Backend

Bu proje, kişisel finans yönetimi uygulaması olan **BudgetFlow**'un backend kısmını oluşturur. Kullanıcıların gelir, gider ve yatırım işlemlerini takip edebileceği; cüzdanlarını yönetebileceği bir API servisi sunar.

## 🔍 Amaç

Kullanıcılara finansal farkındalık kazandırmak ve bütçe yönetimini kolaylaştırmak. Gelir-gider takibi, kategori bazlı raporlama ve yatırım izleme gibi özelliklerle kişisel mali verilerin kontrolünü sağlar.

## ⚙️ Genel Yapı

Proje, .NET Core ile geliştirilmiştir ve katmanlı mimari prensipleri kullanır:

- **Api**: HTTP isteklerinin karşılandığı dış katman
- **Application**: Uygulama mantığını barındırır (command-query yapısı)
- **Domain**: İş kuralları ve temel modeller
- **Infrastructure**: Harici servisler ve veritabanı işlemleri
- **Persistence**: Entity Framework ile veri erişimi

## 📌 Temel Özellikler

- Kullanıcı kayıt ve giriş işlemleri
- JWT tabanlı kimlik doğrulama
- Cüzdan ve kategori oluşturma
- Gelir/gider/yatırım işlemleri
- Hisse senedi, emtia ve döviz verisi senkronizasyonu
- Kullanıcılar arası davet sistemi

## 🏗️ Teknolojiler

- .NET 8
- PostgreSQL
- Entity Framework Core
- MediatR & FluentValidation
- MailKit ile SMTP e-posta desteği

---

Proje geliştirme aşamasındadır ve ihtiyaçlara göre yeni özelliklerle zenginleştirilmektedir.

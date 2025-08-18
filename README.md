# 🚀 BackendDevTask - .NET Core 8.0 API Projesi

Bu proje, modern bir **.NET Core 8.0** altyapısı ile geliştirilmiş, **CQRS** ve **Redis Caching** örneği sunan bir backend projesidir. Mimari olarak **Onion Architecture** kullanılmıştır.
🎯 Token ile Yetkilendirme

📌 ÖNEMLİ NOT: Swagger'daki Authorize butonuna tıklayarak açılan pencereye token'ınızı yapıştırın.
⚠️ Token'ın başına Bearer yazmanıza gerek yoktur, Swagger bunu otomatik olarak ekler. 
## 🛠 1. Proje Gereksinimleri

Projeyi çalıştırabilmek için aşağıdaki yazılımların sisteminizde kurulu olması gerekmektedir:

* ✅ **.NET 8.0 SDK**
* ✅ **PostgreSQL**
* ✅ **Redis**
* ✅ **Git**

⚠️ **Not:** Projeyi çalıştırmadan önce **Redis**'in çalışır durumda olduğundan emin olun. *****************************************

## 📥 2. Projeyi Bilgisayarınıza Alın

### Visual Studio 2022
1. Visual Studio'yu açın.
2. **"Git Repository'yi Klonla"** seçeneğini seçin.
3. URL alanına aşağıdaki adresi yapıştırın ve **Klonla** butonuna tıklayın:

```
https://github.com/SerhatCiftcii/BackendDevTask.git
```

### Terminal / VS Code
Aşağıdaki Git komutunu kullanarak projeyi klonlayabilirsiniz:

```
git clone https://github.com/SerhatCiftcii/BackendDevTask.git
```

## 🔗 3. Veritabanı ve Bağlantı Ayarları

1. **PostgreSQL**'de `BackendDevTaskDb` adında bir veritabanı oluşturun.
2. `BackendDevTask.API/appsettings.json` dosyasını açın ve connection string'leri kendi bilgilerinize göre güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=BackendDevTaskDb;Username=senin_kullanıcı_adın;Password=senin_şifren",
    "RedisConnection": "localhost:6379"
  }
}
```

## 🗂 4. Veritabanı Migrasyonları

### Visual Studio 2022
1. **Araçlar > NuGet Paket Yöneticisi > Paket Yöneticisi Konsolu** yolunu izleyin.
2. Konsolda `Default project` olarak `BackendDevTask.Infrastructure`'ı seçin.
3. Migrasyon oluşturmak için aşağıdaki komutu çalıştırın:

```
Add-Migration InitialCreate
```

4. Migrasyonu veritabanına uygulamak için:

```
Update-Database
```

### Terminal / VS Code
Projenin kök dizininde aşağıdaki komutları çalıştırarak migrasyonları uygulayabilirsiniz:

```
dotnet ef migrations add InitialCreate --project BackendDevTask.Infrastructure --startup-project BackendDevTask.API
dotnet ef database update --project BackendDevTask.Infrastructure --startup-project BackendDevTask.API
```

⚠️ **Not:** Eğer hata alırsanız, PostgreSQL'in çalıştığından ve `ConnectionStrings` ayarının doğru olduğundan emin olun.

## ▶️ 5. Projeyi Başlatın

### Visual Studio 2022
1. Çözüm Gezgini'nden `BackendDevTask.API` projesine sağ tıklayın.
2. **"Başlangıç Projesi Olarak Ayarla"** seçeneğini seçin.
3. **F5** tuşuna basarak projeyi çalıştırın. Otomatik olarak **Swagger** arayüzü açılacaktır.

### Terminal / VS Code
`BackendDevTask.API` dizinine giderek projeyi çalıştırın:

```
cd BackendDevTask.API
dotnet run
```

Proje çalıştıktan sonra **Swagger** arayüzüne aşağıdaki adresten erişebilirsiniz:

```
https://localhost:port/swagger
```

## ⚡ 6. Test Etme

Swagger UI üzerinden `Auth` ve `Product` controller'larını test edebilirsiniz.

* **Product** endpoint'lerini kullanmadan önce, `/auth/login` endpoint'i ile bir **JWT token** alın.
* Swagger'daki `Authorize` butonuna tıklayarak açılan pencereye token'ınızı yapıştırın. (Token'ın başına `Bearer` yazmanıza gerek yoktur, Swagger bunu otomatik olarak ekler.)

🎉 **Proje Hazır!** Artık projeyi kullanabilir ve geliştirmeye başlayabilirsiniz.

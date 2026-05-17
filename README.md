# Takas API

Kampüs içi öğrenci takas uygulaması için ASP.NET Core Web API backend projesi.

## Teknolojiler

- ASP.NET Core Web API (.NET 8)
- EF Core
- Supabase Auth
- Supabase Database
- Swagger

## Proje Yapısı

```text
src/Takas.Api
├── Controllers
├── DTOs
├── Data
├── Entities
├── Enums
├── Extensions
├── Helpers
├── Middleware
└── Services
```

## Local .NET Çalıştırma

```bash
dotnet build src/Takas.Api/Takas.Api.csproj
dotnet run --project src/Takas.Api/Takas.Api.csproj
```

Swagger:

- `http://localhost:5050/swagger`
- `https://localhost:7050/swagger`

## Supabase Auth Akışı

- Frontend, Supabase Auth ile giriş yapar.
- API, gelen Bearer token içindeki `sub` claim değerini kullanıcı kimliği olarak kullanır.
- `email` claim e-posta olarak alınır.
- `user_metadata.full_name` varsa ad soyad olarak kullanılır.
- Admin yetkisi token’dan değil, backend `Users.Role` alanından okunur.

## Konfigürasyon

`src/Takas.Api/appsettings.json` içinde aşağıdaki ayarlar bulunur:

- `ConnectionStrings:DefaultConnection`
- `Supabase:Url`
- `Supabase:JwtSecret`
- `Supabase:Audience`

Environment variable ile override edebilirsiniz:

- `ConnectionStrings__DefaultConnection`
- `Supabase__Url`
- `Supabase__JwtSecret`
- `Supabase__Audience`

Uygulama ayrıca `DATABASE_URL` değişkenini de destekler. Bu değer Railway veya Supabase bağlantısı olarak verildiğinde otomatik olarak EF/Npgsql uyumlu connection string formatına çevrilir ve şu güvenlik ayarları eklenir:

- `SSL Mode=Require`
- `Trust Server Certificate=true`

`ConnectionStrings__DefaultConnection` tanımlıysa uygulama onu doğrudan kullanır.

## Supabase Projesi Kurulumu

1. Supabase üzerinde yeni bir proje oluşturun.
2. `Project Settings > API` ekranından proje URL’sini alın ve `Supabase__Url` olarak ekleyin.
3. Aynı ekrandan JWT secret değerini alın ve `Supabase__JwtSecret` olarak ekleyin.
4. `Supabase__Audience` için varsayılan olarak `authenticated` kullanın.
5. Frontend tarafında kullanıcı oturumu açıp API çağrılarına Supabase access token gönderin.

## Supabase Database Connection String Alma

1. Supabase dashboard içinde `Project Settings > Database` ekranına gidin.
2. Connection string bölümünden veritabanı bağlantı bilgisini alın.
3. Bunu `ConnectionStrings__DefaultConnection` olarak Railway veya local environment içine ekleyin.
4. Gerekirse connection string sonunda `SSL Mode=Require;Trust Server Certificate=true` bulunduğunu doğrulayın.

## Railway Variables

Railway deploy için aşağıdaki environment variable’ları ekleyin:

- `ConnectionStrings__DefaultConnection`
- `Supabase__Url`
- `Supabase__JwtSecret`
- `Supabase__Audience`

Railway size yalnızca `DATABASE_URL` veriyorsa uygulama bunu otomatik dönüştürür. Yine de açık bir override isterseniz `ConnectionStrings__DefaultConnection` ekleyebilirsiniz.

## Auth Endpointleri

- `POST /api/auth/register`: KVKK kabulünü mevcut kullanıcı kaydına işlemeye devam eder.
- `GET /api/auth/me`: Senkronize edilmiş mevcut kullanıcı bilgisini döner.
- `POST /api/auth/sync-user`: Supabase token ile kullanıcıyı `Users` tablosuna senkronize eder.

Response formatı korunmuştur; dış sözleşmede `keycloakId` alanı geriye dönük uyumluluk için tutulur, ancak içerideki değer artık Supabase kullanıcı kimliğidir.

## EF Core

Migration dosyaları proje içinde tutulur. Uygulama normal çalışmada `Database.Migrate()` çağırır.

Manuel migration komutları:

```bash
dotnet ef migrations add MigrationName --project src/Takas.Api/Takas.Api.csproj --startup-project src/Takas.Api/Takas.Api.csproj --output-dir Data/Migrations
dotnet ef database update --project src/Takas.Api/Takas.Api.csproj --startup-project src/Takas.Api/Takas.Api.csproj
```

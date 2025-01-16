# StockMaster Pro - Stok Takip Sistemi

StockMaster Pro, işletmelerin stok, satış ve müşteri yönetimini kolaylaştıran modern bir web uygulamasıdır.

## Özellikler

- 📦 Ürün stok takibi
- 💰 Satış yönetimi
- 👥 Müşteri yönetimi
- 🏷️ Kategori yönetimi
- 📊 Dashboard ile anlık istatistikler
- ⚠️ Düşük stok uyarıları
- 🔍 Gelişmiş arama özellikleri

## Teknolojiler

- ASP.NET MVC 5
- Entity Framework
- MS SQL Server
- Bootstrap 5
- Font Awesome
- jQuery

## Kurulum

1. Projeyi klonlayın
```bash
git clone https://github.com/eneskeltepe/StockMasterPro.git
```

2. Visual Studio'da projeyi açın

3. `Web.config` dosyasında veritabanı bağlantı dizesini kendi SQL Server'ınıza göre güncelleyin

4. Package Manager Console'da Entity Framework migration'ları çalıştırın:
```bash
Update-Database
```

5. Projeyi çalıştırın ve tarayıcıda `http://localhost:port` adresine gidin

## Ekran Görüntüleri
![Dashboard](https://github.com/user-attachments/assets/71265999-5958-42f0-a30a-c3d3010c09b8)
![Urunler](https://github.com/user-attachments/assets/452de673-f3ef-4ebc-9b74-a4785fd1eeac)
![Kategoriler](https://github.com/user-attachments/assets/60a8353b-582d-49dd-9116-aa12e1689397)
![Musteriler](https://github.com/user-attachments/assets/d0024f4b-ef85-4d22-b99f-c848e50612e3)
![Satislar](https://github.com/user-attachments/assets/2a66716b-4f28-4754-8032-a44ecd9d0cb7)
![YeniSatis](https://github.com/user-attachments/assets/0c40bfc8-d487-406c-b7d2-fe44cc58c071)


## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakınız.

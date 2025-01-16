using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcStok.Models.Entity;

namespace MvcStok.Controllers
{
    public class SatisController : Controller
    {
        // Veritabanı bağlantısı için Entity Framework context'i
        MvcDbStokEntities db = new MvcDbStokEntities();

        // GET: Satis - Ana sayfa listesi
        public ActionResult Index(string searchString)
        {
            var satislar = from s in db.TBLSATISLAR
                          select s;

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                satislar = satislar.Where(s => 
                    s.TBLURUNLER.URUNAD.Contains(searchString) ||
                    s.TBLMUSTERILER.MUSTERIAD.Contains(searchString) ||
                    s.TBLMUSTERILER.MUSTERISOYAD.Contains(searchString)
                );
            }

            return View(satislar.OrderByDescending(s => s.SATISID).ToList());
        }

        // GET: Yeni satış sayfası
        [HttpGet]
        public ActionResult YeniSatis()
        {
            // Ürün listesi için dropdown
            List<SelectListItem> urunler = (from x in db.TBLURUNLER.Where(x => x.STOK > 0)
                                          select new SelectListItem
                                          {
                                              Text = x.URUNAD + " (Stok: " + x.STOK + ")",
                                              Value = x.URUNID.ToString()
                                          }).ToList();

            // Müşteri listesi için dropdown
            List<SelectListItem> musteriler = (from x in db.TBLMUSTERILER
                                             select new SelectListItem
                                             {
                                                 Text = x.MUSTERIAD + " " + x.MUSTERISOYAD,
                                                 Value = x.MUSTERIID.ToString()
                                             }).ToList();

            // Dropdown listelerini view'a gönderiyoruz
            ViewBag.Urunler = urunler;
            ViewBag.Musteriler = musteriler;
            return View();
        }

        // POST: Yeni satış kaydetme
        [HttpPost]
        public ActionResult YeniSatis(TBLSATISLAR satis)
        {
            if (satis == null || satis.ADET <= 0)
            {
                TempData["Hata"] = "Geçersiz satış bilgileri.";
                return RedirectToAction("YeniSatis");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var urun = db.TBLURUNLER.Find(satis.URUN);
                    if (urun == null)
                    {
                        TempData["Hata"] = "Ürün bulunamadı.";
                        return RedirectToAction("YeniSatis");
                    }

                    if (urun.STOK < satis.ADET)
                    {
                        TempData["Hata"] = "Yetersiz stok. Mevcut stok: " + urun.STOK;
                        return RedirectToAction("YeniSatis");
                    }

                    // Toplam tutarı hesapla
                    satis.FIYAT = satis.ADET * urun.FIYAT;

                    // Stok güncelleme
                    urun.STOK -= satis.ADET;
                    
                    db.TBLSATISLAR.Add(satis);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Satış kaydedilirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("YeniSatis");
            }
        }

        // Satış silme işlemi
        public ActionResult Sil(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var satis = db.TBLSATISLAR.Find(id);
                if (satis == null)
                {
                    return HttpNotFound();
                }

                // Stok iade
                var urun = db.TBLURUNLER.Find(satis.URUN);
                if (urun != null)
                {
                    urun.STOK += satis.ADET;
                }

                db.TBLSATISLAR.Remove(satis);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Satış silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Satış güncelleme işlemi
        public ActionResult SatisGetir(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var satis = db.TBLSATISLAR.Find(id);
            if (satis == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> urunler = (from x in db.TBLURUNLER
                                          select new SelectListItem
                                          {
                                              Text = x.URUNAD + " (Stok: " + x.STOK + ")",
                                              Value = x.URUNID.ToString()
                                          }).ToList();

            List<SelectListItem> musteriler = (from x in db.TBLMUSTERILER
                                             select new SelectListItem
                                             {
                                                 Text = x.MUSTERIAD + " " + x.MUSTERISOYAD,
                                                 Value = x.MUSTERIID.ToString()
                                             }).ToList();

            ViewBag.Urunler = urunler;
            ViewBag.Musteriler = musteriler;

            return View(satis);
        }

        [HttpPost]
        public ActionResult Guncelle(TBLSATISLAR yeniSatis)
        {
            try
            {
                var eskiSatis = db.TBLSATISLAR.Find(yeniSatis.SATISID);
                if (eskiSatis == null)
                {
                    return HttpNotFound();
                }

                var urun = db.TBLURUNLER.Find(yeniSatis.URUN);
                if (urun == null)
                {
                    ViewBag.Hata = "Ürün bulunamadı!";
                    return View("SatisGetir", yeniSatis);
                }

                // Eğer ürün değişmediyse sadece adet farkını kontrol et
                if (eskiSatis.URUN == yeniSatis.URUN)
                {
                    int adetFarki = (int)(yeniSatis.ADET - eskiSatis.ADET);
                    if (adetFarki > urun.STOK)
                    {
                        ViewBag.Hata = "Yetersiz stok!";
                        return View("SatisGetir", yeniSatis);
                    }
                    urun.STOK = (short?)(urun.STOK - adetFarki);
                }
                else // Ürün değiştiyse
                {
                    // Eski ürünün stoğunu iade et
                    var eskiUrun = db.TBLURUNLER.Find(eskiSatis.URUN);
                    if (eskiUrun != null)
                    {
                        eskiUrun.STOK += eskiSatis.ADET;
                    }

                    // Yeni ürünün stoğunu kontrol et ve düş
                    if (yeniSatis.ADET > urun.STOK)
                    {
                        ViewBag.Hata = "Yetersiz stok!";
                        return View("SatisGetir", yeniSatis);
                    }
                    urun.STOK -= yeniSatis.ADET;
                }

                // Satış bilgilerini güncelle
                eskiSatis.URUN = yeniSatis.URUN;
                eskiSatis.MUSTERI = yeniSatis.MUSTERI;
                eskiSatis.ADET = yeniSatis.ADET;
                eskiSatis.FIYAT = yeniSatis.ADET * urun.FIYAT;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Güncelleme sırasında bir hata oluştu: " + ex.Message;
                return View("SatisGetir", yeniSatis);
            }
        }

        // Controller'ı dispose ediyoruz
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
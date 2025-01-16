using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcStok.Models.Entity;

namespace MvcStok.Controllers
{
    public class KategoriController : Controller
    {
        MvcDbStokEntities db = new MvcDbStokEntities();

        public ActionResult Index(string searchString)
        {
            var kategoriler = from k in db.TBLKATEGORILER
                            select k;

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                kategoriler = kategoriler.Where(k => k.KATEGORIAD.Contains(searchString));
            }

            return View(kategoriler.OrderBy(k => k.KATEGORIID).ToList());
        }

        [HttpGet]
        public ActionResult YeniKategori()
        {
            return View();
        }

        [HttpPost]
        public ActionResult YeniKategori(TBLKATEGORILER kategori)
        {
            if (!ModelState.IsValid)
            {
                return View("YeniKategori");
            }

            try
            {
                db.TBLKATEGORILER.Add(kategori);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Hata = "Kategori eklenirken bir hata oluştu.";
                return View();
            }
        }

        public ActionResult Sil(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            try
            {
                var kategori = db.TBLKATEGORILER.Find(id);
                if (kategori == null)
                {
                    return HttpNotFound();
                }

                // Kategoriye bağlı ürünleri kontrol et
                if (db.TBLURUNLER.Any(u => u.URUNKATEGORI == id))
                {
                    TempData["Hata"] = "Bu kategoriye ait ürünler bulunmaktadır. Önce ürünleri silmelisiniz.";
                    return RedirectToAction("Index");
                }

                db.TBLKATEGORILER.Remove(kategori);
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Hata"] = "Kategori silinirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        public ActionResult KategoriGetir(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var kategori = db.TBLKATEGORILER.Find(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }

            return View("KategoriGetir", kategori);
        }

        [HttpPost]
        public ActionResult Guncelle(TBLKATEGORILER kategori)
        {
            if (!ModelState.IsValid)
            {
                return View("KategoriGetir");
            }

            try
            {
                var guncellenecekKategori = db.TBLKATEGORILER.Find(kategori.KATEGORIID);
                if (guncellenecekKategori == null)
                {
                    return HttpNotFound();
                }

                guncellenecekKategori.KATEGORIAD = kategori.KATEGORIAD;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Hata = "Güncelleme sırasında bir hata oluştu.";
                return View("KategoriGetir");
            }
        }

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
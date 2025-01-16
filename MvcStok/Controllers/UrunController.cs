using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MvcStok.Models.Entity;

namespace MvcStok.Controllers
{
    public class UrunController : Controller
    {
        MvcDbStokEntities db = new MvcDbStokEntities();

        public ActionResult Index(string searchString)
        {
            var urunler = from u in db.TBLURUNLER
                         select u;

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                urunler = urunler.Where(u => 
                    u.URUNAD.Contains(searchString) ||
                    u.TBLKATEGORILER.KATEGORIAD.Contains(searchString)
                );
            }

            return View(urunler.OrderBy(u => u.URUNID).ToList());
        }

        [HttpGet]
        public ActionResult YeniUrun()
        {
            List<SelectListItem> degerler = (from i in db.TBLKATEGORILER.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.KATEGORIAD,
                                               Value = i.KATEGORIID.ToString()
                                           }).ToList();
            ViewBag.dgr = degerler;
            return View();
        }

        [HttpPost]
        public ActionResult YeniUrun(TBLURUNLER p1)
        {
            var kategori = db.TBLKATEGORILER.Where(x => x.KATEGORIID == p1.TBLKATEGORILER.KATEGORIID).FirstOrDefault();
            p1.TBLKATEGORILER = kategori;
            db.TBLURUNLER.Add(p1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Sil(int id)
        {
            var urun = db.TBLURUNLER.Find(id);
            db.TBLURUNLER.Remove(urun);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UrunGetir(int? id)
        {
            var urun = db.TBLURUNLER.Find(id);
            List<SelectListItem> degerler = (from i in db.TBLKATEGORILER.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.KATEGORIAD,
                                               Value = i.KATEGORIID.ToString()
                                           }).ToList();
            ViewBag.dgr = degerler;
            return View("UrunGetir", urun);
        }

        public ActionResult Guncelle(TBLURUNLER p1)
        {
            var urun = db.TBLURUNLER.Find(p1.URUNID);
            urun.URUNAD = p1.URUNAD;
            urun.MARKA = p1.MARKA;
            urun.STOK = p1.STOK;
            urun.FIYAT = p1.FIYAT;
            var ktgr = db.TBLKATEGORILER.Where(x => x.KATEGORIID == p1.TBLKATEGORILER.KATEGORIID).FirstOrDefault();
            urun.URUNKATEGORI = ktgr.KATEGORIID;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
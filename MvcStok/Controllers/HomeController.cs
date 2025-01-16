using System;
using System.Linq;
using System.Web.Mvc;
using MvcStok.Models.Entity;

namespace MvcStok.Controllers
{
    public class HomeController : Controller
    {
        private MvcDbStokEntities db = new MvcDbStokEntities();

        public ActionResult Index()
        {
            try
            {
                ViewBag.UrunSayisi = db.TBLURUNLER.Count();
                ViewBag.MusteriSayisi = db.TBLMUSTERILER.Count();
                ViewBag.KategoriSayisi = db.TBLKATEGORILER.Count();
                ViewBag.ToplamSatisTutari = db.TBLSATISLAR.Sum(x => x.FIYAT) ?? 0;

                var sonSatislar = db.TBLSATISLAR
                    .Include("TBLURUNLER")
                    .Include("TBLMUSTERILER")
                    .OrderByDescending(x => x.SATISID)
                    .Take(5)
                    .ToList();

                var stokUyari = db.TBLURUNLER
                    .Where(x => x.STOK <= 10)
                    .ToList();

                ViewBag.SonSatislar = sonSatislar;
                ViewBag.StokuAzalanUrunler = stokUyari;

                return View();
            }
            catch (Exception ex)
            {
                // Hata durumunda boş değerler atayalım
                ViewBag.UrunSayisi = 0;
                ViewBag.MusteriSayisi = 0;
                ViewBag.KategoriSayisi = 0;
                ViewBag.ToplamSatisTutari = 0;
                ViewBag.SonSatislar = new TBLSATISLAR[] { };
                ViewBag.StokuAzalanUrunler = new TBLURUNLER[] { };
                
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
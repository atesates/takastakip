using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Business.Abstract;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.DataAccess.Abstract.IlacTakip;
using WM.Core.Aspects.PostSharp.CacheAspects;
using WM.Core.CrossCuttingConcerns.Caching.Microsoft;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Core.Aspects.PostSharp.AutorizationAspects;
using System.Linq.Expressions;
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;
//using WM.Optimization.Abstract.Samples;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    public class TeklifManager : ITeklifService
    {
        #region ctor
        private ITeklifDal _teklifDal;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IAlimService _alimService;


        public TeklifManager(ITeklifDal teklifDal,
                             IEczaneGrupService eczaneGrupService,
                             IAlimService alimService,
                             IEczaneUserService eczaneUserService)
        {
            _eczaneGrupService = eczaneGrupService;
            _teklifDal = teklifDal;
            _alimService = alimService;
            _eczaneUserService = eczaneUserService;
        }
        #endregion
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int teklifId)
        {
            _teklifDal.Delete(new Teklif { Id = teklifId });
        }

        public Teklif GetById(int teklifId)
        {
            return _teklifDal.Get(x => x.Id == teklifId);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
       // [SecuredOperation(Roles = "Admin,Grup Yöneticisi,Eczane")]

        public List<Teklif> GetList()
        {
            return _teklifDal.GetList().Select(p=>new Teklif {
               // Alim = p.Alim,
                AlimMiktari = p.AlimMiktari,
                BaslangicTarihi = p.BaslangicTarihi,
                BitisTarihi = p.BitisTarihi,
                DepoFiyat = p.DepoFiyat,
                HedeflenenAlim = p.HedeflenenAlim,
                Id = p.Id,
                IlacId = p.IlacId,
                IlacMiad = p.IlacMiad,
                KayitTarihi =p.KayitTarihi,
                Maksimum = p.Maksimum,
                MalFazlasi=p.MalFazlasi,
                Minimum =p.Minimum,
                NetFiyat = p.NetFiyat,
                TeklifDurumId = p.TeklifDurumId,
                TeklifiVerenEczaneGrupId = p.TeklifiVerenEczaneGrupId,
                TeklifTurId = p.TeklifTurId,
                YayinlamaTurId = p.YayinlamaTurId

            }).ToList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(Teklif teklif)
        {
            _teklifDal.Insert(teklif);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(Teklif teklif)
        {
            _teklifDal.Update(teklif);
        }
        public TeklifDetay GetDetayById(int teklifId)
        {
            return _teklifDal.GetDetay(x => x.Id == teklifId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<TeklifDetay> GetDetaylar(Expression<Func<TeklifDetay, bool>> filter = null)
        {
            return _teklifDal.GetDetayList(filter);
        }
       
        public List<TeklifDetay> GetListByEczaneGrupId(int eczaneGrupId)
        {//grubundaki tüm teklifler
            var gruplar = _eczaneGrupService.GetList().Where(w => w.Id == eczaneGrupId).Select(s=>s.GrupId);
            var gruplardakiTumEczaneGruplar = _eczaneGrupService.GetList().Where(w => gruplar.Contains(w.GrupId)).Select(s => s.Id);
            List<TeklifDetay> teklifDetaylar = _teklifDal.GetDetayList();

            return teklifDetaylar.Where(w => gruplardakiTumEczaneGruplar.Contains(w.TeklifiVerenEczaneGrupId)).ToList();

        }
        public List<TeklifDetay> GetListByEczaneGruplar(List<int> eczaneGrupIdler, List<int> grupIdler)
        {//kendi eczaneGrupIdleri olmayacak ama kendi grubundaki grupIdlerin eczaneGrupIdleri olacak
       
            var teklifteGosterilecekEczaneGrupIdler = _eczaneGrupService.GetList()
                .Where(w=> !eczaneGrupIdler.Contains(w.Id) && grupIdler.Contains(w.GrupId))
                .Select(s => s.Id).ToList();

            List<TeklifDetay> teklifDetayList = _teklifDal.GetDetayList();

            return teklifDetayList.Where(w => teklifteGosterilecekEczaneGrupIdler
            .Contains(w.TeklifiVerenEczaneGrupId)).ToList();

        }  
        public List<TeklifDetay> GetMyListByEczaneGrupId(int eczaneGrupId)
        {//kendi teklifleri
           
            List<TeklifDetay> teklifDetay = _teklifDal.GetDetayList();

            return teklifDetay.Where(w => w.TeklifiVerenEczaneGrupId == eczaneGrupId).ToList();

        }      
        public List<Teklif> GetListByUser(User user)
        {
            //var eczaneId = _eczaneUserDal.GetList(e => e.UserId == user.Id)
            //.Select(s => s.EczaneId).FirstOrDefault();
            var eczaneId = _eczaneUserService.GetListByUserId(user.Id).Select(s => s.EczaneId).FirstOrDefault();
            return _teklifDal.GetList(w => w.TeklifiVerenEczaneGrupId == eczaneId);               
        }
        public List<Teklif> GetMyListByEczaneGruplar(List<EczaneGrupDetay> eczaneGruplar)
        {//teklifVerenEczaneGrupId leri o eczaneye ait olan teklifler döner
            List<Teklif> teklif = _teklifDal.GetList();
            var teklifIdler = teklif.Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.TeklifiVerenEczaneGrupId)).Select(s => s.Id);
            return teklif.Where(w => teklifIdler.Contains(w.Id)).ToList();
        }
        public List<Teklif> GetMyListByEczaneGruplar(List<EczaneGrup> eczaneGruplar)
        {//teklifVerenEczaneGrupId leri o eczaneye ait olan teklifler döner
            List<Teklif> teklif = _teklifDal.GetList();
            var teklifIdler = teklif.Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.TeklifiVerenEczaneGrupId)).Select(s => s.Id);
            return teklif.Where(w => teklifIdler.Contains(w.Id)).ToList();
        }
        public List<TeklifDetay> GetMyDetayListByEczaneGruplar(List<EczaneGrupDetay> eczaneGruplar)
        {//teklifVerenEczaneGrupId leri o eczaneye ait olan teklifler döner
            List<TeklifDetay> teklifDetay = _teklifDal.GetDetayList().Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.TeklifiVerenEczaneGrupId)).ToList();
            var teklifIdler = teklifDetay.Select(s => s.Id);
            return teklifDetay.Where(w => teklifIdler.Contains(w.Id)).ToList();
        }  
        public List<TeklifDetay> GetMyDetayListByEczaneGruplar(List<EczaneGrup> eczaneGruplar)
        {//teklifVerenEczaneGrupId leri o eczaneye ait olan teklifler döner
            List<TeklifDetay> teklifDetay = _teklifDal.GetDetayList();
            var teklifIdler = teklifDetay.Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.TeklifiVerenEczaneGrupId)).Select(s => s.Id);
            return teklifDetay.Where(w => teklifIdler.Contains(w.Id)).ToList();
        }
    }
}
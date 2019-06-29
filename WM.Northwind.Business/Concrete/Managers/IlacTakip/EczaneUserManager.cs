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
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    public class EczaneUserManager : IEczaneUserService
    {
        private IEczaneUserDal _eczaneUserDal;

        public EczaneUserManager(IEczaneUserDal eczaneUserDal)
        {
            _eczaneUserDal = eczaneUserDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int eczaneUserId)
        {
            _eczaneUserDal.Delete(new EczaneUser { Id = eczaneUserId });
        }

        public EczaneUser GetById(int eczaneUserId)
        {
            return _eczaneUserDal.Get(x => x.Id == eczaneUserId);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
        public List<EczaneUser> GetList()
        {
            return _eczaneUserDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(EczaneUser eczaneUser)
        {
            _eczaneUserDal.Insert(eczaneUser);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(EczaneUser eczaneUser)
        {
            _eczaneUserDal.Update(eczaneUser);
        }
        public EczaneUserDetay GetDetayById(int eczaneUserId)
        {
            return _eczaneUserDal.GetDetay(x => x.Id == eczaneUserId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<EczaneUserDetay> GetDetaylar()
        {
            return _eczaneUserDal.GetDetayList();
        }
        public List<EczaneUser> GetListByUserId(int userId)
        {
            return _eczaneUserDal.GetList(w => w.UserId == userId);
        }
        public List<EczaneUser> GetListByUser(User user)
        {
            var eczaneUserlar = _eczaneUserDal.GetList();
            var eczaneId = eczaneUserlar.Where(e => e.UserId == user.Id)
            .Select(s => s.EczaneId).FirstOrDefault();

            return _eczaneUserDal.GetList(w => w.EczaneId == eczaneId);
        }
        public List<EczaneUserDetay> GetDetayListByUser(User user)
        {
            var eczaneUserlar = _eczaneUserDal.GetList();
            var eczaneId = eczaneUserlar.Where(e => e.UserId == user.Id)
            .Select(s => s.EczaneId).FirstOrDefault();

            return _eczaneUserDal.GetDetayList(w => w.EczaneId == eczaneId);
        }
    }
}
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
using WM.Northwind.Business.Abstract.Authorization;
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;
//using WM.Optimization.Abstract.Samples;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    public class GrupManager : IGrupService
    {
        private IGrupDal _grupDal;
        private IEczaneDal _eczaneDal;
        private IUserService _userService;
        private IEczaneUserService _eczaneUserService;
        private IEczaneGrupService _eczaneGrupService;

        public GrupManager(IEczaneDal eczaneDal,
                             IEczaneUserService eczaneUserService,
                             IEczaneGrupService eczaneGrupService,
                             IUserService userService,
                             IGrupDal grupDal)
        {
            _userService = userService;
            _eczaneDal = eczaneDal;
            _eczaneGrupService = eczaneGrupService;
            _eczaneUserService = eczaneUserService;
            _grupDal = grupDal;
        }
       
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int grupId)
        {
            _grupDal.Delete(new Grup { Id = grupId });
        }

        public Grup GetById(int grupId)
        {
            return _grupDal.Get(x => x.Id == grupId);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
        public List<Grup> GetList()
        {
            return _grupDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(Grup grup)
        {
            _grupDal.Insert(grup);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(Grup grup)
        {
            _grupDal.Update(grup);
        }

        public List<Grup> GetListByUser(User user)
        {
            //user roller
            var rolIdler = _userService.GetUserRoles(user).OrderBy(s => s.RoleId).Select(u => u.RoleId).ToArray();
            var rolId = rolIdler.FirstOrDefault();

            var gruplar = new List<Grup>();

            if (rolId == 2)
            {//yetkili olduğu eczaneler
                var eczaneIdler = _eczaneUserService.GetListByUserId(user.Id).Select(x => x.EczaneId).ToList();
                
                var grupIdler = _eczaneGrupService.GetList()
                    .Where(x=> eczaneIdler.Contains(x.EczaneId)).Select(s=>s.GrupId).ToList();
                gruplar = GetList().Where(w => grupIdler.Contains(w.Id)).ToList(); 
               // gruplar = _grupDal.GetList(x => grupIdler.Contains(x.Id));
            }
            else if (rolId == 3)
            {//yetkili olduğu eczaneler
                var eczaneIdler = _eczaneUserService.GetListByUserId(user.Id).Select(x => x.EczaneId).ToList();

                var grupIdler = _eczaneGrupService.GetList()
                    .Where(x => eczaneIdler.Contains(x.EczaneId)).Select(s => s.GrupId).ToList();
                gruplar = GetList().Where(w=> grupIdler.Contains(w.Id)).ToList();
                // gruplar = _grupDal.GetList(x => grupIdler.Contains(x.Id));
            }
            else
            {//yetkili olduğu gruplar
             //var eczaneGruplar = _eczaneGrupService.GetListByUser(user).Select(g => g.Id).FirstOrDefault();

                gruplar = GetList().ToList();
            }

            return gruplar;
        }
    } 
}
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
using System.Linq.Expressions;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    public class EczaneManager : IEczaneService
    {
        private IEczaneDal _eczaneDal;
        private IUserService _userService;
        private IEczaneUserService _eczaneUserService;
        private IEczaneGrupService _eczaneGrupService;

        public EczaneManager(IEczaneDal eczaneDal,
                             IEczaneUserService eczaneUserService,
                             IEczaneGrupService eczaneGrupService,
                             IUserService userService)
        {
            _userService = userService;
            _eczaneDal = eczaneDal;
            _eczaneGrupService = eczaneGrupService;
            _eczaneUserService = eczaneUserService;

        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int eczaneId)
        {
            _eczaneDal.Delete(new Eczane { Id = eczaneId });
        }

        public Eczane GetById(int eczaneId)
        {
            return _eczaneDal.Get(x => x.Id == eczaneId);
        }
        public Eczane GetByGln(string eczaneGln)
        {
            return _eczaneDal.Get(x => x.EczaneGln == eczaneGln);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
        public List<Eczane> GetList()
        {
            return _eczaneDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(Eczane eczane)
        {
            _eczaneDal.Insert(eczane);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(Eczane eczane)
        {
            _eczaneDal.Update(eczane);
        }
        public EczaneDetay GetDetayById(int eczaneId)
        {
            return _eczaneDal.GetDetay(x => x.Id == eczaneId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<EczaneDetay> GetDetaylar(Expression<Func<EczaneDetay, bool>> filter = null)
        {
            return _eczaneDal.GetDetayList(filter);
        }
        public List<Eczane> GetListByUser(User user)
        {
            //user roller
            var rolIdler = _userService.GetUserRoles(user).OrderBy(s => s.RoleId).Select(u => u.RoleId).ToArray();
            var rolId = rolIdler.FirstOrDefault();

            var eczaneler = new List<Eczane>();

            if (rolId == 2)
            {//yetkili olduğu eczaneler
                var userEczaneler = _eczaneUserService.GetListByUserId(user.Id);
                eczaneler = GetList().Where(x => userEczaneler.Select(s => s.EczaneId).Contains(x.Id)).ToList();
            }
            else
            {//yetkili olduğu gruplar
             //var eczaneGruplar = _eczaneGrupService.GetListByUser(user).Select(g => g.Id).FirstOrDefault();

                eczaneler = GetList().ToList();
            }

            return eczaneler;
        }
    }

}
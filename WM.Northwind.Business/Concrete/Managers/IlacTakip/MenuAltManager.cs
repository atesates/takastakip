using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Business.Abstract;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.Business.ValidationRules.FluentValidation;
using WM.Northwind.DataAccess.Abstract.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Core.Aspects.PostSharp.ValidationAspects;
using WM.Core.CrossCuttingConcerns.Caching.Microsoft;
using WM.Core.Aspects.PostSharp.CacheAspects;
using WM.Core.Aspects.PostSharp.LogAspects;
using WM.Core.CrossCuttingConcerns.Logging.Log4Net.Logger;
using WM.Core.Aspects.PostSharp.PerformansCounterAspects;
using System.Threading;
using WM.Core.Aspects.PostSharp.AutorizationAspects;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    //[SecuredOperation(Roles = "Admin")]
    public class MenuAltManager : IMenuAltService
    {
        private IMenuAltDal _menuAltDal;
        
        public MenuAltManager(IMenuAltDal menuAltDal)
        {
            _menuAltDal = menuAltDal;
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        [PerformansCounterAspect(2)]
        //[SecuredOperation(Roles="Admin,Editor,Student")]
        public List<MenuAlt> GetList()
        {
            //Thread.Sleep(3000);    //performanscounterı test için yazıldı
            return _menuAltDal.GetList();
        }

        public MenuAlt GetById(int menuAltId)
        {
            return _menuAltDal.Get(x => x.Id == menuAltId);
        }

        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        [LogAspect(typeof(DatabaseLogger))]
        public void Insert(MenuAlt menuAlt)
        {
            _menuAltDal.Insert(menuAlt);
        }
        
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        [LogAspect(typeof(DatabaseLogger))]
        public void Update(MenuAlt menuAlt)
        {
            _menuAltDal.Update(menuAlt);
        }

        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        [LogAspect(typeof(DatabaseLogger))]
        public void Delete(int menuAltId)
        {
            _menuAltDal.Delete(new MenuAlt { Id = menuAltId });
        }

        public List<MenuAlt> GetByMenuId(int menuId)
        {
            return _menuAltDal.GetList(x => x.MenuId == menuId);
        }
    }
}

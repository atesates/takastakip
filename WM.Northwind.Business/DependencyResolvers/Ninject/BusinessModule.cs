using Ninject.Modules;
using System.Data.Entity;
using WM.Core.DAL;
using WM.Core.DAL.EntityFramework;
using WM.Northwind.Business.Abstract;
using WM.Northwind.Business.Abstract.Authorization;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.Business.Concrete.Managers;
using WM.Northwind.Business.Concrete.Managers.Authorization;
using WM.Northwind.Business.Concrete.Managers.IlacTakip;
using WM.Northwind.DataAccess.Abstract.IlacTakip;
using WM.Northwind.DataAccess.Abstract.Authorization;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Contexts;
using WM.Northwind.DataAccess.Concrete.EntityFramework.IlacTakip;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Authorization;
using WM.Northwind.DataAccess.Concrete.EntityFramework.EczaneNobet;

namespace WM.BLL.DependencyResolvers.Ninject
{
    public class BusinessModule : NinjectModule
    {
        public override void Load()
        {
                    
            #region Yetki

            Bind<IUserRoleService>().To<UserRoleManager>().InSingletonScope();
            Bind<IUserRoleDal>().To<EfUserRoleDal>();

            Bind<IUserService>().To<UserManager>().InSingletonScope();
            Bind<IUserDal>().To<EfUserDal>();

            Bind<IRoleService>().To<RoleManager>().InSingletonScope();
            Bind<IRoleDal>().To<EfRoleDal>();

        

            Bind<IAdminService>().To<AdminManager>().InSingletonScope();

            #endregion

            #region IlacTakip

            Bind<IEczaneService>().To<EczaneManager>().InSingletonScope();
            Bind<IEczaneDal>().To<EfEczaneDal>();

            Bind<IEczaneUserService>().To<EczaneUserManager>().InSingletonScope();
            Bind<IEczaneUserDal>().To<EfEczaneUserDal>();

            Bind<IEczaneGrupService>().To<EczaneGrupManager>().InSingletonScope();
            Bind<IEczaneGrupDal>().To<EfEczaneGrupDal>();

            Bind<IAlimService>().To<AlimManager>().InSingletonScope();
            Bind<IAlimDal>().To<EfAlimDal>();

            Bind<IAlimDurumService>().To<AlimDurumManager>().InSingletonScope();
            Bind<IAlimDurumDal>().To<EfAlimDurumDal>();

            Bind<IGrupService>().To<GrupManager>().InSingletonScope();
            Bind<IGrupDal>().To<EfGrupDal>();

            Bind<IIlacService>().To<IlacManager>().InSingletonScope();
            Bind<IIlacDal>().To<EfIlacDal>();

            Bind<IIlacDurumService>().To<IlacDurumManager>().InSingletonScope();
            Bind<IIlacDurumDal>().To<EfIlacDurumDal>();

            Bind<IITStransferDurumService>().To<ITStransferDurumManager>().InSingletonScope();
            Bind<IITStransferDurumDal>().To<EfITStransferDurumDal>();

            Bind<IKatilimService>().To<KatilimManager>().InSingletonScope();
            Bind<IKatilimDal>().To<EfKatilimDal>();

            Bind<IQRKodService>().To<QRKodManager>().InSingletonScope();
            Bind<IQRKodDal>().To<EfQRKodDal>();

            Bind<ISehirService>().To<SehirManager>().InSingletonScope();
            Bind<ISehirDal>().To<EfSehirDal>();

            Bind<ITalepService>().To<TalepManager>().InSingletonScope();
            Bind<ITalepDal>().To<EfTalepDal>();

            Bind<ITeklifDurumService>().To<TeklifDurumManager>().InSingletonScope();
            Bind<ITeklifDurumDal>().To<EfTeklifDurumDal>();

            Bind<ITeklifService>().To<TeklifManager>().InSingletonScope();
            Bind<ITeklifDal>().To<EfTeklifDal>();

            Bind<ITalepDurumService>().To<TalepDurumManager>().InSingletonScope();
            Bind<ITalepDurumDal>().To<EfTalepDurumDal>();

            Bind<ITeklifTurService>().To<TeklifTurManager>().InSingletonScope();
            Bind<ITeklifTurDal>().To<EfTeklifTurDal>();

            Bind<IYayinlamaTurService>().To<YayinlamaTurManager>().InSingletonScope();
            Bind<IYayinlamaTurDal>().To<EfYayinlamaTurDal>();

            #endregion

            #region Menu 
            Bind<IMenuService>().To<MenuManager>().InSingletonScope();
            Bind<IMenuDal>().To<EfMenuDal>();

            Bind<IMenuRoleService>().To<MenuRoleManager>().InSingletonScope();
            Bind<IMenuRoleDal>().To<EfMenuRoleDal>();

            Bind<IMenuAltService>().To<MenuAltManager>().InSingletonScope();
            Bind<IMenuAltDal>().To<EfMenuAltDal>();

            Bind<IMenuAltRoleService>().To<MenuAltRoleManager>().InSingletonScope();
            Bind<IMenuAltRoleDal>().To<EfMenuAltRoleDal>();
            #endregion

            Bind(typeof(IQueryableRepository<>)).To(typeof(EfQueryableRepository<>));
            Bind<DbContext>().To<IlacTakipContext>();
        }
    }
}

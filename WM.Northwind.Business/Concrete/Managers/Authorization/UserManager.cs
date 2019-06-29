using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Aspects.PostSharp.ValidationAspects;
using WM.Core.DAL;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Core.Aspects.PostSharp.LogAspects;
using WM.Core.CrossCuttingConcerns.Logging.Log4Net.Logger;
using WM.Northwind.Entities.Concrete.Authorization;
using WM.Northwind.DataAccess.Abstract.Authorization;
using WM.Northwind.Business.Abstract.Authorization;
using WM.Northwind.Business.ValidationRules.FluentValidation;
using WM.Core.Aspects.PostSharp.TranstionAspects;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.DataAccess.Abstract.IlacTakip;
using WM.Northwind.Business.Abstract.IlacTakip;

namespace WM.Northwind.Business.Concrete.Managers.Authorization
{
    public class UserManager : IUserService
    {
        #region ctor
        private IUserDal _userDal;
        private IUserRoleDal _userRoleDal;
        private IEczaneUserDal _eczaneUserDal;
        private IEczaneGrupDal _eczaneGrupDal;
        private IEczaneDal _eczaneDal;
        public UserManager(IUserDal userDal,
                        IUserRoleDal userRoleDal,
                        IEczaneUserDal eczaneUserDal,
                        IEczaneGrupDal eczaneGrupDal,
                        IEczaneDal eczaneDal)
        {
            _userDal = userDal;
            _userRoleDal = userRoleDal;
            _eczaneUserDal = eczaneUserDal;
            _eczaneGrupDal = eczaneGrupDal;
            _eczaneDal = eczaneDal;
        }
        #endregion
        [LogAspect(typeof(DatabaseLogger))]
        //[FluentValidationAspect(typeof(LoginItemValidator))]
        public User GetByEMailAndPassword(LoginItem loginItem)
        {
            return _userDal.Get(u => u.Email == loginItem.Email 
                                  && u.Password == loginItem.Password);
        }
        [LogAspect(typeof(DatabaseLogger))]

        public User GetByUserNameAndPassword(string userName, string password)
        {
            return _userDal.Get(u => u.UserName == userName & u.Password == password);
        }

        public User GetByUserName(string userName)
        {
            return _userDal.Get(u => u.UserName == userName);
        }
        public User GetByEmail(string Email)
        {
            return _userDal.Get(u => u.Email == Email);
        }
        public List<UserRoleItem> GetUserRoles(User user)
        {
            return _userDal.GetUserRoles(user);
        }

        public void Insert(User user)
        {
            _userDal.Insert(user);
        }

        public void Update(User user)
        {
            _userDal.Update(user);
        }

        public void Delete(int userId)
        {
            _userDal.Delete(new User {Id = userId });
        }
        [LogAspect(typeof(DatabaseLogger))]

        public List<User> GetList()
        {
            return _userDal.GetList();
        }
        [TransactionScopeAspect]
        [LogAspect(typeof(DatabaseLogger))]

        public void UserRoleEczaneGrupRegister(User user, string ezaneGln, string eczaneMail,
            string eczaneAdi, int eczaneGrupId)
        {           
            UserRole userRole = new UserRole();
            EczaneUser eczaneUser = new EczaneUser();
            EczaneGrup eczaneGrup = new EczaneGrup();
            Eczane eczane = new Eczane();
            eczane.Adi = eczaneAdi;
            eczane.EczaneGln = ezaneGln;
            eczane.Email = eczaneMail;
            eczaneGrup.GrupId = eczaneGrupId;

            eczaneGrup.BaslangicTarihi = DateTime.Now;

            _userDal.Insert(user);
            var myuser = GetByEmail(eczaneMail);
            userRole.UserId = myuser.Id;
            userRole.RoleId = 3;
            _userRoleDal.Insert(userRole);
            _eczaneDal.Insert(eczane);
            var myeczane = _eczaneDal.Get(x => x.EczaneGln == ezaneGln);
            eczaneUser.EczaneId = myeczane.Id;
            eczaneUser.UserId = myuser.Id;
            _eczaneUserDal.Insert(eczaneUser);
            eczaneGrup.EczaneId = myeczane.Id;
            _eczaneGrupDal.Insert(eczaneGrup);
        }

    }
}

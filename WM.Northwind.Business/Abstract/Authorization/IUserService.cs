using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;


namespace WM.Northwind.Business.Abstract.Authorization
{
    public interface IUserService
    {
        User GetByUserNameAndPassword(string userName, string password);
        User GetByEMailAndPassword(LoginItem loginItem);
        User GetByUserName(string userName);
        User GetByEmail(string Email);
        List<User> GetList();
        List<UserRoleItem> GetUserRoles(User user);
        void Insert(User user);
        void Update(User user);
        void Delete(int userId);
        void UserRoleEczaneGrupRegister(User user, string ezaneGln, string eczaneMail,
            string eczaneAdi, int eczaneGrupId);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Aspects.PostSharp.AutorizationAspects;
using WM.Core.Aspects.PostSharp.CacheAspects;
using WM.Core.Aspects.PostSharp.LogAspects;
using WM.Core.Aspects.PostSharp.ValidationAspects;
using WM.Core.CrossCuttingConcerns.Caching.Microsoft;
using WM.Core.CrossCuttingConcerns.Logging.Log4Net.Logger;
using WM.Core.DAL;
using WM.Northwind.Business.Abstract;
using WM.Northwind.Business.ValidationRules.FluentValidation;

namespace WM.Northwind.Business.Concrete.Managers
{
    public class AdminManager : IAdminService
    {
        [SecuredOperation(Roles = "Admin")]
        public AdminManager()
        {

        }
        
    }
}

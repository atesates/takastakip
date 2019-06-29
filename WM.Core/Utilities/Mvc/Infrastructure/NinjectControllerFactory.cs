using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace WM.Core.Utilities.Mvc.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel _kernel;

        public NinjectControllerFactory(params INinjectModule[] modules)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            _kernel = new StandardKernel(modules);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)_kernel.Get(controllerType);
        }
    }
}

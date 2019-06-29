using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using WM.BLL.DependencyResolvers.Ninject;
using WM.Core.CrossCuttingConcerns.Security.Web;
using WM.Core.Utilities.Mvc.Infrastructure;
using WM.Northwind.Business;
using WM.UI.Mvc.App_Start;
using WM.Northwind.Business.DependencyResolvers.Ninject;
using FluentValidation.Mvc;
using WM.Core.CrossCuttingConcerns.Validation.FluentValidation;

namespace WM.UI.Mvc
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory(new BusinessModule()));


            //FluentValidationModelValidatorProvider.Configure();

            FluentValidationModelValidatorProvider.Configure(provider =>
            {
                provider.ValidatorFactory = new NinjectValidationFactory(new ValidationModule());
            });

            //ModelValidatorProvider.Providers.Clear();

            //ModelValidatorProvider.Providers.Add(new FluentValidationModelValidatorProvider
            //{
            //    AddImplicitRequiredValidator = false
            //});

            //ModelBinders.Binders.DefaultBinder = new CustomDefaultModelBinder();

        }

        public override void Init()
        {
            PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        private void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie == null)
                {
                    return;
                }

                var encTicket = authCookie.Value;
                if (string.IsNullOrEmpty(encTicket))
                {
                    return;
                }

                var ticket = FormsAuthentication.Decrypt(encTicket);

                var securityUtilities = new SecurityUtilities();
                var identity = securityUtilities.FormsAuthTicketToIdentity(ticket);
                var principal = new GenericPrincipal(identity, identity.Roles);

                HttpContext.Current.User = principal;
                Thread.CurrentPrincipal = principal;
            }
            catch (Exception)
            {

            }
        }

    }
}

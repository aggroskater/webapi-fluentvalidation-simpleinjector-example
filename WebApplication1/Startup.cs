using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using FluentValidation;
using FluentValidation.WebApi;
using WebApplication1.Models;
using WebApplication1.Validators;

[assembly: OwinStartup(typeof(WebApplication1.Startup))]

namespace WebApplication1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // Start IoC bootstrap

            // configs...
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();
            container.RegisterWebApiControllers(configuration);

            // register...
            container.Register<IValidatorFactory, SimpleInjectorValidatorFactory>(Lifestyle.Singleton);
            container.Register<IValidator<RegisterBindingModel>, RegisterBindingModelValidator>(Lifestyle.Scoped);

            // verify...
            container.Verify(VerificationOption.VerifyAndDiagnose);

            configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            // end IoC bootstrap...

            FluentValidationModelValidatorProvider.Configure(configuration, provider => {
                provider.ValidatorFactory = new SimpleInjectorValidatorFactory(container);
            });

            WebApiConfig.Register(configuration);
            ConfigureAuth(app);
            app.UseWebApi(configuration);
        }
    }

    public class SimpleInjectorValidatorFactory : ValidatorFactoryBase {
        private readonly Container _container;

        public SimpleInjectorValidatorFactory(Container container) {
            _container = container;
        }

        public override IValidator CreateInstance(Type validatorType) {
            try {
                return _container.GetInstance(validatorType) as IValidator;
            } catch (Exception) {
                return null;
            }
        }
    }
}

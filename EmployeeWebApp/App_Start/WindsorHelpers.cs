using AutoMapper;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EmployeeWebApp.Mappings;
using EmployeeWebApp.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EmployeeWebApp.App_Start
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer container;

        public WindsorControllerFactory()
        {
            container = ContainerFactory.Current();
        }

        protected override IController GetControllerInstance(
            RequestContext requestContext,
            Type controllerType)
        {
            return (IController)container.Resolve(controllerType);
        }
    }

    public class ContainerFactory
    {
        private static IWindsorContainer container;
        private static readonly object SyncObject = new object();

        public static IWindsorContainer Current()
        {
            if (container == null)
            {
                lock (SyncObject)
                {
                    if (container == null)
                    {
                        container = new WindsorContainer();
                        container.Install(new ControllerInstaller());
                        container.Register(Classes.FromThisAssembly().BasedOn<Controllers.HomeController>().LifestyleTransient());
                        container.Register(Classes.FromThisAssembly().BasedOn<Controllers.EmployeeController>().LifestyleTransient());
                    }
                }
            }
            return container;
        }
    }

    public class ControllerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();
            container.Register(Component.For<IMapper>().UsingFactoryMethod(x =>
            {
                return new MapperConfiguration(c =>
                {
                    c.AddProfile<EmployeeMap>();
                }).CreateMapper();
            }));

            container.Register(Component.For<Controllers.EmployeeController>().LifestyleTransient());
            container.Register(Component.For<AppDbContext>().ImplementedBy<AppDbContext>().LifestyleTransient());
        }
    }
}
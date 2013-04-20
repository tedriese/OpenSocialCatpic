﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Catpic.Web.Configuration;
using Microsoft.Practices.Unity;

namespace Catpic.Host.Engine.Unity
{
    /// <summary>
    /// Provides ability to call unity's register/resolve by catpic's default configurator
    /// </summary>
    public class UnityHostContainer: IHostContainer
    {
        private readonly IUnityContainer _container;
        public UnityHostContainer(IUnityContainer container)
        {
            _container = container;
        }
        public bool IsRegistered<T>()
        {
            try
            {
                return _container.Resolve<T>() != null;
            }
            catch
            {
                return false;
            }
        }

        public IHostContainer RegisterType<T, C>()
        {
             _container.RegisterType(typeof (T), typeof (C));
            return this;
        }


        public IHostContainer RegisterInstance<T>(Type @interface, T instance)
        {
            _container.RegisterInstance(@interface, instance);
            return this;
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
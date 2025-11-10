using System;
using Unity;
using POC_OpenCart.DI;

namespace POC_OpenCart.Framework
{
    public abstract class TestFixture : IDisposable
    {
        #region Members
        private readonly IUnityContainer _container = IoC.Container.CreateChildContainer();
        #endregion

        #region Protected methods
        protected T Resolve<T>(string name = null)
        {
            return name == null ? _container.Resolve<T>() : _container.Resolve<T>(name);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            try
            {
                OnDispose();
            }
            catch (Exception)
            {
                //ignore
            }

            _container.Dispose();
        }

        protected virtual void OnDispose() { }
        #endregion
    }
}
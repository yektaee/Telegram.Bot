using System;

namespace Telegram.Bot.Console
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    internal class DefaultUpdateHandlerActivator : IUpdateHandlerActivator
    {
        public virtual IUpdateHandler ActivateHandler(Type handlerType)
        {
            return Activator.CreateInstance(handlerType) as IUpdateHandler;
        }

        public virtual IUpdateHandlerActivatorScope BeginScope()
        {
            return new DefaultUpdateHandlerActivatorScope(this);
        }

        private class DefaultUpdateHandlerActivatorScope : IUpdateHandlerActivatorScope
        {
            private readonly DefaultUpdateHandlerActivator _activator;

            public DefaultUpdateHandlerActivatorScope(DefaultUpdateHandlerActivator activator)
            {
                _activator = activator ?? throw new ArgumentNullException(nameof(activator));
            }

            public IUpdateHandler Resolve(Type type)
            {
                return _activator.ActivateHandler(type);
            }

            public void Dispose() { }
        }
    }
}

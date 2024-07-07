using System;

namespace DispatcherReport
{
    internal class DisposedEventHandler: IDisposable
    {
        readonly Action _action;

        public DisposedEventHandler(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action.Invoke();
        }
    }
}

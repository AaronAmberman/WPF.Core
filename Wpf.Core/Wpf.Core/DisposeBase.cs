using System;
using System.Runtime.CompilerServices;

namespace WPF.Core
{
    public abstract class DisposeBase : IDisposable
    {
        private bool disposedValue;

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources,
        // this will be done directly by the inheriting classes if they have unmanaged resources
        // ~DisposeBase()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(false);
        // }

        /// <summary>Abstract protected Dispose method inheriting classes need to fill out.</summary>
        /// <param name="disposing">True disposes managed and unmanaged resources. False disposes unmanaged resources only.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>Releases the resources used by this object.</summary>
        public void Dispose()
        {
            VerifyDisposed();

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Verifies that the object's Dispose method has not been called. Throws an exception if Dispose has already been called.
        /// This should be called as the first line of code in every public method.
        /// </summary>
        /// <param name="caller">The method calling this method. .NET will fill this in automatically because of CallerMemberName.</param>
        /// <exception cref="ObjectDisposedException">The exception thrown if Dispose has already been called.</exception>
        protected virtual void VerifyDisposed([CallerMemberName] string caller = "")
        {
            if (disposedValue)
                throw new ObjectDisposedException("WPF.Core.DisposeBase", $"{caller} cannot be accessed because the object instance has been disposed.");
        }
    }
}

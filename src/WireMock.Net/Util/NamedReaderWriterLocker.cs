using System;
using System.Collections.Concurrent;
using System.Threading;

namespace WireMock.Util
{
    /// <summary>
    /// http://johnculviner.com/achieving-named-lock-locker-functionality-in-c-4-0/
    /// </summary>
    internal class NamedReaderWriterLocker
    {
        private readonly ConcurrentDictionary<string, ReaderWriterLockSlim> _lockDict = new ConcurrentDictionary<string, ReaderWriterLockSlim>();

        public ReaderWriterLockSlim GetLock(string name)
        {
            return _lockDict.GetOrAdd(name, s => new ReaderWriterLockSlim());
        }

        public TResult RunWithReadLock<TResult>(string name, Func<TResult> body)
        {
            var rwLock = GetLock(name);
            try
            {
                rwLock.EnterReadLock();
                return body();
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public void RunWithReadLock(string name, Action body)
        {
            var rwLock = GetLock(name);
            try
            {
                rwLock.EnterReadLock();
                body();
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public TResult RunWithWriteLock<TResult>(string name, Func<TResult> body)
        {
            var rwLock = GetLock(name);
            try
            {
                rwLock.EnterWriteLock();
                return body();
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public void RunWithWriteLock(string name, Action body)
        {
            var rwLock = GetLock(name);
            try
            {
                rwLock.EnterWriteLock();
                body();
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public void RemoveLock(string name)
        {
            _lockDict.TryRemove(name, out _);
        }
    }
}
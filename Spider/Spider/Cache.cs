using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Spider
{
    /// <summary>
    /// 缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cache<T> : IDisposable
    {
        /// <summary>
        /// 缓存项
        /// </summary>
        private List<T> _list = new List<T>();
        /// <summary>
        /// 线程同步对象
        /// </summary>
        private object _obj = new object();
        /// <summary>
        /// 等待句柄
        /// </summary>
        private AutoResetEvent _event = new AutoResetEvent(false);
        /// <summary>
        /// 是否已开始
        /// </summary>
        private bool _start;
        /// <summary>
        /// 写出项回掉
        /// </summary>
        private Action<T> _callback;
        /// <summary>
        /// 写入项
        /// </summary>
        /// <param name="items"></param>
        public void Write(IEnumerable<T> items)
        {
            lock (_obj)
            {
                _list.AddRange(items);
                _event.Set();
            }
        }
        /// <summary>
        /// 写入项
        /// </summary>
        /// <param name="item"></param>
        public void Write(T item)
        {
            lock (_obj)
            {
                _list.Add(item);
                _event.Set();
            }
        }
        /// <summary>
        /// 后台写出线程循环
        /// </summary>
        private void WriteOutThread()
        {
            while (_start)
            {
                //等待信号到达
                _event.WaitOne();

                //获取缓存中的项
                T[] items;
                lock (_obj)
                {
                    items = _list.ToArray();
                    //重置信号
                    _event.Reset();
                }
                if (items.Length != 0 && _start)
                {
                    foreach (var item in items)
                    {
                        _callback(item);
                    }
                }
            }
        }

        /// <summary>
        /// 开始写出
        /// </summary>
        /// <param name="callback"></param>
        public void Begin(Action<T> callback)
        {
            if (_start)
                throw new InvalidOperationException();
            if (callback == null)
                throw new ArgumentNullException();
            //开始后台线程写出缓存项
            _callback = callback;
            _event.Reset();
            _start = true;
            new Thread(WriteOutThread) { IsBackground = true }.Start();
        }
        /// <summary>
        /// 结束写出
        /// </summary>
        public void End()
        {
            if (_start)
            {
                _start = false;
                _event.Set();
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            End();
        }

        ~Cache()
        {
            Dispose(false);
        }

        #endregion
    }
}

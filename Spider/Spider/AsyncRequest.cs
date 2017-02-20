using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Spider
{
    /// <summary>
    /// 请求项接口
    /// </summary>
    public interface IRequestItem
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        string Uri { get; }
    }
    /// <summary>
    /// 请求项
    /// </summary>
    public class RequestUri : IRequestItem
    {
        #region IRequestItem 成员
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Uri
        {
            get;
            set;
        }

        #endregion
        public RequestUri()
        {
        }
        public RequestUri(string uri)
        {
            Uri = uri;
        }
    }
    /// <summary>
    /// 请求项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestItem<T> : IRequestItem
    {
        #region IRequestItem 成员
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Uri
        {
            get;
            set;
        }

        #endregion
        /// <summary>
        /// 状态
        /// </summary>
        public T State
        {
            get;
            set;
        }

        public RequestItem()
        {
        }

        public RequestItem(string uri, T state)
        {
            Uri = uri;
            State = state;
        }
    }

    /// <summary>
    /// 异步请求
    /// </summary>
    public class AsyncRequest
    {
        /// <summary>
        /// 所有请求项
        /// </summary>
        private IRequestItem[] _items;
        /// <summary>
        /// 并发数
        /// </summary>
        private int _parallel;
        /// <summary>
        /// 已完成数
        /// </summary>
        private int _completedCount;
        /// <summary>
        /// 是否已开始
        /// </summary>
        private bool _begin;
        /// <summary>
        /// 下一项索引
        /// </summary>
        private int _next;
        /// <summary>
        /// 是否已取消
        /// </summary>
        private bool _canceled;
        /// <summary>
        /// 外部回调
        /// </summary>
        private Action<WebResponse,Exception, IRequestItem> _callback;
        /// <summary>
        /// 线程同步对象
        /// </summary>
        private object _obj = new object();
        /// <summary>
        /// 请求完成时发生
        /// </summary>
        public event Action RequestCompleted;

        /// <summary>
        /// 发起异步请求
        /// </summary>
        /// <param name="items">请求项</param>
        /// <param name="parallel">并发数</param>
        /// <param name="callback">每项请求的回调</param>
        public void BeginRequest(IRequestItem[] items, int parallel, Action<WebResponse, Exception, IRequestItem> callback)
        {
            //校验参数
            if (_begin)
                throw new InvalidOperationException();
            if (items == null || callback == null)
                throw new ArgumentNullException();
            if (items.Length == 0 || items.Any(t => t == null))
                throw new ArgumentException();
            if (parallel < 1 || parallel > 100)
                throw new ArgumentOutOfRangeException();
            //初始化参数
            _items = items;
            _callback = callback;
            _parallel = Math.Min(parallel, items.Length);
            _next = 0;
            _completedCount = 0;
            _begin = true;
            //设置并发连接数
            if (System.Net.ServicePointManager.DefaultConnectionLimit < _parallel)
                System.Net.ServicePointManager.DefaultConnectionLimit = _parallel;
            //发起并发请求
            for (int i = 0; i < _parallel; i++)
                NextRequest();
        }
        /// <summary>
        /// 请求回调
        /// </summary>
        /// <param name="result"></param>
        private void Response(IAsyncResult result)
        {
            var array = result.AsyncState as object[];
            var request = array[0] as WebRequest;
            var item = array[1] as IRequestItem;

            WebResponse response = null;
            Exception error = null;
            try
            {
                response = request.EndGetResponse(result);
            }
            catch (Exception e)
            {
                error = e;
            }
            //外部回调
            if (!_canceled)
                _callback(response, error, item);
            if(response != null)
                response.Close();
            if (!_canceled)
                NextRequest();
        }
        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual WebRequest CreateRequest(IRequestItem item)
        {
            return WebRequest.Create(item.Uri);
        }
        /// <summary>
        /// 下一个请求
        /// </summary>
        private void NextRequest()
        {
            IRequestItem item = null;
            lock (_obj)
            {
                if (_next < _items.Length)
                {
                    item = _items[_next++];
                }
            }
            if (item != null)
            {
                Exception error = null;
                try
                {
                    var request = CreateRequest(item);
                    request.BeginGetResponse(Response, new object[] { request, item });
                }
                catch (Exception e)
                {
                    error = e;
                }
                //错误回掉
                if (error != null)
                {
                    _callback(null, error, item);
                    //开始下一项请求
                    NextRequest();
                }
            }
            //请求完成计数
            else if (++_completedCount == _parallel)
            {
                if (RequestCompleted != null)
                    RequestCompleted();
            }
        }
        /// <summary>
        /// 取消请求
        /// </summary>
        public void Cancel()
        {
            if (!_begin)
                throw new InvalidOperationException();
            if (!_canceled)
                _canceled = true;
        }
    }
}

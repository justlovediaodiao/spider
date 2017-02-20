using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Spider
{
    /// <summary>
    /// http访问类
    /// </summary>
    public class Http
    {
        /// <summary>
        /// 获取请求结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url)
        {
            return Get(url, Encoding.UTF8);
        }
        /// <summary>
        /// 获取请求结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Get(string url, Encoding encoding)
        {
            var req = WebRequest.Create(url);
            var res = req.GetResponse();
            return ReadResponse(res, encoding);
        }
        /// <summary>
        /// 获取请求数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] GetData(string url)
        {
            var req = WebRequest.Create(url);
            var res = req.GetResponse();
            return ReadResponseData(res);
        }
        /// <summary>
        /// 读取响应结果
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static string ReadResponse(WebResponse res)
        {
            return ReadResponse(res, Encoding.UTF8);
        }
        /// <summary>
        /// 读取响应结果
        /// </summary>
        /// <param name="res"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadResponse(WebResponse res, Encoding encoding)
        {
            using (var reader = new StreamReader(res.GetResponseStream(), encoding))
            {
                return reader.ReadToEnd();
            }
        }
        /// <summary>
        /// 读取响应数据
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static byte[] ReadResponseData(WebResponse res)
        {
            //chunk块传输的响应流暂不支持
            if (res.ContentLength == -1)
                throw new NotSupportedException();
            var buffer = new byte[res.ContentLength];
            var len = 0;
            using (var stream = res.GetResponseStream())
            {
                while ((len += stream.Read(buffer, len, buffer.Length - len)) < buffer.Length) ;
            }
            return buffer;
        }
        /// <summary>
        /// Post键值对
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static WebResponse Post(string url, IEnumerable<KeyValuePair<string, string>> data)
        {
            return Post(url, string.Join(",", data.Select(t => t.Key + "=" + t.Value).ToArray()));
        }
        /// <summary>
        /// Post数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static WebResponse Post(string url, string data)
        {
            var req = WebRequest.Create(url);
            req.Method = "POST";
            using (var stream = req.GetRequestStream())
            {
                var buffer = Encoding.UTF8.GetBytes(data);
                stream.Write(buffer, 0, buffer.Length);
            }
            return req.GetResponse();
        }
    }
}

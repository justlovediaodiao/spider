using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Spider
{
    class Spider
    {
        private int _parallel = 10;

        private int _page;

        private string _url;

        private string _folder;

        private DiskCache _disk = new DiskCache();

        private Parser _parser = new Parser();

        public void Start(string url, string folder)
        {
            _url = url;
            _folder = folder;
            _disk.Begin();
            StartNext();
        }

        private void StartNext()
        {
            ++_page;
            var url = _url + "&page=" + _page;
            var html = Http.Get(url);
            var items = _parser.ParseList(html).ToArray();
            if (items.Length != 0)
            {
                Console.WriteLine("start page " + _page);
                var req = new AsyncRequest();
                req.RequestCompleted += PageCompleted;
                req.BeginRequest(items, _parallel, Download);
            }
            else
            {
                Console.WriteLine("completed");
            }
        }

        private void Download(WebResponse res, Exception error, IRequestItem item)
        {
            if (error == null)
            {
                var html = Http.ReadResponse(res);
                var url = _parser.ParseUrl(html);
                if (url != null)
                {
                    var content = Http.GetData(url);
                    _disk.Write("", content);
                }
            }
            else
            {
                Console.WriteLine(error.Message);
            }
        }

        private void PageCompleted()
        {
            StartNext();
        }
    }
}

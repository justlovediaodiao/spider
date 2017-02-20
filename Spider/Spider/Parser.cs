using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    interface IParser
    {
        IEnumerable<IRequestItem> ParseList(string html);
        string ParseUrl(string html);
    }

    class Parser : IParser
    {
        public IEnumerable<IRequestItem> ParseList(string html)
        {
            throw new NotImplementedException();
        }

        public string ParseUrl(string html)
        {
            throw new NotImplementedException();
        }
    }
}

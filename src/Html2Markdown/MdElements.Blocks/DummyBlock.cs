using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html2Markdown.MdElements.Blocks
{
    class DummyBLock : IMdBlock
    {
        public static readonly DummyBLock Instance = new();

        public IEnumerable<string> ToMarkdown()
        {
            throw new NotImplementedException();
        }
    }
}

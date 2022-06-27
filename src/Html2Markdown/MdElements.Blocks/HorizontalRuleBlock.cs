using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html2Markdown.MdElements.Blocks
{
    public class HorizontalRuleBlock : IMdBlock
    {
        public IEnumerable<string> ToMarkdown()
        {
            yield return "* * *";
        }
    }
}

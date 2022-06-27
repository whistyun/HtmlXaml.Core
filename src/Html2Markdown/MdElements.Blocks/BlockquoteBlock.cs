using System.Collections.Generic;
using System.Linq;
using Html2Markdown.Utils;

namespace Html2Markdown.MdElements.Blocks
{
    public class BlockquoteBlock : IMdBlock
    {
        public IEnumerable<IMdBlock> Content { get; }

        public BlockquoteBlock(IEnumerable<IMdBlock> content)
        {
            Content = content;
        }

        public IEnumerable<string> ToMarkdown()
        {
            bool isRepeated = false;

            foreach (var block in Content)
            {
                if (isRepeated)
                {
                    // insert empty line
                    yield return "> ";
                }

                foreach (var line in block.ToMarkdown().SelectMany(ln => ln.SplitLine()))
                {
                    yield return "> " + line;
                }
            }
        }
    }
}

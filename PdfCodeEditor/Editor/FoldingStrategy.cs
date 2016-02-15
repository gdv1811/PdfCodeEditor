// Copyright (c) 2016 Dmitry Goryachev
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace PdfCodeEditor.Editor
{
    internal class FoldingStrategy
    {
        public List<FoldingTemplate> FoldingTemplates { get; set; }

        public FoldingManager FoldingManager { get; set; }

        /// <summary>
        /// Creates a new FoldingStrategy.
        /// </summary>
        public FoldingStrategy(FoldingManager foldingManager)
        {
            FoldingManager = foldingManager;
        }

        /// <summary>
        /// Creates a new FoldingStrategy.
        /// </summary>
        public FoldingStrategy()
        {
        }

        public void UpdateFoldings(TextDocument document)
        {
            int firstErrorOffset;
            var newFoldings = CreateNewFoldings(document, out firstErrorOffset);
            FoldingManager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            var newFoldings = new List<NewFolding>();

            var template = FoldingTemplates[0];
            var regexOpenFolding = new Regex(template.OpeningPhrase);
            var matchesOpenFolding = regexOpenFolding.Matches(document.Text);

            var regexCloseFolding = new Regex(template.ClosingPhrase);
            var matchesCloseFolding = regexCloseFolding.Matches(document.Text);

            var currentOpenIndex = 0;
            for (var i = 0; i < matchesCloseFolding.Count && currentOpenIndex < matchesOpenFolding.Count; i++)
            {
                if (matchesOpenFolding[currentOpenIndex].Index >= matchesCloseFolding[i].Index)
                    continue;
                var folding = new NewFolding(matchesOpenFolding[currentOpenIndex].Index + 1,
                    matchesCloseFolding[i].Index + 10)
                {
                    DefaultClosed = template.IsDefaultFolded,
                    Name = template.Name
                };
                newFoldings.Add(folding);
                while (currentOpenIndex < matchesOpenFolding.Count &&
                    matchesOpenFolding[currentOpenIndex].Index < matchesCloseFolding[i].Index)
                {
                    currentOpenIndex++;
                }
            }
            return newFoldings;
        }
    }
}

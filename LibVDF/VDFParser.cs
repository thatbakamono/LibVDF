using System;
using System.IO;
using System.Collections.Generic;

namespace LibVDF
{
    public class VDFParser
    {
        public VDFObject ParseFile(string file)
        {
            if (File.Exists(file))
            {
                return ParseObject(new List<string>(File.ReadAllLines(file)));
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public VDFObject ParseContent(string content)
        {
            if (content.Contains(Environment.NewLine))
            {
                return ParseObject(new List<string>(content.Split(Environment.NewLine.ToCharArray())));
            }
            else
            {
                return ParseObject(new List<string> { content });
            }
        }

        public VDFObject ParseObject(List<string> lines)
            => ParseObject(lines, 0);

        private VDFObject ParseObject(List<string> lines, int lineOffset)
        {
            var values = new Dictionary<string, object>();
            var names = new Stack<string>();

            var depth = 0;
            var expectOpeningBracket = false;
            var expectedClosingBrackets = 0;
            var openingBracketLines = new Stack<int>();

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                var lineNumber = i + lineOffset + 1;

                //Skip empty lines
                if (line.Equals(""))
                    continue;

                //Skip comments
                if (line.StartsWith("//"))
                    continue;

                if (line.Equals("{"))
                {
                    if (expectOpeningBracket)
                    {
                        depth++;
                        expectOpeningBracket = false;
                        expectedClosingBrackets++;
                        openingBracketLines.Push(i);

                        continue;
                    }
                    else
                    {
                        throw new SyntaxErrorException($"Line: {lineNumber}, didn't expect an opening bracket but found one", lineNumber);
                    }                
                }
                else
                {
                    if (expectOpeningBracket)
                    {
                        throw new SyntaxErrorException($"Line: {lineNumber}, expected an opening bracket but didn't find one", lineNumber);
                    }
                }

                if (line.Equals("}"))
                {
                    if (expectedClosingBrackets > 0)
                    {
                        depth--;
                        expectedClosingBrackets--;

                        var start = openingBracketLines.Pop() + 1;
                        var end = i - 1;

                        values.Add(names.Pop(), ParseObject(lines.GetRange(start, end - (start - 1)), start));

                        continue;
                    }
                    else
                    {
                        throw new SyntaxErrorException($"Line: {lineNumber}, didn't expect an closing bracket but found one", lineNumber);
                    }
                }

                if (line.StartsWith("\""))
                {
                    var checkedText = line.Substring(1);
                    var indexOfQuotationMark = checkedText.IndexOf('\"');

                    if (indexOfQuotationMark != -1)
                    {
                        names.Push(checkedText.Substring(0, indexOfQuotationMark));
                        checkedText = checkedText.Substring(indexOfQuotationMark + 1);

                        if (checkedText.TrimStart(' ').StartsWith("\""))
                        {
                            checkedText = checkedText.Substring(checkedText.Substring(1).IndexOf("\"") + 2);

                            if (checkedText.Contains("\""))
                            {
                                if (checkedText.CountOf('\"') == 1)
                                {
                                    indexOfQuotationMark = checkedText.IndexOf("\"");
                                    values.Add(names.Pop(), checkedText.Substring(0, indexOfQuotationMark));
                                }
                                else
                                {
                                    throw new SyntaxErrorException($"Line: {lineNumber}, found too many quotation marks", lineNumber);
                                }
                            }
                            else
                            {
                                throw new SyntaxErrorException($"Line: {lineNumber}, expected a quotation mark after the value but didn't find one", lineNumber);
                            }
                        }
                        else
                        {
                            expectOpeningBracket = true;
                        }
                    }
                    else
                    {
                        throw new SyntaxErrorException($"Line: {lineNumber}, expected a quotation mark after the name but didn't find one", lineNumber);
                    }
                }
                else
                {
                    throw new SyntaxErrorException($"Line: {lineNumber}, expected a quotation mark before the name but didn't find one", lineNumber);
                }
            }

            return new VDFObject(values);
        }
    }
}
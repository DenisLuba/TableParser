using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TableParser;

[TestFixture]
public class FieldParserTaskTests
{
    [TestCase("text", new[] { "text" })]
    [TestCase("hello world", new[] { "hello", "world" })]
    [TestCase("hello \"world\"", new[] { "hello", "world" })]
    [TestCase("\"hello world", new[] { "hello world" })]
    [TestCase("''", new[] { "" })]
    [TestCase("a\"b\"", new[] { "a", "b" })]
    [TestCase("'a'b", new[] { "a", "b" })]
    [TestCase("  b", new[] { "b" })]
    // [TestCase("'\\\\'", new[] { "\\" })]
    [TestCase("'\"", new[] { "\"" })]
    [TestCase(@"""''""", new[] { "''" })]
    [TestCase(@"""a \""c\""""", new[] { @"a ""c""" })]
    [TestCase("", new string[0])]
    [TestCase("a  b", new[] { "a", "b" })]
    [TestCase(@"'\''", new[] { "'" })]
    [TestCase(@"' ", new[] { " " })]
    [TestCase(@"aa ", new[] { "aa" })]
    public static void RunTests(string input, string[] expectedOutput)
        => Test(input, expectedOutput);

    private static void Test(string input, string[] expectedResult)
    {
        var actualResult = FieldsParserTask.ParseLine(input);
        Assert.AreEqual(expectedResult.Length, actualResult.Count);
        for (int i = 0; i < expectedResult.Length; ++i)
        {
            Assert.AreEqual(expectedResult[i], actualResult[i].Value);
        }
    }
}

public class FieldsParserTask
{
    public static List<Token> ParseLine(string line)
    {
        var result = new List<Token>();
        var i = 0;
        while (i < line.Length)
        {
            var token = ReadField(line, i);
            if (token == null) break;
            result.Add(token);
            i = token.GetIndexNextToToken();
        }

        return result;
    }

    private static Token? ReadField(string line, int startIndex)
    {
        while (startIndex < line.Length && line[startIndex] == ' ') startIndex++;
        if (startIndex >= line.Length) return null;

        return line[startIndex] == '\'' || line[startIndex] == '\"'
            ? ReadQuotedField(line, startIndex)
            : ReadSimpleField(line, startIndex);
    }

    private static Token ReadQuotedField(string line, int startIndex) =>
        QuotedFieldTask.ReadQuotedField(line, startIndex);

    private static Token ReadSimpleField(string line, int startIndex)
    {
        line = new string(line[startIndex..]
                .TakeWhile(ch => ch != '\'' && ch != '\"' && ch != ' ')
                .ToArray())
            .Replace(" ", "");
        return new Token(line, startIndex, line.Length);
    }
}
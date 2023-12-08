using System.Text.RegularExpressions;
using NUnit.Framework;

namespace TableParser;

[TestFixture]
public class QuotedFieldTaskTests
{
    [TestCase("''", 0, "", 2)]
    [TestCase("'a'", 0, "a", 3)]
    [TestCase(@"ab""a b\""", 2, @"a b""", 6)]
    [TestCase("\"abc\"", 0, "abc", 5)]
    [TestCase("b \"a'\"", 2, "a'", 4)]
    [TestCase("'a'b", 0, "a", 3)]
    [TestCase("a'b'", 1, "b", 3)]
    [TestCase(@"'a\' b'", 0, "a' b", 7)]
    [TestCase(@"some_text ""QF \"""" other_text", 10, "QF \"", 7)]
    

    public void Test(string line, int startIndex, string expectedValue, int expectedLength)
    {
        var actualToken = QuotedFieldTask.ReadQuotedField(line, startIndex);
        Assert.AreEqual(new Token(expectedValue, startIndex, expectedLength), actualToken);
    }
}

class QuotedFieldTask
{
    public static Token ReadQuotedField(string line, int startIndex)
    { ;
        var quote = line[startIndex];
        int tokenLength;
        var tokenValue = line[startIndex..];
        Regex regex = new($@"{quote}.*?(?<![^\\]\\){quote}");
        MatchCollection matches = regex.Matches(tokenValue);
        if (matches.Count > 0)
        {
            tokenLength = matches[0].Value.Length;
            tokenValue = Replace(matches[0].Value)[1..^1];
        }
        else
        {
            tokenLength = line[startIndex..].Length;
            tokenValue = Replace(line[(startIndex + 1)..]);
        }
        
        return new Token(tokenValue, startIndex, tokenLength);
    }
    
    private static string Replace(string text) => text
        .Replace(@"\\", @"\")
        .Replace(@"\'", @"'")
        .Replace(@"\""", @"""");
}
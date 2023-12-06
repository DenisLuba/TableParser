using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace TableParser;

[TestFixture]
public class QuotedFieldTaskTests
{
    [TestCase("''", 0, "", 2)]
    [TestCase("'a'", 0, "a", 3)]
    [TestCase(@"a ""c""", 2, @"c""", 4)]
    public void Test(string line, int startIndex, string expectedValue, int expectedLength)
    {
        var actualToken = QuotedFieldTask.ReadQuotedField(line, startIndex);
        Assert.AreEqual(new Token(expectedValue, startIndex, expectedLength), actualToken);
    }

    // Добавьте свои тесты
}

class QuotedFieldTask
{
    public static Token ReadQuotedField(string line, int startIndex)
    {
        var quote = line[startIndex];
        int tokenLength;
        var tokenValue = line[startIndex..];
        Regex regex = new($@"{quote}.+?(?<![^\\]\\){quote}");
        MatchCollection matches = regex.Matches(tokenValue);
        if (matches.Count > 0)
        {
            tokenValue = matches[0]
                .Value
                .Replace(@"\\", @"\")
                .Replace(@"\'", @"'")
                .Replace(@"\""", @"""");
            tokenLength = tokenValue.Length;
            tokenValue = tokenValue.EndsWith(quote) ? 
                tokenValue[1..^1] : 
                tokenValue[1..];
        }
        else
        {
            tokenValue = line[(startIndex + 1)..];
            tokenLength = tokenValue.Length + 1;
        }
        
        return new Token(tokenValue, startIndex, tokenLength);
    }
}
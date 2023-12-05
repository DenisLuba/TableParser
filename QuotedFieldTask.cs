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

/// <summary>
/// 
/// </summary>
/// 
class QuotedFieldTask
{
    public static Token ReadQuotedField(string line, int startIndex)
    {
        Regex regex = new(@""".+?(?<![^\\]\\)""");
        MatchCollection matches = regex.Matches(line[startIndex..]);
        line = (matches.FirstOrDefault()?.Value ?? "")
            .Replace(@"\\", @"\")
            .Replace(@"\""", @"""");
        
        return new Token(line, startIndex, line.Length - startIndex);
    }
}
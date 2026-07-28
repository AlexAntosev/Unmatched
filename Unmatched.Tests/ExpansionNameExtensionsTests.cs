namespace Unmatched.Tests;

using Unmatched.Extensions;

public class ExpansionNameExtensionsTests
{
    [Theory]
    [InlineData("Unmatched: Cobble & Fog", "Cobble & Fog")]
    [InlineData("Unmatched: Battle of Legends, Volume One", "Battle of Legends, Volume One")]
    [InlineData("Unmatched Adventures: Tales to Amaze", "Tales to Amaze")]
    [InlineData("Unmatched Marvel: Teen Spirit", "Teen Spirit")]
    [InlineData("Unmatched: Robin Hood vs. Bigfoot", "Robin Hood vs. Bigfoot")]
    public void ToShortName_DropsTheSharedPrefix(string fullName, string expected)
    {
        Assert.Equal(expected, fullName.ToShortName());
    }

    [Fact]
    public void ToShortName_LeavesNamesWithoutAKnownPrefixAlone()
    {
        Assert.Equal("Homebrew Box", "Homebrew Box".ToShortName());
    }

    [Fact]
    public void ToShortName_PassesNullAndBlankThrough()
    {
        Assert.Null(((string?)null).ToShortName());
        Assert.Equal("  ", "  ".ToShortName());
    }
}

namespace Unmatched.Tests;

using Unmatched.Dtos;

public class VillainHpScalingTests
{
    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(3, 30)]
    [InlineData(4, 40)]
    public void EffectiveHp_PerPlayerVillain_ScalesLinearlyFromBase(int players, int expected)
    {
        // Nettle / the Martian Invaders: "10 HP per player" is just the case where the per-extra-player
        // increment happens to equal the base.
        var villain = new VillainDto { BaseHp = 10, HpPerExtraPlayer = 10 };

        Assert.Equal(expected, villain.EffectiveHp(players));
    }

    [Theory]
    [InlineData(1, 14)]
    [InlineData(2, 21)]
    [InlineData(3, 28)]
    [InlineData(4, 35)]
    public void EffectiveHp_ShredderStylePattern_AddsLessThanBasePerExtraPlayer(int players, int expected)
    {
        var villain = new VillainDto { BaseHp = 14, HpPerExtraPlayer = 7 };

        Assert.Equal(expected, villain.EffectiveHp(players));
    }

    [Fact]
    public void EffectiveHp_SinglePlayer_IsExactlyTheBase()
    {
        var villain = new VillainDto { BaseHp = 14, HpPerExtraPlayer = 7 };

        Assert.Equal(14, villain.EffectiveHp(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void EffectiveHp_ZeroOrFewerPlayers_NeverGoesBelowTheBase(int players)
    {
        var villain = new VillainDto { BaseHp = 14, HpPerExtraPlayer = 7 };

        Assert.Equal(14, villain.EffectiveHp(players));
    }
}

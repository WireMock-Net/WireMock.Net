// Copyright © WireMock.Net

using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace WireMock.Services;

internal class RandomizerDoubleBetween0And1 : IRandomizerDoubleBetween0And1
{
    private readonly IRandomizerNumber<double> _randomizerDoubleBetween0And1 = RandomizerFactory.GetRandomizer(new FieldOptionsDouble { Min = 0, Max = 1 });

    public double Generate()
    {
        return _randomizerDoubleBetween0And1.Generate() ?? 0;
    }
}
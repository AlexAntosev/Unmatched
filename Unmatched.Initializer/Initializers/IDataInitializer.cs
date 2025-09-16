namespace Unmatched.Initializer.Initializers;

public interface IDataInitializer
{
    //Task InitializeHeroesAsync();

    Task InitializeMapsAsync();

    Task InitializeMinionsAsync();

    Task InitializeVillainsAsync();
}

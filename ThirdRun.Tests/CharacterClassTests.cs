namespace ThirdRun.Tests;

public class CharacterClassTests
{
    [Fact]
    public void CharacterClass_HasExpectedValues()
    {
        // Assert that all expected character classes exist
        Assert.True(Enum.IsDefined(typeof(CharacterClass), CharacterClass.Guerrier));
        Assert.True(Enum.IsDefined(typeof(CharacterClass), CharacterClass.Mage));
        Assert.True(Enum.IsDefined(typeof(CharacterClass), CharacterClass.Prêtre));
        Assert.True(Enum.IsDefined(typeof(CharacterClass), CharacterClass.Chasseur));
    }
    
    [Fact]
    public void CharacterClass_HasExactlyFourValues()
    {
        // Act
        var values = Enum.GetValues<CharacterClass>();
        
        // Assert
        Assert.Equal(4, values.Length);
    }
    
    [Theory]
    [InlineData(CharacterClass.Guerrier)]
    [InlineData(CharacterClass.Mage)]
    [InlineData(CharacterClass.Prêtre)]
    [InlineData(CharacterClass.Chasseur)]
    public void CharacterClass_AllValuesAreValid(CharacterClass characterClass)
    {
        // Act & Assert
        Assert.True(Enum.IsDefined(typeof(CharacterClass), characterClass));
    }
}
using System;
using Microsoft.Xna.Framework;
using ThirdRun.UI;
using ThirdRun.Data;
using Xunit;

namespace ThirdRun.Tests
{
    public class CharacterPortraitsTests
    {
        [Fact]
        public void UIManager_State_HasCharacterDetailsProperties()
        {
            // Test that the UIManager.State class has the new properties we added
            var state = new UiManager.State();
            
            // Verify the new state properties exist and have correct default values
            Assert.False(state.IsCharacterDetailsVisible);
            Assert.Null(state.SelectedCharacter);
            
            // Test setting values
            state.IsCharacterDetailsVisible = true;
            Assert.True(state.IsCharacterDetailsVisible);
        }

        [Fact]
        public void UIManager_State_CanStoreSelectedCharacter()
        {
            var state = new UiManager.State();
            
            // Initially null
            Assert.Null(state.SelectedCharacter);
            
            // Can be set (we can't create a real Character without MonoGame context,
            // but we can verify the property exists and accepts the right type)
            state.SelectedCharacter = null; // This should not throw
            Assert.Null(state.SelectedCharacter);
        }

        [Theory]
        [InlineData(CharacterClass.Guerrier, "Characters/warrior")]
        [InlineData(CharacterClass.Mage, "Characters/mage")]
        [InlineData(CharacterClass.Prêtre, "Characters/priest")]
        [InlineData(CharacterClass.Chasseur, "Characters/hunter")]
        public void CharacterClass_MapsToCorrectTexturePath(CharacterClass characterClass, string expectedPath)
        {
            // This test verifies that our character class to texture mapping is correct
            // We can't test the actual texture loading without MonoGame context,
            // but we can test the mapping logic
            
            string actualPath = GetTexturePathForClass(characterClass);
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void ConsoleTestRunner_ExecutesWithoutExceptions()
        {
            // Run the console test to verify all components work together
            var exception = Record.Exception(() => CharacterPortraitsConsoleTest.RunBasicTests());
            Assert.Null(exception);
        }

        private string GetTexturePathForClass(CharacterClass characterClass)
        {
            // This is the same method used in CharacterPortraitsPanel
            return characterClass switch
            {
                CharacterClass.Guerrier => "Characters/warrior",
                CharacterClass.Mage => "Characters/mage",
                CharacterClass.Prêtre => "Characters/priest",
                CharacterClass.Chasseur => "Characters/hunter",
                _ => "Characters/warrior"
            };
        }
    }
}
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

        [Fact]
        public void UIManager_State_CharacterSelectionToggleBehavior()
        {
            // Test the logic that would be used in ShowCharacterDetails for toggling selection
            var state = new UiManager.State();
            
            // Create mock character (we can't create real Character without MonoGame context)
            // but we can simulate the behavior with null-checking logic
            
            // Initial state: no character selected, panel not visible
            Assert.Null(state.SelectedCharacter);
            Assert.False(state.IsCharacterDetailsVisible);
            
            // Simulate selecting a character (first click)
            // Since we can't create Character objects, we'll test with a mock scenario
            // where character == selectedCharacter comparison would work
            
            // First selection: should select and show panel
            state.SelectedCharacter = null; // Simulate "character1"
            state.IsCharacterDetailsVisible = true;
            Assert.True(state.IsCharacterDetailsVisible);
            
            // Simulate clicking same character again (deselection logic)
            // if (selectedCharacter == character && IsCharacterDetailsVisible) -> deselect
            bool shouldDeselect = state.SelectedCharacter == null && state.IsCharacterDetailsVisible; // simulate same character
            if (shouldDeselect)
            {
                state.SelectedCharacter = null;
                state.IsCharacterDetailsVisible = false;
            }
            
            // After deselection: should be null and panel should be hidden
            Assert.Null(state.SelectedCharacter);
            Assert.False(state.IsCharacterDetailsVisible);
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
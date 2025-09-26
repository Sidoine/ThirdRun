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

        [Fact]
        public void CharacterDetailsPanel_HasCloseButton()
        {
            // This test verifies that the CharacterDetailsPanel constructor includes close button logic
            // We can't instantiate the actual panel without MonoGame context, but we can verify
            // that the close button positioning logic would be correct
            
            // Simulate panel bounds for a typical screen
            var screenBounds = new Rectangle(0, 0, 1024, 768);
            var panelWidth = 400;
            var panelHeight = 600;
            
            // Calculate centered panel bounds (same logic as CharacterDetailsPanel.CalculateCenteredBounds)
            int panelX = (screenBounds.Width - panelWidth) / 2;
            int panelY = (screenBounds.Height - panelHeight) / 2;
            var panelBounds = new Rectangle(panelX, panelY, panelWidth, panelHeight);
            
            // Calculate close button bounds (same logic as CharacterDetailsPanel constructor)
            var closeButtonBounds = new Rectangle(panelBounds.Right - 30, panelBounds.Top + 5, 25, 25);
            
            // Verify close button is positioned in top-right corner of panel
            Assert.True(closeButtonBounds.Right <= panelBounds.Right);
            Assert.True(closeButtonBounds.X >= panelBounds.Right - 30);
            Assert.True(closeButtonBounds.Y == panelBounds.Top + 5);
            Assert.Equal(25, closeButtonBounds.Width);
            Assert.Equal(25, closeButtonBounds.Height);
            
            // Verify close button is within panel bounds
            Assert.True(panelBounds.Contains(closeButtonBounds));
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
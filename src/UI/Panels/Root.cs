using Microsoft.Xna.Framework;
using ThirdRun.UI.Components;

namespace ThirdRun.UI.Panels;

public class Root : Container
{
    private const int PortraitsPanelWidth = 92; // 60 portrait + 32 padding
    private const int InventoryPanelWidth = 320;
    
    private CharacterDetailsPanel? _characterDetailsPanel;

    public Root(UiManager uiManager, Rectangle bounds) : base(uiManager, bounds)
    {
        // Character portraits panel on the far left
        AddChild(new CharacterPortraitsPanel(uiManager, 
            new Rectangle(bounds.Left, bounds.Top, PortraitsPanelWidth, bounds.Height)));
        
        // Inventory panel moved to the right of portraits
        AddChild(new InventoryPanel(uiManager, 
            new Rectangle(bounds.Left + PortraitsPanelWidth, bounds.Top, InventoryPanelWidth, bounds.Height)));
        
        // Buttons panel remains in the bottom right
        AddChild(new ButtonsPanel(uiManager, 
            new Rectangle(bounds.Right - 200, bounds.Bottom - 60, 200, 60)));
        
        // Character details panel (initially hidden)
        _characterDetailsPanel = new CharacterDetailsPanel(uiManager, bounds);
        _characterDetailsPanel.Visible = false;
        AddChild(_characterDetailsPanel);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Update character details panel visibility and content
        if (_characterDetailsPanel != null)
        {
            bool shouldBeVisible = UiManager.CurrentState.IsCharacterDetailsVisible;
            _characterDetailsPanel.Visible = shouldBeVisible;
            
            if (shouldBeVisible && UiManager.CurrentState.SelectedCharacter != null)
            {
                _characterDetailsPanel.SetCharacter(UiManager.CurrentState.SelectedCharacter);
            }
        }
    }
}
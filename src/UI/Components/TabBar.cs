using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;

namespace ThirdRun.UI.Components
{
    /// <summary>
    /// Composant qui regroupe plusieurs TabButton et gère leur sélection
    /// </summary>
    public class TabBar : Container
    {
        private List<TabButton> _tabs = new List<TabButton>();
        private int _selectedIndex = 0;
        private readonly DynamicSpriteFont _font;
        private readonly int maxNumberOfTabs;

        public event Action<int>? TabChanged;

        public TabBar(UiManager uiManager, Rectangle bounds, int maxNumberOfTabs)
            : base(uiManager, bounds)
        {
            _font = uiManager.FontSystem.GetFont(16);
            this.maxNumberOfTabs = maxNumberOfTabs;
        }

        public void AddTab(string icon, string? tooltipText = null)
        {
            var index = _tabs.Count;
            var tabWidth = Bounds.Width / maxNumberOfTabs;
            var tab = new TabButton(UiManager, new Rectangle(Bounds.Left + tabWidth * index, Bounds.Top, tabWidth, Bounds.Height), icon, () => SelectTab(index), _font);
            UIElement toAdd = tab;
            if (!string.IsNullOrEmpty(tooltipText))
            {
                // On suppose que le TabButton a accès à GraphicsDevice et SpriteBatch via héritage ou propriété
                toAdd = new ToolTip(UiManager, tab, tooltipText, _font);
            }
            _tabs.Add(tab);
            AddChild(toAdd);
        }

        public void SelectTab(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;
            for (int i = 0; i < _tabs.Count; i++)
                _tabs[i].IsSelected = i == index;
            _selectedIndex = index;
            TabChanged?.Invoke(index);
        }

        public int SelectedIndex => _selectedIndex;
        public TabButton SelectedTab => _tabs[_selectedIndex];
        public IReadOnlyList<TabButton> Tabs => _tabs;
    }
}

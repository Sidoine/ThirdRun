using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ThirdRun.UI.Components
{
    public class DataGrid<T> : Container
    {
        public class Column
        {
            public string Header { get; set; }
            public int Width { get; set; }
            public Func<T, object> ValueSelector { get; set; }
            public Column(string header, int width, Func<T, object> valueSelector)
            {
                Header = header;
                Width = width;
                ValueSelector = valueSelector;
            }
        }

        private List<Column> _columns = new List<Column>();
        private SpriteFontBase _headerFont;
        private SpriteFontBase _cellFont;
        private int _rowHeight = 24;
        private int _headerHeight = 28;
        private Color _headerBg = Color.DimGray;
        private Color _rowBg = Color.Black * 0.2f;
        private Color _altRowBg = Color.Black * 0.4f;
        private Color _borderColor = Color.Gray;
        private readonly Func<IEnumerable<T>> dataFactory;

        public DataGrid(UiManager uiManager, Rectangle bounds, Func<IEnumerable<T>> dataFactory) : base(uiManager, bounds)
        {
            _headerFont = uiManager.FontSystem.GetFont(16);
            _cellFont = uiManager.FontSystem.GetFont(14);
            this.dataFactory = dataFactory;
        }

        public void AddColumn(string header, int width, Func<T, object> valueSelector)
        {
            _columns.Add(new Column(header, width, valueSelector));
        }

        public override void Draw()
        {
            if (!Visible) return;
            var sb = UiManager.SpriteBatch;
            int x = Bounds.X;
            int y = Bounds.Y;

            // Draw header background
            sb.Draw(UiManager.Pixel, new Rectangle(x, y, Bounds.Width, _headerHeight), _headerBg);
            int colX = x;

            var rows = dataFactory().ToArray();
            var columnRatio = (float)Bounds.Width / _columns.Sum(c => c.Width);

            for (int i = 0; i < _columns.Count; i++)
            {
                var col = _columns[i];
                _headerFont.DrawText(sb, col.Header, new Vector2(colX + 6, y + 4), Color.White);
                // Draw column border
                sb.Draw(UiManager.Pixel, new Rectangle(colX, y, 1, _headerHeight + rows.Length * _rowHeight), _borderColor);
                colX += (int)(col.Width * columnRatio);
            }
            // Draw right border
            sb.Draw(UiManager.Pixel, new Rectangle(x + Bounds.Width - 1, y, 1, _headerHeight + rows.Length * _rowHeight), _borderColor);

            y += _headerHeight;
            // Draw rows
            for (int rowIdx = 0; rowIdx < rows.Length; rowIdx++)
            {
                var row = rows[rowIdx];
                var bgColor = (rowIdx % 2 == 0) ? _rowBg : _altRowBg;
                sb.Draw(UiManager.Pixel, new Rectangle(x, y, Bounds.Width, _rowHeight), bgColor);
                colX = x;
                for (int colIdx = 0; colIdx < _columns.Count; colIdx++)
                {
                    var col = _columns[colIdx];
                    string text = col.ValueSelector(row)?.ToString() ?? "";
                    _cellFont.DrawText(sb, text, new Vector2(colX + 6, y + 3), Color.White);
                    colX += (int)(col.Width * columnRatio);
                }
                y += _rowHeight;
            }
            // Draw bottom border
            sb.Draw(UiManager.Pixel, new Rectangle(x, y, Bounds.Width, 1), _borderColor);

            base.Draw();
        }
    }
}

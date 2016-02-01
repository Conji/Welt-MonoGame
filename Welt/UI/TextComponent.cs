﻿#region Copyright
// COPYRIGHT 2016 JUSTIN COX (CONJI)
#endregion

using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Welt.UI
{
    public class TextComponent : UIComponent
    {
        private SpriteFont _spriteFont;
        private bool _dirty;
        private string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value) return;
                _text = value;
                _dirty = true;
            }
        }

        public string Font { get; set; } = "Fonts/console";
        public Color Foreground { get; set; } = Color.Black;

        public override Cursor Cursor => IsActive ? Cursors.IBeam : Cursor.Current;

        public override float Opacity
        {
            get { return Foreground.A; }
            set { Foreground = Color.FromNonPremultiplied(Foreground.R, Foreground.G, Foreground.B, (int) (value*255)); }
        }

        public TextComponent(string text, string name, GraphicsDevice device) : this(text, name, -2, -2, device)
        {
            
        }

        public TextComponent(string text, string name, int width, int height, GraphicsDevice device)
            : this(text, name, width, height, null, device)
        {
        }

        public TextComponent(string text, string name, int width, int height, UIComponent parent, GraphicsDevice device)
            : base(name, width, height, parent, device)
        {
            Text = text; // TODO: process text like escaping or regexes?
        }

        public override void Initialize()
        {
            _spriteFont = WeltGame.Instance.Content.Load<SpriteFont>(Font);
            if (Height == -2 || Width == -2) RecalculateBounds();
            base.Initialize();
            //ProcessArea();
        }

        public override void Draw(GameTime time)
        {
            base.Draw(time);
            Sprite.Begin();
            Sprite.DrawString(_spriteFont, Text, new Vector2(X, Y), Foreground);
            Sprite.End();
            
        }

        public override void Update(GameTime time)
        {
            base.Update(time);  
            if (_dirty) RecalculateBounds();   
        }

        private void RecalculateBounds()
        {
            _dirty = false;
            var w = (float) Width;
            w = Text.Split('\r', '\n')
                        .Select(line => _spriteFont.MeasureString(line))
                        .Select(m => m.X)
                        .Concat(new[] { w })
                        .Max();
            Width = (int) w;
            Height = (int) Text.Split('\r', '\n')
                .Select(line => _spriteFont.MeasureString(line))
                .Select(m => m.Y)
                .Sum();
        }
    }
}
﻿#region Copyright
// COPYRIGHT 2016 JUSTIN COX (CONJI)
#endregion

using Microsoft.Xna.Framework.Graphics;

namespace Welt.UI
{
    public class ImageComponent : UIComponent
    {
        public string File { get; }
        private readonly SpriteBatch _sprite;
        private readonly Texture2D _image;

        public ImageComponent(string file, string name, int width, int height, GraphicsDevice device)
            : this(file, name, width, height, null, device)
        {

        }

        public ImageComponent(string file, string name, int width, int height, UIComponent parent, GraphicsDevice device)
            : base(name, width, height, parent, device)
        {
            File = file;
            _sprite = new SpriteBatch(device);
            _image = WeltGame.Instance.Content.Load<Texture2D>(file);
        }
    }
}
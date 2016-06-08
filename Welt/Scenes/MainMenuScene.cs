﻿#region Copyright
// COPYRIGHT 2015 JUSTIN COX (CONJI)
#endregion

using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Welt.Controllers;
using Welt.Forge;
using Welt.Forge.Renderers;
using Welt.UI;
using Welt.UI.Components;

namespace Welt.Scenes
{
    public class MainMenuScene : Scene
    {
        protected override Color BackColor => Color.GhostWhite;

        public MainMenuScene(Game game) : base(game)
        {
            //game.Window.AllowUserResizing = true;
            AddComponent(new ImageComponent("Images/welt", "background", GraphicsDevice)
            {
                Opacity = 0.8f
            });
            
            var button = new ButtonComponent("Singleplayer", "spbutton", 300, 100, GraphicsDevice)
            {
                TextHorizontalAlignment = HorizontalAlignment.Center,
                BorderWidth = new BoundsBox(2, 2, 2, 2),
                BackgroundActiveColor = Color.CadetBlue,
                BackgroundColor = Color.LightSteelBlue,
                ForegroundColor = Color.Black,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            button.MouseLeftDown += (sender, args) =>
            {
                SceneController.Load(new LoadScene(game, new World("DEMO WORLD"))); // TODO: fetch world data
            };

            AddComponent(button);
            
        }
    }
}

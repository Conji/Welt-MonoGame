﻿#region Copyright
// COPYRIGHT 2015 JUSTIN COX (CONJI)
#endregion
using System;
using Microsoft.Xna.Framework;
using Welt.Cameras;
using Welt.Controllers;
using Welt.Forge.Renderers;
using Welt.API;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using EmptyKeys.UserInterface.Controls;
using EmptyKeys.UserInterface.Generated;
using GameUILibrary.Models;
using Welt.Extensions;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Welt.Core.Forge;
using Welt.Forge;
using Microsoft.Xna.Framework.Content;
using Welt.Components;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Welt.Scenes
{
    public class PlayScene : Scene
    {
        public static PlayScene Instance;

        private PlayUI m_PlayUi => new PlayUI
        {
            DataContext = m_PlayViewModel
        };
        private Pause m_PauseUi => new Pause
        {
            DataContext = new PauseViewModel
            {
                ResumeButtonCommand = 
                new Action(SwitchToPlayUi).CreateButtonCommand(),
                OptionsButtonCommand = 
                new Action(SwitchToSettingsUi).CreateButtonCommand(),
                QuitButtonCommand = 
                new Action(() => Next(new MainMenuScene(Game))).CreateButtonCommand()
            }
        };
        private SettingsMenu m_SettingsUi => new SettingsMenu
        {
            DataContext = new SettingsModel
            {
                ExitCommand =
                new Action(SwitchToPauseUi).CreateButtonCommand()
            }
        };

        private PlayViewModel m_PlayViewModel = new PlayViewModel();
        
        private HudRenderer m_Hud;
        private PlayerRenderer m_PlayerRenderer;
        private ChunkComponent m_ChunkComp;
        private SkyComponent m_SkyComp;
        private FpsComponent m_Fps;

        private bool m_TooltipReady = true;
        private Queue<string> m_TooltipQueue;

        internal override Color BackColor => Color.Blue;
        internal override UIRoot UI { get; set; }

        public PlayScene(WeltGame game, ChunkComponent chunks, SkyComponent sky, PlayerRenderer player) : base(game)
        {   
            UI = m_PlayUi;
            m_ChunkComp = chunks;
            m_SkyComp = sky;
            m_PlayerRenderer = player;
            m_Hud = new HudRenderer(GraphicsDevice, game.Client.World, m_PlayerRenderer);
            m_Fps = new FpsComponent(game);
            m_TooltipQueue = new Queue<string>();
            Instance = this;
        }

        private async void ShowNextTooltip()
        {
            while (!m_TooltipReady)
            {
                await Task.Delay(500);
            }
            if (m_TooltipQueue.Count == 0) return;
            m_TooltipReady = false;
            var tooltip = m_TooltipQueue.Dequeue();
            m_PlayViewModel.TooltipOpacity = 0;
            m_PlayViewModel.TooltipText = tooltip;
            while (m_PlayViewModel.TooltipOpacity < 1)
            {
                m_PlayViewModel.TooltipOpacity += 0.05;
                await Task.Delay(20);
            }
            await Task.Delay(3000);
            while (m_PlayViewModel.TooltipOpacity > 0)
            {
                m_PlayViewModel.TooltipOpacity -= 0.05;
                await Task.Delay(100);
            }
            m_TooltipReady = true;
        }

        internal void ShowTooltip(string tooltip)
        {
            if (tooltip == null) return;
            m_TooltipQueue.Enqueue(tooltip);
            //m_TooltipQueue.Enqueue(tooltip);
            //new Thread(() =>
            //{
            //    m_PlayViewModel.TooltipOpacity = 0;
            //    m_PlayViewModel.TooltipText = m_TooltipQueue.Dequeue();
            //    while(m_PlayViewModel.TooltipOpacity < 1)
            //    {
            //        m_PlayViewModel.TooltipOpacity += 0.05;
            //        Thread.Sleep(20);
            //    }

            //    Thread.Sleep(3000);
            //    while(m_PlayViewModel.TooltipOpacity > 0)
            //    {
            //        m_PlayViewModel.TooltipOpacity -= 0.05;
            //        Thread.Sleep(100);
            //    }
            //})
            //{ IsBackground = true }.Start();
        }

        #region Initialize

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            // all world/sky renderers will be initialized before this in the loading scene
            m_PlayerRenderer.Initialize();
            m_Hud.Initialize();
            m_PlayerRenderer.Player._Position = Game.Client.World.GetSpawnPoint();
            m_Fps.Initialize();
            Input.Assign(() => m_PlayerRenderer.Player.IsFlying = !m_PlayerRenderer.Player.IsFlying, Keys.F);
            //Input.Assign(() => m_PlayerRenderer.Player.IsPaused = !m_PlayerRenderer.Player.IsPaused, Keys.Escape);
            Game.ShowTooltip("Welcome to Welt!");
            Game.ShowTooltip("To move around, you can use the standard WSAD controls.");
            Game.ShowTooltip("To leave flight-mode, press F.");
            Game.ShowTooltip("To run, press Left Shift. To jump, press Spacebar. In flight-mode, these are your ascend and decend controls.");
        }

        #endregion

        public override void OnExiting(object sender, EventArgs args)
        {
            
            base.OnExiting(sender, args);
        }

        #region LoadContent

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent(ContentManager content)
        {
            m_Hud.LoadContent(content);
            m_Fps.LoadContent(content);
        }

        #endregion

        #region UnloadContent

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion
        
        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            WeltGame.Instance.IsMouseVisible = Game.Client.IsPaused;
            if (Game.IsActive)
            {
                Input.Update(gameTime);
                m_SkyComp.Update(gameTime);
                m_ChunkComp.Update(gameTime);
                m_PlayerRenderer.Update(gameTime);
                m_Fps.Update(gameTime);
                // this needs to go last
                ShowNextTooltip();
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            m_SkyComp.Draw(gameTime);
            m_ChunkComp.Draw(gameTime);
            m_PlayerRenderer.Draw(gameTime);
            m_Hud.Draw(gameTime);
            m_Fps.Draw(gameTime);
        }

        #endregion

        public override void Dispose()
        {
            Game.Client.Disconnect();
            m_PlayerRenderer = null;
            GC.WaitForPendingFinalizers();
            GC.Collect();
            base.Dispose();
        }

        #region Private methods
       
        private void SwitchToPlayUi()
        {
            UI = m_PlayUi;
            Game.Client.IsPaused = false;
        }

        private void SwitchToPauseUi()
        {
            UI = m_PauseUi;
        }

        private void SwitchToSettingsUi()
        {
            UI = m_SettingsUi;
        }

        #endregion
    }
}

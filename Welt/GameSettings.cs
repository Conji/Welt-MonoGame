﻿using Microsoft.Xna.Framework.Input;

namespace Welt
{
    public class GameSettings
    {
        /* Audio */
        public float MasterVolume { get; set; } = 1;
        public float MusicVolume { get; set; } = 1;
        public float EffectVolume { get; set; } = 1;

        /* Keybindings */
        public Keys MoveForwardKey { get; set; } = Keys.W;
        public Keys MoveBackwardKey { get; set; } = Keys.S;
        public Keys StrafeLeftKey { get; set; } = Keys.A;
        public Keys StrafeRightKey { get; set; } = Keys.D;
        public Keys JumpKey { get; set; } = Keys.Space;
        public Keys CrouchKey { get; set; } = Keys.LeftControl;
        public Keys SprintKey { get; set; } = Keys.LeftShift;
        public Keys Hotbar0 { get; set; } = Keys.D1;
        public Keys Hotbar1 { get; set; } = Keys.D2;
        public Keys Hotbar2 { get; set; } = Keys.D3;
        public Keys Hotbar3 { get; set; } = Keys.D4;
        public Keys Hotbar4 { get; set; } = Keys.D5;
        public Keys Hotbar5 { get; set; } = Keys.D6;
        public Keys Hotbar6 { get; set; } = Keys.D7;
        public Keys Hotbar7 { get; set; } = Keys.D8;
        public Keys Hotbar8 { get; set; } = Keys.D9;
        public Keys Hotbar9 { get; set; } = Keys.D0;
        public Keys FlightKey { get; set; } = Keys.F;
        public Keys InventoryKey { get; set; } = Keys.R;
        public Keys InteractKey { get; set; } = Keys.E;

        /* Mouse */
        public bool IsMouseRightHanded { get; set; } = true;
        public float HorizontalSensitivity { get; set; } = 0.5f;
        public float VerticalSensitivity { get; set; } = 0.5f;

        /* Visual */
        public WindowDisplayMode DisplayMode { get; set; }
#if DEBUG
            = WindowDisplayMode.Windowed;
#else
            = WindowDisplayMode.FakeFullScreen;
#endif
        public int RenderDistance { get; set; } = 14;
        public int CacheDistance { get; set; } = 16;

        public bool ShowTooltips { get; set; } = true;
    }
}

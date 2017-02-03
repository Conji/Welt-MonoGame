﻿using System;

namespace Welt.API.Forge
{
    /// <summary>
    /// Describes an item that can be burnt as fuel in a furnace.
    /// </summary>
    public interface IBurnableItem
    {
        /// <summary>
        /// The duration of time this item can act as fuel.
        /// </summary>
        TimeSpan BurnTime { get; }
    }
}
﻿using Microsoft.Xna.Framework;
using Welt.Cameras;

namespace Welt.Controllers
{
    public class VehicleCameraController : CameraController
    {
        public float Friction;

        public VehicleCameraController(VehicleCamera camera) : base(camera)
        {
        }

        public override void ProcessInput(GameTime gameTime)
        {
            
        }
    }
}

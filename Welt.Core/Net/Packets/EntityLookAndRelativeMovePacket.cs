﻿using System;
using Welt.API.Net;

namespace Welt.Core.Net.Packets
{
    public struct EntityLookAndRelativeMovePacket : IPacket
    {
        public byte Id { get { return 0x21; } }

        public int EntityID;
        public sbyte DeltaX, DeltaY, DeltaZ;
        public sbyte Yaw, Pitch;

        public void ReadPacket(IWeltStream stream)
        {
            EntityID = stream.ReadInt32();
            DeltaX = stream.ReadInt8();
            DeltaY = stream.ReadInt8();
            DeltaZ = stream.ReadInt8();
            Yaw = stream.ReadInt8();
            Pitch = stream.ReadInt8();
        }

        public void WritePacket(IWeltStream stream)
        {
            stream.WriteInt32(EntityID);
            stream.WriteInt8(DeltaX);
            stream.WriteInt8(DeltaY);
            stream.WriteInt8(DeltaZ);
            stream.WriteInt8(Yaw);
            stream.WriteInt8(Pitch);
        }
    }
}
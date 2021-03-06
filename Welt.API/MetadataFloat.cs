using Lidgren.Network;
using System;
using Welt.API.Net;

namespace Welt.API
{
    public class MetadataFloat : MetadataEntry
    {
        public override byte Identifier { get { return 3; } }
        public override string FriendlyName { get { return "float"; } }

        public float Value;

        public static implicit operator MetadataFloat(float value)
        {
            return new MetadataFloat(value);
        }

        public MetadataFloat()
        {
        }

        public MetadataFloat(float value)
        {
            Value = value;
        }

        public override void FromStream(NetIncomingMessage stream)
        {
            Value = stream.ReadSingle();
        }

        public override void WriteTo(NetOutgoingMessage stream, byte index)
        {
            stream.Write(GetKey(index));
            stream.Write(Value);
        }
    }
}

﻿using SilkroadSecurityApi;
using SimpleCL.Enums.Common;
using SimpleCL.Model.Coord;
using SimpleCL.Util.Extension;

namespace SimpleCL.Interaction.Pathing
{
    public static class Movement
    {
        public static Packet WalkTo(LocalPoint localPoint)
        {
            return WalkTo(localPoint.Region, localPoint.X, localPoint.Y, localPoint.Z);
        }

        public static Packet WalkTo(ushort region, float x, float y, float z)
        {
            Packet packet = new Packet(Opcodes.Agent.Request.CHAR_MOVEMENT);
            packet.WriteByte(1);
            packet.WriteUShort(region);

            if (region > short.MaxValue)
            {
                packet.WriteInt(x);
                packet.WriteInt(z);
                packet.WriteInt(y);
            }
            else
            {
                packet.WriteUShort(x);
                packet.WriteUShort(z);
                packet.WriteUShort(y);
            }

            return packet;
        }
    }
}
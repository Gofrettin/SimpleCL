﻿using SilkroadSecurityApi;
using SimpleCL.Model.Game;
using SimpleCL.Network;
using SimpleCL.Network.Enums;

namespace SimpleCL.Service.Game
{
    public class ChatService : Service
    {
        [PacketHandler(Opcodes.Agent.Response.CHAT_UPDATE)]
        public void ChatUpdated(Server server, Packet packet)
        {
            ChatChannel channel = (ChatChannel) packet.ReadUInt8();
            ChatMessage chatMessage = new ChatMessage(channel);

            if (channel == ChatChannel.General || channel == ChatChannel.GM ||
                channel == ChatChannel.NPC)
            {
                uint senderId = packet.ReadUInt32();
                chatMessage.SenderId = senderId;
            }
            else
            {
                string senderName = packet.ReadAscii();
                chatMessage.SenderName = senderName;
            }

            string message = packet.ReadUnicode();
            chatMessage.Message = message;

            Program.Gui.AddChatMessage(chatMessage.ToString());
        }
    }
}
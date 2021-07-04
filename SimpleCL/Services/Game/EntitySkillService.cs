﻿using System;
using System.Linq;
using SimpleCL.Enums.Commons;
using SimpleCL.Interaction.Providers;
using SimpleCL.Models.Character;
using SimpleCL.Models.Entities;
using SimpleCL.Models.Skills;
using SimpleCL.Network;
using SimpleCL.SilkroadSecurityApi;
using SimpleCL.Util.Extension;

namespace SimpleCL.Services.Game
{
    public class EntitySkillService : Service
    {
        #region BuffAdded

        [PacketHandler(Opcodes.Agent.Response.SKILL_BUFF_START)]
        public void BuffStart(Server server, Packet packet)
        {
            var targetUid = packet.ReadUInt();
            var buff = new Buff(packet.ReadUInt());
            buff.Uid = packet.ReadUInt();
            buff.TargetUid = targetUid;
            Entities.Buffed(buff);
        }

        #endregion

        #region BuffRemoved

        [PacketHandler(Opcodes.Agent.Response.SKILL_BUFF_END)]
        public void BuffEnd(Server server, Packet packet)
        {
            if (packet.ReadBool())
            {
                Entities.BuffEnded(packet.ReadUInt());
            }
        }

        #endregion

        #region EntityAction

        [PacketHandler(Opcodes.Agent.Response.CHAR_ACTION_DATA)]
        public void SkillCast(Server server, Packet packet)
        {
            if (!packet.ReadBool())
            {
                return;
            }


            try
            {
                Skill.CastType type = (Skill.CastType) packet.ReadByte();
                packet.ReadByte();
                var id = packet.ReadUInt();
                var sourceUid = packet.ReadUInt();
                var skillUid = packet.ReadUInt();
                packet.ReadUInt();
                var mainTargetUid = packet.ReadUInt();

                if (sourceUid == LocalPlayer.Get.Uid)
                {
                    var skillUsed = LocalPlayer.Get.Skills.FirstOrDefault(x => x.Id == id);
                    if (skillUsed == null)
                    {
                        return;
                    }
                    
                    skillUsed.StartCooldownTimer();
                }

                var hasDamage = packet.ReadBool();
                if (!hasDamage)
                {
                    return;
                }
                
                var targetCount = packet.ReadByte();
                var hitCount = packet.ReadByte();
                
                packet.ReadByte();
            
                targetCount.Repeat(i =>
                {
                    var targetUid = packet.ReadUInt();
                    var dmgEffect = packet.ReadByte();
                    if (dmgEffect == 128)
                    {
                        if (Entities.AllEntities.ContainsKey(targetUid)
                            && Entities.AllEntities[targetUid] is Monster)
                        {
                            Entities.Despawned(targetUid);
                        }
                    } 
                    else if (dmgEffect.HasFlags((byte)(Skill.DamageEffect.Block | Skill.DamageEffect.Cancel)))
                    {
                        return;
                    }

                    var damageType = (Skill.DamageType) packet.ReadByte();
                    var damageValue = packet.ReadUInt();
                    packet.ReadByte();
                    packet.ReadByte();
                    packet.ReadByte();
                });
            }
            catch (Exception e)
            {
                server.DebugPacket(packet);
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion
    }
}
﻿using System.Collections.Generic;
using System.Linq;
using SimpleCL.Models.Coordinates;
using SimpleCL.Models.Entities;
using SimpleCL.Models.Entities.Teleporters;

namespace SimpleCL.Interaction.Providers
{
    public static class Entities
    {
        public static readonly Dictionary<uint, Entity> AllEntities = new Dictionary<uint, Entity>();

        public static void Respawn()
        {
            AllEntities.Clear();
            SimpleCL.Gui.ClearMarkers();
        }

        public static void Spawn(Entity e)
        {
            if (AllEntities.ContainsKey(e.Uid))
            {
                return;
            }
            
            AllEntities[e.Uid] = e;
            SimpleCL.Gui.AddMinimapMarker(e);
        }

        public static void Despawn(uint uid)
        {
            if (!AllEntities.ContainsKey(uid))
            {
                return;
            }
            
            AllEntities.Remove(uid);
            SimpleCL.Gui.RemoveMinimapMarker(uid);
        }

        public static void Moved(uint uid, LocalPoint point = null, ushort angle = 0)
        {
            if (!AllEntities.ContainsKey(uid))
            {
                return;
            }
            
            var entity = AllEntities[uid];
            if (entity is PathingEntity pathingEntity)
            {
                if (point != null)
                {
                    pathingEntity.LocalPoint = point;
                }
                    
                if (angle > 0)
                {
                    pathingEntity.Angle = angle;
                }
            }

            SimpleCL.Gui.RefreshMap();
        }

        public static void SpeedChanged(uint uid, float walkSpeed, float runSpeed)
        {
            if (!AllEntities.ContainsKey(uid))
            {
                return;
            }
            
            var entity = AllEntities[uid];
            if (!(entity is PathingEntity pathingEntity))
            {
                return;
            }
            
            pathingEntity.WalkSpeed = walkSpeed;
            pathingEntity.RunSpeed = runSpeed;
        }

        public static List<Player> GetPlayers()
        {
            return AllEntities.Values.Where(x => x is Player).Cast<Player>().ToList();
        }
        
        public static List<Monster> GetMonsters()
        {
            return AllEntities.Values.Where(x => x is Monster).Cast<Monster>().ToList();
        }

        public static List<TalkNpc> GetNpcs()
        {
            return AllEntities.Values.Where(x => x is TalkNpc).Cast<TalkNpc>().ToList();
        }
        
        public static List<CharacterPet> GetPets()
        {
            return AllEntities.Values.Where(x => x is CharacterPet).Cast<CharacterPet>().ToList();
        }

        public static List<Teleport> GetTeleports()
        {
            return AllEntities.Values.Where(x => x is Teleport).Cast<Teleport>().ToList();
        }
    }
}
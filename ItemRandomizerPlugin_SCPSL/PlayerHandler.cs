﻿using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem;
using ItemRandomizerPlugin_SCPSL.RoomPoints;
using MEC;
using PluginAPI.Core.Zones.Light;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace ItemRandomizerPlugin {
    public class PlayerHandler {
        private static readonly RoomType Scp173RoomType = RoomType.Lcz173;
        private static readonly Random Random = new Random();
        private static readonly Vector3 DropLocation = new Vector3(257.3519f, 13.14142f, 127.7134f);
        private static List<ItemType> allowedItems = AddAllowedItems();
        List<Item> usedCoins = new List<Item>();

        public void OnItemDropped(DroppingItemEventArgs ev) {
            if (ev.Player.CurrentRoom.Type == Scp173RoomType && ev.Item.Type == ItemType.Coin && IsNearDropLocation(ev.Player)) {
                var randomItemType = allowedItems[Random.Next(allowedItems.Count)];

                if (ev.Item?.Base != null) {
                    ev.Player.Inventory.ServerRemoveItem(ev.Item.Base.ItemSerial, ev.Item.Base.PickupDropModel);
                    ev.Player.AddItem(randomItemType);
                    Log.Info($"Item {randomItemType} added");
                }
            }
        }

        

        public void CoinSpawn() {
            foreach (var player in Player.List) {
                player.Inventory.ServerAddItem(ItemType.Coin);
            }
        }

        public void OnFlip(FlippingCoinEventArgs ev)
        {
            if(!ev.IsTails)
            {
                if (usedCoins.Contains(ev.Item))
                {
                     ev.IsAllowed = false;
                } else if(ev.Player.CurrentRoom.Zone == ZoneType.LightContainment)
                {
                    RoomType tpRoom;
                    do {
                        tpRoom = GetRandomLCZRoom();
                    } while (ev.Player.CurrentRoom.Equals(tpRoom));
                    
                    Timing.CallDelayed(3f, () => {
                        ev.Player.Teleport(tpRoom);
                        ev.Player.EnableEffect(EffectType.SinkHole, 5f);
                        usedCoins.Add(ev.Item);
                    });

                } else if (ev.Player.CurrentRoom.Zone == ZoneType.HeavyContainment && Map.IsLczDecontaminated) {
                    RoomType tpRoom;
                    do {
                        tpRoom = GetRandomHCZRoom();
                    } while (ev.Player.CurrentRoom.Equals(tpRoom));

                    Timing.CallDelayed(3f, () => {
                        ev.Player.Teleport(tpRoom);
                        ev.Player.EnableEffect(EffectType.SinkHole, 5f);
                        usedCoins.Add(ev.Item);
                    });
                }
            }
        }

        public void ClearCoinList(RoundEndedEventArgs ev) {
            usedCoins.Clear();
        }


        private bool IsNearDropLocation(Player player) {
 
            var RequiredDropLocation = new RoomPointObject(Room.Get(Scp173RoomType).GameObject.transform.TransformPoint(DropLocation.x, DropLocation.y, DropLocation.z));
          

            Log.Info(Vector3.Distance(player.Position,
                new Vector3(RequiredDropLocation.relativePosition.X, RequiredDropLocation.relativePosition.Y, RequiredDropLocation.relativePosition.Z)));

            if(Vector3.Distance(player.Position, 
                new Vector3(RequiredDropLocation.relativePosition.X, RequiredDropLocation.relativePosition.Y, RequiredDropLocation.relativePosition.Z)) <= 274.0f) {

                return true;

            }
            return false;
        }



        private RoomType GetRandomLCZRoom()
        {
            Random random = new Random();
            int index = random.Next(lczRooms.Count);
            return lczRooms[index];
        }

        public RoomType GetRandomHCZRoom() {
            Random random = new Random();
            int index = random.Next(hczRooms.Count);
            return hczRooms[index];
        }

        private static List<ItemType> AddAllowedItems()
        {
            Log.Info("AllowedItems");
            var itemTypes = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToList();
            var allowedItems = itemTypes.GetRange(1, 15);
            allowedItems.AddRange(itemTypes.GetRange(17, 2));
            allowedItems.AddRange(itemTypes.GetRange(25, 2));
            allowedItems.AddRange(itemTypes.GetRange(29, 7));
            allowedItems.AddRange(itemTypes.GetRange(42, 5));
            allowedItems.AddRange(itemTypes.GetRange(48, 2));
            allowedItems.AddRange(itemTypes.GetRange(51, 1));
            allowedItems.AddRange(itemTypes.GetRange(54, 1));
            Log.Info(allowedItems.Count);
            return allowedItems;
        }

        private List<RoomType> lczRooms = new List<RoomType>
        {
        RoomType.LczCurve,
        RoomType.LczStraight,
        RoomType.LczCrossing,
        RoomType.LczTCross,
        RoomType.LczCafe,
        RoomType.LczPlants,
        RoomType.LczToilets,
        RoomType.LczAirlock,
        RoomType.Lcz173,
        RoomType.LczClassDSpawn,
        RoomType.LczCheckpointB,
        RoomType.LczGlassBox,
        RoomType.LczCheckpointA,
        };

        private List<RoomType> hczRooms = new List<RoomType>
        {
        RoomType.Hcz079,
        RoomType.HczEzCheckpointA,
        RoomType.HczEzCheckpointB,
        RoomType.HczArmory,
        RoomType.Hcz939,
        RoomType.HczHid,
        RoomType.Hcz049,
        RoomType.HczCrossing,
        RoomType.Hcz106,
        RoomType.HczNuke,
        RoomType.HczTestRoom,
        RoomType.HczElevatorA,
        RoomType.HczElevatorB,
        RoomType.HczTesla,
        RoomType.HczServers,
        RoomType.HczTCross,
        RoomType.HczCurve,
        RoomType.Hcz096
        };

        
    }
}
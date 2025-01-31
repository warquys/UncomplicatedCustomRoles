﻿using CommandSystem;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using UncomplicatedCustomRoles.Interfaces;
using UncomplicatedCustomRoles.Manager;
using Handler = UncomplicatedCustomRoles.Events.EventHandler;

namespace UncomplicatedCustomRoles.Commands
{
    public class UCRSpawn : IUCRCommand
    {
        public string Name { get; } = "spawn";

        public string Description { get; } = "Spawn a player with a UCR Role";

        public string RequiredPermission { get; } = "ucr.spawn";

        public bool Executor(List<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: ucr spawn <Player Id> <Role Id>";
                return false;
            }

            Player Player = Player.Get(arguments[0]);
            if (Player is null)
            {
                response = $"Player not found: {arguments[0]}";
                return false;
            }

            if (arguments[1] is not null)
            {
                int Id = int.Parse(arguments[1]);

                Log.Debug($"Selected role Id as Int32: {Id}");
                if (!Plugin.CustomRoles.ContainsKey(Id))
                {
                    response = $"Role with the Id {Id} was not found!";
                    return false;
                } 
                else
                {
                    // Summon the player to the role
                    response = $"Player {Player.Nickname} will be spawned as {Id}!";

                    if (arguments.Count > 2 && arguments[2] is not null && arguments[2] == "sync")
                    {
                        Log.Debug("Spawning player sync");
                        SpawnManager.SummonCustomSubclass(Player, Id, true);
                    }
                    else
                    {
                        Log.Debug("Spawning player async");
                        Timing.RunCoroutine(Handler.DoSpawnPlayer(Player, Id));
                    }
                    return true;
                }
            } 
            else
            {
                response = $"You must define a role Id!";
                return false;
            }
        }
    }
}
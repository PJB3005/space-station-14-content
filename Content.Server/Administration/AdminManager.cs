﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Content.Server.Database;
using Content.Server.Players;
using Content.Shared;
using Content.Shared.Administration;
using Content.Shared.Network.NetMessages;
using Robust.Server.Console;
using Robust.Server.Interfaces.Console;
using Robust.Server.Interfaces.Player;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.Interfaces.Configuration;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Interfaces.Resources;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

#nullable enable

namespace Content.Server.Administration
{
    public sealed class AdminManager : IAdminManager, IPostInjectInit, IConGroupControllerImplementation
    {
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly IServerDbManager _dbManager = default!;
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        [Dependency] private readonly IServerNetManager _netMgr = default!;
        [Dependency] private readonly IConGroupController _conGroup = default!;
        [Dependency] private readonly IResourceManager _res = default!;
        [Dependency] private readonly IConsoleShell _consoleShell = default!;

        private readonly Dictionary<IPlayerSession, AdminReg> _admins = new Dictionary<IPlayerSession, AdminReg>();

        public IEnumerable<IPlayerSession> ActiveAdmins => _admins
            .Where(p => p.Value.Data.Active)
            .Select(p => p.Key);

        // If a command isn't in this list it's server-console only.
        // if a command is in but the flags value is null it's available to everybody.
        private readonly HashSet<string> _anyCommands = new HashSet<string>();
        private readonly Dictionary<string, AdminFlags[]> _adminCommands = new Dictionary<string, AdminFlags[]>();

        public AdminData? GetAdminData(IPlayerSession session, bool includeDeAdmin = false)
        {
            if (_admins.TryGetValue(session, out var reg) && (reg.Data.Active || includeDeAdmin))
            {
                return reg.Data;
            }

            return null;
        }

        public void DeAdmin(IPlayerSession session)
        {
            if (!_admins.TryGetValue(session, out var reg))
            {
                throw new ArgumentException($"Player {session} is not an admin");
            }

            if (!reg.Data.Active)
            {
                return;
            }

            var plyData = session.ContentData()!;
            plyData.ExplicitlyDeadminned = true;
            reg.Data.Active = false;

            // TODO: Send messages to all admins.

            UpdateAdminStatus(session);
        }

        public void ReAdmin(IPlayerSession session)
        {
            if (!_admins.TryGetValue(session, out var reg))
            {
                throw new ArgumentException($"Player {session} is not an admin");
            }

            var plyData = session.ContentData()!;
            plyData.ExplicitlyDeadminned = true;
            reg.Data.Active = true;

            // TODO: Send messages to all admins.

            UpdateAdminStatus(session);
        }

        public void Initialize()
        {
            _netMgr.RegisterNetMessage<MsgUpdateAdminStatus>(MsgUpdateAdminStatus.NAME);

            // Cache permissions for loaded console commands with the requisite attributes.
            foreach (var (cmdName, cmd) in _consoleShell.AvailableCommands)
            {
                var (isAvail, flagsReq) = GetRequiredFlag(cmd);

                if (!isAvail)
                {
                    continue;
                }

                if (flagsReq.Length != 0)
                {
                    _adminCommands.Add(cmdName, flagsReq);
                }
                else
                {
                    _anyCommands.Add(cmdName);
                }
            }

            // Load flags for engine commands, since those don't have the attributes.
            if (_res.TryContentFileRead(new ResourcePath("/engineCommandPerms.yml"), out var fs))
            {
                using var reader = new StreamReader(fs, EncodingHelpers.UTF8);
                var yStream = new YamlStream();
                yStream.Load(reader);
                var root = (YamlSequenceNode) yStream.Documents[0].RootNode;

                foreach (var child in root)
                {
                    var map = (YamlMappingNode) child;
                    var commands = map.GetNode<YamlSequenceNode>("Commands").Select(p => p.AsString());
                    if (map.TryGetNode("Flags", out var flagsNode))
                    {
                        var flagNames = flagsNode.AsString().Split(",", StringSplitOptions.RemoveEmptyEntries);
                        var flags = AdminFlagsExt.NamesToFlags(flagNames);
                        foreach (var cmd in commands)
                        {
                            if (!_adminCommands.TryGetValue(cmd, out var exFlags))
                            {
                                _adminCommands.Add(cmd, new []{flags});
                            }
                            else
                            {
                                var newArr = new AdminFlags[exFlags.Length + 1];
                                exFlags.CopyTo(newArr, 0);
                                exFlags[^1] = flags;
                                _adminCommands[cmd] = newArr;
                            }
                        }
                    }
                    else
                    {
                        _anyCommands.UnionWith(commands);
                    }
                }
            }
        }

        void IPostInjectInit.PostInject()
        {
            _playerManager.PlayerStatusChanged += PlayerStatusChanged;
            _conGroup.Implementation = this;
        }

        // NOTE: Also sends commands list for non admins..
        private void UpdateAdminStatus(IPlayerSession session)
        {
            var msg = _netMgr.CreateNetMessage<MsgUpdateAdminStatus>();

            var commands = new List<string>(_anyCommands);

            if (_admins.TryGetValue(session, out var adminData))
            {
                msg.Admin = adminData.Data;

                commands.AddRange(_adminCommands
                    .Where(p => p.Value.Any(f => adminData.Data.HasFlag(f)))
                    .Select(p => p.Key));
            }

            msg.AvailableCommands = commands.ToArray();

            _netMgr.ServerSendMessage(msg, session.ConnectedClient);
        }

        private void PlayerStatusChanged(object? sender, SessionStatusEventArgs e)
        {
            if (e.NewStatus == SessionStatus.Connected)
            {
                // Run this so that available commands list gets sent.
                UpdateAdminStatus(e.Session);
            }
            else if (e.NewStatus == SessionStatus.InGame)
            {
                LoginAdminMaybe(e.Session);
            }
            else if (e.NewStatus == SessionStatus.Disconnected)
            {
                _admins.Remove(e.Session);
            }
        }

        private async void LoginAdminMaybe(IPlayerSession session)
        {
            AdminReg reg;
            if (IsLocal(session) && _cfg.GetCVar(CCVars.ConsoleLoginLocal))
            {
                var data = new AdminData
                {
                    Title = Loc.GetString("Host"),
                    Flags = AdminFlagsExt.Everything,
                };

                reg = new AdminReg(session, data)
                {
                    IsSpecialLogin = true,
                };
            }
            else
            {
                var dbData = await _dbManager.GetAdminDataForAsync(session.UserId);

                if (dbData == null)
                {
                    // Not an admin!
                    return;
                }

                var flags = AdminFlags.None;

                if (dbData.AdminRank != null)
                {
                    flags = AdminFlagsExt.NamesToFlags(dbData.AdminRank.Flags.Select(p => p.Flag));
                }

                foreach (var dbFlag in dbData.Flags)
                {
                    var flag = AdminFlagsExt.NameToFlag(dbFlag.Flag);
                    if (dbFlag.Negative)
                    {
                        flags &= ~flag;
                    }
                    else
                    {
                        flags |= flag;
                    }
                }

                var data = new AdminData
                {
                    Flags = flags
                };

                if (dbData.Title != null)
                {
                    data.Title = dbData.Title;
                }
                else if (dbData.AdminRank != null)
                {
                    data.Title = dbData.AdminRank.Name;
                }

                reg = new AdminReg(session, data);
            }

            _admins.Add(session, reg);

            if (!session.ContentData()!.ExplicitlyDeadminned)
            {
                reg.Data.Active = true;
            }

            UpdateAdminStatus(session);
        }

        private static bool IsLocal(IPlayerSession player)
        {
            var ep = player.ConnectedClient.RemoteEndPoint;
            var addr = ep.Address;
            if (addr.IsIPv4MappedToIPv6)
            {
                addr = addr.MapToIPv4();
            }

            return Equals(addr, IPAddress.Loopback) || Equals(addr, IPAddress.IPv6Loopback);
        }

        public bool CanCommand(IPlayerSession session, string cmdName)
        {
            if (_anyCommands.Contains(cmdName))
            {
                // Anybody can use this command.
                return true;
            }

            if (!_adminCommands.TryGetValue(cmdName, out var flagsReq))
            {
                // Server-console only.
                return false;
            }

            var data = GetAdminData(session);
            if (data == null)
            {
                // Player isn't an admin.
                return false;
            }

            foreach (var flagReq in flagsReq)
            {
                if (data.HasFlag(flagReq))
                {
                    return true;
                }
            }

            return false;
        }

        private static (bool isAvail, AdminFlags[] flagsReq) GetRequiredFlag(IClientCommand cmd)
        {
            var type = cmd.GetType();
            if (Attribute.IsDefined(type, typeof(AnyCommandAttribute)))
            {
                // Available to everybody.
                return (true, Array.Empty<AdminFlags>());
            }

            var attribs = type.GetCustomAttributes(typeof(AdminCommandAttribute))
                .Cast<AdminCommandAttribute>()
                .Select(p => p.Flags)
                .ToArray();

            // If attribs.length == 0 then no access attribute is specified,
            // and this is a server-only command.
            return (attribs.Length != 0, attribs);
        }

        public bool CanViewVar(IPlayerSession session)
        {
            return GetAdminData(session)?.CanViewVar() ?? false;
        }

        public bool CanAdminPlace(IPlayerSession session)
        {
            return GetAdminData(session)?.CanAdminPlace() ?? false;
        }

        public bool CanScript(IPlayerSession session)
        {
            return GetAdminData(session)?.CanScript() ?? false;
        }

        public bool CanAdminMenu(IPlayerSession session)
        {
            return GetAdminData(session)?.CanAdminMenu() ?? false;
        }

        private sealed class AdminReg
        {
            public IPlayerSession Session;

            public AdminData Data;

            // Such as console.loginlocal
            // Means that stuff like permissions editing is blocked.
            public bool IsSpecialLogin;

            public AdminReg(IPlayerSession session, AdminData data)
            {
                Data = data;
                Session = session;
            }
        }
    }
}

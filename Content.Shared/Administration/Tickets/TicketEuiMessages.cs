﻿using System;
using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Tickets
{
    [NetSerializable, Serializable]
    public class TicketEuiState : EuiStateBase
    {
        public Ticket? ticket { get; }

        public TicketEuiState(Ticket? _ticket)
        {
            ticket = _ticket;
        }
    }

    public static class TicketsEuiMsg
    {
        [Serializable, NetSerializable]
        public sealed class TicketSendMessage : EuiMessageBase
        {
            public string Message;

            public TicketSendMessage(string _message)
            {
                Message = _message;
            }
        }

        [Serializable, NetSerializable]
        public sealed class TicketReceiveMessage : EuiMessageBase
        {
            public TicketMessage Message;

            public TicketReceiveMessage(TicketMessage _message)
            {
                Message = _message;
            }
        }

        [Serializable, NetSerializable]
        public sealed class TicketChangeStatus : EuiMessageBase
        {
            public TicketStatus Status;
            public NetUserId? Admin;
            public string? AdminUsername;

            public TicketChangeStatus(TicketStatus status, NetUserId? admin = null, string? username = null)
            {
                Status = status;
                Admin = admin;
                AdminUsername = username;
            }
        }

        /*[Serializable, NetSerializable]
        public sealed class AddAdmin : EuiMessageBase
        {
            public string UserNameOrId = string.Empty;
            public string? Title;
            public AdminFlags PosFlags;
            public AdminFlags NegFlags;
            public int? RankId;
        }

        [Serializable, NetSerializable]
        public sealed class RemoveAdmin : EuiMessageBase
        {
            public NetUserId UserId;
        }*/
    }
}


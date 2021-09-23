using Lidgren.Network;
using Robust.Shared.Network;
using JetBrains.Annotations;

#nullable disable

namespace Content.Shared.Administration.Tickets
{
    public class MsgViewTicket : NetMessage
    {
        public override MsgGroups MsgGroup => MsgGroups.Command;

        public int TicketId { get; set; }

        public override void ReadFromBuffer(NetIncomingMessage buffer)
        {
            TicketId = buffer.ReadInt32();
        }

        public override void WriteToBuffer(NetOutgoingMessage buffer)
        {
            buffer.Write(TicketId);
        }
    }
}

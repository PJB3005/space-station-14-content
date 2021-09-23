using Lidgren.Network;
using Robust.Shared.Network;

namespace Content.Shared.Administration.AdminMenu
{
    public class AdminMenuTicketListRequest : NetMessage
    {
        public override MsgGroups MsgGroup => MsgGroups.Command;

        public override void ReadFromBuffer(NetIncomingMessage buffer)
        {
        }

        public override void WriteToBuffer(NetOutgoingMessage buffer)
        {
        }
    }
}

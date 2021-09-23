using JetBrains.Annotations;
using Lidgren.Network;
using Robust.Shared.Network;

namespace Content.Shared.Administration.Tickets
{
    [UsedImplicitly]
    public class MsgTicketMessage : NetMessage
    {
        public override MsgGroups MsgGroup => MsgGroups.Command;

        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;

        public NetUserId TargetPlayer { get; set; }

        public NetUserId? ClaimedAdmin { get; set; }

        public TicketAction Action { get; set; }

        public override void ReadFromBuffer(NetIncomingMessage buffer)
        {
            /*Channel = (ChatChannel) buffer.ReadInt16();
            Message = buffer.ReadString();
            MessageWrap = buffer.ReadString();

            switch (Channel)
            {
                case ChatChannel.Local:
                case ChatChannel.Dead:
                case ChatChannel.AdminChat:
                case ChatChannel.Emotes:
                    SenderEntity = buffer.ReadEntityUid();
                    break;
            }*/
        }

        public override void WriteToBuffer(NetOutgoingMessage buffer)
        {
            /*buffer.Write((short)Channel);
            buffer.Write(Message);
            buffer.Write(MessageWrap);

            switch (Channel)
            {
                case ChatChannel.Local:
                case ChatChannel.Dead:
                case ChatChannel.AdminChat:
                case ChatChannel.Emotes:
                    buffer.Write(SenderEntity);
                    break;
            }*/
        }
    }
}

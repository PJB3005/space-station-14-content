using Content.Shared.GameObjects;
using Lidgren.Network;
using SS14.Shared;
using SS14.Shared.GameObjects;

namespace Content.Server.GameObjects.Components.Mobs
{
    public class ServerIntentComponent : SharedIntentComponent
    {
        public Intent Intent { get; set; }

        public override ComponentState GetComponentState()
        {
            return new IntentComponentState(Intent);
        }

        public override void HandleNetworkMessage(IncomingEntityComponentMessage message, NetConnection sender)
        {
            if (message.MessageParameters.Count != 1)
            {
                return;
            }
            Intent = (Intent)message.MessageParameters[0];
        }
    }
}

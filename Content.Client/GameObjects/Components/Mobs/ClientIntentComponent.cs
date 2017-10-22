using Lidgren.Network;
using Content.Shared.GameObjects;
using SS14.Shared.GameObjects;
using Content.Client.UserInterface;
using SS14.Client.UserInterface;
using SS14.Client.Interfaces.UserInterface;
using SS14.Shared.IoC;
using SS14.Shared;

namespace Content.Client.GameObjects
{
    public class ClientIntentComponent : SharedIntentComponent
    {
        public Intent Intent { get; private set; }

        public void SendChangeIntent(Intent intent)
        {
            Owner.SendComponentNetworkMessage(this, NetDeliveryMethod.ReliableUnordered, intent);
        }

        public override void HandleComponentState(ComponentState state)
        {
            var castState = (IntentComponentState)state;
            Intent = castState.Intent;

            var uiMgr = (UserInterfaceManager)IoCManager.Resolve<IUserInterfaceManager>();

            if (uiMgr.GetSingleComponentByGuiComponentType(GuiComponentType.HandsUi) == null)
            {
                uiMgr.AddComponent(new IntentGui());
            }
            uiMgr.ComponentUpdate(GuiComponentType.HandsUi, this);
        }
    }
}

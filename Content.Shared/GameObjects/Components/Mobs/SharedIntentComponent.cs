using System;
using SS14.Shared.GameObjects;

namespace Content.Shared.GameObjects
{
    public enum Intent
    {
        Help,
        Harm
    }

    public abstract class SharedIntentComponent : Component
    {
        public override string Name => "Intent";
        public override uint? NetID => ContentNetIDs.INTENT;
        public override Type StateType => typeof(IntentComponentState);
    }

    [Serializable]
    public class IntentComponentState : ComponentState
    {
        public readonly Intent Intent;
        public IntentComponentState(Intent intent) : base(ContentNetIDs.INTENT)
        {
            Intent = intent;
        }
    }
}

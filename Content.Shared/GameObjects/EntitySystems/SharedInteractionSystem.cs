using Content.Shared.Input;
using Content.Shared.Interfaces.GameObjects.Components;
using Content.Shared.Utility;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Input.Binding;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Physics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Players;

namespace Content.Shared.GameObjects.EntitySystems
{
    /// <summary>
    /// Governs interactions during clicking on entities
    /// </summary>
    [UsedImplicitly]
    public abstract partial class SharedInteractionSystem : EntitySystem
    {
        [Dependency] private readonly IPhysicsManager _physicsManager = default!;

        public const float InteractionRange = 2;
        public const float InteractionRangeSquared = InteractionRange * InteractionRange;

        public override void Initialize()
        {
            CommandBinds.Builder
                .Bind(ContentKeyFunctions.ActivateItemInWorld,
                    new PointerInputCmdHandler(HandleActivateItemInWorld))
                .Register<SharedInteractionSystem>();
        }

        public override void Shutdown()
        {
            CommandBinds.Unregister<SharedInteractionSystem>();
            base.Shutdown();
        }

        private bool HandleActivateItemInWorld(ICommonSession session, EntityCoordinates coords, EntityUid uid)
        {
            if (!EntityManager.TryGetEntity(uid, out var used))
                return false;

            var playerEnt = session.AttachedEntity;

            if (playerEnt == null || !playerEnt.IsValid())
            {
                return false;
            }

            if (!playerEnt.Transform.Coordinates.InRange(EntityManager, used.Transform.Coordinates, InteractionRange))
            {
                return false;
            }

            InteractionActivate(playerEnt, used);
            return true;
        }

        protected void InteractionActivate(IEntity user, IEntity used)
        {
            var activateMsg = new ActivateInWorldMessage(user, used);
            RaiseLocalEvent(activateMsg);
            if (activateMsg.Handled)
            {
                return;
            }

            if (!used.TryGetComponent(out IActivate activateComp))
            {
                return;
            }

            // all activates should only fire when in range / unobstructed
            var activateEventArgs = new ActivateEventArgs { User = user, Target = used };
            if (activateEventArgs.InRangeUnobstructed(ignoreInsideBlocker: true, popup: true))
            {
                activateComp.Activate(activateEventArgs);
            }
        }
    }
}

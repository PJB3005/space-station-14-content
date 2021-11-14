using Robust.Server.Console;
using Robust.Server.Mapping;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Sandbox;

public sealed class SandboxSystem : EntitySystem
{
    [Dependency] private readonly ISandboxManager _sandboxMgr = default!;
    [Dependency] private readonly IConGroupController _conGroup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MappingTryAction>(OnMappingTryAction);
    }

    private void OnMappingTryAction(MappingTryAction msg, EntitySessionEventArgs args)
    {
        if (_sandboxMgr.IsSandboxEnabled)
            return;

        if (!_conGroup.CanAdminPlace(msg.Session))
            msg.Blocked = true;
    }
}

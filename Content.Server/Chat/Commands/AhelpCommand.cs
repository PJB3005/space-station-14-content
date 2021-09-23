#nullable enable
using Content.Server.Administration;
using Content.Shared.Interfaces;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.IoC;

namespace Content.Server.Chat.Commands
{
    [AnyCommand]
    internal class AhelpCommand : IConsoleCommand
    {
        public string Command => "ahelp";
        public string Description => "Send a message to the admins.";
        public string Help => "ahelp <text>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player as IPlayerSession;
            if (player == null)
            {
                shell.WriteLine("This command cannot be run from the server.");
                return;
            }

            if (player.Status != SessionStatus.InGame || !player.AttachedEntityUid.HasValue)
                return;

            if (args.Length < 1)
                return;

            var message = string.Join(" ", args).Trim();
            if (string.IsNullOrEmpty(message))
                return;

            var ticketManager = IoCManager.Resolve<ITicketManager>();
            if (ticketManager.HasTicket(player.UserId))
            {
                shell.WriteError("You already have an open ticket!");
                return;
            }
            ticketManager.CreateTicket(player.UserId, player.UserId, message);
        }
    }
}

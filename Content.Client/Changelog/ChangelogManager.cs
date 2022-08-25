using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Utility;


namespace Content.Client.Changelog
{
    public sealed class ChangelogManager
    {
        [Dependency] private readonly IResourceManager _resource = default!;
        [Dependency] private readonly ISerializationManager _serialization = default!;
        [Dependency] private readonly IConfigurationManager _configManager = default!;

        public bool NewChangelogEntries { get; private set; }
        public int LastReadId { get; private set; }
        public int MaxId { get; private set; }

        public event Action? NewChangelogEntriesChanged;

        /// <summary>
        ///     Ran when the user opens ("read") the changelog,
        ///     stores the new ID to disk and clears <see cref="NewChangelogEntries"/>.
        /// </summary>
        /// <remarks>
        ///     <see cref="LastReadId"/> is NOT cleared
        ///     since that's used in the changelog menu to show the "since you last read" bar.
        /// </remarks>
        public void SaveNewReadId()
        {
            NewChangelogEntries = false;
            NewChangelogEntriesChanged?.Invoke();

            using var sw = _resource.UserData.OpenWriteText(new ResourcePath($"/changelog_last_seen_{_configManager.GetCVar(CCVars.ServerId)}"));

            sw.Write(MaxId.ToString());
        }

        public async void Initialize()
        {
            // Open changelog purely to compare to the last viewed date.
            var changelog = await LoadChangelog();

            if (changelog.Count == 0)
            {
                return;
            }

            MaxId = changelog.Max(c => c.Id);

            var path = new ResourcePath($"/changelog_last_seen_{_configManager.GetCVar(CCVars.ServerId)}");
            if(_resource.UserData.TryReadAllText(path, out var lastReadIdText))
            {
                LastReadId = int.Parse(lastReadIdText);
            }

            NewChangelogEntries = LastReadId < MaxId;

            NewChangelogEntriesChanged?.Invoke();
        }

        public Task<List<ChangelogEntry>> LoadChangelog()
        {
            return Task.Run(() =>
            {
                var yamlData = _resource.ContentFileReadText(new ResourcePath("/Changelog/Changelog.yml"));
                var documents = DataNodeParser.ParseYamlStream(yamlData).ToArray();

                if (documents.Length == 0)
                    return new List<ChangelogEntry>();

                var node = (MappingDataNode)documents[0].Root;
                return _serialization.Read<List<ChangelogEntry>>(node["Entries"]);
            });
        }

        [DataDefinition]
        public sealed class ChangelogEntry : ISerializationHooks
        {
            [DataField("id")]
            public int Id { get; private set; }

            [DataField("author")]
            public string Author { get; } = "";

            [DataField("time")] private string _time = default!;

            public DateTime Time { get; private set; }

            [DataField("changes")]
            public List<ChangelogChange> Changes { get; } = default!;

            void ISerializationHooks.AfterDeserialization()
            {
                Time = DateTime.Parse(_time, null, DateTimeStyles.RoundtripKind);
            }
        }

        [DataDefinition]
        public sealed class ChangelogChange : ISerializationHooks
        {
            [DataField("type")]
            public ChangelogLineType Type { get; private set; }

            [DataField("message")]
            public string Message { get; private set; } = "";
        }

        public enum ChangelogLineType
        {
            Add,
            Remove,
            Fix,
            Tweak,
        }
    }
}

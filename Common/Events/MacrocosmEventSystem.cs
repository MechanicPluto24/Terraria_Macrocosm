using Macrocosm.Common.Subworlds;
using SubworldLibrary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Events;

public class MacrocosmEventSystem : ModSystem
{
    private const string EventsTagName = "MacrocosmEvents";
    private const string GlobalEventsTagName = "Global";
    private const string LocalEventsTagName = "Local";
    private const string EventListTagName = "Events";

    public static MacrocosmEventSystem Instance => ModContent.GetInstance<MacrocosmEventSystem>();

    private readonly Dictionary<string, MacrocosmEventState> globalStates = [];
    private readonly Dictionary<string, Dictionary<string, MacrocosmEventState>> localStates = [];

    public static bool HasLoadedEvents => MacrocosmEvent.Events.Count > 0;

    public override void ClearWorld()
    {
        globalStates.Clear();
        localStates.Clear();
    }

    public override void Unload()
    {
        globalStates.Clear();
        localStates.Clear();
        MacrocosmEvent.Events.Clear();
    }

    public override void PreUpdateWorld()
    {
        UpdateGlobalEvents();
    }

    public override void SaveWorldData(TagCompound tag) => SaveData(tag);

    public override void LoadWorldData(TagCompound tag) => LoadData(tag);

    public override void NetSend(BinaryWriter writer) => NetSendData(writer);

    public override void NetReceive(BinaryReader reader) => NetReceiveData(reader);

    public static T GetEvent<T>() where T : MacrocosmEvent
        => MacrocosmEvent.Events.OfType<T>().FirstOrDefault();

    public static MacrocosmEvent GetEvent(string eventFullName)
        => MacrocosmEvent.Events.FirstOrDefault(@event => @event.EventFullName == eventFullName);

    public static bool IsActive<T>(string subworldId = null) where T : MacrocosmEvent
        => GetState<T>(subworldId)?.Active ?? false;

    public static bool IsActive(string eventFullName, string subworldId = null)
        => Instance.TryGetState(eventFullName, subworldId, out var state) && state.Active;

    public static MacrocosmEventState GetState<T>(string subworldId = null) where T : MacrocosmEvent
    {
        MacrocosmEvent @event = GetEvent<T>();
        return @event is null ? null : Instance.GetOrCreateState(@event, Instance.ResolveSubworldId(@event, subworldId));
    }

    public static MacrocosmEventState GetState(string eventFullName, string subworldId = null)
    {
        MacrocosmEvent @event = GetEvent(eventFullName);
        return @event is null ? null : Instance.GetOrCreateState(@event, Instance.ResolveSubworldId(@event, subworldId));
    }

    public static void Start<T>(string subworldId = null) where T : MacrocosmEvent
    {
        MacrocosmEvent @event = GetEvent<T>();
        if (@event is not null)
            Instance.SetActive(@event, Instance.ResolveSubworldId(@event, subworldId), true);
    }

    public static void Start(string eventFullName, string subworldId = null)
    {
        MacrocosmEvent @event = GetEvent(eventFullName);
        if (@event is not null)
            Instance.SetActive(@event, Instance.ResolveSubworldId(@event, subworldId), true);
    }

    public static void End<T>(string subworldId = null) where T : MacrocosmEvent
    {
        MacrocosmEvent @event = GetEvent<T>();
        if (@event is not null)
            Instance.SetActive(@event, Instance.ResolveSubworldId(@event, subworldId), false);
    }

    public static void End(string eventFullName, string subworldId = null)
    {
        MacrocosmEvent @event = GetEvent(eventFullName);
        if (@event is not null)
            Instance.SetActive(@event, Instance.ResolveSubworldId(@event, subworldId), false);
    }

    public static void UpdateLocalEvents(MacrocosmSubworld subworld)
    {
        if (subworld is not null)
            Instance.UpdateLocalEventsImpl(subworld);
    }

    public static void SaveData(TagCompound tag)
    {
        TagCompound eventTag = new();

        Dictionary<string, TagCompound> globalEventTags = [];
        foreach (var (eventFullName, state) in Instance.globalStates)
        {
            MacrocosmEvent @event = GetEvent(eventFullName);
            if (@event is null)
                continue;

            globalEventTags[eventFullName] = Instance.SaveStateToTag(@event, state);
        }

        if (globalEventTags.Count > 0)
            eventTag[GlobalEventsTagName] = SaveEventTags(globalEventTags);

        List<TagCompound> localEntries = [];
        foreach (var (subworldId, eventStates) in Instance.localStates)
        {
            Dictionary<string, TagCompound> localEventTags = [];

            foreach (var (eventFullName, state) in eventStates)
            {
                MacrocosmEvent @event = GetEvent(eventFullName);
                if (@event is null)
                    continue;

                localEventTags[eventFullName] = Instance.SaveStateToTag(@event, state);
            }

            if (localEventTags.Count == 0)
                continue;

            TagCompound localEntry = new()
            {
                ["ID"] = subworldId,
                [EventListTagName] = SaveEventTags(localEventTags)
            };

            localEntries.Add(localEntry);
        }

        if (localEntries.Count > 0)
            eventTag[LocalEventsTagName] = localEntries;

        if (eventTag.Count > 0)
            tag[EventsTagName] = eventTag;
    }

    public static void LoadData(TagCompound tag)
    {
        Instance.globalStates.Clear();
        Instance.localStates.Clear();

        if (tag.ContainsKey(EventsTagName))
            Instance.LoadSavedEvents(tag.Get<TagCompound>(EventsTagName));
    }

    public static void NetSendData(BinaryWriter writer)
    {
        var globalEntries = Instance.globalStates
            .Select(pair => (EventFullName: pair.Key, Event: GetEvent(pair.Key), State: pair.Value))
            .Where(entry => entry.Event is not null)
            .ToList();

        writer.Write(globalEntries.Count);
        foreach (var entry in globalEntries)
        {
            writer.Write(entry.EventFullName);
            Instance.WriteStatePayload(writer, entry.Event, entry.State);
        }

        var localEntries = Instance.localStates
            .Select(pair => new
            {
                SubworldId = pair.Key,
                States = pair.Value
                    .Select(eventPair => (EventFullName: eventPair.Key, Event: GetEvent(eventPair.Key), State: eventPair.Value))
                    .Where(entry => entry.Event is not null)
                    .ToList()
            })
            .Where(entry => entry.States.Count > 0)
            .ToList();

        writer.Write(localEntries.Count);
        foreach (var entry in localEntries)
        {
            writer.Write(entry.SubworldId);
            writer.Write(entry.States.Count);

            foreach (var stateEntry in entry.States)
            {
                writer.Write(stateEntry.EventFullName);
                Instance.WriteStatePayload(writer, stateEntry.Event, stateEntry.State);
            }
        }
    }

    public static void NetReceiveData(BinaryReader reader)
    {
        Instance.globalStates.Clear();
        Instance.localStates.Clear();

        int globalCount = reader.ReadInt32();
        for (int i = 0; i < globalCount; i++)
        {
            string eventFullName = reader.ReadString();
            MacrocosmEvent @event = GetEvent(eventFullName);
            MacrocosmEventState state = Instance.ReadStatePayload(reader, @event, null);
            if (state is not null)
                Instance.globalStates[eventFullName] = state;
        }

        int subworldCount = reader.ReadInt32();
        for (int i = 0; i < subworldCount; i++)
        {
            string subworldId = reader.ReadString();
            int eventCount = reader.ReadInt32();
            Dictionary<string, MacrocosmEventState> eventStates = [];
            Instance.localStates[subworldId] = eventStates;

            for (int j = 0; j < eventCount; j++)
            {
                string eventFullName = reader.ReadString();
                MacrocosmEvent @event = GetEvent(eventFullName);
                MacrocosmEventState state = Instance.ReadStatePayload(reader, @event, subworldId);
                if (state is not null)
                    eventStates[eventFullName] = state;
            }
        }
    }

    private void UpdateGlobalEvents()
    {
        foreach (MacrocosmEvent @event in MacrocosmEvent.Events.Where(@event => @event.Scope == MacrocosmEventScope.Global))
        {
            MacrocosmEventState state = GetOrCreateState(@event, null);
            @event.Update(CreateContext(@event, null), state);
        }
    }

    private void UpdateLocalEventsImpl(MacrocosmSubworld subworld)
    {
        foreach (MacrocosmEvent @event in MacrocosmEvent.Events.Where(@event => @event.Scope == MacrocosmEventScope.SubworldLocal))
        {
            MacrocosmEventState state = GetOrCreateState(@event, subworld.ID);
            @event.Update(CreateContext(@event, subworld.ID), state);
        }
    }

    private MacrocosmEventContext CreateContext(MacrocosmEvent @event, string subworldId)
    {
        MacrocosmSubworld activeSubworld = SubworldSystem.AnyActive<Macrocosm>() ? MacrocosmSubworld.Current : null;
        MacrocosmSubworld currentSubworld = activeSubworld is not null && (subworldId is null || activeSubworld.ID == subworldId)
            ? activeSubworld
            : null;

        return new MacrocosmEventContext(@event, currentSubworld, subworldId, Main.desiredWorldEventsUpdateRate);
    }

    private string ResolveSubworldId(MacrocosmEvent @event, string subworldId)
    {
        if (@event.Scope == MacrocosmEventScope.Global)
            return null;

        return string.IsNullOrEmpty(subworldId) && SubworldSystem.AnyActive<Macrocosm>()
            ? MacrocosmSubworld.CurrentID
            : subworldId;
    }

    private bool TryGetState(string eventFullName, string subworldId, out MacrocosmEventState state)
    {
        state = null;
        MacrocosmEvent @event = GetEvent(eventFullName);
        if (@event is null)
            return false;

        string resolvedSubworldId = ResolveSubworldId(@event, subworldId);

        if (@event.Scope == MacrocosmEventScope.Global)
            return globalStates.TryGetValue(eventFullName, out state);

        return resolvedSubworldId is not null
            && localStates.TryGetValue(resolvedSubworldId, out var eventStates)
            && eventStates.TryGetValue(eventFullName, out state);
    }

    private MacrocosmEventState GetOrCreateState(MacrocosmEvent @event, string subworldId)
    {
        if (@event.Scope == MacrocosmEventScope.Global)
        {
            if (!globalStates.TryGetValue(@event.EventFullName, out var globalState))
            {
                globalState = InitializeState(@event, null);
                globalStates[@event.EventFullName] = globalState;
            }

            return globalState;
        }

        if (string.IsNullOrEmpty(subworldId))
            return null;

        Dictionary<string, MacrocosmEventState> eventStates = GetOrCreateLocalStateMap(subworldId);
        if (!eventStates.TryGetValue(@event.EventFullName, out var localState))
        {
            localState = InitializeState(@event, subworldId);
            eventStates[@event.EventFullName] = localState;
        }

        return localState;
    }

    private Dictionary<string, MacrocosmEventState> GetOrCreateLocalStateMap(string subworldId)
    {
        if (!localStates.TryGetValue(subworldId, out var eventStates))
        {
            eventStates = [];
            localStates[subworldId] = eventStates;
        }

        return eventStates;
    }

    private MacrocosmEventState InitializeState(MacrocosmEvent @event, string subworldId)
    {
        MacrocosmEventState state = @event.CreateState();
        @event.OnInitializeState(state, subworldId);
        state.Initialized = true;
        return state;
    }

    private void SetActive(MacrocosmEvent @event, string subworldId, bool active)
    {
        MacrocosmEventState state = GetOrCreateState(@event, subworldId);
        if (state is null || state.Active == active)
            return;

        state.Active = active;

        MacrocosmEventContext context = CreateContext(@event, subworldId);
        if (active)
            @event.OnStarted(context, state);
        else
            @event.OnEnded(context, state);

        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.WorldData);
    }

    private TagCompound SaveStateToTag(MacrocosmEvent @event, MacrocosmEventState state)
    {
        TagCompound tag = new()
        {
            ["Active"] = state.Active,
            ["Initialized"] = state.Initialized
        };

        @event.SaveState(state, tag);
        return tag;
    }

    private MacrocosmEventState LoadStateFromTag(MacrocosmEvent @event, string subworldId, TagCompound tag)
    {
        MacrocosmEventState state = InitializeState(@event, subworldId);
        state.Active = tag.GetBool("Active");
        state.Initialized = !tag.ContainsKey("Initialized") || tag.GetBool("Initialized");
        @event.LoadState(state, tag);
        return state;
    }

    private void WriteStatePayload(BinaryWriter writer, MacrocosmEvent @event, MacrocosmEventState state)
    {
        using MemoryStream stream = new();
        using BinaryWriter payloadWriter = new(stream);

        payloadWriter.Write(state.Active);
        payloadWriter.Write(state.Initialized);
        @event.NetSendState(state, payloadWriter);
        payloadWriter.Flush();

        byte[] payload = stream.ToArray();
        writer.Write(payload.Length);
        writer.Write(payload);
    }

    private MacrocosmEventState ReadStatePayload(BinaryReader reader, MacrocosmEvent @event, string subworldId)
    {
        int payloadLength = reader.ReadInt32();
        byte[] payload = reader.ReadBytes(payloadLength);

        if (@event is null)
            return null;

        using MemoryStream stream = new(payload);
        using BinaryReader payloadReader = new(stream);

        MacrocosmEventState state = InitializeState(@event, subworldId);
        state.Active = payloadReader.ReadBoolean();
        state.Initialized = payloadReader.ReadBoolean();
        @event.NetReceiveState(state, payloadReader);
        return state;
    }

    private void LoadSavedEvents(TagCompound eventTag)
    {
        if (eventTag.ContainsKey(GlobalEventsTagName))
            LoadGlobalTags(LoadEventTags(eventTag.GetList<TagCompound>(GlobalEventsTagName)));

        if (!eventTag.ContainsKey(LocalEventsTagName))
            return;

        foreach (TagCompound entry in eventTag.GetList<TagCompound>(LocalEventsTagName))
        {
            string subworldId = entry.GetString("ID");
            if (string.IsNullOrEmpty(subworldId) || !entry.ContainsKey(EventListTagName))
                continue;

            LoadLocalTags(subworldId, LoadEventTags(entry.GetList<TagCompound>(EventListTagName)));
        }
    }

    private void LoadGlobalTags(IEnumerable<KeyValuePair<string, TagCompound>> eventTags)
    {
        foreach (var (eventFullName, stateTag) in eventTags)
        {
            MacrocosmEvent @event = GetEvent(eventFullName);
            if (@event is null)
                continue;

            globalStates[eventFullName] = LoadStateFromTag(@event, null, stateTag);
        }
    }

    private void LoadLocalTags(string subworldId, IEnumerable<KeyValuePair<string, TagCompound>> eventTags)
    {
        Dictionary<string, MacrocosmEventState> eventStates = GetOrCreateLocalStateMap(subworldId);

        foreach (var (eventFullName, stateTag) in eventTags)
        {
            MacrocosmEvent @event = GetEvent(eventFullName);
            if (@event is null)
                continue;

            eventStates[eventFullName] = LoadStateFromTag(@event, subworldId, stateTag);
        }
    }

    private static List<TagCompound> SaveEventTags(Dictionary<string, TagCompound> events)
    {
        List<TagCompound> entries = [];
        foreach (var (eventId, eventTag) in events)
        {
            TagCompound entry = new()
            {
                ["ID"] = eventId,
                ["State"] = eventTag
            };

            entries.Add(entry);
        }

        return entries;
    }

    private static IEnumerable<KeyValuePair<string, TagCompound>> LoadEventTags(IList<TagCompound> eventTags)
    {
        foreach (TagCompound entry in eventTags)
            yield return new(entry.GetString("ID"), entry.Get<TagCompound>("State"));
    }

}

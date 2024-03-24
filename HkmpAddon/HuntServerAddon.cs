using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Hkmp.Api.Server;
using Newtonsoft.Json;

namespace TheHuntIsOn.HkmpAddon;

public class HuntServerAddon : ServerAddon
{
    private const string NetworkedItemsFileName = "networked-items.json";
    private const string DefaultNetworkedItemsFilePath = "TheHuntIsOn.Resources.default-networked-items.json";
    
    protected override string Name => AddonIdentifier.Name;
    protected override string Version => AddonIdentifier.Version;
    public override bool NeedsNetwork => true;
    
    private ServerNetManager NetManager { get; set; }
    
    private Dictionary<NetItem, ItemGrant> NetworkedItems { get; set; }
    
    public override void Initialize(IServerApi serverApi)
    {
        NetManager = new ServerNetManager(this, serverApi.NetServer);

        if (!File.Exists(GetNetworkedItemsFilePath()))
        {
            Logger.Info($"Could not find networked items file: {NetworkedItemsFileName}, copying default");
            ExportDefaultNetworkedItems();
        }
        
        LoadNetworkedItems();

        NetManager.ItemObtainedEvent += OnItemObtained;
    }

    private void OnItemObtained(ushort id, NetItem item)
    {
        if (!NetworkedItems.TryGetValue(item, out var itemGrant))
        {
            return;
        }

        if (itemGrant.Items != null)
            NetManager.SendGrantItems(itemGrant.Items);
        
        if (itemGrant.Message != null)
            ServerApi.ServerManager.BroadcastMessage(itemGrant.Message);
    }

    private void ExportDefaultNetworkedItems()
    {
        using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DefaultNetworkedItemsFilePath);
        using var fileStream = new FileStream(GetNetworkedItemsFilePath(), FileMode.Create, FileAccess.Write);

        resourceStream?.CopyTo(fileStream);
        fileStream.Flush();
    }

    private void LoadNetworkedItems()
    {
        try
        {
            var fileContents = File.ReadAllText(GetNetworkedItemsFilePath());

            NetworkedItems = JsonConvert.DeserializeObject<Dictionary<NetItem, ItemGrant>>(fileContents);
            
            Logger.Info("Loaded networked items");
        }
        catch (Exception e)
        {
            Logger.Error($"Could not read networked items:\n{e}");
        }
    }

    private string GetNetworkedItemsFilePath()
    {
        var location = Assembly.GetExecutingAssembly().Location;
        var directory = Directory.GetParent(location)!;
        
        return Path.Combine(directory.FullName, NetworkedItemsFileName);
    }

    private class ItemGrant
    {
        [JsonProperty("items")]
        public List<NetItem> Items { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
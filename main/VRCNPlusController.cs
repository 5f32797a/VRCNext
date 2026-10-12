using Newtonsoft.Json.Linq;
using VRCNext.Services;
using VRCNext.Services.VRCNPlusAPI;

namespace VRCNext;

public class VRCNPlusController
{
    private readonly CoreLibrary _core;
    private readonly VRCNPlusService _service;
    private string _loggedEntitlementForUser = "";
    private readonly object _logGate = new();

    public VRCNPlusController(CoreLibrary core)
    {
        _core    = core;
        _service = new VRCNPlusService();
    }

    public void OnOwnUserKnown(string userId)
    {
        if (!Database.IsValidVrcUserId(userId)) return;
        lock (_logGate)
        {
            if (_loggedEntitlementForUser == userId) return;
            _loggedEntitlementForUser = userId;
        }
        _ = Task.Run(async () =>
        {
            try
            {
                var isPlus = await _service.CheckEntitlementAsync(userId);
                if (isPlus)
                    _core.SendToJS("log", new { msg = "[VRCN] - VRCN+ Active", color = "ok" });
                _core.SendToJS("vrcnPlusEntitlement", new { userId, isPlus });
            }
            catch { }
        });
    }

    public async Task HandleMessage(string action, JObject msg)
    {
        switch (action)
        {
            case "vrcnPlusCheckEntitlement": {
                var userId = msg["userId"]?.ToString() ?? "";
                if (!Database.IsValidVrcUserId(userId))
                {
                    _core.SendToJS("vrcnPlusEntitlement", new { userId, isPlus = false });
                    return;
                }
                var isPlus = await _service.CheckEntitlementAsync(userId);
                _core.SendToJS("vrcnPlusEntitlement", new { userId, isPlus });
                return;
            }
        }
    }
}

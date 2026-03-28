using UnityEngine;
using HazardSpam.Menu.Settings;

namespace HazardSpam.Menu;

public class MenuHandler
{
    private MenuCanvas _menuCanvas = null!;
    private readonly KeyCode _toggleKey = KeyCode.Delete;

    public void Initialize()
    {
        try
        {
            if (Plugin.DebugMenu)
                Plugin.Log.LogInfo("Starting menu initialization...");
            
            // Load settings from file (or create default if file doesn't exist)
            MenuSettings.LoadSettings();
            Plugin.Log.LogInfo("MenuSettings loaded from file");
            
            _menuCanvas = new MenuCanvas();
            if (Plugin.DebugMenu)
                Plugin.Log.LogInfo("MenuCanvas created successfully");
            
            if (_menuCanvas.Canvas != null)
            {
                if (Plugin.DebugMenu)
                    Plugin.Log.LogInfo("Menu system initialized successfully");
                _menuCanvas.SetActive(false); // Start hidden but ready
            }
            else
            {
                Plugin.Log.LogError("MenuCanvas.Canvas is null after creation!");
                _menuCanvas = null!;
            }
        }
        catch (System.Exception ex)
        {
            Plugin.Log.LogError($"Failed to initialize menu system: {ex.Message}");
            Plugin.Log.LogError($"Stack trace: {ex.StackTrace}");
            _menuCanvas = null!;
        }
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (_menuCanvas == null)
        {
            Plugin.Log.LogError("MenuCanvas is null! Attempting to reinitialize...");
            Initialize();
            return;
        }
        
        if (_menuCanvas.Canvas == null)
        {
            Plugin.Log.LogError("MenuCanvas.Canvas is null! Attempting to reinitialize...");
            Initialize();
            return;
        }
        
        bool isVisible = _menuCanvas.Canvas.gameObject.activeSelf;
        _menuCanvas.SetActive(!isVisible);
        
        // Save settings when closing the menu
        if (isVisible)
        {
            MenuSettings.SaveSettings();
            HazardConfigSettings.SaveSettings();
            Plugin.Log.LogInfo("Settings saved to file");
        }
        
        if (Plugin.DebugMenu)
            Plugin.Log.LogInfo($"Menu toggled: {(isVisible ? "Closed" : "Open")}");
    }
    
    public void UpdateSidebarTabCount(string biomeName, int totalHazards)
    {
        if (_menuCanvas != null)
        {
            _menuCanvas.UpdateSidebarTabCount(biomeName, totalHazards);
        }
    }
}

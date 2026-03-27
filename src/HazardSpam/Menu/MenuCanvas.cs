using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using HazardSpam.Menu.Settings;
using HazardSpam.Menu.Descriptors;
using HazardSpam.Menu.TabContent;
using NetGameState.Level;
using Object = UnityEngine.Object; // Explicitly use UnityEngine.Object

namespace HazardSpam.Menu;

public class MenuCanvas
{
    public Canvas Canvas { get; private set; } = null!;
    public RectTransform Panel { get; private set; } = null!;
    public RectTransform Sidebar { get; private set; } = null!;
    public RectTransform MainContent { get; private set; } = null!;
    public RectTransform ScrollContent { get; private set; } = null!;
    
    private readonly List<MenuTab> _tabs = [];
    private MenuTab _activeTab = null!;
    private GameObject _hazardConfigContent = null!;
    private Button _hazardConfigButton = null!;
    
    // Layout constants - now using UIStyle
    private const float CANVAS_WIDTH = 800f;
    private const float CANVAS_HEIGHT = 600f;
    
    public MenuCanvas()
    {
        try
        {
            if (Plugin.DebugMenu)
                Plugin.Log.LogDebug("MenuCanvas: Starting construction...");
            CreateCanvas();
            if (Plugin.DebugMenu)
                Plugin.Log.LogDebug("MenuCanvas: Canvas created");
            CreateSidebar();
            if (Plugin.DebugMenu)
                Plugin.Log.LogDebug("MenuCanvas: Sidebar created");
            CreateMainContent();
            if (Plugin.DebugMenu)
                Plugin.Log.LogDebug("MenuCanvas: Main content created");
            CreateTabs();
            if (Plugin.DebugMenu)
                Plugin.Log.LogDebug("MenuCanvas: Tabs created");
            if (Plugin.DebugMenu)
                Plugin.Log.LogDebug("MenuCanvas: Construction completed successfully");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"MenuCanvas: Construction failed: {ex.Message}");
            Plugin.Log.LogError($"MenuCanvas: Stack trace: {ex.StackTrace}");
            throw;
        }
    }
    
    private void CreateCanvas()
    {
        // Create the main canvas
        GameObject canvasGo = new GameObject("HazardSpamMenu");
        if (Plugin.DebugMenu)
            Plugin.Log.LogDebug("MenuCanvas: Created canvas GameObject");
        
        Canvas = canvasGo.AddComponent<Canvas>();
        if (Plugin.DebugMenu)
            Plugin.Log.LogDebug($"MenuCanvas: Canvas component added, Canvas is null: {Canvas == null}");
        
        // Don't destroy the canvas when loading new scenes
        if (Canvas != null)
        {
            Object.DontDestroyOnLoad(Canvas.gameObject);
        }
        
        // Ensure Canvas is not null before proceeding
        if (Canvas == null)
        {
            Plugin.Log.LogError("MenuCanvas: Canvas component is null! Retrying...");
            Canvas = canvasGo.GetComponent<Canvas>();
            if (Canvas == null)
            {
                Canvas = canvasGo.AddComponent<Canvas>();
            }
        }
        
        Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Canvas.sortingOrder = 1000;
        
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();
        if (Plugin.DebugMenu)
            Plugin.Log.LogDebug("MenuCanvas: Canvas components added");
        
        // Final verification
        if (Plugin.DebugMenu)
            Plugin.Log.LogDebug($"MenuCanvas: Final Canvas check - Canvas is null: {Canvas == null}");
        if (Canvas != null)
        {
            if (Plugin.DebugMenu)
                Plugin.Log.LogDebug($"MenuCanvas: Canvas GameObject name: {Canvas.gameObject.name}");
        }
        
        // Create main panel
        GameObject panelGo = new GameObject("MainPanel");
        panelGo.transform.SetParent(canvasGo.transform, false);
        Panel = panelGo.AddComponent<RectTransform>();
        
        Panel.anchorMin = Panel.anchorMax = new Vector2(0.5f, 0.5f);
        Panel.pivot = new Vector2(0.5f, 0.5f);
        Panel.sizeDelta = new Vector2(CANVAS_WIDTH, CANVAS_HEIGHT);
        Panel.anchoredPosition = Vector2.zero;
        
        var panelImage = panelGo.AddComponent<Image>();
        panelImage.color = UIStyle.Colors.CanvasBackground;

        // Attach a resize watcher to handle responsive sizing
        var watcher = canvasGo.AddComponent<ResizeWatcher>();
        watcher.OnResolutionChanged = () => ApplyResponsiveSizing();
        // Initial sizing
        ApplyResponsiveSizing();
    }
    
    private void CreateSidebar()
    {
        GameObject sidebarGo = new GameObject("Sidebar");
        sidebarGo.transform.SetParent(Panel, false);
        Sidebar = sidebarGo.AddComponent<RectTransform>();
        
        Sidebar.anchorMin = new Vector2(0, 0);
        Sidebar.anchorMax = new Vector2(0, 1);
        Sidebar.pivot = new Vector2(0, 0.5f);
        Sidebar.sizeDelta = new Vector2(UIStyle.Layout.SidebarWidth, -UIStyle.Layout.PanelPadding);
        Sidebar.anchoredPosition = new Vector2(UIStyle.Layout.SidebarPadding, 0f);
        
        var sidebarImage = sidebarGo.AddComponent<Image>();
        sidebarImage.color = UIStyle.Colors.SidebarBackground;
        
        // Add mod title
        CreateModTitle();
    }
    
    private void CreateModTitle()
    {
        GameObject titleGo = new GameObject("ModTitle");
        titleGo.transform.SetParent(Sidebar, false);
        
        RectTransform titleRT = titleGo.AddComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0, 1);
        titleRT.anchorMax = new Vector2(1, 1);
        titleRT.pivot = new Vector2(0.5f, 1);
        titleRT.sizeDelta = new Vector2(-10f, 30f);
        titleRT.anchoredPosition = new Vector2(0, -10f);
        
        Text titleText = titleGo.AddComponent<Text>();
        titleText.font = UIStyle.DefaultFont;
        titleText.color = UIStyle.Colors.TextDefault;
        titleText.text = "HazardSpam";
        titleText.fontSize = UIStyle.FontSizes.Title;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
    }
    
    private void CreateMainContent()
    {
        GameObject contentGo = new GameObject("MainContent");
        contentGo.transform.SetParent(Panel, false);
        MainContent = contentGo.AddComponent<RectTransform>();
        
        MainContent.anchorMin = new Vector2(0, 0);
        MainContent.anchorMax = new Vector2(1, 1);
        MainContent.pivot = new Vector2(0.5f, 0.5f);
        MainContent.sizeDelta = new Vector2(-UIStyle.Layout.SidebarWidth - UIStyle.Layout.PanelPadding, -UIStyle.Layout.PanelPadding);
        MainContent.anchoredPosition = new Vector2(UIStyle.Layout.SidebarWidth / 2f + UIStyle.Spacing.Small, 0f);
        
        var contentImage = contentGo.AddComponent<Image>();
        contentImage.color = UIStyle.Colors.MainContentBackground;
        
        // Add ScrollRect to main content for scrollable content
        var scrollRect = contentGo.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 5f; // Reduced sensitivity
        scrollRect.movementType = ScrollRect.MovementType.Clamped; // Prevent over-scrolling
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.135f;
        
        // Create viewport
        GameObject viewportGo = new GameObject("Viewport");
        viewportGo.transform.SetParent(contentGo.transform, false);
        
        var viewportRT = viewportGo.AddComponent<RectTransform>();
        viewportRT.anchorMin = Vector2.zero;
        viewportRT.anchorMax = Vector2.one;
        viewportRT.sizeDelta = Vector2.zero;
        viewportRT.anchoredPosition = Vector2.zero;
        
        var viewportImage = viewportGo.AddComponent<Image>();
        viewportImage.color = UIStyle.Colors.MainContentBackground;
        
        var mask = viewportGo.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        
        // Create scrollable content area
        GameObject scrollContentGo = new GameObject("ScrollContent");
        scrollContentGo.transform.SetParent(viewportGo.transform, false);
        
        var scrollContentRT = scrollContentGo.AddComponent<RectTransform>();
        scrollContentRT.anchorMin = new Vector2(0, 1);
        scrollContentRT.anchorMax = new Vector2(1, 1);
        scrollContentRT.pivot = new Vector2(0.5f, 1);
        scrollContentRT.sizeDelta = Vector2.zero;
        scrollContentRT.anchoredPosition = Vector2.zero;
        
        // Set up ScrollRect
        scrollRect.viewport = viewportRT;
        scrollRect.content = scrollContentRT;
        
        // Create scrollbar
        CreateScrollbar(contentGo, scrollRect);
        
        // Initially disable scrolling (will be enabled only for Hazard Config)
        scrollRect.enabled = false;
        
        // Store the scroll content for tab content to use
        ScrollContent = scrollContentRT;
    }

    private void ApplyResponsiveSizing()
    {
        // Size panel to 75% of screen and center
        float targetWidth = Mathf.Round(Screen.width * UIStyle.Layout.CanvasScaleFactor);
        float targetHeight = Mathf.Round(Screen.height * UIStyle.Layout.CanvasScaleFactor);
        if (Panel != null)
        {
            Panel.sizeDelta = new Vector2(targetWidth, targetHeight);
            Panel.anchoredPosition = Vector2.zero;
        }
        // Sidebar and MainContent positions are relative to Panel and do not need adjustment here
    }
    
    private void CreateScrollbar(GameObject parent, ScrollRect scrollRect)
    {
        // Create scrollbar
        GameObject scrollbarGo = new GameObject("Scrollbar");
        scrollbarGo.transform.SetParent(parent.transform, false);
        
        var scrollbarRT = scrollbarGo.AddComponent<RectTransform>();
        scrollbarRT.anchorMin = new Vector2(1, 0);
        scrollbarRT.anchorMax = new Vector2(1, 1);
        scrollbarRT.pivot = new Vector2(1, 0.5f);
        scrollbarRT.sizeDelta = new Vector2(20f, 0f);
        scrollbarRT.anchoredPosition = new Vector2(-10f, 0f);
        
        var scrollbar = scrollbarGo.AddComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        
        // Create scrollbar background
        GameObject backgroundGo = new GameObject("Background");
        backgroundGo.transform.SetParent(scrollbarGo.transform, false);
        
        var backgroundRT = backgroundGo.AddComponent<RectTransform>();
        backgroundRT.anchorMin = Vector2.zero;
        backgroundRT.anchorMax = Vector2.one;
        backgroundRT.sizeDelta = Vector2.zero;
        backgroundRT.anchoredPosition = Vector2.zero;
        
        var backgroundImage = backgroundGo.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Create scrollbar handle
        GameObject handleGo = new GameObject("Handle");
        handleGo.transform.SetParent(scrollbarGo.transform, false);
        
        var handleRT = handleGo.AddComponent<RectTransform>();
        handleRT.anchorMin = Vector2.zero;
        handleRT.anchorMax = Vector2.one;
        handleRT.sizeDelta = Vector2.zero;
        handleRT.anchoredPosition = Vector2.zero;
        
        var handleImage = handleGo.AddComponent<Image>();
        handleImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        
        // Set up scrollbar
        scrollbar.targetGraphic = handleImage;
        scrollbar.handleRect = handleRT;
        
        // Connect scrollbar to scroll rect
        scrollRect.verticalScrollbar = scrollbar;
    }

    private class ResizeWatcher : MonoBehaviour
    {
        public Action OnResolutionChanged = null!;
        private int _lastWidth;
        private int _lastHeight;

        private void Awake()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
        }

        private void Update()
        {
            if (_lastWidth != Screen.width || _lastHeight != Screen.height)
            {
                _lastWidth = Screen.width;
                _lastHeight = Screen.height;
                OnResolutionChanged?.Invoke();
            }
        }
    }
    
    private void CreateTabs()
    {
        // Create tabs using descriptors instead of hardcoded zones
        var zoneDescriptors = ZoneDescriptors.GetDisplayableZones().ToArray();

        for (int i = 0; i < zoneDescriptors.Length; i++)
        {
            var descriptor = zoneDescriptors[i];
            var tab = new MenuTab(descriptor, Sidebar, ScrollContent, -50f - (i * UIStyle.Layout.TabSpacing), UIStyle.Layout.TabHeight);
            tab.Button.onClick.AddListener(() => ShowTab(tab));
            _tabs.Add(tab);
        }

        // Create Hazard Config tab at the bottom
        CreateHazardConfigTab();

        // Show first tab by default
        if (_tabs.Count > 0)
        {
            ShowTab(_tabs[0]);
        }

        // Update initial sidebar counts after all tabs are created
        // Defer this to avoid unnecessary updates during initialization
        UpdateInitialSidebarCounts();
    }
    
    private void CreateHazardConfigTab()
    {
        // Create Hazard Config tab button at the bottom of sidebar
        var tabButtonGo = new GameObject("HazardConfigTab");
        tabButtonGo.transform.SetParent(Sidebar, false);
        
        var tabButtonRT = tabButtonGo.AddComponent<RectTransform>();
        tabButtonRT.anchorMin = new Vector2(0, 0);
        tabButtonRT.anchorMax = new Vector2(1, 0);
        tabButtonRT.pivot = new Vector2(0.5f, 0);
        tabButtonRT.sizeDelta = new Vector2(-20f, UIStyle.Layout.TabHeight);
        tabButtonRT.anchoredPosition = new Vector2(0, 10f);
        
        var tabButton = tabButtonGo.AddComponent<Button>();
        var tabButtonImage = tabButtonGo.AddComponent<Image>();
        tabButtonImage.color = UIStyle.Colors.ButtonDefault;
        tabButton.targetGraphic = tabButtonImage;
        
        // Create button text
        var textGo = new GameObject("Text");
        textGo.transform.SetParent(tabButtonGo.transform, false);
        
        var textRT = textGo.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;
        
        var buttonText = textGo.AddComponent<Text>();
        buttonText.font = UIStyle.DefaultFont;
        buttonText.color = UIStyle.Colors.TextDefault;
        buttonText.text = "Hazard Config";
        buttonText.fontSize = UIStyle.FontSizes.Default;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        // Create content area
        var contentGo = new GameObject("HazardConfigContent");
        contentGo.transform.SetParent(ScrollContent, false);
        
        var contentRT = contentGo.AddComponent<RectTransform>();
        contentRT.anchorMin = Vector2.zero;
        contentRT.anchorMax = Vector2.one;
        contentRT.pivot = new Vector2(0.5f, 0.5f);
        contentRT.sizeDelta = Vector2.zero;
        contentRT.anchoredPosition = Vector2.zero;
        
        // Add background image
        var contentImage = contentGo.AddComponent<Image>();
        contentImage.color = UIStyle.Colors.MainContentBackground;
        
        // Initially hide the content
        contentGo.SetActive(false);
        
        // Create Hazard Config tab content
        var hazardConfigTab = new HazardConfigTabContent(contentRT);
        
        // Store references
        _hazardConfigContent = contentGo;
        _hazardConfigButton = tabButton;
        
        // Set up click handler
        tabButton.onClick.AddListener(() => ShowHazardConfigTab());
    }
    
    private void ShowHazardConfigTab()
    {
        // Hide all other tab contents
        foreach (var tab in _tabs)
        {
            tab.SetActive(false);
        }
        
        // Ensure main content area remains visible (keep background and container active)
        MainContent.gameObject.SetActive(true);
        
        // Enable scrolling for Hazard Config
        EnableScrolling(true);
        
        // Show hazard config content
        _hazardConfigContent.SetActive(true);
        
        // Update scroll content size
        UpdateScrollContentSize();
        
        // Update button appearance
        var buttonImage = _hazardConfigButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = UIStyle.Colors.ButtonActive;
        }
    }
    
    private void UpdateScrollContentSize()
    {
        if (ScrollContent == null) return;
        
        // Calculate the total height needed for all content
        float totalHeight = 0f;
        
        // Add header height
        totalHeight += 50f;
        
        // Add content height based on active content
        if (_hazardConfigContent != null && _hazardConfigContent.activeSelf)
        {
            // Calculate height for hazard config content (two-column layout)
            var configurableTypes = SpawnTypeDescriptors.GetConfigurableTypes().ToList();
            totalHeight += 50f; // Header
            totalHeight += (configurableTypes.Count + 1) / 2 * 220f; // Sections in two columns (ceiling division)
            totalHeight += 50f; // Reset button
        }
        else if (_activeTab != null)
        {
            // For regular tabs, set content to fit viewport (no scrolling needed)
            totalHeight = 0f; // Let content size naturally
        }
        
        // Set the scroll content size
        ScrollContent.sizeDelta = new Vector2(0, totalHeight);
    }
    
    private void EnableScrolling(bool enable)
    {
        var scrollRect = MainContent.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.enabled = enable;
            
            if (enable)
            {
                // Reset scroll position to top when enabling
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }
        
        // Show/hide scrollbar based on scrolling state
        var scrollbar = MainContent.GetComponentInChildren<Scrollbar>();
        if (scrollbar != null)
        {
            scrollbar.gameObject.SetActive(enable);
        }
    }

    private void ShowTab(MenuTab tab)
    {
        // Hide all other tab contents
        foreach (var t in _tabs)
            t.SetActive(false);
        
        // Hide hazard config content
        if (_hazardConfigContent != null)
            _hazardConfigContent.SetActive(false);
        
        // Show main content
        MainContent.gameObject.SetActive(true);
        
        // Disable scrolling for regular tabs
        EnableScrolling(false);
        
        // Show the selected tab
        tab.SetActive(true);
        _activeTab = tab;
        
        // Update scroll content size
        UpdateScrollContentSize();
        
        // Reset hazard config button appearance
        if (_hazardConfigButton != null)
        {
            var buttonImage = _hazardConfigButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = UIStyle.Colors.ButtonDefault;
            }
        }
    }
    
    public void SetActive(bool active)
    {
        Canvas.gameObject.SetActive(active);
    }
    
    public void UpdateSidebarTabCount(string biomeName, int totalHazards)
    {
        // Find the tab for this biome and update its count
        foreach (var tab in _tabs)
        {
            if (tab.Name != biomeName)
                continue;
            
            tab.UpdateCount(totalHazards);
            break;
        }
    }
    
    private void UpdateInitialSidebarCounts()
    {
        // Update counts for all biomes after all tabs are created
        // This ensures the sidebar shows correct initial counts
        foreach (var tab in _tabs)
        {
            // Calculate total hazards for this biome
            int totalHazards = CalculateBiomeHazardCount(tab.Name);
            tab.UpdateCount(totalHazards);
        }
    }
    
    private static int CalculateBiomeHazardCount(string biomeName)
    {
        // Find zone by display name from descriptors
        var descriptor = ZoneDescriptors.GetDisplayableZones()
            .FirstOrDefault(d => d.DisplayName == biomeName);
        
        if (descriptor == null)
        {
            // Fallback to ZoneDescriptors for compatibility
            Zone zone = ZoneDescriptors.ParseZone(biomeName) ?? Zone.Unknown;
            return MenuSettings.GetHazardCountForZone(zone);
        }
        
        return MenuSettings.GetHazardCountForZone(descriptor.Id);
    }
}

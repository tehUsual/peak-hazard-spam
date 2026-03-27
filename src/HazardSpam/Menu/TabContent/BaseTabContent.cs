using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using HazardSpam.Types;
using HazardSpam.Menu.Settings;
using HazardSpam.Menu.Descriptors;
using NetGameState.Level;

namespace HazardSpam.Menu.TabContent;

public abstract class BaseTabContent
{
    protected RectTransform Parent { get; private set; }
    protected string ZoneName { get; private set; }
    protected SubZoneArea[] SubZoneAreas { get; private set; }

    private Dictionary<SubZoneArea, Button> _subZoneTabButtons = new(); // Store tab button references
    private Dictionary<SubZoneArea, List<GameObject>> _hazardRows = new(); // Store hazard row GameObjects for each sub-zone
    private Dictionary<SubZoneArea, GameObject> _addButtons = new(); // Store Add button GameObjects for each sub-zone
    private Dictionary<SubZoneArea, Text> _subZoneCountTexts = new(); // Store count text for horizontal tabs
    private Dictionary<SubZoneArea, List<Dropdown>> _typeDropdowns = new(); // Track dropdowns per sub-zone for filtering
    private Dictionary<GameObject, string> _hazardRowGuids = new(); // Map hazard row GameObjects to their GUIDs
    private Dictionary<SubZoneArea, bool> _refreshInProgress = new(); // Prevent race conditions in RefreshDropdowns
    
    protected BaseTabContent(RectTransform parent, string zoneName, SubZoneArea[] subZoneAreas)
    {
        Parent = parent;
        ZoneName = zoneName;
        SubZoneAreas = subZoneAreas;
        
        // Subscribe to MenuSettings events for automatic updates
        MenuSettings.Changed += OnSettingsChanged;
        MenuSettings.ZoneChanged += OnZoneChanged;
        
        CreateContent();
    }
    
    /// <summary>
    /// Handle general settings changes
    /// </summary>
    private void OnSettingsChanged()
    {
        // Refresh all counts when settings change
        RefreshAllCounts();
    }
    
    /// <summary>
    /// Handle zone-specific changes
    /// </summary>
    private void OnZoneChanged(Zone zone)
    {
        if (GetZoneFromName(ZoneName) == zone)
        {
            RefreshAllCounts();
        }
    }
    
    /// <summary>
    /// Refresh all count displays
    /// </summary>
    private void RefreshAllCounts()
    {
        foreach (var subZoneArea in SubZoneAreas)
        {
            UpdateSubZoneCount(subZoneArea);
        }
    }
    
    /// <summary>
    /// Update count display for a specific sub-zone using centralized calculation
    /// </summary>
    private void UpdateSubZoneCount(SubZoneArea subZoneArea)
    {
        Zone zone = GetZoneFromName(ZoneName);
        int count = MenuSettings.GetHazardDataCountForSubZone(zone, subZoneArea);
        
        if (_subZoneCountTexts.TryGetValue(subZoneArea, out var countText))
        {
            countText.text = count.ToString();
        }
    }
    
    protected abstract void CreateContent();
    
    /// <summary>
    /// Convert ZoneName string to Zone enum using ZoneDescriptors
    /// </summary>
    private Zone GetZoneFromName(string zoneName)
    {
        return ZoneDescriptors.ParseZone(zoneName) ?? Zone.Unknown;
    }
    
    
    protected void CreateHorizontalTabs()
    {
        // Create horizontal tab container
        GameObject tabContainerGO = new GameObject("SubZoneTabs");
        tabContainerGO.transform.SetParent(Parent, false);
        
        RectTransform tabContainerRT = tabContainerGO.AddComponent<RectTransform>();
        tabContainerRT.anchorMin = new Vector2(0, 1);
        tabContainerRT.anchorMax = new Vector2(1, 1);
        tabContainerRT.pivot = new Vector2(0.5f, 1);
        tabContainerRT.sizeDelta = new Vector2(-20f, 40f);
        tabContainerRT.anchoredPosition = new Vector2(0, -50f); // Move below header
        
        // Create horizontal layout group
        HorizontalLayoutGroup layoutGroup = tabContainerGO.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 10f;
        layoutGroup.padding = new RectOffset(10, 10, 5, 5);
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = true;
        
        // Create tabs for each SubZoneArea
        for (int i = 0; i < SubZoneAreas.Length; i++)
        {
            CreateSubZoneTab(SubZoneAreas[i], tabContainerGO.transform, i);
        }
        
        // Show the first tab after all tabs are created
        if (SubZoneAreas.Length > 0)
        {
            ShowSubZoneContent(SubZoneAreas[0]);
        }
        
        // Update initial counts
        UpdateHazardCounts();
    }
    
    private void CreateSubZoneTab(SubZoneArea subZoneArea, Transform parent, int index)
    {
        // Create tab button
        GameObject tabButtonGO = new GameObject($"SubZoneTab_{subZoneArea}");
        tabButtonGO.transform.SetParent(parent, false);
        
        RectTransform tabButtonRT = tabButtonGO.AddComponent<RectTransform>();
        tabButtonRT.sizeDelta = new Vector2(0, 30f); // Height will be controlled by layout group
        
        Button tabButton = tabButtonGO.AddComponent<Button>();
        Image tabButtonImage = tabButtonGO.AddComponent<Image>();
        tabButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        tabButton.targetGraphic = tabButtonImage;
        
        // Create tab text
        GameObject tabTextGo = new GameObject("Text");
        tabTextGo.transform.SetParent(tabButtonGO.transform, false);
        
        RectTransform tabTextRT = tabTextGo.AddComponent<RectTransform>();
        tabTextRT.anchorMin = Vector2.zero;
        tabTextRT.anchorMax = Vector2.one;
        tabTextRT.sizeDelta = Vector2.zero;
        tabTextRT.anchoredPosition = Vector2.zero;
        
        Text tabText = tabTextGo.AddComponent<Text>();
        tabText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        tabText.color = Color.white;
        tabText.text = subZoneArea.ToString();
        tabText.fontSize = 12;
        tabText.alignment = TextAnchor.MiddleLeft;
        
        // Create a count indicator
        GameObject countGo = new GameObject("CountIndicator");
        countGo.transform.SetParent(tabButtonGO.transform, false);
        
        RectTransform countRT = countGo.AddComponent<RectTransform>();
        countRT.anchorMin = new Vector2(1, 0.5f);
        countRT.anchorMax = new Vector2(1, 0.5f);
        countRT.pivot = new Vector2(1, 0.5f);
        countRT.sizeDelta = new Vector2(20f, 12f); // Half-height, centered
        countRT.anchoredPosition = new Vector2(-10f, 0f);
        
        Image countImage = countGo.AddComponent<Image>();
        countImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Dark background
        
        // Add black border
        Outline countOutline = countGo.AddComponent<Outline>();
        countOutline.effectColor = Color.black;
        countOutline.effectDistance = new Vector2(1f, 1f);
        
        GameObject countTextGo = new GameObject("CountText");
        countTextGo.transform.SetParent(countGo.transform, false);
        
        RectTransform countTextRT = countTextGo.AddComponent<RectTransform>();
        countTextRT.anchorMin = Vector2.zero;
        countTextRT.anchorMax = Vector2.one;
        countTextRT.sizeDelta = Vector2.zero;
        countTextRT.anchoredPosition = Vector2.zero;
        
        Text countText = countTextGo.AddComponent<Text>();
        countText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        countText.color = Color.white;
        countText.text = "0";
        countText.fontSize = 10;
        countText.alignment = TextAnchor.MiddleCenter;
        
        // Store count text reference
        _subZoneCountTexts[subZoneArea] = countText;
        
        // Create content area for this sub-zone
        GameObject contentAreaGo = new GameObject($"Content_{subZoneArea}");
        contentAreaGo.transform.SetParent(Parent, false);
        
        RectTransform contentAreaRT = contentAreaGo.AddComponent<RectTransform>();
        contentAreaRT.anchorMin = new Vector2(0, 0);
        contentAreaRT.anchorMax = new Vector2(1, 1);
        contentAreaRT.pivot = new Vector2(0.5f, 0.5f);
        contentAreaRT.sizeDelta = new Vector2(-20f, -100f); // Account for tab height + header
        contentAreaRT.anchoredPosition = new Vector2(0, -50f); // Offset for tabs and header
        
        // Create content for this subzone
        CreateSubZoneContent(subZoneArea, contentAreaRT);
        
        // Store button reference for highlighting
        _subZoneTabButtons[subZoneArea] = tabButton;
        
        // Set up tab button click
        tabButton.onClick.AddListener(() => ShowSubZoneContent(subZoneArea));
        
        // Initially hide all content areas except the first one
        if (index != 0)
        {
            // Hide all other content areas initially
            contentAreaGo.SetActive(false);
        }
    }
    
    private void CreateSubZoneContent(SubZoneArea subZoneArea, RectTransform contentParent)
    {
        // Initialize hazard rows list for this sub-zone
        _hazardRows[subZoneArea] = new List<GameObject>();
        
        // Load existing hazards from MenuSettings for this subzone
        LoadHazardsFromSettings(subZoneArea, contentParent);
        
        // Create the "Add" button for this subzone
        CreateAddButton(subZoneArea, contentParent);
    }
    
    private void LoadHazardsFromSettings(SubZoneArea subZoneArea, RectTransform contentParent)
    {
        Zone zone = GetZoneFromName(ZoneName);
        var hazards = MenuSettings.GetHazardsForSubZone(zone, subZoneArea);
        
        int hazardNumber = 1;
        foreach (var kvp in hazards)
        {
            HazardType hazardType = kvp.Key;
            int amount = kvp.Value;
            
            float hazardY = -10f - ((hazardNumber - 1) * 30f);
            CreateHazardRowFromSettings(hazardNumber, hazardY, contentParent, subZoneArea, hazardType, amount);
            hazardNumber++;
        }
        
        // CRITICAL FIX: Refresh dropdowns after loading all hazards to ensure proper filtering
        // This fixes the issue where dropdowns show all options instead of filtered ones on initial load
        RefreshDropdowns(zone, subZoneArea);
    }
    
    private void CreateHazardRowFromSettings(int hazardNumber, float yPosition, RectTransform parent, SubZoneArea subZoneArea, HazardType hazardType, int amount)
    {
        Zone zone = GetZoneFromName(ZoneName);
        
        // CRITICAL FIX: Find the existing hazard in MenuSettings instead of creating a new one
        // The hazard already exists from JSON loading, we just need to find its GUID
        string hazardId = MenuSettings.FindHazardId(zone, subZoneArea, hazardType);
        
        // Check if hazard was found
        if (string.IsNullOrEmpty(hazardId))
        {
            if (Plugin.DebugMenu)
                Debug.LogWarning($"Failed to find existing hazard for {zone}/{subZoneArea}/{hazardType}");
            return;
        }
        
        // Create a container for this hazard row
        GameObject hazardRowGo = new GameObject($"HazardRow_{hazardId}");
        hazardRowGo.transform.SetParent(parent, false);
        
        // CRITICAL FIX: Store the GUID mapping for the existing hazard
        _hazardRowGuids[hazardRowGo] = hazardId;
        
        RectTransform hazardRowRT = hazardRowGo.AddComponent<RectTransform>();
        hazardRowRT.anchorMin = new Vector2(0, 1);
        hazardRowRT.anchorMax = new Vector2(1, 1);
        hazardRowRT.pivot = new Vector2(0, 1);
        hazardRowRT.sizeDelta = new Vector2(0, 25f);
        hazardRowRT.anchoredPosition = new Vector2(0, yPosition);
        
        // Build filtered options: allowed for zone minus selections in this sub-zone (other rows)
        string[] allHazardTypes = BuildAvailableSpawnTypeNames(zone, subZoneArea, hazardRowGo);
        
        // Hazard number label
        UIBuilder.CreateLabel(hazardRowRT, $"Hazard #{hazardNumber}:", new Vector2(20f, 0f), new Vector2(100f, 25f), true);
        
        // Type label
        UIBuilder.CreateLabel(hazardRowRT, "Type:", new Vector2(130f, 0f), new Vector2(50f, 25f), false);
        
        // Type dropdown
        Dropdown typeDropdown = UIBuilder.CreateDropdown(hazardRowRT, allHazardTypes, new Vector2(190f, 0f), new Vector2(150f, 25f));
        // Set selection to the loaded spawn type
        int desiredIdx = System.Array.IndexOf(allHazardTypes, hazardType.ToString());
        typeDropdown.value = desiredIdx >= 0 ? desiredIdx : 0;
        
        // Amount label
        UIBuilder.CreateLabel(hazardRowRT, "Amount:", new Vector2(350f, 0f), new Vector2(60f, 25f), false);
        
        // Amount input
        InputField amountInput = UIBuilder.CreateNumericInput(hazardRowRT, new Vector2(420f, 0f), new Vector2(80f, 25f));
        amountInput.text = amount.ToString(); // Set to the loaded amount
        
        // Delete button (X)
        CreateDeleteButton(hazardRowGo, subZoneArea);
        
        // Set up a dropdown change handler
        typeDropdown.onValueChanged.AddListener(value =>
        {
            string selectedText = typeDropdown.options[value].text;
            HazardType newType = System.Enum.TryParse<HazardType>(selectedText, out var parsed) ? parsed : HazardType.Unknown;
            int currentAmount = int.TryParse(amountInput.text, out int amt) ? amt : 0;
            
            // CRITICAL FIX: Now we have a GUID for loaded hazards
            if (_hazardRowGuids.TryGetValue(hazardRowGo, out string hazardId))
            {
                // Update the hazard with new type and amount
                MenuSettings.UpdateHazard(hazardId, zone, subZoneArea, newType, currentAmount);
            }
            
            // Refresh other dropdowns in this sub-zone to enforce uniqueness
            RefreshDropdowns(zone, subZoneArea);
        });
        
        // Set up amount input change handler
        amountInput.onEndEdit.AddListener(value =>
        {
            if (int.TryParse(value, out int newAmount))
            {
                string selectedText = typeDropdown.options[typeDropdown.value].text;
                HazardType currentType = System.Enum.TryParse<HazardType>(selectedText, out var parsed) ? parsed : HazardType.Unknown;
                
                // CRITICAL FIX: Now we have a GUID for loaded hazards
                if (_hazardRowGuids.TryGetValue(hazardRowGo, out string hazardId))
                {
                    // Update the hazard with new amount
                    MenuSettings.UpdateHazard(hazardId, zone, subZoneArea, currentType, newAmount);
                }
            }
        });
        
        // Store the hazard row GameObject
        _hazardRows[subZoneArea].Add(hazardRowGo);
        if (!_typeDropdowns.ContainsKey(subZoneArea)) _typeDropdowns[subZoneArea] = new List<Dropdown>();
        _typeDropdowns[subZoneArea].Add(typeDropdown);
    }
    
    private void CreateEmptyHazardRow(int hazardNumber, float yPosition, RectTransform parent, SubZoneArea subZoneArea)
    {
        Zone zone = GetZoneFromName(ZoneName);
        
        // Create a new hazard with GUID (start with first available SpawnType)
        // First, get all allowed SpawnTypes for this zone/sub-zone
        string[] allowedTypes = GetAllowedSpawnTypeNames(zone, subZoneArea);
        
        // Then filter out already selected ones
        var selectedTypes = new HashSet<string>();
        if (_typeDropdowns.ContainsKey(subZoneArea))
        {
            foreach (var dd in _typeDropdowns[subZoneArea])
            {
                if (dd == null) continue;
                var idx = dd.value;
                if (idx >= 0 && idx < dd.options.Count)
                {
                    selectedTypes.Add(dd.options[idx].text);
                }
            }
        }
        
        // Find first available type
        string[] availableTypes = allowedTypes.Where(t => !selectedTypes.Contains(t)).ToArray();
        HazardType initialType = availableTypes.Length > 0 ? 
            System.Enum.TryParse<HazardType>(availableTypes[0], out var parsed) ? parsed : HazardType.Unknown : 
            HazardType.Unknown;
        
        string hazardId = MenuSettings.AddHazard(zone, subZoneArea, initialType, 0);
        
        // Check if hazard creation failed (empty string indicates failure)
        if (string.IsNullOrEmpty(hazardId))
        {
            if (Plugin.DebugMenu)
                Debug.LogWarning($"Failed to create hazard for {zone}/{subZoneArea} with type {initialType}");
            return;
        }
        
        // Create a container for this hazard row
        GameObject hazardRowGo = new GameObject($"HazardRow_{hazardId}");
        hazardRowGo.transform.SetParent(parent, false);
        
        // Store the GUID mapping
        _hazardRowGuids[hazardRowGo] = hazardId;
        
        RectTransform hazardRowRT = hazardRowGo.AddComponent<RectTransform>();
        hazardRowRT.anchorMin = new Vector2(0, 1);
        hazardRowRT.anchorMax = new Vector2(1, 1);
        hazardRowRT.pivot = new Vector2(0, 1);
        hazardRowRT.sizeDelta = new Vector2(0, 25f);
        hazardRowRT.anchoredPosition = new Vector2(0, yPosition);
        
        // Build filtered options for a new row
        string[] allHazardTypes = BuildAvailableSpawnTypeNames(zone, subZoneArea, hazardRowGo);
        
        // Hazard number label
        UIBuilder.CreateLabel(hazardRowRT, $"Hazard #{hazardNumber}:", new Vector2(20f, 0f), new Vector2(100f, 25f), true);
        
        // Type label
        UIBuilder.CreateLabel(hazardRowRT, "Type:", new Vector2(130f, 0f), new Vector2(50f, 25f), false);
        
        // Type dropdown
        Dropdown typeDropdown = UIBuilder.CreateDropdown(hazardRowRT, allHazardTypes, new Vector2(190f, 0f), new Vector2(150f, 25f));
        
        // Set the dropdown to the initial type we created the hazard with
        int initialIndex = System.Array.IndexOf(allHazardTypes, initialType.ToString());
        if (initialIndex >= 0)
        {
            typeDropdown.value = initialIndex;
        }
        
        // Amount label
        UIBuilder.CreateLabel(hazardRowRT, "Amount:", new Vector2(350f, 0f), new Vector2(60f, 25f), false);
        
        // Amount input
        InputField amountInput = UIBuilder.CreateNumericInput(hazardRowRT, new Vector2(420f, 0f), new Vector2(80f, 25f));
        // Leave at the default value (0)
        
        // Delete button (X)
        CreateDeleteButton(hazardRowGo, subZoneArea);
        
        // Set up event handlers for MenuSettings integration
        
        // Set up a dropdown change handler
        typeDropdown.onValueChanged.AddListener(value =>
        {
            string selectedText = typeDropdown.options[value].text;
            HazardType newType = System.Enum.TryParse<HazardType>(selectedText, out var parsed) ? parsed : HazardType.Unknown;
            int currentAmount = int.TryParse(amountInput.text, out int amt) ? amt : 0;
            
            // Update the hazard with new type and amount
            if (_hazardRowGuids.TryGetValue(hazardRowGo, out string hazardId))
            {
                MenuSettings.UpdateHazard(hazardId, zone, subZoneArea, newType, currentAmount);
            }
            
            // Update counts
            UpdateHazardCounts();
            // Refresh dropdowns to enforce uniqueness
            RefreshDropdowns(zone, subZoneArea);
        });
        
        // Set up amount input change handler
        amountInput.onEndEdit.AddListener(value =>
        {
            if (int.TryParse(value, out int newAmount))
            {
                string selectedText = typeDropdown.options[typeDropdown.value].text;
                HazardType currentType = System.Enum.TryParse<HazardType>(selectedText, out var parsed) ? parsed : HazardType.Unknown;
                
                // Update the hazard with new amount
                if (_hazardRowGuids.TryGetValue(hazardRowGo, out string hazardId))
                {
                    if (newAmount > 0)
                    {
                        MenuSettings.UpdateHazard(hazardId, zone, subZoneArea, currentType, newAmount);
                    }
                    else
                    {
                        MenuSettings.RemoveHazard(hazardId);
                    }
                }
                
                // Update counts
                UpdateHazardCounts();
            }
        });
        
        // Store the hazard row GameObject
        _hazardRows[subZoneArea].Add(hazardRowGo);
        if (!_typeDropdowns.ContainsKey(subZoneArea)) _typeDropdowns[subZoneArea] = new List<Dropdown>();
        _typeDropdowns[subZoneArea].Add(typeDropdown);
        
        // Update counts immediately after adding the row (in case it has initial values)
        UpdateHazardCounts();
        // Refresh all dropdowns after adding a row
        RefreshDropdowns(zone, subZoneArea);
    }
    
    private void ShowSubZoneContent(SubZoneArea selectedSubZone)
    {
        // Update tab button colors
        foreach (var kvp in _subZoneTabButtons)
        {
            SubZoneArea subZone = kvp.Key;
            Button button = kvp.Value;
            Image buttonImage = button.GetComponent<Image>();
            
            if (subZone == selectedSubZone)
            {
                // Highlight the active tab (same color as the sidebar active tab)
                buttonImage.color = UIStyle.Colors.ButtonTabActive;
            }
            else
            {
                // Default tab color
                buttonImage.color = UIStyle.Colors.ButtonTabDefault;
            }
        }
        
        // Hide all content areas
        for (int i = 0; i < Parent.childCount; i++)
        {
            Transform child = Parent.GetChild(i);
            if (child.name.StartsWith("Content_"))
            {
                child.gameObject.SetActive(false);
            }
        }
        
        // Show the selected content area
        Transform selectedContent = Parent.Find($"Content_{selectedSubZone}");
        if (selectedContent != null)
        {
            selectedContent.gameObject.SetActive(true);
        }
    }
    
    
    private void CreateAddButton(SubZoneArea subZoneArea, RectTransform contentParent)
    {
        // Create the "Add" button
        GameObject addButtonGo = new GameObject($"AddButton_{subZoneArea}");
        addButtonGo.transform.SetParent(contentParent, false);
        
        RectTransform addButtonRT = addButtonGo.AddComponent<RectTransform>();
        addButtonRT.anchorMin = new Vector2(0, 1);
        addButtonRT.anchorMax = new Vector2(0, 1);
        addButtonRT.pivot = new Vector2(0, 1);
        addButtonRT.sizeDelta = new Vector2(120f, 25f); // Fixed width, left-aligned
        addButtonRT.anchoredPosition = new Vector2(20f, -10f - (_hazardRows[subZoneArea].Count * 30f)); // Position below last hazard
        
        Button addButton = addButtonGo.AddComponent<Button>();
        Image addButtonImage = addButtonGo.AddComponent<Image>();
        addButtonImage.color = UIStyle.Colors.ButtonAdd;
        addButton.targetGraphic = addButtonImage;
        
        // Add button text
        GameObject addTextGo = new GameObject("Text");
        addTextGo.transform.SetParent(addButtonGo.transform, false);
        
        RectTransform addTextRT = addTextGo.AddComponent<RectTransform>();
        addTextRT.anchorMin = Vector2.zero;
        addTextRT.anchorMax = Vector2.one;
        addTextRT.sizeDelta = Vector2.zero;
        addTextRT.anchoredPosition = Vector2.zero;
        
        Text addText = addTextGo.AddComponent<Text>();
        addText.font = UIStyle.DefaultFont;
        addText.color = UIStyle.Colors.TextDefault;
        addText.text = "+ Add Hazard";
        addText.fontSize = UIStyle.FontSizes.Default;
        addText.alignment = TextAnchor.MiddleCenter;
        
        // Set up click listener
        addButton.onClick.AddListener(() => AddHazard(subZoneArea, contentParent));
        
        // Store the Add button reference
        _addButtons[subZoneArea] = addButtonGo;
    }
    
    private void CreateDeleteButton(GameObject hazardRowGo, SubZoneArea subZoneArea)
    {
        // Create Delete button (X)
        GameObject deleteButtonGo = new GameObject("DeleteButton");
        deleteButtonGo.transform.SetParent(hazardRowGo.transform, false);
        
        RectTransform deleteButtonRT = deleteButtonGo.AddComponent<RectTransform>();
        deleteButtonRT.anchorMin = new Vector2(1, 0.5f);
        deleteButtonRT.anchorMax = new Vector2(1, 0.5f);
        deleteButtonRT.pivot = new Vector2(1, 0.5f);
        deleteButtonRT.sizeDelta = new Vector2(20f, 18f); // Slightly taller for better visibility
        deleteButtonRT.anchoredPosition = new Vector2(-10f, 0f);
        
        Button deleteButton = deleteButtonGo.AddComponent<Button>();
        Image deleteButtonImage = deleteButtonGo.AddComponent<Image>();
        deleteButtonImage.color = UIStyle.Colors.ButtonDelete;
        deleteButton.targetGraphic = deleteButtonImage;
        
        // Delete button text
        GameObject deleteTextGo = new GameObject("Text");
        deleteTextGo.transform.SetParent(deleteButtonGo.transform, false);
        
        RectTransform deleteTextRT = deleteTextGo.AddComponent<RectTransform>();
        deleteTextRT.anchorMin = Vector2.zero;
        deleteTextRT.anchorMax = Vector2.one;
        deleteTextRT.sizeDelta = Vector2.zero;
        deleteTextRT.anchoredPosition = Vector2.zero;
        
        Text deleteText = deleteTextGo.AddComponent<Text>();
        deleteText.font = UIStyle.DefaultFont;
        deleteText.color = UIStyle.Colors.TextDefault;
        deleteText.text = "X";
        deleteText.fontSize = UIStyle.FontSizes.Default; // Smaller font to fit better
        deleteText.alignment = TextAnchor.MiddleCenter;
        
        // Set up click listener
        deleteButton.onClick.AddListener(() => RemoveHazard(hazardRowGo, subZoneArea));
    }
    
    private void AddHazard(SubZoneArea subZoneArea, RectTransform contentParent)
    {
        if (Plugin.DebugMenu)
            Debug.Log($"AddHazard: Adding hazard to {subZoneArea}");
        int newHazardNumber = _hazardRows[subZoneArea].Count + 1;
        float newHazardY = -10f - ((newHazardNumber - 1) * 30f);
        
        // Create a new empty hazard row (no pre-configured values)
        CreateEmptyHazardRow(newHazardNumber, newHazardY, contentParent, subZoneArea);
        
        // Move the "Add" button down
        UpdateAddButtonPosition(subZoneArea);
        
        // Update count indicators
        if (Plugin.DebugMenu)
            Debug.Log("AddHazard: Calling UpdateHazardCounts");
        UpdateHazardCounts();
    }
    
    private void RemoveHazard(GameObject hazardRowGo, SubZoneArea subZoneArea)
    {
        if (Plugin.DebugMenu)
            Debug.Log($"RemoveHazard: Removing hazard from {subZoneArea}");
        
        // Get the GUID for this hazard row and remove from MenuSettings
        if (_hazardRowGuids.TryGetValue(hazardRowGo, out string hazardId))
        {
            if (Plugin.DebugMenu)
                Debug.Log($"RemoveHazard: Removing hazard with ID {hazardId} from MenuSettings");
            MenuSettings.RemoveHazard(hazardId);
            _hazardRowGuids.Remove(hazardRowGo);
        }
        
        // Remove from the list
        _hazardRows[subZoneArea].Remove(hazardRowGo);
        
        // CRITICAL FIX: Clean up the dropdown reference from _typeDropdowns more robustly
        if (_typeDropdowns.ContainsKey(subZoneArea))
        {
            // Find and remove the dropdown that belongs to this hazard row
            var dropdownToRemove = _typeDropdowns[subZoneArea].FirstOrDefault(dd => 
                dd != null && dd.transform.IsChildOf(hazardRowGo.transform));
            if (dropdownToRemove != null)
            {
                _typeDropdowns[subZoneArea].Remove(dropdownToRemove);
            }
            // Also clean up any null dropdowns
            _typeDropdowns[subZoneArea].RemoveAll(dd => dd == null);
        }
        
        // Destroy the GameObject
        Object.Destroy(hazardRowGo);
        
        // Renumber remaining hazards and reposition them
        ReorderHazards(subZoneArea);
        
        // Move the "Add" button up
        UpdateAddButtonPosition(subZoneArea);
        
        // Update count indicators
        if (Plugin.DebugMenu)
            UnityEngine.Debug.Log("RemoveHazard: Calling UpdateHazardCounts");
        UpdateHazardCounts();
        // Refresh dropdowns to make removed type available again
        Zone zone = GetZoneFromName(ZoneName);
        RefreshDropdowns(zone, subZoneArea);
    }
    
    private void ReorderHazards(SubZoneArea subZoneArea)
    {
        for (int i = 0; i < _hazardRows[subZoneArea].Count; i++)
        {
            GameObject hazardRow = _hazardRows[subZoneArea][i];
            RectTransform hazardRowRT = hazardRow.GetComponent<RectTransform>();
            
            // Update hazard number
            int newHazardNumber = i + 1;
            float newY = -10f - (i * 30f);
            
            // Update position
            hazardRowRT.anchoredPosition = new Vector2(0, newY);
            
            // Update hazard number label (find the first Text component in the row)
            Text[] texts = hazardRow.GetComponentsInChildren<Text>();
            if (texts.Length > 0 && texts[0].text.StartsWith("Hazard #"))
            {
                texts[0].text = $"Hazard #{newHazardNumber}:";
            }
        }
    }
    
    private void UpdateAddButtonPosition(SubZoneArea subZoneArea)
    {
        if (_addButtons.ContainsKey(subZoneArea))
        {
            GameObject addButton = _addButtons[subZoneArea];
            RectTransform addButtonRT = addButton.GetComponent<RectTransform>();
            
            // Position the "Add" button below the last hazard
            float newY = -10f - (_hazardRows[subZoneArea].Count * 30f);
            addButtonRT.anchoredPosition = new Vector2(20f, newY); // Keep left-aligned
        }
    }

    // Build available spawn type option names for a sub-zone, excluding selections in other rows
    private string[] BuildAvailableSpawnTypeNames(Zone zone, SubZoneArea subZoneArea, GameObject? excludeRow)
    {
        var allowed = GetAllowedSpawnTypeNames(zone, subZoneArea);
        if (!_typeDropdowns.ContainsKey(subZoneArea)) return allowed;
        
        // CRITICAL FIX: Clean up null dropdowns first
        _typeDropdowns[subZoneArea].RemoveAll(dd => dd == null);
        
        var selected = new HashSet<string>();
        foreach (var dd in _typeDropdowns[subZoneArea])
        {
            if (dd == null) continue;
            
            // CRITICAL FIX: Skip the dropdown belonging to the row being built/updated
            if (excludeRow != null && dd.transform.IsChildOf(excludeRow.transform)) continue;
            
            // CRITICAL FIX: Get current selection safely with additional validation
            if (dd.value >= 0 && dd.value < dd.options.Count && dd.options[dd.value] != null)
            {
                string selectedText = dd.options[dd.value].text;
                if (!string.IsNullOrEmpty(selectedText))
                {
                    selected.Add(selectedText);
                }
            }
        }
        
        // CRITICAL FIX: Ensure we always return at least one option if possible
        var available = allowed.Where(n => !selected.Contains(n)).ToArray();
        return available.Length > 0 ? available : allowed;
    }
    
    /// <summary>
    /// Get allowed spawn type names for a zone/sub-zone combination
    /// </summary>
    protected virtual string[] GetAllowedSpawnTypeNames(Zone zone, SubZoneArea subZoneArea)
    {
        return ZoneDescriptors.GetAllowedSpawnTypeNames(zone);
    }

    // Refresh all dropdowns in a sub-zone to enforce uniqueness
    private void RefreshDropdowns(Zone zone, SubZoneArea subZoneArea)
    {
        if (!_typeDropdowns.ContainsKey(subZoneArea)) return;
        
        // CRITICAL FIX: Prevent race conditions
        if (_refreshInProgress.ContainsKey(subZoneArea) && _refreshInProgress[subZoneArea])
        {
            return; // Already refreshing this sub-zone
        }
        
        _refreshInProgress[subZoneArea] = true;
        
        try
        {
            // Clean up null dropdowns first
            _typeDropdowns[subZoneArea].RemoveAll(dd => dd == null);
            
            foreach (var dd in _typeDropdowns[subZoneArea])
            {
                if (dd == null) continue;
                
                // Preserve current selection if still available
                string? current = null;
                if (dd.value >= 0 && dd.value < dd.options.Count)
                {
                    current = dd.options[dd.value].text;
                }
                
                // Get available options for this dropdown
                var rowGo = dd.transform.parent?.gameObject;
                var options = BuildAvailableSpawnTypeNames(zone, subZoneArea, rowGo);
                
                // Update dropdown options
                dd.options.Clear();
                foreach (var opt in options) 
                {
                    dd.options.Add(new Dropdown.OptionData(opt));
                }
                
                // Set selection to current value if still available, otherwise first option
                int newIdx = 0;
                if (!string.IsNullOrEmpty(current))
                {
                    int idx = System.Array.IndexOf(options, current);
                    newIdx = idx >= 0 ? idx : 0;
                }
                
                dd.value = newIdx;
                dd.RefreshShownValue();
            }
        }
        finally
        {
            // CRITICAL FIX: Always reset the flag
            _refreshInProgress[subZoneArea] = false;
        }
    }
    
    
    private void UpdateHazardCounts()
    {
        Zone zone = GetZoneFromName(ZoneName);
        if (Plugin.DebugMenu)
            UnityEngine.Debug.Log($"UpdateHazardCounts: Zone={zone}, ZoneName={ZoneName}");
        
        // Update horizontal tab counts
        foreach (var kvp in _subZoneCountTexts)
        {
            SubZoneArea subZone = kvp.Key;
            Text countText = kvp.Value;
            
            // Count actual UI hazard rows (not just MenuSettings entries)
            int hazardCount = _hazardRows.ContainsKey(subZone) ? _hazardRows[subZone].Count : 0;
            if (Plugin.DebugMenu)
                UnityEngine.Debug.Log($"UpdateHazardCounts: SubZone={subZone}, Count={hazardCount}");
            countText.text = hazardCount.ToString();
        }
        
        // Update sidebar tab count (sum of all hazards in this biome)
        UpdateSidebarTabCount();
    }
    
    private void UpdateSidebarTabCount()
    {
        // Calculate total hazards for this biome using UI row counts
        Zone zone = GetZoneFromName(ZoneName);
        int totalHazards = 0;
        foreach (var kvp in _hazardRows)
        {
            totalHazards += kvp.Value.Count;
        }
        if (Plugin.DebugMenu)
            Debug.Log($"UpdateSidebarTabCount: Zone={zone}, TotalHazards={totalHazards}, ZoneName={ZoneName}");
        
        // Update the sidebar tab count via Plugin
        if (Plugin.Instance != null)
        {
            if (Plugin.DebugMenu)
                Debug.Log($"UpdateSidebarTabCount: Calling Plugin.Instance.UpdateSidebarTabCount");
            Plugin.Instance.UpdateSidebarTabCount(ZoneName, totalHazards);
        }
        else
        {
            if (Plugin.DebugMenu)
                Debug.Log("UpdateSidebarTabCount: Plugin.Instance is null!");
        }
    }
    
}

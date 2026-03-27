using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using BepInEx;
using HazardSpam.Types;
using NetGameState.Level;
using Sirenix.OdinInspector;

namespace HazardSpam.Menu.Settings;



/// <summary>
/// Centralized settings management for the hazard menu
/// </summary>
public static class MenuSettings
{
    // GUID-based storage for hazard configurations
    private static readonly Dictionary<string, HazardData> _hazardData = new();
    
    // Events for UI updates
    public static event Action? Changed;
    public static event Action<Zone>? ZoneChanged;
    
    /// <summary>
    /// Get the total number of configured hazards across all zones
    /// </summary>
    /// <returns>Total number of hazard configurations</returns>
    public static int GetTotalHazardCount()
    {
        return _hazardData.Count;
    }
    
    /// <summary>
    /// Find the GUID of an existing hazard by its zone, sub-zone, and spawn type
    /// </summary>
    /// <param name="zone">The zone</param>
    /// <param name="subZoneArea">The sub-zone area</param>
    /// <param name="hazardType">The spawn type</param>
    /// <returns>The hazard GUID if found, empty string if not found</returns>
    public static string FindHazardId(Zone zone, SubZoneArea subZoneArea, HazardType hazardType)
    {
        foreach (var kvp in _hazardData)
        {
            var hazard = kvp.Value;
            if (hazard.Zone == zone && hazard.SubZoneArea == subZoneArea && hazard.hazardType == hazardType)
            {
                return kvp.Key;
            }
        }
        return string.Empty;
    }
    
    /// <summary>
    /// Get all configured hazards for a specific zone (legacy compatibility)
    /// </summary>
    /// <param name="zone">The biome zone</param>
    /// <returns>Dictionary of (SubZoneArea, SpawnType) to HazardSettings for this zone</returns>
    public static Dictionary<(SubZoneArea, HazardType), HazardSettings> GetHazardsForZone(Zone zone)
    {
        var result = new Dictionary<(SubZoneArea, HazardType), HazardSettings>();
        
        foreach (var hazard in _hazardData.Values)
        {
            if (hazard.Zone == zone)
            {
                var key = (hazard.SubZoneArea, SpawnType: hazard.hazardType);
                result[key] = new HazardSettings(hazard.Amount);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Get all configured hazards for a specific sub-zone (legacy compatibility)
    /// </summary>
    /// <param name="zone">The biome zone</param>
    /// <param name="subZoneArea">The sub-zone area within the biome</param>
    /// <returns>Dictionary of SpawnType to amount for this sub-zone</returns>
    public static Dictionary<HazardType, int> GetHazardsForSubZone(Zone zone, SubZoneArea subZoneArea)
    {
        var result = new Dictionary<HazardType, int>();
        
        foreach (var hazard in _hazardData.Values)
        {
            if (hazard.Zone == zone && hazard.SubZoneArea == subZoneArea)
            {
                if (result.ContainsKey(hazard.hazardType))
                {
                    result[hazard.hazardType] += hazard.Amount;
                }
                else
                {
                    result[hazard.hazardType] = hazard.Amount;
                }
            }
        }
        
        return result;
    }

    /// <summary>
    /// Retrieves a list of all hazards configured across all zones.
    /// </summary>
    /// <returns>A list containing all configured hazards.</returns>
    public static List<HazardData> GetAllHazards()
    {
        List<HazardData> result = new List<HazardData>();
        foreach (var hazard in _hazardData.Values)
        {
            result.Add(hazard);
        }
        return result;
    }

    /// <summary>
    /// Get the count of configured hazards for a specific zone (legacy compatibility)
    /// </summary>
    /// <param name="zone">The biome zone</param>
    /// <returns>Total number of different hazard types configured for this zone</returns>
    public static int GetHazardCountForZone(Zone zone)
    {
        return _hazardData.Values.Count(h => h.Zone == zone);
    }
    
    // ===== NEW GUID-BASED API =====
    
    /// <summary>
    /// Add a new hazard with stable GUID identity
    /// </summary>
    public static string AddHazard(Zone zone, SubZoneArea subZoneArea, HazardType hazardType, int amount)
    {
        // Validate enum values
        if (zone == Zone.Unknown || zone == Zone.Any ||
            subZoneArea == SubZoneArea.Unknown ||
            hazardType == HazardType.Unknown)
        {
            Plugin.Log.LogWarning($"Cannot add hazard with invalid enum values: {zone}/{subZoneArea}/{hazardType}");
            return string.Empty; // Return empty string to indicate failure
        }
        
        // Check for duplicates (same Zone/SubZoneArea/SpawnType combination)
        if (_hazardData.Values.Any(h => h.Zone == zone && h.SubZoneArea == subZoneArea && h.hazardType == hazardType))
        {
            Plugin.Log.LogWarning($"Cannot add duplicate hazard: {zone}/{subZoneArea}/{hazardType}");
            return string.Empty; // Return empty string to indicate failure
        }
        
        var hazard = new HazardData(zone, subZoneArea, hazardType, amount);
        _hazardData[hazard.Id] = hazard;
        RaiseChanged(zone);
        return hazard.Id;
    }
    
    /// <summary>
    /// Update an existing hazard by GUID
    /// </summary>
    public static bool UpdateHazard(string id, Zone zone, SubZoneArea subZoneArea, HazardType hazardType, int amount)
    {
        if (_hazardData.TryGetValue(id, out var hazard))
        {
            // Validate enum values
            if (zone == Zone.Unknown || zone == Zone.Any ||
                subZoneArea == SubZoneArea.Unknown ||
                hazardType == HazardType.Unknown)
            {
                Plugin.Log.LogWarning($"Cannot update hazard with invalid enum values: {zone}/{subZoneArea}/{hazardType}");
                return false;
            }
            
            // Check for duplicates (same Zone/SubZoneArea/SpawnType combination, excluding current hazard)
            if (_hazardData.Values.Any(h => h.Id != id && h.Zone == zone && h.SubZoneArea == subZoneArea && h.hazardType == hazardType))
            {
                Plugin.Log.LogWarning($"Cannot update to duplicate hazard: {zone}/{subZoneArea}/{hazardType}");
                return false;
            }
            
            var oldZone = hazard.Zone;
            hazard.Zone = zone;
            hazard.SubZoneArea = subZoneArea;
            hazard.hazardType = hazardType;
            hazard.Amount = amount;
            _hazardData[id] = hazard;
            RaiseChanged(oldZone);
            RaiseChanged(zone);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Remove a hazard by GUID
    /// </summary>
    public static bool RemoveHazard(string id)
    {
        if (_hazardData.TryGetValue(id, out var hazard))
        {
            var zone = hazard.Zone;
            _hazardData.Remove(id);
            RaiseChanged(zone);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Get all hazards for a specific zone (GUID-based)
    /// </summary>
    public static HazardData[] GetHazardDataForZone(Zone zone)
    {
        return _hazardData.Values.Where(h => h.Zone == zone).ToArray();
    }
    
    /// <summary>
    /// Get all hazards for a specific zone and sub-zone (GUID-based)
    /// </summary>
    public static HazardData[] GetHazardDataForSubZone(Zone zone, SubZoneArea subZoneArea)
    {
        return _hazardData.Values.Where(h => h.Zone == zone && h.SubZoneArea == subZoneArea).ToArray();
    }
    
    /// <summary>
    /// Get hazard count for a specific zone (GUID-based)
    /// </summary>
    public static int GetHazardDataCountForZone(Zone zone)
    {
        return _hazardData.Values.Count(h => h.Zone == zone);
    }
    
    /// <summary>
    /// Get hazard count for a specific zone and sub-zone (GUID-based)
    /// </summary>
    public static int GetHazardDataCountForSubZone(Zone zone, SubZoneArea subZoneArea)
    {
        return _hazardData.Values.Count(h => h.Zone == zone && h.SubZoneArea == subZoneArea);
    }
    
    /// <summary>
    /// Get all hazards (GUID-based)
    /// </summary>
    public static HazardData[] GetAllHazardData()
    {
        return _hazardData.Values.ToArray();
    }
    
    /// <summary>
    /// Clear all hazards
    /// </summary>
    public static void ClearAllHazards()
    {
        _hazardData.Clear();
        RaiseChanged();
    }
    
    /// <summary>
    /// Raise change events
    /// </summary>
    private static void RaiseChanged(Zone? zone = null)
    {
        Changed?.Invoke();
        if (zone.HasValue)
        {
            ZoneChanged?.Invoke(zone.Value);
        }
    }
    
    
    /// <summary>
    /// Get the path to the settings file
    /// </summary>
    private static string GetSettingsFilePath()
    {
        // Use BepInEx plugins config directory
        string configPath = Path.Combine(Paths.ConfigPath, "ModMenuTest");
        return Path.Combine(configPath, "Settings.xml");
    }
    
    /// <summary>
    /// Get the settings file path for debugging
    /// </summary>
    public static string GetSettingsFilePathForDebug()
    {
        return GetSettingsFilePath();
    }
    
    /// <summary>
    /// Load settings from XML file
    /// </summary>
    public static void LoadSettings()
    {
        try
        {
            string filePath = GetSettingsFilePath();
            if (!File.Exists(filePath))
            {
                Plugin.Log.LogInfo("Settings file not found, using default settings");
                InitializeWithExampleData();
                return;
            }
            
            string xml = File.ReadAllText(filePath);
            
            // Debug: Log the loaded XML to help diagnose parsing issues
            if (Plugin.DebugMenu)
            {
                Plugin.Log.LogInfo($"Loaded XML length: {xml.Length} characters");
                Plugin.Log.LogInfo($"Loaded XML preview: {xml.Substring(0, Math.Min(200, xml.Length))}...");
            }
            
            // Check if XML is empty or just whitespace
            if (string.IsNullOrWhiteSpace(xml) || xml.Trim() == "")
            {
                Plugin.Log.LogWarning("Settings file is empty, using default settings");
                InitializeWithExampleData();
                return;
            }
            
            // Parse XML using XmlSerializer
            var serializer = new XmlSerializer(typeof(XmlMenuSettingsData));
            XmlMenuSettingsData settingsData;
            
            using (var stringReader = new StringReader(xml))
            {
                settingsData = (XmlMenuSettingsData)serializer.Deserialize(stringReader);
            }
            
            if (settingsData == null)
            {
                Plugin.Log.LogWarning("Failed to deserialize XML data, using default settings");
                InitializeWithExampleData();
                return;
            }
            
            // Clear existing settings
            _hazardData.Clear();
            
            // Load SpawnRates
            if (settingsData.SpawnRates != null && settingsData.SpawnRates.Length > 0)
            {
                int validEntries = 0;
                var seenCombinations = new HashSet<(Zone, SubZoneArea, HazardType)>();
                var tempHazardData = new List<HazardData>();
                
                foreach (var entry in settingsData.SpawnRates)
                {
                    if (Enum.TryParse<Zone>(entry.Zone, out var zone) &&
                        Enum.TryParse<SubZoneArea>(entry.SubZoneArea, out var subZoneArea) &&
                        Enum.TryParse<HazardType>(entry.SpawnType, out var spawnType))
                    {
                        // Skip Unknown, Any, or invalid enum values
                        if (zone == Zone.Unknown || zone == Zone.Any ||
                            subZoneArea == SubZoneArea.Unknown ||
                            spawnType == HazardType.Unknown)
                        {
                            if (Plugin.DebugMenu)
                                Plugin.Log.LogInfo($"Skipping invalid enum entry: {entry.Zone}/{entry.SubZoneArea}/{entry.SpawnType}");
                            continue;
                        }
                        
                        // Check for duplicates (same Zone/SubZoneArea/SpawnType combination)
                        var combination = (zone, subZoneArea, spawnType);
                        if (seenCombinations.Contains(combination))
                        {
                            Plugin.Log.LogWarning($"Skipping duplicate entry: {entry.Zone}/{entry.SubZoneArea}/{entry.SpawnType} (keeping first occurrence)");
                            continue;
                        }
                        
                        seenCombinations.Add(combination);
                        var hazard = new HazardData(zone, subZoneArea, spawnType, entry.Amount);
                        tempHazardData.Add(hazard);
                        validEntries++;
                    }
                    else
                    {
                        Plugin.Log.LogWarning($"Invalid settings entry: {entry.Zone}/{entry.SubZoneArea}/{entry.SpawnType}");
                    }
                }
                
                // Check if we actually loaded any valid entries
                if (validEntries == 0)
                {
                    Plugin.Log.LogWarning("No valid settings entries found in file, using default settings");
                    InitializeWithExampleData();
                    return;
                }
                
                // Batch update to GUID-based system (no UI updates during this process)
                foreach (var hazard in tempHazardData)
                {
                    _hazardData[hazard.Id] = hazard;
                }
                
                Plugin.Log.LogInfo($"Successfully loaded {validEntries} hazard configurations from settings file");
            }
            else
            {
                Plugin.Log.LogWarning("No SpawnRates section found in XML, using default settings");
                InitializeWithExampleData();
                return;
            }
            
            // Load HazardConfigs if present
            if (settingsData.HazardConfigs != null && settingsData.HazardConfigs.Length > 0)
            {
                if (Plugin.DebugMenu)
                    Plugin.Log.LogInfo($"Loading {settingsData.HazardConfigs.Length} hazard config types");
                
                // Convert XmlSpawnTypeConfig to SerializableSpawnTypeConfig and load into HazardConfigSettings
                var configs = new List<SerializableSpawnTypeConfig>();
                foreach (var xmlConfig in settingsData.HazardConfigs)
                {
                    var fieldValues = new List<FieldValue>();
                    foreach (var xmlValue in xmlConfig.Values)
                    {
                        fieldValues.Add(new FieldValue
                        {
                            Name = xmlValue.Name,
                            Value = xmlValue.Value,
                            Type = xmlValue.Type
                        });
                    }

                    configs.Add(new SerializableSpawnTypeConfig
                    {
                        Type = xmlConfig.Type,
                        Values = fieldValues.ToArray()
                    });
                }
                
                // Load the configs into HazardConfigSettings
                HazardConfigSettings.LoadFromSerializableConfigs(configs.ToArray());
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to load settings: {ex.Message}");
            Plugin.Log.LogError($"Stack trace: {ex.StackTrace}");
            Plugin.Log.LogInfo("Using default settings");
            InitializeWithExampleData();
        }
    }
    
    
    /// <summary>
    /// Save current settings to XML file
    /// </summary>
    public static void SaveSettings()
    {
        try
        {
            if (Plugin.DebugMenu)
                Plugin.Log.LogInfo($"SaveSettings: Starting save with {_hazardData.Count} hazards and {HazardConfigSettings.Configs.Count} configs");
            
            // Build XML data structures
            var seenCombinations = new HashSet<(Zone, SubZoneArea, HazardType)>();
            var spawnRates = new List<XmlHazardEntry>();
            int hazardIndex = 1;
            
            // Convert hazards to XML entries
            foreach (var hazard in _hazardData.Values)
            {
                var combination = (hazard.Zone, hazard.SubZoneArea, SpawnType: hazard.hazardType);
                if (seenCombinations.Contains(combination))
                {
                    Plugin.Log.LogWarning($"Skipping duplicate hazard during save: {hazard.Zone}/{hazard.SubZoneArea}/{hazard.hazardType}");
                    continue;
                }
                
                seenCombinations.Add(combination);
                spawnRates.Add(new XmlHazardEntry(hazard, hazardIndex));
                hazardIndex++;
            }
            
            // Convert hazard configs to XML entries
            var hazardConfigs = new List<XmlSpawnTypeConfig>();
            foreach (var kvp in HazardConfigSettings.Configs)
            {
                hazardConfigs.Add(new XmlSpawnTypeConfig(kvp.Key, kvp.Value));
            }
            
            // Create XML data container
            var settingsData = new XmlMenuSettingsData
            {
                SpawnRates = spawnRates.ToArray(),
                HazardConfigs = hazardConfigs.ToArray()
            };
            
            // Serialize to XML
            var serializer = new XmlSerializer(typeof(XmlMenuSettingsData));
            var stringWriter = new StringWriter();
            
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = false
            }))
            {
                serializer.Serialize(xmlWriter, settingsData);
            }
            
            string xml = stringWriter.ToString();
            
            // Debug: Log the generated XML
            if (Plugin.DebugMenu)
            {
                Plugin.Log.LogInfo($"Generated XML length: {xml.Length} characters");
                Plugin.Log.LogInfo($"Generated XML preview: {xml.Substring(0, Math.Min(200, xml.Length))}...");
            }
            
            string filePath = GetSettingsFilePath();
            
            // Ensure directory exists
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(filePath, xml);
            Plugin.Log.LogInfo($"Successfully saved {spawnRates.Count} hazard configurations and {hazardConfigs.Count} config types to settings file");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to save settings: {ex.Message}");
            Plugin.Log.LogError($"Stack trace: {ex.StackTrace}");
        }
    }
    
    
    
    /// <summary>
    /// Test XML functionality to ensure it works correctly
    /// </summary>
    public static void TestXmlFunctionality()
    {
        if (!Plugin.DebugMenu) return;
        
        try
        {
            Plugin.Log.LogInfo("Testing XML functionality...");
            
            // Store current data count for verification
            int originalHazardCount = _hazardData.Count;
            int originalConfigCount = HazardConfigSettings.Configs.Count;
            
            Plugin.Log.LogInfo($"Testing with {originalHazardCount} hazards and {originalConfigCount} configs");
            
            // Test saving with current data
            SaveSettings();
            Plugin.Log.LogInfo("XML save test completed");
            
            // Test loading
            _hazardData.Clear();
            LoadSettings();
            Plugin.Log.LogInfo($"XML load test completed - loaded {_hazardData.Count} hazards");
            
            // Verify data integrity - check if we loaded the expected amount
            if (_hazardData.Count == originalHazardCount)
            {
                Plugin.Log.LogInfo($"XML functionality test PASSED - data integrity verified ({_hazardData.Count} hazards loaded)");
            }
            else
            {
                Plugin.Log.LogError($"XML functionality test FAILED - data integrity compromised (expected {originalHazardCount}, got {_hazardData.Count})");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"XML functionality test FAILED: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Test XML formatting to verify XML output
    /// </summary>
    public static void TestXmlFormatting()
    {
        if (!Plugin.DebugMenu) return;
        
        try
        {
            Plugin.Log.LogInfo("Testing XML formatting...");
            
            // Clear existing data
            _hazardData.Clear();
            HazardConfigSettings.Configs.Clear();
            
            // Add test hazard data
            AddHazard(Zone.Shore, SubZoneArea.Plateau, HazardType.Jelly, 5);
            AddHazard(Zone.Tropics, SubZoneArea.Wall, HazardType.PoisonIvy, 3);
            
            // Add test hazard config data
            HazardConfigSettings.Configs[HazardType.Jelly] = new Dictionary<string, object>
            {
                {"damage", 15f},
                {"knockback", 5f}
            };
            
            // Generate XML and log it
            SaveSettings();
            
            // Read the generated XML file
            string filePath = GetSettingsFilePath();
            if (File.Exists(filePath))
            {
                string xml = File.ReadAllText(filePath);
                Plugin.Log.LogInfo("Generated XML:");
                Plugin.Log.LogInfo(xml);
                
                // Test XML validity by parsing it back
                try
                {
                    var serializer = new XmlSerializer(typeof(XmlMenuSettingsData));
                    XmlMenuSettingsData parsedData;
                    
                    using (var stringReader = new StringReader(xml))
                    {
                        parsedData = (XmlMenuSettingsData)serializer.Deserialize(stringReader);
                    }
                    
                    if (parsedData != null)
                    {
                        Plugin.Log.LogInfo("XML formatting test PASSED - XML is valid and parseable");
                        
                        // Check for proper structure
                        if (parsedData.SpawnRates != null && parsedData.HazardConfigs != null)
                        {
                            Plugin.Log.LogInfo($"XML structure test PASSED - Found {parsedData.SpawnRates.Length} SpawnRates and {parsedData.HazardConfigs.Length} HazardConfigs");
                        }
                        else
                        {
                            Plugin.Log.LogError("XML structure test FAILED - missing required sections");
                        }
                    }
                    else
                    {
                        Plugin.Log.LogError("XML formatting test FAILED - XmlSerializer returned null");
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"XML formatting test FAILED - Invalid XML: {ex.Message}");
                }
            }
            else
            {
                Plugin.Log.LogError("XML formatting test FAILED - no XML file generated");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"XML formatting test FAILED: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Initialize MenuSettings with example data for testing
    /// </summary>
    public static void InitializeWithExampleData()
    {
        // Clear any existing data
        _hazardData.Clear();
        
        // Shore biome examples
        AddHazard(Zone.Shore, SubZoneArea.Plateau, HazardType.Dynamite, 15);
        AddHazard(Zone.Shore, SubZoneArea.Plateau, HazardType.Urchin, 8);
        AddHazard(Zone.Shore, SubZoneArea.Wall, HazardType.Jelly, 12);
        
        // Tropics biome examples
        AddHazard(Zone.Tropics, SubZoneArea.Plateau, HazardType.Cactus, 20);
        AddHazard(Zone.Tropics, SubZoneArea.Wall, HazardType.Urchin, 6);
        
        // Alpine biome examples
        AddHazard(Zone.Alpine, SubZoneArea.Plateau, HazardType.Jelly, 25);
        AddHazard(Zone.Alpine, SubZoneArea.Wall, HazardType.Dynamite, 10);
        AddHazard(Zone.Alpine, SubZoneArea.WallLeft, HazardType.Cactus, 15);
        AddHazard(Zone.Alpine, SubZoneArea.WallRight, HazardType.Urchin, 18);
        
        // Mesa biome examples
        AddHazard(Zone.Mesa, SubZoneArea.Plateau, HazardType.Cactus, 30);
        AddHazard(Zone.Mesa, SubZoneArea.Wall, HazardType.Jelly, 14);
        
        // Caldera biome examples
        AddHazard(Zone.Caldera, SubZoneArea.Plateau, HazardType.Dynamite, 22);
        
        // Kiln biome examples
        AddHazard(Zone.Kiln, SubZoneArea.Wall, HazardType.Jelly, 16);
        
        // Peak biome examples
        AddHazard(Zone.Peak, SubZoneArea.Plateau, HazardType.Cactus, 12);
    }
}
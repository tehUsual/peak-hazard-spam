using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using HazardSpam.Types;
using HazardSpam.Menu.Descriptors;

namespace HazardSpam.Menu.Settings;

/// <summary>
/// Settings for individual SpawnType configurations
/// </summary>
[Serializable]
public class SpawnTypeConfig
{
    public HazardType Type { get; set; }
    public Dictionary<string, object> Values { get; set; } = new();
    
    public SpawnTypeConfig() { }
    
    public SpawnTypeConfig(HazardType type)
    {
        Type = type;
    }
}

/// <summary>
/// Main storage for Hazard Config settings
/// </summary>
[Serializable]
public class HazardConfigData
{
    public List<SpawnTypeConfig> SpawnTypeConfigs { get; set; } = new();
}

/// <summary>
/// Manages Hazard Config settings storage and retrieval
/// </summary>
public static class HazardConfigSettings
{
    public static readonly Dictionary<HazardType, Dictionary<string, object>> _configs = new();
    
    /// <summary>
    /// Get a configurable value for a field
    /// </summary>
    public static T GetValue<T>(string fieldName, T defaultValue = default!)
    {
        // Find the SpawnType that has this field
        var descriptor = SpawnTypeDescriptors.GetAll().FirstOrDefault(d => d.Fields.Any(f => f.Name == fieldName));
        if (descriptor != null && _configs.TryGetValue(descriptor.Type, out var typeConfig))
        {
            if (typeConfig.TryGetValue(fieldName, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        
        return defaultValue;
    }
    
    /// <summary>
    /// Set a configurable value for a field
    /// </summary>
    public static void SetValue(string fieldName, object value)
    {
        // Find the SpawnType that has this field
        var descriptor = SpawnTypeDescriptors.GetAll().FirstOrDefault(d => d.Fields.Any(f => f.Name == fieldName));
        if (descriptor != null)
        {
            if (!_configs.ContainsKey(descriptor.Type))
            {
                _configs[descriptor.Type] = new Dictionary<string, object>();
            }
            
            _configs[descriptor.Type][fieldName] = value;
            SaveSettings();
        }
    }
    
    /// <summary>
    /// Get all configured values for a SpawnType
    /// </summary>
    public static Dictionary<string, object> GetTypeConfig(HazardType type)
    {
        return _configs.TryGetValue(type, out var config) ? config : new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Reset a SpawnType to default values
    /// </summary>
    public static void ResetToDefaults(HazardType type)
    {
        var descriptor = SpawnTypeDescriptors.GetByType(type);
        if (descriptor != null)
        {
            _configs[type] = new Dictionary<string, object>();
            foreach (var field in descriptor.Fields)
            {
                _configs[type][field.Name] = field.DefaultValue;
            }
            SaveSettings();
        }
    }
    
    /// <summary>
    /// Reset all configurations to defaults
    /// </summary>
    public static void ResetAllToDefaults()
    {
        _configs.Clear();
        foreach (var descriptor in SpawnTypeDescriptors.GetConfigurableTypes())
        {
            _configs[descriptor.Type] = new Dictionary<string, object>();
            foreach (var field in descriptor.Fields)
            {
                _configs[descriptor.Type][field.Name] = field.DefaultValue;
            }
        }
        SaveSettings();
    }
    
    /// <summary>
    /// Get the settings file path (same as MenuSettings)
    /// </summary>
    private static string GetSettingsFilePath()
    {
        string configPath = Path.Combine(Paths.ConfigPath, "ModMenuTest");
        return Path.Combine(configPath, "Settings.json");
    }
    
    /// <summary>
    /// Load settings from JSON file (integrated with MenuSettings)
    /// </summary>
    public static void LoadSettings()
    {
        try
        {
            string filePath = GetSettingsFilePath();
            if (!File.Exists(filePath))
            {
                Plugin.Log.LogInfo("Settings file not found, using default Hazard Config settings");
                ResetAllToDefaults();
                return;
            }
            
            string json = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                Plugin.Log.LogWarning("Settings file is empty, using default Hazard Config settings");
                ResetAllToDefaults();
                return;
            }
            
            // Parse JSON manually for better control and debugging
            var parsedData = ParseHazardConfigXml(json);
            if (parsedData == null || parsedData.Length == 0)
            {
                Plugin.Log.LogInfo("No HazardConfigs found in settings file, using default settings");
                ResetAllToDefaults();
                return;
            }
            
            // Convert from serializable format back to internal format
            _configs.Clear();
            foreach (var config in parsedData)
            {
                if (Enum.TryParse<HazardType>(config.Type, out var spawnType))
                {
                    var values = new Dictionary<string, object>();
                    foreach (var field in config.Values)
                    {
                        // Convert string values back to proper types
                        object value = field.Value;
                        if (field.Type == "int32" && int.TryParse(field.Value, out int intVal))
                            value = intVal;
                        else if (field.Type == "single" && float.TryParse(field.Value, out float floatVal))
                            value = floatVal;
                        else if (field.Type == "boolean" && bool.TryParse(field.Value, out bool boolVal))
                            value = boolVal;
                        
                        values[field.Name] = value;
                    }
                    _configs[spawnType] = values;
                }
            }
            
            if (Plugin.DebugMenu)
                Plugin.Log.LogInfo($"Successfully loaded Hazard Config for {_configs.Count} SpawnTypes");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to load Hazard Config: {ex.Message}");
            Plugin.Log.LogError($"Stack trace: {ex.StackTrace}");
            Plugin.Log.LogInfo("Using default settings");
            ResetAllToDefaults();
        }
    }
    
    /// <summary>
    /// Parse HazardConfigs from XML
    /// </summary>
    private static SerializableSpawnTypeConfig[]? ParseHazardConfigXml(string xml)
    {
        try
        {
            // Parse the entire XML to get the HazardConfigs section
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(XmlMenuSettingsData));
            XmlMenuSettingsData settingsData;
            
            using (var stringReader = new StringReader(xml))
            {
                settingsData = (XmlMenuSettingsData)serializer.Deserialize(stringReader);
            }
            
            if (settingsData?.HazardConfigs == null)
            {
                if (Plugin.DebugMenu)
                    Plugin.Log.LogInfo("No HazardConfigs section found in XML");
                return new SerializableSpawnTypeConfig[0];
            }
            
            // Convert XmlSpawnTypeConfig to SerializableSpawnTypeConfig
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
            
            if (Plugin.DebugMenu)
                Plugin.Log.LogInfo($"Parsed {configs.Count} HazardConfig entries");
            return configs.ToArray();
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to parse HazardConfigs XML: {ex.Message}");
            return new SerializableSpawnTypeConfig[0];
        }
    }
    
    /// <summary>
    /// Save settings to JSON file (integrated with MenuSettings)
    /// </summary>
    public static void SaveSettings()
    {
        try
        {
            // This will be called by MenuSettings.SaveSettings() to integrate
            // the HazardConfigs into the main settings JSON file
            Plugin.Log.LogInfo($"Hazard Config settings updated for {_configs.Count} SpawnTypes");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to save Hazard Config: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Load hazard configs from serializable configs (called by MenuSettings)
    /// </summary>
    public static void LoadFromSerializableConfigs(SerializableSpawnTypeConfig[] configs)
    {
        try
        {
            _configs.Clear();
            
            foreach (var config in configs)
            {
                if (Enum.TryParse<HazardType>(config.Type, out HazardType spawnType))
                {
                    var values = new Dictionary<string, object>();
                    foreach (var field in config.Values)
                    {
                        // Convert string values back to appropriate types
                        if (field.Type == "int32" && int.TryParse(field.Value, out int intValue))
                        {
                            values[field.Name] = intValue;
                        }
                        else if (field.Type == "single" && float.TryParse(field.Value, out float floatValue))
                        {
                            values[field.Name] = floatValue;
                        }
                        else
                        {
                            // Fallback to string
                            values[field.Name] = field.Value;
                        }
                    }
                    _configs[spawnType] = values;
                }
            }
            
            if (Plugin.DebugMenu)
                Plugin.Log.LogInfo($"Loaded {_configs.Count} hazard config types from XML");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to load hazard configs from serializable configs: {ex.Message}");
        }
    }
}

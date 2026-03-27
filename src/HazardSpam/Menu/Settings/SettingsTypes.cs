using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using HazardSpam.Types;
using NetGameState.Level;
using UnityEngine.Serialization;

namespace HazardSpam.Menu.Settings;

/// <summary>
/// Settings for a single hazard configuration
/// </summary>
[Serializable]
public struct HazardSettings
{
    public int Amount;
    
    public HazardSettings(int amount)
    {
        Amount = amount;
    }
}

/// <summary>
/// Stable hazard identity with GUID
/// </summary>
[Serializable]
public class HazardData
{
    public string Id; // GUID as string
    public Zone Zone;
    public SubZoneArea SubZoneArea;
    [FormerlySerializedAs("SpawnType")] public HazardType hazardType;
    public int Amount;
    
    public HazardData()
    {
        Id = Guid.NewGuid().ToString();
    }
    
    public HazardData(Zone zone, SubZoneArea subZoneArea, HazardType hazardType, int amount)
    {
        Id = Guid.NewGuid().ToString();
        Zone = zone;
        SubZoneArea = subZoneArea;
        this.hazardType = hazardType;
        Amount = amount;
    }
}

/// <summary>
/// Serializable hazard configuration entry
/// </summary>
[Serializable]
public class HazardConfigEntry
{
    public string Zone = string.Empty;
    public string SubZoneArea = string.Empty;
    public string SpawnType = string.Empty;
    public int HazardIndex;
    public int Amount;
    
    public HazardConfigEntry() { }
    
    public HazardConfigEntry(Zone zone, SubZoneArea subZoneArea, HazardType hazardType, int hazardIndex, int amount)
    {
        Zone = zone.ToString();
        SubZoneArea = subZoneArea.ToString();
        SpawnType = hazardType.ToString();
        HazardIndex = hazardIndex;
        Amount = amount;
    }
}

/// <summary>
/// Serializable field value for HazardConfigs
/// </summary>
[Serializable]
public class FieldValue
{
    public string Name = string.Empty;
    public string Value = string.Empty;
    public string Type = string.Empty; // "int", "float", "bool", "string"
    
    public FieldValue() { }
    
    public FieldValue(string name, object value)
    {
        Name = name;
        Type = value.GetType().Name.ToLower();
        Value = value.ToString() ?? string.Empty;
    }
}

/// <summary>
/// Serializable SpawnType configuration for JSON
/// </summary>
[Serializable]
public class SerializableSpawnTypeConfig
{
    public string Type = string.Empty;
    public FieldValue[] Values = Array.Empty<FieldValue>();
    
    public SerializableSpawnTypeConfig() { }
    
    public SerializableSpawnTypeConfig(HazardType hazardType, Dictionary<string, object> values)
    {
        Type = hazardType.ToString();
        Values = values.Select(kvp => new FieldValue(kvp.Key, kvp.Value)).ToArray();
    }
}

/// <summary>
/// XML serializable hazard entry
/// </summary>
[Serializable]
[XmlRoot("HazardEntry")]
public class XmlHazardEntry
{
    [XmlAttribute("Zone")]
    public string Zone = "";
    
    [XmlAttribute("SubZoneArea")]
    public string SubZoneArea = "";
    
    [XmlAttribute("SpawnType")]
    public string SpawnType = "";
    
    [XmlAttribute("HazardIndex")]
    public int HazardIndex;
    
    [XmlAttribute("Amount")]
    public int Amount;
    
    public XmlHazardEntry() { }
    
    public XmlHazardEntry(HazardData hazard, int index)
    {
        Zone = hazard.Zone.ToString();
        SubZoneArea = hazard.SubZoneArea.ToString();
        SpawnType = hazard.hazardType.ToString();
        HazardIndex = index;
        Amount = hazard.Amount;
    }
}

/// <summary>
/// XML serializable field value
/// </summary>
[Serializable]
[XmlRoot("FieldValue")]
public class XmlFieldValue
{
    [XmlAttribute("Name")]
    public string Name = "";
    
    [XmlAttribute("Value")]
    public string Value = "";
    
    [XmlAttribute("Type")]
    public string Type = "";
    
    public XmlFieldValue() { }
    
    public XmlFieldValue(string name, object value)
    {
        Name = name;
        Value = value.ToString() ?? "";
        Type = GetTypeName(value);
    }
    
    private static string GetTypeName(object value)
    {
        if (value is int) return "int32";
        if (value is float) return "single";
        if (value is bool) return "boolean";
        if (value is string) return "string";
        return "object";
    }
}

/// <summary>
/// XML serializable spawn type config
/// </summary>
[Serializable]
[XmlRoot("SpawnTypeConfig")]
public class XmlSpawnTypeConfig
{
    [XmlAttribute("Type")]
    public string Type = "";
    
    [XmlArray("Values")]
    [XmlArrayItem("FieldValue")]
    public XmlFieldValue[] Values = new XmlFieldValue[0];
    
    public XmlSpawnTypeConfig() { }
    
    public XmlSpawnTypeConfig(HazardType hazardType, Dictionary<string, object> values)
    {
        Type = hazardType.ToString();
        Values = values.Select(kvp => new XmlFieldValue(kvp.Key, kvp.Value)).ToArray();
    }
}

/// <summary>
/// XML serializable settings container
/// </summary>
[Serializable]
[XmlRoot("MenuSettings")]
public class XmlMenuSettingsData
{
    [XmlArray("SpawnRates")]
    [XmlArrayItem("HazardEntry")]
    public XmlHazardEntry[] SpawnRates = new XmlHazardEntry[0];
    
    [XmlArray("HazardConfigs")]
    [XmlArrayItem("SpawnTypeConfig")]
    public XmlSpawnTypeConfig[] HazardConfigs = new XmlSpawnTypeConfig[0];
    
    public XmlMenuSettingsData() { }
}
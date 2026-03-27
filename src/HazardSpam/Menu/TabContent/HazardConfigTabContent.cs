using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using HazardSpam.Types;
using HazardSpam.Menu.Settings;
using HazardSpam.Menu.Descriptors;

namespace HazardSpam.Menu.TabContent;

/// <summary>
/// Tab content for Hazard Config settings
/// </summary>
public class HazardConfigTabContent
{
    private readonly RectTransform _parent;
    private readonly Dictionary<string, InputField> _inputFields = new();
    private readonly Dictionary<string, Text> _defaultValueLabels = new();
    private readonly Dictionary<string, HazardType> _fieldSpawnTypes = new(); // Track which SpawnType each field belongs to
    
    public HazardConfigTabContent(RectTransform parent)
    {
        _parent = parent;
        if (Plugin.DebugMenu)
            Plugin.Log.LogInfo("HazardConfigTabContent: Creating content...");
        CreateContent();
        if (Plugin.DebugMenu)
            Plugin.Log.LogInfo("HazardConfigTabContent: Content creation completed.");
    }
    
    private void CreateContent()
    {
        // Create header
        CreateHeader();
        
        // Create content for each configurable SpawnType directly in parent
        var configurableTypes = SpawnTypeDescriptors.GetConfigurableTypes().ToList();
        if (Plugin.DebugMenu)
            Plugin.Log.LogInfo($"HazardConfigTabContent: Found {configurableTypes.Count} configurable types");
        for (int i = 0; i < configurableTypes.Count; i++)
        {
            if (Plugin.DebugMenu)
                Plugin.Log.LogInfo($"HazardConfigTabContent: Creating section for {configurableTypes[i].Type}");
            CreateSpawnTypeSection(configurableTypes[i], _parent, i);
        }
        
        // Create reset button
        CreateResetButton();
    }
    
    private void CreateHeader()
    {
        var headerGo = new GameObject("Header");
        headerGo.transform.SetParent(_parent, false);
        
        var headerRT = headerGo.AddComponent<RectTransform>();
        headerRT.anchorMin = new Vector2(0, 1);
        headerRT.anchorMax = new Vector2(1, 1);
        headerRT.pivot = new Vector2(0.5f, 1);
        headerRT.sizeDelta = new Vector2(-20f, 40f);
        headerRT.anchoredPosition = new Vector2(0, -10f);
        
        var headerText = headerGo.AddComponent<Text>();
        headerText.font = UIStyle.DefaultFont;
        headerText.color = UIStyle.Colors.TextDefault;
        headerText.text = "--- Hazard Configuration ---";
        headerText.fontSize = UIStyle.FontSizes.Header;
        headerText.fontStyle = FontStyle.Bold;
        headerText.alignment = TextAnchor.MiddleCenter;
    }
    
    
    private void CreateSpawnTypeSection(SpawnTypeDescriptor descriptor, RectTransform parent, int index)
    {
        var sectionGo = new GameObject($"Section_{descriptor.Type}");
        sectionGo.transform.SetParent(parent, false);
        
        var sectionRT = sectionGo.AddComponent<RectTransform>();
        
        // Calculate column and row for two-column layout
        int column = index % 2; // 0 for left column, 1 for right column
        int row = index / 2;    // Row number (0, 1, 2, etc.)
        
        // Set up two-column layout
        sectionRT.anchorMin = new Vector2(column * 0.5f, 1);
        sectionRT.anchorMax = new Vector2((column + 1) * 0.5f, 1);
        sectionRT.pivot = new Vector2(0.5f, 1);
        sectionRT.sizeDelta = new Vector2(-20f, 200f); // Add padding from edges
        sectionRT.anchoredPosition = new Vector2(0, -50f - (row * 220f)); // Stack vertically within columns
        
        // Create section header
        CreateSectionHeader(descriptor, sectionRT);
        
        // Create fields
        // Note: _inputFields and _defaultValueLabels are now keyed by field name directly
        
        for (int i = 0; i < descriptor.Fields.Count; i++)
        {
            CreateField(descriptor.Fields[i], descriptor.Type, sectionRT, i);
        }
        
        // Update section height based on field count
        sectionRT.sizeDelta = new Vector2(0, 40f + (descriptor.Fields.Count * 35f));
    }
    
    private void CreateSectionHeader(SpawnTypeDescriptor descriptor, RectTransform parent)
    {
        var headerGo = new GameObject("SectionHeader");
        headerGo.transform.SetParent(parent, false);
        
        var headerRT = headerGo.AddComponent<RectTransform>();
        headerRT.anchorMin = new Vector2(0, 1);
        headerRT.anchorMax = new Vector2(1, 1);
        headerRT.pivot = new Vector2(0.5f, 1);
        headerRT.sizeDelta = new Vector2(-20f, 30f); // Add padding from edges
        headerRT.anchoredPosition = new Vector2(10f, -5f); // Move slightly right
        
        var headerText = headerGo.AddComponent<Text>();
        headerText.font = UIStyle.DefaultFont;
        headerText.color = UIStyle.Colors.TextDefault;
        headerText.text = descriptor.DisplayName;
        headerText.fontSize = UIStyle.FontSizes.Header;
        headerText.fontStyle = FontStyle.Bold;
        headerText.alignment = TextAnchor.MiddleLeft;
        
        // Add description if available
        if (!string.IsNullOrEmpty(descriptor.Description))
        {
            var descGo = new GameObject("Description");
            descGo.transform.SetParent(parent, false);
            
            var descRT = descGo.AddComponent<RectTransform>();
            descRT.anchorMin = new Vector2(0, 1);
            descRT.anchorMax = new Vector2(1, 1);
            descRT.pivot = new Vector2(0.5f, 1);
            descRT.sizeDelta = new Vector2(-20f, 20f); // Add padding from edges
            descRT.anchoredPosition = new Vector2(10f, -25f); // Move slightly right
            
            var descText = descGo.AddComponent<Text>();
            descText.font = UIStyle.DefaultFont;
            descText.color = UIStyle.Colors.TextDefault;
            descText.text = descriptor.Description;
            descText.fontSize = UIStyle.FontSizes.Small;
            descText.alignment = TextAnchor.MiddleLeft;
            
            // Add padding under description by adjusting the field positions
            // This will be handled in CreateField method by adding extra offset
        }
    }
    
    private void CreateField(SpawnTypeField field, HazardType hazardType, RectTransform parent, int index)
    {
        var fieldGo = new GameObject($"Field_{field.Name}");
        fieldGo.transform.SetParent(parent, false);
        
        var fieldRT = fieldGo.AddComponent<RectTransform>();
        fieldRT.anchorMin = new Vector2(0, 1);
        fieldRT.anchorMax = new Vector2(1, 1);
        fieldRT.pivot = new Vector2(0.5f, 1);
        fieldRT.sizeDelta = new Vector2(-40f, 30f); // Add more padding from edges
        
        // Add extra padding under description if it exists
        var hasDescription = !string.IsNullOrEmpty(SpawnTypeDescriptors.GetByType(hazardType)?.Description);
        float descriptionPadding = hasDescription ? 8f : 0f; // Add 8 pixels of padding under description
        fieldRT.anchoredPosition = new Vector2(10f, -40f - (index * 35f) - descriptionPadding); // Move slightly right
        
        // Create field label
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(fieldGo.transform, false);
        
        var labelRT = labelGo.AddComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0, 0);
        labelRT.anchorMax = new Vector2(0.4f, 1);
        labelRT.sizeDelta = Vector2.zero;
        labelRT.anchoredPosition = Vector2.zero;
        
        var labelText = labelGo.AddComponent<Text>();
        labelText.font = UIStyle.DefaultFont;
        labelText.color = UIStyle.Colors.TextDefault;
        labelText.text = field.DisplayName;
        labelText.fontSize = UIStyle.FontSizes.Default;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        // Create input field (same width as amount fields in spawn rates)
        var inputGo = new GameObject("Input");
        inputGo.transform.SetParent(fieldGo.transform, false);
        
        var inputRT = inputGo.AddComponent<RectTransform>();
        inputRT.anchorMin = new Vector2(0, 0);
        inputRT.anchorMax = new Vector2(0, 1);
        inputRT.pivot = new Vector2(0, 0.5f);
        inputRT.sizeDelta = new Vector2(UIStyle.Layout.InputWidth, 0); // Use same width as amount fields
        inputRT.anchoredPosition = new Vector2(120f, 0); // Position closer to label
        
        var inputImage = inputGo.AddComponent<Image>();
        
        // Check if field value differs from default and apply lighter background
        var currentValue = GetValueForSpawnType(field, hazardType);
        bool isModified = !IsValueEqualToDefault(currentValue, field);
        inputImage.color = isModified ? GetModifiedFieldColor() : UIStyle.Colors.InputBackground;
        
        var inputField = inputGo.AddComponent<InputField>();
        inputField.contentType = field.Type == FieldType.Int ? 
            InputField.ContentType.IntegerNumber : InputField.ContentType.DecimalNumber;
        inputField.targetGraphic = inputImage;
        
        // Create input text
        var inputTextGo = new GameObject("Text");
        inputTextGo.transform.SetParent(inputGo.transform, false);
        
        var inputTextRT = inputTextGo.AddComponent<RectTransform>();
        inputTextRT.anchorMin = new Vector2(0.1f, 0);
        inputTextRT.anchorMax = new Vector2(1f, 1);
        inputTextRT.sizeDelta = Vector2.zero;
        inputTextRT.anchoredPosition = Vector2.zero;
        
        var inputText = inputTextGo.AddComponent<Text>();
        inputText.font = UIStyle.DefaultFont;
        inputText.color = UIStyle.Colors.TextDefault;
        inputText.fontSize = UIStyle.FontSizes.Default;
        inputText.alignment = TextAnchor.MiddleLeft;
        
        inputField.textComponent = inputText;
        
        // Set initial value - get value directly from the specific SpawnType's config
        var initialValue = GetValueForSpawnType(field, hazardType);
        inputField.text = initialValue.ToString();
        
        // Create default value label (with padding and lighter color)
        var defaultGo = new GameObject("DefaultValue");
        defaultGo.transform.SetParent(fieldGo.transform, false);
        
        var defaultRT = defaultGo.AddComponent<RectTransform>();
        defaultRT.anchorMin = new Vector2(0, 0);
        defaultRT.anchorMax = new Vector2(1, 1);
        defaultRT.pivot = new Vector2(0, 0.5f);
        defaultRT.sizeDelta = Vector2.zero;
        defaultRT.anchoredPosition = new Vector2(220f, 0); // Position after input field with padding
        
        var defaultText = defaultGo.AddComponent<Text>();
        defaultText.font = UIStyle.DefaultFont;
        defaultText.color = new Color(0.7f, 0.7f, 0.7f, 1f); // Lighter, more visible color
        defaultText.text = $"Default: {field.DefaultValue}{field.Unit}";
        defaultText.fontSize = UIStyle.FontSizes.Small;
        defaultText.alignment = TextAnchor.MiddleLeft;
        
        // Create unique key for this field (SpawnType + FieldName)
        string uniqueKey = $"{hazardType}_{field.Name}";
        
        // Store references
        _inputFields[uniqueKey] = inputField;
        _defaultValueLabels[uniqueKey] = defaultText;
        _fieldSpawnTypes[uniqueKey] = hazardType;
        
        // Set up change handler
        inputField.onEndEdit.AddListener(value =>
        {
            if (field.Type == FieldType.Int && int.TryParse(value, out int intValue))
            {
                SetValueForSpawnType(field.Name, hazardType, intValue);
                UpdateFieldBackgroundColor(inputImage, intValue, field);
            }
            else if (field.Type == FieldType.Float && float.TryParse(value, out float floatValue))
            {
                SetValueForSpawnType(field.Name, hazardType, floatValue);
                UpdateFieldBackgroundColor(inputImage, floatValue, field);
            }
        });
    }
    
    private void CreateResetButton()
    {
        var buttonGo = new GameObject("ResetButton");
        buttonGo.transform.SetParent(_parent, false);
        
        var buttonRT = buttonGo.AddComponent<RectTransform>();
        buttonRT.anchorMin = new Vector2(0.5f, 0);
        buttonRT.anchorMax = new Vector2(0.5f, 0);
        buttonRT.pivot = new Vector2(0.5f, 0);
        buttonRT.sizeDelta = new Vector2(150f, 30f);
        buttonRT.anchoredPosition = new Vector2(0, 20f); // Move up to be more visible
        
        var button = buttonGo.AddComponent<Button>();
        var buttonImage = buttonGo.AddComponent<Image>();
        buttonImage.color = UIStyle.Colors.ButtonDefault;
        button.targetGraphic = buttonImage;
        
        // Create button text
        var textGo = new GameObject("Text");
        textGo.transform.SetParent(buttonGo.transform, false);
        
        var textRT = textGo.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;
        
        var buttonText = textGo.AddComponent<Text>();
        buttonText.font = UIStyle.DefaultFont;
        buttonText.color = UIStyle.Colors.TextDefault;
        buttonText.text = "Reset All to Defaults";
        buttonText.fontSize = UIStyle.FontSizes.Default;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        // Set up click handler
        button.onClick.AddListener(() =>
        {
            HazardConfigSettings.ResetAllToDefaults();
            RefreshAllFields();
        });
    }
    
    private void RefreshAllFields()
    {
        foreach (var fieldKvp in _inputFields)
        {
            string uniqueKey = fieldKvp.Key;
            if (_fieldSpawnTypes.TryGetValue(uniqueKey, out var spawnType))
            {
                // Find the field descriptor for this specific SpawnType
                var descriptor = SpawnTypeDescriptors.GetAll().FirstOrDefault(d => d.Type == spawnType);
                if (descriptor != null)
                {
                    var field = descriptor.Fields.FirstOrDefault(f => f.Name == uniqueKey.Split('_')[1]);
                    if (field != null)
                    {
                        // Get the current value directly from the specific SpawnType's config
                        var currentValue = GetValueForSpawnType(field, spawnType);
                        fieldKvp.Value.text = currentValue.ToString();
                        
                        // Update background color based on whether field is modified
                        var inputImage = fieldKvp.Value.targetGraphic as Image;
                        if (inputImage != null)
                        {
                            UpdateFieldBackgroundColor(inputImage, currentValue, field);
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Get value for a specific SpawnType and field, bypassing the generic GetValue method
    /// </summary>
    private object GetValueForSpawnType(SpawnTypeField field, HazardType hazardType)
    {
        if (HazardConfigSettings.Configs.TryGetValue(hazardType, out var typeConfig))
        {
            if (typeConfig.TryGetValue(field.Name, out var value))
            {
                try
                {
                    if (field.Type == FieldType.Int)
                        return Convert.ChangeType(value, typeof(int));
                    else if (field.Type == FieldType.Float)
                        return Convert.ChangeType(value, typeof(float));
                }
                catch
                {
                    // Fall back to default value
                }
            }
        }
        
        // Return default value if not found
        return field.DefaultValue;
    }
    
    /// <summary>
    /// Set value for a specific SpawnType and field, bypassing the generic SetValue method
    /// </summary>
    private void SetValueForSpawnType(string fieldName, HazardType hazardType, object value)
    {
        if (!HazardConfigSettings.Configs.ContainsKey(hazardType))
        {
            HazardConfigSettings.Configs[hazardType] = new Dictionary<string, object>();
        }
        
        HazardConfigSettings.Configs[hazardType][fieldName] = value;
        HazardConfigSettings.SaveSettings();
    }
    
    /// <summary>
    /// Check if a value equals the default value for a field
    /// </summary>
    private bool IsValueEqualToDefault(object value, SpawnTypeField field)
    {
        if (value == null) return true;
        
        try
        {
            if (field.Type == FieldType.Int)
            {
                return Convert.ToInt32(value) == Convert.ToInt32(field.DefaultValue);
            }
            else if (field.Type == FieldType.Float)
            {
                return Math.Abs(Convert.ToSingle(value) - Convert.ToSingle(field.DefaultValue)) < 0.001f;
            }
        }
        catch
        {
            // If conversion fails, assume it's not equal to default
            return false;
        }
        
        return false;
    }
    
    /// <summary>
    /// Get the background color for modified fields (slightly lighter)
    /// </summary>
    private Color GetModifiedFieldColor()
    {
        // Create a lighter version of the default input background
        var baseColor = UIStyle.Colors.InputBackground;
        return new Color(
            Math.Min(1f, baseColor.r + 0.1f),
            Math.Min(1f, baseColor.g + 0.1f),
            Math.Min(1f, baseColor.b + 0.1f),
            baseColor.a
        );
    }
    
    /// <summary>
    /// Update the background color of a field based on whether it's modified
    /// </summary>
    private void UpdateFieldBackgroundColor(Image inputImage, object value, SpawnTypeField field)
    {
        bool isModified = !IsValueEqualToDefault(value, field);
        inputImage.color = isModified ? GetModifiedFieldColor() : UIStyle.Colors.InputBackground;
    }
}
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
/// Tab content for Hazard Tweaks settings (per HazardType fields).
/// </summary>
public class HazardConfigTabContent
{
    private const float MainHeaderReserve = 50f;
    private const float SectionGap = 16f;
    private const float ResetBlockHeight = 48f;

    /// <summary>Vertical spacing between field rows (must match <see cref="CreateField"/>).</summary>
    private const float FieldRowStride = 40f;

    /// <summary>Rect height of one field row.</summary>
    private const float FieldRowHeight = 38f;

    private readonly RectTransform _parent;
    private readonly Dictionary<string, InputField> _inputFields = new();
    private readonly Dictionary<string, Toggle> _boolToggles = new();
    private readonly Dictionary<string, Dropdown> _enumDropdowns = new();
    private readonly Dictionary<string, int[]> _enumOptionInts = new();
    private readonly Dictionary<string, Text> _defaultValueLabels = new();
    private readonly Dictionary<string, HazardType> _fieldSpawnTypes = new();
    
    public HazardConfigTabContent(RectTransform parent)
    {
        _parent = parent;
        if (Plugin.DebugMenu)
            Plugin.Log.LogInfo("HazardConfigTabContent: Creating content...");
        CreateContent();
        if (Plugin.DebugMenu)
            Plugin.Log.LogInfo("HazardConfigTabContent: Content creation completed.");
    }
    
    /// <summary>
    /// Total scrollable height for the hazard tweaks view (must match <see cref="CreateContent"/> layout).
    /// </summary>
    public static float CalculateTotalScrollHeight()
    {
        var types = SpawnTypeDescriptors.GetConfigurableTypes().ToList();
        float y = MainHeaderReserve;
        foreach (var d in types)
            y += EstimateSectionHeight(d) + SectionGap;
        y += ResetBlockHeight;
        return y;
    }

    /// <summary>
    /// Vertical size for one HazardType block; must match <see cref="CreateField"/> / header placement.
    /// </summary>
    private static float EstimateSectionHeight(SpawnTypeDescriptor descriptor)
    {
        bool hasDesc = !string.IsNullOrEmpty(descriptor.Description);
        float descPad = hasDesc ? 8f : 0f;
        int n = descriptor.Fields.Count;
        if (n == 0)
            return 40f + (hasDesc ? 28f : 0f);
        // First field top at -40 - descPad; last field height FieldRowHeight; stride FieldRowStride between rows.
        float bottomFromTop = 40f + descPad + (n - 1) * FieldRowStride + FieldRowHeight;
        return bottomFromTop + 12f;
    }

    private static string FormatFieldRange(SpawnTypeField field)
    {
        if (field.Type is FieldType.Bool or FieldType.Enum)
            return string.Empty;

        float? min = field.MinValue;
        float? max = field.MaxValue;
        string u = field.Unit ?? string.Empty;

        if (min == null && max == null)
            return "Range: —";

        string F(float v) => field.Type == FieldType.Int ? Mathf.RoundToInt(v).ToString() : v.ToString("0.###");

        if (min != null && max != null)
            return $"Range: {F(min.Value)} – {F(max.Value)}{u}";
        if (min != null)
            return $"Range: ≥ {F(min.Value)}{u}";
        if (max != null)
            return $"Range: ≤ {F(max.Value)}{u}";
        return "Range: —";
    }

    private static string FieldNameFromUniqueKey(string uniqueKey)
    {
        int i = uniqueKey.IndexOf('_');
        return i < 0 ? uniqueKey : uniqueKey.Substring(i + 1);
    }

    private static string DefaultValueText(SpawnTypeField field)
    {
        if (field.Type == FieldType.Bool)
            return $"Default: {field.DefaultValue}";
        if (field.Type == FieldType.Enum && field.EnumType != null && field.EnumType.IsEnum)
        {
            int v = Convert.ToInt32(field.DefaultValue);
            object ev = Enum.ToObject(field.EnumType, v);
            return $"Default: {Enum.GetName(field.EnumType, ev) ?? v.ToString()}";
        }
        return $"Default: {field.DefaultValue}{field.Unit}";
    }

    private void CreateContent()
    {
        CreateHeader();

        var configurableTypes = SpawnTypeDescriptors.GetConfigurableTypes().ToList();
        if (Plugin.DebugMenu)
            Plugin.Log.LogInfo($"HazardConfigTabContent: Found {configurableTypes.Count} configurable types");

        float y = MainHeaderReserve;
        foreach (var descriptor in configurableTypes)
        {
            if (Plugin.DebugMenu)
                Plugin.Log.LogInfo($"HazardConfigTabContent: Creating section for {descriptor.Type}");
            float h = EstimateSectionHeight(descriptor);
            CreateSpawnTypeSection(descriptor, _parent, y, h);
            y += h + SectionGap;
        }

        CreateResetButton(y);
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
        headerText.text = "--- Hazard Tweaks ---";
        headerText.fontSize = UIStyle.FontSizes.Header;
        headerText.fontStyle = FontStyle.Bold;
        headerText.alignment = TextAnchor.MiddleCenter;
    }
    
    
    private void CreateSpawnTypeSection(SpawnTypeDescriptor descriptor, RectTransform parent, float yFromTop, float height)
    {
        var sectionGo = new GameObject($"Section_{descriptor.Type}");
        sectionGo.transform.SetParent(parent, false);

        var sectionRT = sectionGo.AddComponent<RectTransform>();
        sectionRT.anchorMin = new Vector2(0f, 1f);
        sectionRT.anchorMax = new Vector2(1f, 1f);
        sectionRT.pivot = new Vector2(0.5f, 1f);
        sectionRT.sizeDelta = new Vector2(-20f, height);
        sectionRT.anchoredPosition = new Vector2(0f, -yFromTop);

        CreateSectionHeader(descriptor, sectionRT);

        for (int i = 0; i < descriptor.Fields.Count; i++)
            CreateField(descriptor.Fields[i], descriptor.Type, sectionRT, i);
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
        switch (field.Type)
        {
            case FieldType.Float:
            case FieldType.Int:
                CreateNumericFieldRow(field, hazardType, parent, index);
                break;
            case FieldType.Bool:
                CreateBoolFieldRow(field, hazardType, parent, index);
                break;
            case FieldType.Enum:
                CreateEnumFieldRow(field, hazardType, parent, index);
                break;
            default:
                CreateNumericFieldRow(field, hazardType, parent, index);
                break;
        }
    }

    private (GameObject fieldGo, Text defaultText, Text metaText, string uniqueKey) CreateSharedFieldRow(
        SpawnTypeField field, HazardType hazardType, RectTransform parent, int index)
    {
        var fieldGo = new GameObject($"Field_{field.Name}");
        fieldGo.transform.SetParent(parent, false);

        var fieldRT = fieldGo.AddComponent<RectTransform>();
        fieldRT.anchorMin = new Vector2(0, 1);
        fieldRT.anchorMax = new Vector2(1, 1);
        fieldRT.pivot = new Vector2(0.5f, 1);
        fieldRT.sizeDelta = new Vector2(-40f, FieldRowHeight);

        var hasDescription = !string.IsNullOrEmpty(SpawnTypeDescriptors.GetByType(hazardType)?.Description);
        float descriptionPadding = hasDescription ? 8f : 0f;
        fieldRT.anchoredPosition = new Vector2(10f, -40f - (index * FieldRowStride) - descriptionPadding);

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(fieldGo.transform, false);

        var labelRT = labelGo.AddComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0, 0);
        labelRT.anchorMax = new Vector2(0, 1);
        labelRT.pivot = new Vector2(0, 0.5f);
        labelRT.sizeDelta = new Vector2(112f, 0f);
        labelRT.anchoredPosition = Vector2.zero;

        var labelText = labelGo.AddComponent<Text>();
        labelText.font = UIStyle.DefaultFont;
        labelText.color = UIStyle.Colors.TextDefault;
        labelText.text = field.DisplayName;
        labelText.fontSize = UIStyle.FontSizes.Default;
        labelText.alignment = TextAnchor.MiddleLeft;

        var defaultGo = new GameObject("DefaultValue");
        defaultGo.transform.SetParent(fieldGo.transform, false);

        var defaultRT = defaultGo.AddComponent<RectTransform>();
        defaultRT.anchorMin = new Vector2(0, 0);
        defaultRT.anchorMax = new Vector2(0, 1);
        defaultRT.pivot = new Vector2(0, 0.5f);
        defaultRT.sizeDelta = new Vector2(118f, 0f);
        defaultRT.anchoredPosition = new Vector2(206f, 0f);

        var defaultText = defaultGo.AddComponent<Text>();
        defaultText.font = UIStyle.DefaultFont;
        defaultText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        defaultText.text = DefaultValueText(field);
        defaultText.fontSize = UIStyle.FontSizes.Small;
        defaultText.alignment = TextAnchor.MiddleLeft;
        defaultText.horizontalOverflow = HorizontalWrapMode.Wrap;
        defaultText.verticalOverflow = VerticalWrapMode.Truncate;

        var metaGo = new GameObject("RangeAndDescription");
        metaGo.transform.SetParent(fieldGo.transform, false);

        var metaRT = metaGo.AddComponent<RectTransform>();
        metaRT.anchorMin = new Vector2(0f, 0f);
        metaRT.anchorMax = new Vector2(1f, 1f);
        metaRT.pivot = new Vector2(0f, 1f);
        metaRT.offsetMin = new Vector2(332f, 2f);
        metaRT.offsetMax = new Vector2(-8f, -2f);

        var metaText = metaGo.AddComponent<Text>();
        metaText.font = UIStyle.DefaultFont;
        metaText.color = new Color(0.65f, 0.72f, 0.78f, 1f);
        metaText.fontSize = UIStyle.FontSizes.Small;
        metaText.alignment = TextAnchor.UpperLeft;
        metaText.horizontalOverflow = HorizontalWrapMode.Wrap;
        metaText.verticalOverflow = VerticalWrapMode.Truncate;
        metaText.supportRichText = false;
        ApplyMetaText(metaText, field);

        string uniqueKey = $"{hazardType}_{field.Name}";
        _defaultValueLabels[uniqueKey] = defaultText;
        _fieldSpawnTypes[uniqueKey] = hazardType;

        return (fieldGo, defaultText, metaText, uniqueKey);
    }

    private static void ApplyMetaText(Text metaText, SpawnTypeField field)
    {
        string fd = field.Description?.Trim() ?? string.Empty;
        if (field.Type == FieldType.Bool || field.Type == FieldType.Enum)
        {
            metaText.text = fd;
            return;
        }

        string line1 = FormatFieldRange(field);
        if (string.IsNullOrEmpty(line1) && string.IsNullOrEmpty(fd))
            metaText.text = string.Empty;
        else if (string.IsNullOrEmpty(fd))
            metaText.text = line1;
        else if (string.IsNullOrEmpty(line1))
            metaText.text = fd;
        else
            metaText.text = $"{line1}\n{fd}";
    }

    private void CreateNumericFieldRow(SpawnTypeField field, HazardType hazardType, RectTransform parent, int index)
    {
        var (fieldGo, _, _, uniqueKey) = CreateSharedFieldRow(field, hazardType, parent, index);

        var inputGo = new GameObject("Input");
        inputGo.transform.SetParent(fieldGo.transform, false);

        var inputRT = inputGo.AddComponent<RectTransform>();
        inputRT.anchorMin = new Vector2(0, 0);
        inputRT.anchorMax = new Vector2(0, 1);
        inputRT.pivot = new Vector2(0, 0.5f);
        inputRT.sizeDelta = new Vector2(UIStyle.Layout.InputWidth, 0);
        inputRT.anchoredPosition = new Vector2(118f, 0f);

        var inputImage = inputGo.AddComponent<Image>();
        object currentValue = GetValueForSpawnType(field, hazardType);
        inputImage.color = !IsValueEqualToDefault(currentValue, field) ? GetModifiedFieldColor() : UIStyle.Colors.InputBackground;

        var inputField = inputGo.AddComponent<InputField>();
        inputField.contentType = field.Type == FieldType.Int
            ? InputField.ContentType.IntegerNumber
            : InputField.ContentType.DecimalNumber;
        inputField.targetGraphic = inputImage;

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
        inputField.text = currentValue.ToString() ?? string.Empty;

        _inputFields[uniqueKey] = inputField;

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

    private void CreateBoolFieldRow(SpawnTypeField field, HazardType hazardType, RectTransform parent, int index)
    {
        var (fieldGo, _, _, uniqueKey) = CreateSharedFieldRow(field, hazardType, parent, index);

        var toggleGo = new GameObject("Toggle");
        toggleGo.transform.SetParent(fieldGo.transform, false);

        var toggleRT = toggleGo.AddComponent<RectTransform>();
        toggleRT.anchorMin = new Vector2(0f, 0.5f);
        toggleRT.anchorMax = new Vector2(0f, 0.5f);
        toggleRT.pivot = new Vector2(0f, 0.5f);
        toggleRT.sizeDelta = new Vector2(24f, 24f);
        toggleRT.anchoredPosition = new Vector2(118f, 0f);

        var bg = toggleGo.AddComponent<Image>();
        object currentValue = GetValueForSpawnType(field, hazardType);
        bool on = Convert.ToBoolean(currentValue);
        bg.color = !IsValueEqualToDefault(on, field) ? GetModifiedFieldColor() : UIStyle.Colors.InputBackground;

        var toggle = toggleGo.AddComponent<Toggle>();
        toggle.targetGraphic = bg;
        toggle.isOn = on;

        var checkGo = new GameObject("Checkmark");
        checkGo.transform.SetParent(toggleGo.transform, false);
        var checkRT = checkGo.AddComponent<RectTransform>();
        checkRT.anchorMin = Vector2.zero;
        checkRT.anchorMax = Vector2.one;
        checkRT.offsetMin = new Vector2(3f, 3f);
        checkRT.offsetMax = new Vector2(-3f, -3f);
        var checkImg = checkGo.AddComponent<Image>();
        checkImg.color = UIStyle.Colors.ButtonActive;
        toggle.graphic = checkImg;

        _boolToggles[uniqueKey] = toggle;

        toggle.onValueChanged.AddListener(v =>
        {
            SetValueForSpawnType(field.Name, hazardType, v);
            bg.color = !IsValueEqualToDefault(v, field) ? GetModifiedFieldColor() : UIStyle.Colors.InputBackground;
        });
    }

    private void CreateEnumFieldRow(SpawnTypeField field, HazardType hazardType, RectTransform parent, int index)
    {
        var (fieldGo, _, _, uniqueKey) = CreateSharedFieldRow(field, hazardType, parent, index);

        var enumType = field.EnumType;
        if (enumType == null || !enumType.IsEnum)
        {
            Plugin.Log.LogError($"Hazard Tweaks: Enum field '{field.Name}' has no valid {nameof(SpawnTypeField.EnumType)}.");
            return;
        }

        string[] names = Enum.GetNames(enumType);
        var ints = new int[names.Length];
        for (int i = 0; i < names.Length; i++)
            ints[i] = Convert.ToInt32(Enum.Parse(enumType, names[i]));

        var dropdown = UIBuilder.CreateDropdown(fieldGo.transform, names, new Vector2(118f, 0f),
            new Vector2(UIStyle.Layout.InputWidth, 24f), null);
        var bg = dropdown.targetGraphic as Image;
        object currentValue = GetValueForSpawnType(field, hazardType);
        int currentInt = Convert.ToInt32(currentValue);
        int selIdx = Array.IndexOf(ints, currentInt);
        if (selIdx < 0)
            selIdx = 0;
        dropdown.value = selIdx;
        if (bg != null)
            bg.color = !IsValueEqualToDefault(currentInt, field) ? GetModifiedFieldColor() : UIStyle.Colors.DropdownBackground;

        _enumDropdowns[uniqueKey] = dropdown;
        _enumOptionInts[uniqueKey] = ints;

        dropdown.onValueChanged.AddListener(idx =>
        {
            int v = ints[idx];
            SetValueForSpawnType(field.Name, hazardType, v);
            if (bg != null)
                bg.color = !IsValueEqualToDefault(v, field) ? GetModifiedFieldColor() : UIStyle.Colors.DropdownBackground;
        });
    }
    
    private void CreateResetButton(float yFromTop)
    {
        var buttonGo = new GameObject("ResetButton");
        buttonGo.transform.SetParent(_parent, false);

        var buttonRT = buttonGo.AddComponent<RectTransform>();
        buttonRT.anchorMin = new Vector2(0.5f, 1f);
        buttonRT.anchorMax = new Vector2(0.5f, 1f);
        buttonRT.pivot = new Vector2(0.5f, 1f);
        buttonRT.sizeDelta = new Vector2(150f, 30f);
        buttonRT.anchoredPosition = new Vector2(0f, -yFromTop);
        
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
        var keys = new HashSet<string>();
        foreach (var k in _inputFields.Keys) keys.Add(k);
        foreach (var k in _boolToggles.Keys) keys.Add(k);
        foreach (var k in _enumDropdowns.Keys) keys.Add(k);

        foreach (string uniqueKey in keys)
        {
            if (!_fieldSpawnTypes.TryGetValue(uniqueKey, out var spawnType))
                continue;
            var descriptor = SpawnTypeDescriptors.GetAll().FirstOrDefault(d => d.Type == spawnType);
            if (descriptor == null)
                continue;
            string fieldName = FieldNameFromUniqueKey(uniqueKey);
            var field = descriptor.Fields.FirstOrDefault(f => f.Name == fieldName);
            if (field == null)
                continue;

            object currentValue = GetValueForSpawnType(field, spawnType);

            switch (field.Type)
            {
                case FieldType.Float:
                case FieldType.Int:
                    if (_inputFields.TryGetValue(uniqueKey, out var inputField))
                    {
                        inputField.text = currentValue?.ToString() ?? string.Empty;
                        if (inputField.targetGraphic is Image inputImage)
                            UpdateFieldBackgroundColor(inputImage, currentValue!, field);
                    }
                    break;
                case FieldType.Bool:
                    if (_boolToggles.TryGetValue(uniqueKey, out var toggle))
                    {
                        toggle.isOn = Convert.ToBoolean(currentValue);
                        if (toggle.targetGraphic is Image bg)
                            bg.color = !IsValueEqualToDefault(currentValue, field) ? GetModifiedFieldColor() : UIStyle.Colors.InputBackground;
                    }
                    break;
                case FieldType.Enum:
                    if (_enumDropdowns.TryGetValue(uniqueKey, out var dropdown) &&
                        _enumOptionInts.TryGetValue(uniqueKey, out var ints))
                    {
                        int iv = Convert.ToInt32(currentValue);
                        int idx = Array.IndexOf(ints, iv);
                        if (idx < 0) idx = 0;
                        dropdown.value = idx;
                        if (dropdown.targetGraphic is Image dgb)
                            dgb.color = !IsValueEqualToDefault(currentValue, field) ? GetModifiedFieldColor() : UIStyle.Colors.DropdownBackground;
                    }
                    break;
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
                    switch (field.Type)
                    {
                        case FieldType.Int:
                            return Convert.ChangeType(value, typeof(int));
                        case FieldType.Float:
                            return Convert.ChangeType(value, typeof(float));
                        case FieldType.Bool:
                            return Convert.ToBoolean(value);
                        case FieldType.Enum:
                            return Convert.ToInt32(value);
                    }
                }
                catch
                {
                    // Fall back to default value
                }
            }
        }

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
            switch (field.Type)
            {
                case FieldType.Int:
                    return Convert.ToInt32(value) == Convert.ToInt32(field.DefaultValue);
                case FieldType.Float:
                    return Math.Abs(Convert.ToSingle(value) - Convert.ToSingle(field.DefaultValue)) < 0.001f;
                case FieldType.Bool:
                    return Convert.ToBoolean(value) == Convert.ToBoolean(field.DefaultValue);
                case FieldType.Enum:
                    return Convert.ToInt32(value) == Convert.ToInt32(field.DefaultValue);
            }
        }
        catch
        {
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
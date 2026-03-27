using System;
using UnityEngine;
using UnityEngine.UI;

namespace HazardSpam.Menu;

/// <summary>
/// Helper class for creating common UI elements with consistent styling
/// </summary>
public static class UIBuilder
{
    /// <summary>
    /// Create a panel with background
    /// </summary>
    public static RectTransform CreatePanel(Transform parent, string name, Vector2 size, Vector2 position, Color? backgroundColor = null)
    {
        GameObject panelGo = new GameObject(name);
        panelGo.transform.SetParent(parent, false);
        
        RectTransform panelRT = panelGo.AddComponent<RectTransform>();
        panelRT.anchorMin = UIStyle.Anchors.Center;
        panelRT.anchorMax = UIStyle.Anchors.Center;
        panelRT.pivot = UIStyle.Anchors.Center;
        panelRT.sizeDelta = size;
        panelRT.anchoredPosition = position;
        
        if (backgroundColor.HasValue)
        {
            Image panelImage = panelGo.AddComponent<Image>();
            panelImage.color = backgroundColor.Value;
        }
        
        return panelRT;
    }
    
    /// <summary>
    /// Create a label with text
    /// </summary>
    public static Text CreateLabel(Transform parent, string text, Vector2 position, Vector2 size, bool bold = false, TextAnchor alignment = TextAnchor.MiddleLeft)
    {
        GameObject labelGo = new GameObject("Label");
        labelGo.transform.SetParent(parent, false);
        
        RectTransform labelRT = labelGo.AddComponent<RectTransform>();
        labelRT.anchorMin = UIStyle.Anchors.TopLeft;
        labelRT.anchorMax = UIStyle.Anchors.TopLeft;
        labelRT.pivot = UIStyle.Anchors.TopLeft;
        labelRT.sizeDelta = size;
        labelRT.anchoredPosition = position;
        
        Text labelText = labelGo.AddComponent<Text>();
        labelText.font = UIStyle.DefaultFont;
        labelText.color = UIStyle.Colors.TextDefault;
        labelText.text = text;
        labelText.fontSize = UIStyle.FontSizes.Default;
        labelText.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        labelText.alignment = alignment;
        
        return labelText;
    }
    
    /// <summary>
    /// Create a button with text
    /// </summary>
    public static Button CreateButton(Transform parent, string text, Vector2 position, Vector2 size, Color? backgroundColor = null, Action? onClick = null)
    {
        GameObject buttonGo = new GameObject("Button");
        buttonGo.transform.SetParent(parent, false);
        
        RectTransform buttonRT = buttonGo.AddComponent<RectTransform>();
        buttonRT.anchorMin = UIStyle.Anchors.TopLeft;
        buttonRT.anchorMax = UIStyle.Anchors.TopLeft;
        buttonRT.pivot = UIStyle.Anchors.TopLeft;
        buttonRT.sizeDelta = size;
        buttonRT.anchoredPosition = position;
        
        Button button = buttonGo.AddComponent<Button>();
        Image buttonImage = buttonGo.AddComponent<Image>();
        buttonImage.color = backgroundColor ?? UIStyle.Colors.ButtonDefault;
        button.targetGraphic = buttonImage;
        
        // Create button text
        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(buttonGo.transform, false);
        
        RectTransform textRT = textGo.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;
        
        Text buttonText = textGo.AddComponent<Text>();
        buttonText.font = UIStyle.DefaultFont;
        buttonText.color = UIStyle.Colors.TextDefault;
        buttonText.text = text;
        buttonText.fontSize = UIStyle.FontSizes.Default;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        if (onClick != null)
        {
            button.onClick.AddListener(() => onClick());
        }
        
        return button;
    }
    
    /// <summary>
    /// Create a dropdown with options
    /// </summary>
    public static Dropdown CreateDropdown(Transform parent, string[] options, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction<int>? onValueChanged = null)
    {
        GameObject dropdownGo = new GameObject("Dropdown");
        dropdownGo.transform.SetParent(parent, false);
        
        RectTransform dropdownRT = dropdownGo.AddComponent<RectTransform>();
        dropdownRT.anchorMin = UIStyle.Anchors.TopLeft;
        dropdownRT.anchorMax = UIStyle.Anchors.TopLeft;
        dropdownRT.pivot = UIStyle.Anchors.TopLeft;
        dropdownRT.sizeDelta = size;
        dropdownRT.anchoredPosition = position;
        
        Image dropdownImage = dropdownGo.AddComponent<Image>();
        dropdownImage.color = UIStyle.Colors.DropdownBackground;
        
        Dropdown dropdown = dropdownGo.AddComponent<Dropdown>();
        dropdown.targetGraphic = dropdownImage;
        
        // Create caption text
        GameObject captionGo = new GameObject("Caption");
        captionGo.transform.SetParent(dropdownGo.transform, false);
        
        RectTransform captionRT = captionGo.AddComponent<RectTransform>();
        captionRT.anchorMin = Vector2.zero;
        captionRT.anchorMax = Vector2.one;
        captionRT.sizeDelta = Vector2.zero;
        captionRT.anchoredPosition = Vector2.zero;
        
        Text captionText = captionGo.AddComponent<Text>();
        captionText.font = UIStyle.DefaultFont;
        captionText.color = UIStyle.Colors.TextDefault;
        captionText.fontSize = UIStyle.FontSizes.Default;
        captionText.alignment = TextAnchor.MiddleLeft;
        
        dropdown.captionText = captionText;
        
        // Create template
        GameObject templateGo = new GameObject("Template");
        templateGo.transform.SetParent(dropdownGo.transform, false);
        templateGo.SetActive(false);
        
        RectTransform templateRT = templateGo.AddComponent<RectTransform>();
        templateRT.anchorMin = new Vector2(0, 0);
        templateRT.anchorMax = new Vector2(1, 0);
        templateRT.pivot = new Vector2(0.5f, 1);
        templateRT.sizeDelta = Vector2.zero;
        templateRT.anchoredPosition = new Vector2(0, 2);
        
        Image templateImage = templateGo.AddComponent<Image>();
        templateImage.color = UIStyle.Colors.DropdownTemplateBackground;
        
        // Create item template
        GameObject itemGo = new GameObject("Item");
        itemGo.transform.SetParent(templateGo.transform, false);
        
        RectTransform itemRT = itemGo.AddComponent<RectTransform>();
        itemRT.anchorMin = new Vector2(0, 1);
        itemRT.anchorMax = new Vector2(1, 1);
        itemRT.pivot = new Vector2(0.5f, 1);
        itemRT.sizeDelta = new Vector2(0, 16);
        itemRT.anchoredPosition = Vector2.zero;
        
        Toggle itemToggle = itemGo.AddComponent<Toggle>();
        itemToggle.toggleTransition = Toggle.ToggleTransition.Fade;
        
        // Create item background
        GameObject itemBgGo = new GameObject("Item Background");
        itemBgGo.transform.SetParent(itemGo.transform, false);
        
        RectTransform itemBgRT = itemBgGo.AddComponent<RectTransform>();
        itemBgRT.anchorMin = Vector2.zero;
        itemBgRT.anchorMax = Vector2.one;
        itemBgRT.sizeDelta = Vector2.zero;
        itemBgRT.anchoredPosition = Vector2.zero;
        
        Image itemBgImage = itemBgGo.AddComponent<Image>();
        itemBgImage.color = UIStyle.Colors.DropdownItemBackground;
        
        // Create item label
        GameObject itemLabelGo = new GameObject("Item Label");
        itemLabelGo.transform.SetParent(itemGo.transform, false);
        
        RectTransform itemLabelRT = itemLabelGo.AddComponent<RectTransform>();
        itemLabelRT.anchorMin = Vector2.zero;
        itemLabelRT.anchorMax = Vector2.one;
        itemLabelRT.sizeDelta = Vector2.zero;
        itemLabelRT.anchoredPosition = Vector2.zero;
        
        Text itemLabelText = itemLabelGo.AddComponent<Text>();
        itemLabelText.font = UIStyle.DefaultFont;
        itemLabelText.color = UIStyle.Colors.TextDefault;
        itemLabelText.fontSize = UIStyle.FontSizes.Default;
        itemLabelText.alignment = TextAnchor.MiddleLeft;
        
        // Set up dropdown
        itemToggle.targetGraphic = itemBgImage;
        dropdown.template = templateRT;
        dropdown.captionText = captionText;
        dropdown.itemText = itemLabelText;
        
        // Set options
        dropdown.options.Clear();
        foreach (string option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        dropdown.value = 0;
        
        if (onValueChanged != null)
        {
            dropdown.onValueChanged.AddListener(onValueChanged);
        }
        
        return dropdown;
    }
    
    /// <summary>
    /// Create a numeric input field
    /// </summary>
    public static InputField CreateNumericInput(Transform parent, Vector2 position, Vector2 size, string defaultValue = "0", UnityEngine.Events.UnityAction<string>? onEndEdit = null)
    {
        GameObject inputGo = new GameObject("NumericInput");
        inputGo.transform.SetParent(parent, false);
        
        RectTransform inputRT = inputGo.AddComponent<RectTransform>();
        inputRT.anchorMin = UIStyle.Anchors.TopLeft;
        inputRT.anchorMax = UIStyle.Anchors.TopLeft;
        inputRT.pivot = UIStyle.Anchors.TopLeft;
        inputRT.sizeDelta = size;
        inputRT.anchoredPosition = position;
        
        Image inputImage = inputGo.AddComponent<Image>();
        inputImage.color = UIStyle.Colors.InputBackground;
        
        InputField inputField = inputGo.AddComponent<InputField>();
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.targetGraphic = inputImage;
        
        // Create text component
        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(inputGo.transform, false);
        
        RectTransform textRT = textGo.AddComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.1f, 0);
        textRT.anchorMax = new Vector2(1f, 1);
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;
        
        Text textComponent = textGo.AddComponent<Text>();
        textComponent.font = UIStyle.DefaultFont;
        textComponent.color = UIStyle.Colors.TextDefault;
        textComponent.fontSize = UIStyle.FontSizes.Default;
        textComponent.alignment = TextAnchor.MiddleLeft;
        
        inputField.textComponent = textComponent;
        inputField.text = defaultValue;
        
        // Create placeholder
        GameObject placeholderGo = new GameObject("Placeholder");
        placeholderGo.transform.SetParent(inputGo.transform, false);
        
        RectTransform placeholderRT = placeholderGo.AddComponent<RectTransform>();
        placeholderRT.anchorMin = new Vector2(0.1f, 0);
        placeholderRT.anchorMax = new Vector2(1f, 1);
        placeholderRT.sizeDelta = Vector2.zero;
        placeholderRT.anchoredPosition = Vector2.zero;
        
        Text placeholderText = placeholderGo.AddComponent<Text>();
        placeholderText.font = UIStyle.DefaultFont;
        placeholderText.color = UIStyle.Colors.TextPlaceholder;
        placeholderText.fontSize = UIStyle.FontSizes.Default;
        placeholderText.alignment = TextAnchor.MiddleLeft;
        placeholderText.text = defaultValue;
        
        inputField.placeholder = placeholderText;
        
        if (onEndEdit != null)
        {
            inputField.onEndEdit.AddListener(onEndEdit);
        }
        
        return inputField;
    }
    
    /// <summary>
    /// Create a count indicator with text
    /// </summary>
    public static Text CreateCountIndicator(Transform parent, Vector2 position, Vector2 size, string initialText = "0")
    {
        // Create background
        GameObject countGo = new GameObject("CountIndicator");
        countGo.transform.SetParent(parent, false);
        
        RectTransform countRT = countGo.AddComponent<RectTransform>();
        countRT.anchorMin = UIStyle.Anchors.Right;
        countRT.anchorMax = UIStyle.Anchors.Right;
        countRT.pivot = UIStyle.Anchors.Right;
        countRT.sizeDelta = size;
        countRT.anchoredPosition = position;
        
        Image countImage = countGo.AddComponent<Image>();
        countImage.color = UIStyle.Colors.CountBackground;
        
        // Add outline
        Outline countOutline = countGo.AddComponent<Outline>();
        countOutline.effectColor = UIStyle.Colors.CountOutline;
        countOutline.effectDistance = new Vector2(1f, 1f);
        
        // Create text
        GameObject textGo = new GameObject("CountText");
        textGo.transform.SetParent(countGo.transform, false);
        
        RectTransform textRT = textGo.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;
        
        Text countText = textGo.AddComponent<Text>();
        countText.font = UIStyle.DefaultFont;
        countText.color = UIStyle.Colors.TextDefault;
        countText.text = initialText;
        countText.fontSize = UIStyle.FontSizes.Small;
        countText.alignment = TextAnchor.MiddleCenter;
        
        return countText;
    }
    
    /// <summary>
    /// Create a horizontal layout group
    /// </summary>
    public static HorizontalLayoutGroup CreateHorizontalLayout(Transform parent, float spacing = 10f, RectOffset? padding = null)
    {
        GameObject layoutGo = new GameObject("HorizontalLayout");
        layoutGo.transform.SetParent(parent, false);
        
        RectTransform layoutRT = layoutGo.AddComponent<RectTransform>();
        layoutRT.anchorMin = UIStyle.Anchors.TopLeft;
        layoutRT.anchorMax = UIStyle.Anchors.TopRight;
        layoutRT.pivot = UIStyle.Anchors.TopLeft;
        layoutRT.sizeDelta = Vector2.zero;
        layoutRT.anchoredPosition = Vector2.zero;
        
        HorizontalLayoutGroup layout = layoutGo.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = spacing;
        layout.padding = padding ?? new RectOffset(10, 10, 5, 5);
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        
        return layout;
    }
    
    /// <summary>
    /// Create a vertical layout group
    /// </summary>
    public static VerticalLayoutGroup CreateVerticalLayout(Transform parent, float spacing = 10f, RectOffset? padding = null)
    {
        GameObject layoutGo = new GameObject("VerticalLayout");
        layoutGo.transform.SetParent(parent, false);
        
        RectTransform layoutRT = layoutGo.AddComponent<RectTransform>();
        layoutRT.anchorMin = Vector2.zero;
        layoutRT.anchorMax = Vector2.one;
        layoutRT.pivot = UIStyle.Anchors.Center;
        layoutRT.sizeDelta = Vector2.zero;
        layoutRT.anchoredPosition = Vector2.zero;
        
        VerticalLayoutGroup layout = layoutGo.AddComponent<VerticalLayoutGroup>();
        layout.spacing = spacing;
        layout.padding = padding ?? new RectOffset(10, 10, 10, 10);
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        
        return layout;
    }
}

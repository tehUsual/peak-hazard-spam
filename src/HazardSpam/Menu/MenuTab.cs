using HazardSpam.Menu.TabContent;
using HazardSpam.Menu.Descriptors;
using UnityEngine;
using UnityEngine.UI;

namespace HazardSpam.Menu;

public class MenuTab
{
    public Button Button { get; private set; } = null!;
    private RectTransform Content { get; set; } = null!;
    public string Name { get; private set; }
    public ZoneDescriptor Descriptor { get; private set; } = null!;
    private Text _countText = null!; // Count indicator text

    public MenuTab(ZoneDescriptor descriptor, RectTransform sidebar, RectTransform mainContent, float yOffset, float height)
    {
        Descriptor = descriptor;
        Name = descriptor.DisplayName;
        CreateButton(descriptor.DisplayName, sidebar, yOffset, height);
        CreateContent(mainContent);
    }

    private void CreateButton(string name, RectTransform sidebar, float yOffset, float height)
    {
        GameObject buttonGo = new GameObject(name + "_Button");
        buttonGo.transform.SetParent(sidebar, false);
        
        RectTransform buttonRT = buttonGo.AddComponent<RectTransform>();
        buttonRT.anchorMin = new Vector2(0, 1);
        buttonRT.anchorMax = new Vector2(1, 1);
        buttonRT.pivot = new Vector2(0.5f, 1);
        buttonRT.sizeDelta = new Vector2(-10f, height);
        buttonRT.anchoredPosition = new Vector2(0, yOffset);
        
        Button = buttonGo.AddComponent<Button>();
        var buttonImage = buttonGo.AddComponent<Image>();
        buttonImage.color = UIStyle.Colors.ButtonDefault;
        Button.targetGraphic = buttonImage;

        // Add text
        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(buttonGo.transform, false);
        
        RectTransform textRT = textGo.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;
        
        Text text = textGo.AddComponent<Text>();
        text.font = UIStyle.DefaultFont;
        text.color = UIStyle.Colors.TextDefault;
        text.text = name;
        text.fontSize = UIStyle.FontSizes.Default;
        text.alignment = TextAnchor.MiddleLeft; // Left align to make room for count
        
        // Create count indicator
        GameObject countGo = new GameObject("CountIndicator");
        countGo.transform.SetParent(buttonGo.transform, false);
        
        RectTransform countRT = countGo.AddComponent<RectTransform>();
        countRT.anchorMin = new Vector2(1, 0.5f);
        countRT.anchorMax = new Vector2(1, 0.5f);
        countRT.pivot = new Vector2(1, 0.5f);
        countRT.sizeDelta = new Vector2(20f, 12f);
        countRT.anchoredPosition = new Vector2(-10f, 0f);
        
        Image countImage = countGo.AddComponent<Image>();
        countImage.color = UIStyle.Colors.CountBackground;
        
        // Add black border
        Outline countOutline = countGo.AddComponent<Outline>();
        countOutline.effectColor = UIStyle.Colors.CountOutline;
        countOutline.effectDistance = new Vector2(1f, 1f);
        
        GameObject countTextGo = new GameObject("CountText");
        countTextGo.transform.SetParent(countGo.transform, false);
        
        RectTransform countTextRT = countTextGo.AddComponent<RectTransform>();
        countTextRT.anchorMin = Vector2.zero;
        countTextRT.anchorMax = Vector2.one;
        countTextRT.sizeDelta = Vector2.zero;
        countTextRT.anchoredPosition = Vector2.zero;
        
        _countText = countTextGo.AddComponent<Text>();
        _countText.font = UIStyle.DefaultFont;
        _countText.color = UIStyle.Colors.TextDefault;
        _countText.text = "0";
        _countText.fontSize = UIStyle.FontSizes.Small;
        _countText.alignment = TextAnchor.MiddleCenter;
    }
    
    private void CreateContent(RectTransform mainContent)
    {
        GameObject contentGo = new GameObject(Name + "_Content");
        contentGo.transform.SetParent(mainContent, false);
        Content = contentGo.AddComponent<RectTransform>();
        
        Content.anchorMin = Vector2.zero;
        Content.anchorMax = Vector2.one;
        Content.sizeDelta = Vector2.zero;
        Content.anchoredPosition = Vector2.zero;
        
        // Add header
        CreateHeader();
        
        // Add specific content based on zone
        CreateZoneContent();
        
        Content.gameObject.SetActive(false);
    }
    
    private void CreateHeader()
    {
        GameObject headerGo = new GameObject("Header");
        headerGo.transform.SetParent(Content, false);
        
        RectTransform headerRT = headerGo.AddComponent<RectTransform>();
        headerRT.anchorMin = new Vector2(0, 1);
        headerRT.anchorMax = new Vector2(1, 1);
        headerRT.pivot = new Vector2(0.5f, 1);
        headerRT.sizeDelta = new Vector2(-20f, 30f);
        headerRT.anchoredPosition = new Vector2(0, -10f);
        
        Text headerText = headerGo.AddComponent<Text>();
        headerText.font = UIStyle.DefaultFont;
        headerText.color = UIStyle.Colors.TextDefault;
        headerText.text = $"--- {Name} Configuration ---";
        headerText.fontSize = UIStyle.FontSizes.Header;
        headerText.fontStyle = FontStyle.Bold;
        headerText.alignment = TextAnchor.MiddleCenter;
    }
    
    private void CreateZoneContent()
    {
        // Create content using descriptor instead of hardcoded switch
        // For now, we'll use a generic content builder that works with any zone
        new GenericTabContent(Content, Descriptor);
    }
    
    public void SetActive(bool active)
    {
        Content.gameObject.SetActive(active);
        
        // Update button appearance
        if (Button != null)
        {
            var buttonImage = Button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = active ? 
                    UIStyle.Colors.ButtonActive : 
                    UIStyle.Colors.ButtonDefault;
            }
        }
    }
    
    public void UpdateCount(int count)
    {
        if (_countText != null)
        {
            _countText.text = count.ToString();
        }
    }
}

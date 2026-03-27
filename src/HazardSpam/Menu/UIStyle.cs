using UnityEngine;

namespace HazardSpam.Menu;

/// <summary>
/// Centralized UI styling and layout constants
/// </summary>
public static class UIStyle
{
    // Fonts
    public static Font DefaultFont => Resources.GetBuiltinResource<Font>("Arial.ttf");
    
    // Colors
    public static class Colors
    {
        // Background colors
        public static readonly Color CanvasBackground = new(0.15f, 0.15f, 0.2f, 0.95f);
        public static readonly Color SidebarBackground = new(0.1f, 0.1f, 0.15f, 0.98f);
        public static readonly Color MainContentBackground = new(0.23f, 0.35f, 0.45f, 0.95f);
        
        // Button colors
        public static readonly Color ButtonDefault = new(0.3f, 0.3f, 0.35f, 1f);
        public static readonly Color ButtonActive = new(0.2f, 0.7f, 0.6f, 1f);
        public static readonly Color ButtonTabActive = new(0.2f, 0.4f, 0.6f, 0.9f);
        public static readonly Color ButtonTabDefault = new(0.3f, 0.3f, 0.3f, 0.8f);
        public static readonly Color ButtonAdd = new(0.2f, 0.6f, 0.2f, 0.8f);
        public static readonly Color ButtonDelete = new(0.3f, 0.3f, 0.3f, 0.8f);
        
        // Input colors
        public static readonly Color InputBackground = new(0.2f, 0.2f, 0.2f, 0.8f);
        public static readonly Color DropdownBackground = new(0.2f, 0.2f, 0.2f, 0.8f);
        public static readonly Color DropdownItemBackground = new(0.3f, 0.3f, 0.3f, 0.8f);
        public static readonly Color DropdownTemplateBackground = new(0.2f, 0.2f, 0.2f, 0.95f);
        
        // Text colors
        public static readonly Color TextDefault = Color.white;
        public static readonly Color TextPlaceholder = new(0.5f, 0.5f, 0.5f, 1f);
        
        // Count indicator colors
        public static readonly Color CountBackground = new(0.1f, 0.1f, 0.1f, 0.9f);
        public static readonly Color CountOutline = Color.black;
    }
    
    // Layout constants
    public static class Layout
    {
        // Canvas sizing
        public const float CanvasScaleFactor = 0.75f;
        public const int CanvasSortOrder = 1000;
        
        // Panel sizing
        public const float SidebarWidth = 180f;
        public const float PanelPadding = 20f;
        public const float SidebarPadding = 10f;
        
        // Tab sizing
        public const float TabHeight = 35f;
        public const float TabSpacing = 40f;
        public const float SubZoneTabHeight = 30f;
        public const float SubZoneTabSpacing = 10f;
        
        // Row sizing
        public const float HazardRowHeight = 25f;
        public const float HazardRowSpacing = 30f;
        public const float HeaderHeight = 30f;
        
        // Button sizing
        public const float AddButtonWidth = 120f;
        public const float DeleteButtonWidth = 20f;
        public const float DeleteButtonHeight = 18f;
        public const float CountIndicatorWidth = 20f;
        public const float CountIndicatorHeight = 12f;
        
        // Input sizing
        public const float DropdownWidth = 150f;
        public const float InputWidth = 80f;
        public const float LabelWidth = 100f;
        public const float TypeLabelWidth = 50f;
        public const float AmountLabelWidth = 60f;
    }
    
    // Font sizes
    public static class FontSizes
    {
        public const int Title = 18;
        public const int Header = 16;
        public const int Default = 12;
        public const int Small = 10;
    }
    
    // Anchors and pivots
    public static class Anchors
    {
        public static readonly Vector2 Center = new(0.5f, 0.5f);
        public static readonly Vector2 TopLeft = new(0f, 1f);
        public static readonly Vector2 TopRight = new(1f, 1f);
        public static readonly Vector2 BottomLeft = new(0f, 0f);
        public static readonly Vector2 BottomRight = new(1f, 0f);
        public static readonly Vector2 Left = new(0f, 0.5f);
        public static readonly Vector2 Right = new(1f, 0.5f);
    }
    
    // Common positions
    public static class Positions
    {
        public static readonly Vector2 TitleOffset = new(0f, -10f);
        public static readonly Vector2 TabOffset = new(0f, -50f);
        public static readonly Vector2 ContentOffset = new(0f, -50f);
        public static readonly Vector2 HazardStartOffset = new(0f, -10f);
        public static readonly Vector2 CountOffset = new(-10f, 0f);
        public static readonly Vector2 DeleteOffset = new(-10f, 0f);
    }
    
    // Spacing
    public static class Spacing
    {
        public const float Small = 5f;
        public const float Medium = 10f;
        public const float Large = 20f;
        public const float ExtraLarge = 40f;
    }
}

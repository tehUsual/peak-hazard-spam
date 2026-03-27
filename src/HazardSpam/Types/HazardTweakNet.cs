namespace HazardSpam.Types;

public readonly struct HazardTweakNet(HazardType type, string field, object value)
{
    public readonly HazardType Type = type;
    public readonly string Field = field;
    public readonly object Value = value;
    
    public void Deconstruct(out HazardType type, out string field, out object value)
    {
        type = Type;
        field = Field;
        value = Value;
    }

    public string GenID()
    {
        return $"{Type.ToString()}/{Field}";
    }
}
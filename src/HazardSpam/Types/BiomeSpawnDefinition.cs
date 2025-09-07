using HazardSpam.Spawning;

namespace HazardSpam.Types;

public readonly struct BiomeSpawnDefinition(Biome.BiomeType biomeType, string path, BiomeArea area, string searchName, SpawnType spawnType)
{
    public Biome.BiomeType BiomeType { get; } = biomeType;
    public BiomeArea Area { get; } = area;
    
    public string Path { get; } = path;

    public string SearchName { get; } = searchName;
    public SpawnType SpawnType { get; } = spawnType;
}

# Changelog
**2.0.6**  
Fix crash if the mod PEAKERRpcInfo was present. Thanks to *sappykun* for a fix.

**2.0.5**
Support for 1.60.d  
Fixed bug where menu would not show up.

**2.0.4**
Quick support for PEAK version 1.60.a

**2.0.3**
Smaller readme changes

**2.0.0**
Big update, system rework, new features, more stable
- In-game menu to configure hazard type and rate spawns
- In-game menu to tweak hazards
- Support for 'Roots' biome. (partially tested)
- Support for 'The Kiln' biome.
- Implemented my own NetGameState module to better handle networking/event handling

**1.3.0**
- Feature: Added jelly spawn to caldera.
- Change: Caldera, set fire spawn rate to default. Can be changed in config.
- Change: Changed default spawn rate for mesa.
- Remove: Temporarily removed lava pool random speed due to desync.
- Fix: Reduced lag in caldera (due to increased fire spawn).

**1.2.0**
- Feature: Added a config file. Set custom spawn rates.

**1.1.1**
- Fixed: Plugin stopped working if mesa was selected.

**1.1.0**
- Fixed: Sending spawn info too early to clients when loading the beach. Wait a few seconds for the hazards to spawn.

**1.0.0**
Initial commit


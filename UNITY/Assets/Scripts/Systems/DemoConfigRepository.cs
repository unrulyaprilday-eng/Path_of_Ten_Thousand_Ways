using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PathOfTenThousandWays.Demo.Common;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;

namespace PathOfTenThousandWays.Demo.Systems
{
    public static class DemoConfigRepository
    {
        private static readonly object SyncRoot = new object();
        private static bool loadAttempted;
        private static DemoGameConfig config;
        private static Dictionary<string, DemoSystemConstantEntry> constantsByKey = new Dictionary<string, DemoSystemConstantEntry>();
        private static Dictionary<string, DemoStyleConfig> stylesById = new Dictionary<string, DemoStyleConfig>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, DemoCard> cardsById = new Dictionary<string, DemoCard>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, List<DemoCardPoolEntryConfig>> cardPoolsById = new Dictionary<string, List<DemoCardPoolEntryConfig>>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, DemoRootDefinition> rootsById = new Dictionary<string, DemoRootDefinition>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, DemoRegionDefinition> regionsById = new Dictionary<string, DemoRegionDefinition>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, DemoJourneyLineDefinition> journeyLinesById = new Dictionary<string, DemoJourneyLineDefinition>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<DemoGongfaType, DemoGongfaDefinition> gongfasByType = new Dictionary<DemoGongfaType, DemoGongfaDefinition>();
        private static Dictionary<DemoGongfaSlot, List<DemoGongfaType>> gongfasBySlot = new Dictionary<DemoGongfaSlot, List<DemoGongfaType>>();
        private static Dictionary<DemoArtifactType, DemoArtifactDefinition> artifactsByType = new Dictionary<DemoArtifactType, DemoArtifactDefinition>();
        private static Dictionary<string, DemoRelicDefinition> relicsByName = new Dictionary<string, DemoRelicDefinition>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, DemoRoutePlanDefinition> routePlansById = new Dictionary<string, DemoRoutePlanDefinition>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, DemoEnemyDefinition> enemiesByName = new Dictionary<string, DemoEnemyDefinition>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, Dictionary<string, List<DemoRewardPriorityEntryConfig>>> rewardPriorities
            = new Dictionary<string, Dictionary<string, List<DemoRewardPriorityEntryConfig>>>(StringComparer.OrdinalIgnoreCase);

        public static bool HasLoadedConfig
        {
            get
            {
                EnsureLoaded();
                return config != null;
            }
        }

        public static bool TryCreateCard(string id, out DemoCard card)
        {
            EnsureLoaded();

            if (!string.IsNullOrEmpty(id) && cardsById.TryGetValue(id, out DemoCard template))
            {
                card = template.Clone();
                return true;
            }

            card = null;
            return false;
        }

        public static List<DemoRootDefinition> GetDefaultRoots(int maxCount)
        {
            EnsureLoaded();

            List<DemoRootDefinition> roots = rootsById.Values
                .Where(root => root.IsDefaultPool)
                .OrderBy(root => root.Id)
                .Take(Math.Max(0, maxCount))
                .Select(Clone)
                .ToList();

            return roots;
        }

        public static List<DemoJourneyLineDefinition> GetJourneyLinesForRoot(string rootId, int maxCount)
        {
            EnsureLoaded();

            List<DemoJourneyLineDefinition> lines = journeyLinesById.Values
                .Where(line => string.Equals(line.RootId, rootId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(line => line.Id)
                .Take(Math.Max(0, maxCount))
                .Select(Clone)
                .ToList();

            return lines;
        }

        public static bool TryGetRegion(string regionId, out DemoRegionDefinition region)
        {
            EnsureLoaded();

            if (!string.IsNullOrEmpty(regionId) && regionsById.TryGetValue(regionId, out DemoRegionDefinition cached))
            {
                region = Clone(cached);
                return true;
            }

            region = null;
            return false;
        }

        public static bool TryCreateDeckFromPool(string poolId, out List<DemoCard> deck)
        {
            EnsureLoaded();

            if (!string.IsNullOrEmpty(poolId) && cardPoolsById.TryGetValue(poolId, out List<DemoCardPoolEntryConfig> entries))
            {
                deck = new List<DemoCard>();

                for (int i = 0; i < entries.Count; i++)
                {
                    DemoCardPoolEntryConfig entry = entries[i];
                    if (!string.Equals(entry.EntryType, "card", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    for (int count = 0; count < Math.Max(1, entry.Count); count++)
                    {
                        if (TryCreateCard(entry.RefId, out DemoCard card))
                        {
                            deck.Add(card);
                        }
                    }
                }

                return deck.Count > 0;
            }

            deck = null;
            return false;
        }

        public static bool TryGetBasicPathCardId(DemoSwordStyle style, out string cardId)
        {
            EnsureLoaded();
            string styleKey = GetStyleKey(style);

            if (stylesById.TryGetValue(styleKey, out DemoStyleConfig styleConfig) && !string.IsNullOrEmpty(styleConfig.BasicPathCardId))
            {
                cardId = styleConfig.BasicPathCardId;
                return true;
            }

            cardId = null;
            return false;
        }

        public static bool TryGetGongfa(DemoGongfaType type, out DemoGongfaDefinition definition)
        {
            EnsureLoaded();

            if (gongfasByType.TryGetValue(type, out DemoGongfaDefinition cached))
            {
                definition = Clone(cached);
                return true;
            }

            definition = null;
            return false;
        }

        public static List<DemoGongfaType> GetGongfaTypesForSlot(DemoGongfaSlot slot)
        {
            EnsureLoaded();

            if (gongfasBySlot.TryGetValue(slot, out List<DemoGongfaType> types))
            {
                return new List<DemoGongfaType>(types);
            }

            return new List<DemoGongfaType>();
        }

        public static bool TryGetArtifact(DemoArtifactType type, out DemoArtifactDefinition definition)
        {
            EnsureLoaded();

            if (artifactsByType.TryGetValue(type, out DemoArtifactDefinition cached))
            {
                definition = Clone(cached);
                return true;
            }

            definition = null;
            return false;
        }

        public static bool TryGetRelic(string relicName, out DemoRelicDefinition definition)
        {
            EnsureLoaded();

            if (!string.IsNullOrEmpty(relicName) && relicsByName.TryGetValue(relicName, out DemoRelicDefinition cached))
            {
                definition = Clone(cached);
                return true;
            }

            definition = null;
            return false;
        }

        public static bool TryGetRoutePlan(string routePlanId, out DemoRoutePlanDefinition routePlan)
        {
            EnsureLoaded();

            if (!string.IsNullOrEmpty(routePlanId) && routePlansById.TryGetValue(routePlanId, out DemoRoutePlanDefinition cached))
            {
                routePlan = Clone(cached);
                return true;
            }

            routePlan = null;
            return false;
        }

        public static bool TryGetEnemyByName(string enemyName, out DemoEnemyDefinition enemy)
        {
            EnsureLoaded();

            if (!string.IsNullOrEmpty(enemyName) && enemiesByName.TryGetValue(enemyName, out DemoEnemyDefinition cached))
            {
                enemy = Clone(cached);
                return true;
            }

            enemy = null;
            return false;
        }

        public static IReadOnlyList<string> GetRewardPriorityRefs(string service, DemoSwordStyle style, string refType)
        {
            EnsureLoaded();

            if (!rewardPriorities.TryGetValue(service ?? string.Empty, out Dictionary<string, List<DemoRewardPriorityEntryConfig>> serviceConfig))
            {
                return Array.Empty<string>();
            }

            string styleKey = GetStyleKey(style);
            if (!serviceConfig.TryGetValue(styleKey, out List<DemoRewardPriorityEntryConfig> entries) &&
                !serviceConfig.TryGetValue(GetStyleKey(DemoSwordStyle.General), out entries))
            {
                return Array.Empty<string>();
            }

            List<string> refs = entries
                .Where(entry => string.Equals(entry.RefType, refType, StringComparison.OrdinalIgnoreCase))
                .OrderBy(entry => entry.Seq)
                .Select(entry => entry.RefId)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            return refs;
        }

        public static int GetIntConstant(string group, string key, int fallback)
        {
            EnsureLoaded();

            if (TryGetConstant(group, key, out DemoSystemConstantEntry entry))
            {
                try
                {
                    if (TryConvertInt(entry.Value, out int parsed))
                    {
                        return parsed;
                    }
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static float GetFloatConstant(string group, string key, float fallback)
        {
            EnsureLoaded();

            if (TryGetConstant(group, key, out DemoSystemConstantEntry entry))
            {
                try
                {
                    if (TryConvertFloat(entry.Value, out float parsed))
                    {
                        return parsed;
                    }
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        private static void EnsureLoaded()
        {
            if (loadAttempted)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (loadAttempted)
                {
                    return;
                }

                loadAttempted = true;
                config = LoadConfig();

                if (config == null)
                {
                    return;
                }

                BuildCaches(config);
            }
        }

        private static DemoGameConfig LoadConfig()
        {
            string[] candidatePaths =
            {
                Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Data", "JSON", "game_config.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "UNITY", "Assets", "Data", "JSON", "game_config.json")
            };

            for (int i = 0; i < candidatePaths.Length; i++)
            {
                string path = candidatePaths[i];
                if (!File.Exists(path))
                {
                    continue;
                }

                string json = File.ReadAllText(path, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(json))
                {
                    continue;
                }

                DemoGameConfig loaded = DeserializeConfig(json);
                if (loaded != null)
                {
                    return loaded;
                }
            }

            return null;
        }

        private static void BuildCaches(DemoGameConfig loadedConfig)
        {
            constantsByKey = new Dictionary<string, DemoSystemConstantEntry>(StringComparer.OrdinalIgnoreCase);
            stylesById = new Dictionary<string, DemoStyleConfig>(StringComparer.OrdinalIgnoreCase);
            cardsById = new Dictionary<string, DemoCard>(StringComparer.OrdinalIgnoreCase);
            cardPoolsById = new Dictionary<string, List<DemoCardPoolEntryConfig>>(StringComparer.OrdinalIgnoreCase);
            rootsById = new Dictionary<string, DemoRootDefinition>(StringComparer.OrdinalIgnoreCase);
            regionsById = new Dictionary<string, DemoRegionDefinition>(StringComparer.OrdinalIgnoreCase);
            journeyLinesById = new Dictionary<string, DemoJourneyLineDefinition>(StringComparer.OrdinalIgnoreCase);
            gongfasByType = new Dictionary<DemoGongfaType, DemoGongfaDefinition>();
            gongfasBySlot = new Dictionary<DemoGongfaSlot, List<DemoGongfaType>>();
            artifactsByType = new Dictionary<DemoArtifactType, DemoArtifactDefinition>();
            relicsByName = new Dictionary<string, DemoRelicDefinition>(StringComparer.OrdinalIgnoreCase);
            routePlansById = new Dictionary<string, DemoRoutePlanDefinition>(StringComparer.OrdinalIgnoreCase);
            enemiesByName = new Dictionary<string, DemoEnemyDefinition>(StringComparer.OrdinalIgnoreCase);
            rewardPriorities = new Dictionary<string, Dictionary<string, List<DemoRewardPriorityEntryConfig>>>(StringComparer.OrdinalIgnoreCase);

            if (loadedConfig.SystemConstants != null)
            {
                for (int i = 0; i < loadedConfig.SystemConstants.Count; i++)
                {
                    DemoSystemConstantEntry entry = loadedConfig.SystemConstants[i];
                    if (entry == null || string.IsNullOrEmpty(entry.Group) || string.IsNullOrEmpty(entry.Key))
                    {
                        continue;
                    }

                    constantsByKey[GetConstantKey(entry.Group, entry.Key)] = entry;
                }
            }

            if (loadedConfig.Styles != null)
            {
                for (int i = 0; i < loadedConfig.Styles.Count; i++)
                {
                    DemoStyleConfig style = loadedConfig.Styles[i];
                    if (style == null || string.IsNullOrEmpty(style.StyleId))
                    {
                        continue;
                    }

                    stylesById[style.StyleId] = style;
                }
            }

            DemoOpeningConfig opening = loadedConfig.Opening;
            if (opening != null)
            {
                foreach (KeyValuePair<string, DemoRootConfig> pair in opening.Roots)
                {
                    DemoRootConfig root = pair.Value;
                    if (root == null)
                    {
                        continue;
                    }

                    rootsById[pair.Key] = new DemoRootDefinition
                    {
                        Id = root.Id,
                        Name = root.Name,
                        Rarity = root.Rarity,
                        UnlockCondition = root.UnlockCondition,
                        IsDefaultPool = root.IsDefaultPool,
                        Summary = root.Summary
                    };
                }

                foreach (KeyValuePair<string, DemoRegionConfig> pair in opening.Regions)
                {
                    DemoRegionConfig region = pair.Value;
                    if (region == null)
                    {
                        continue;
                    }

                    DemoRegionDefinition definition = new DemoRegionDefinition
                    {
                        Id = region.Id,
                        Name = region.Name,
                        RewardFocus = region.RewardFocus,
                        Description = region.Description
                    };

                    if (region.NodeWeights != null)
                    {
                        for (int i = 0; i < region.NodeWeights.Count; i++)
                        {
                            DemoRegionNodeWeightConfig weight = region.NodeWeights[i];
                            if (weight == null || string.IsNullOrEmpty(weight.NodeType))
                            {
                                continue;
                            }

                            definition.NodeWeights[weight.NodeType] = weight.Weight;
                        }
                    }

                    regionsById[pair.Key] = definition;
                }

                foreach (KeyValuePair<string, DemoJourneyLineConfig> pair in opening.JourneyLines)
                {
                    DemoJourneyLineConfig line = pair.Value;
                    if (line == null)
                    {
                        continue;
                    }

                    journeyLinesById[pair.Key] = new DemoJourneyLineDefinition
                    {
                        Id = line.Id,
                        RootId = line.RootId,
                        Title = line.Title,
                        OriginText = line.OriginText,
                        CarryItemName = line.CarryItemName,
                        CarryItemEffect = line.CarryItemEffect,
                        FirstRegionId = line.FirstRegionId,
                        RiskLevel = line.RiskLevel,
                        SummaryTags = line.SummaryTags ?? new List<string>(),
                        NodeBiases = line.NodeBiases ?? new List<DemoJourneyNodeBiasConfig>(),
                        RewardBiases = line.RewardBiases ?? new List<DemoJourneyRewardBiasConfig>()
                    };
                }
            }

            DemoRuntimeConfig runtime = loadedConfig.Demo;
            if (runtime == null)
            {
                return;
            }

            foreach (KeyValuePair<string, DemoCardConfig> pair in runtime.Cards)
            {
                if (pair.Value == null)
                {
                    continue;
                }

                cardsById[pair.Key] = new DemoCard
                {
                    Id = pair.Value.Id,
                    Name = pair.Value.Name,
                    IconGlyph = pair.Value.IconGlyph,
                    Type = ParseEnum(pair.Value.Type, DemoCardType.Attack),
                    Style = ParseEnum(pair.Value.Style, DemoSwordStyle.General),
                    Quality = ParseEnum(pair.Value.Quality, DemoQuality.Mortal),
                    Cost = pair.Value.Cost,
                    Damage = pair.Value.Damage,
                    Block = pair.Value.Block,
                    Draw = pair.Value.Draw,
                    EnergyGain = pair.Value.EnergyGain,
                    SwordIntent = pair.Value.SwordIntent,
                    Shock = pair.Value.Shock,
                    Bleed = pair.Value.Bleed,
                    TemporarySwords = pair.Value.TemporarySwords,
                    PermanentSword = pair.Value.PermanentSword,
                    ConsumeAllSwordIntent = pair.Value.ConsumeAllSwordIntent,
                    SelfDamage = pair.Value.SelfDamage,
                    SpecialEffect = ParseEnum(pair.Value.SpecialEffect, DemoCardSpecialEffect.None),
                    RulesOverride = pair.Value.RulesOverride
                };
            }

            foreach (KeyValuePair<string, List<DemoCardPoolEntryConfig>> pair in runtime.CardPools)
            {
                if (pair.Value == null)
                {
                    continue;
                }

                cardPoolsById[pair.Key] = pair.Value.ToList();
            }

            foreach (KeyValuePair<string, DemoGongfaConfig> pair in runtime.Gongfas)
            {
                DemoGongfaConfig definition = pair.Value;
                if (definition == null || !TryParseEnum(definition.RuntimeEnum, out DemoGongfaType gongfaType))
                {
                    continue;
                }

                DemoGongfaDefinition built = new DemoGongfaDefinition
                {
                    Type = gongfaType,
                    Slot = ParseEnum(definition.Slot, DemoGongfaSlot.Main),
                    Style = ParseEnum(definition.Style, DemoSwordStyle.General),
                    Name = definition.Name,
                    IconGlyph = definition.IconGlyph,
                    Title = definition.Title,
                    Description = definition.Description,
                    Quality = ParseEnum(definition.Quality, DemoQuality.Mortal)
                };

                gongfasByType[gongfaType] = built;

                if (!gongfasBySlot.TryGetValue(built.Slot, out List<DemoGongfaType> slotTypes))
                {
                    slotTypes = new List<DemoGongfaType>();
                    gongfasBySlot[built.Slot] = slotTypes;
                }

                slotTypes.Add(gongfaType);
            }

            foreach (KeyValuePair<string, DemoArtifactConfig> pair in runtime.Artifacts)
            {
                DemoArtifactConfig definition = pair.Value;
                if (definition == null || !TryParseEnum(definition.RuntimeEnum, out DemoArtifactType artifactType))
                {
                    continue;
                }

                artifactsByType[artifactType] = new DemoArtifactDefinition
                {
                    Type = artifactType,
                    Name = definition.Name,
                    IconGlyph = definition.IconGlyph,
                    Style = definition.Style,
                    Quality = ParseEnum(definition.Quality, DemoQuality.Mortal),
                    Description = definition.Description
                };
            }

            foreach (KeyValuePair<string, DemoRelicConfig> pair in runtime.Relics)
            {
                DemoRelicConfig definition = pair.Value;
                if (definition == null || string.IsNullOrEmpty(definition.Name))
                {
                    continue;
                }

                relicsByName[definition.Name] = new DemoRelicDefinition
                {
                    Name = definition.Name,
                    IconGlyph = definition.IconGlyph,
                    Style = definition.Style,
                    Quality = ParseEnum(definition.Quality, DemoQuality.Mortal),
                    Description = definition.Description
                };
            }

            foreach (KeyValuePair<string, DemoRoutePlanConfig> pair in runtime.RoutePlans)
            {
                DemoRoutePlanConfig routeConfig = pair.Value;
                if (routeConfig == null)
                {
                    continue;
                }

                DemoMapRoutePlan plan = new DemoMapRoutePlan(
                    routeConfig.Name,
                    routeConfig.Description,
                    routeConfig.Nodes
                        .OrderBy(node => node.Seq)
                        .Select(node => new DemoMapNode(
                            node.Layer,
                            ParseEnum(node.NodeType, DemoNodeType.RouteChoice),
                            node.NodeName))
                        .ToArray());

                routePlansById[pair.Key] = new DemoRoutePlanDefinition
                {
                    Id = routeConfig.Id,
                    Plan = plan,
                    RouteStyle = ParseEnum(routeConfig.RouteStyle, DemoSwordStyle.General),
                    RouteQuality = ParseEnum(routeConfig.RouteQuality, DemoQuality.Mortal),
                    RouteGlyph = routeConfig.RouteGlyph,
                    RouteTag = routeConfig.RouteTag
                };
            }

            foreach (KeyValuePair<string, DemoEnemyConfig> pair in runtime.Enemies)
            {
                DemoEnemyConfig enemyConfig = pair.Value;
                if (enemyConfig == null || string.IsNullOrEmpty(enemyConfig.Name))
                {
                    continue;
                }

                enemiesByName[enemyConfig.Name] = new DemoEnemyDefinition
                {
                    Id = enemyConfig.Id,
                    Name = enemyConfig.Name,
                    BattleRole = enemyConfig.BattleRole,
                    IsBoss = enemyConfig.IsBoss,
                    MaxHealth = enemyConfig.MaxHealth,
                    BaseDamageProfile = enemyConfig.BaseDamageProfile,
                    Notes = enemyConfig.Notes,
                    BossPhases = enemyConfig.BossPhases ?? new List<DemoBossPhaseConfig>()
                };
            }

            foreach (KeyValuePair<string, Dictionary<string, List<DemoRewardPriorityEntryConfig>>> service in runtime.RewardPriorities)
            {
                if (service.Value == null)
                {
                    continue;
                }

                rewardPriorities[service.Key] = new Dictionary<string, List<DemoRewardPriorityEntryConfig>>(service.Value, StringComparer.OrdinalIgnoreCase);
            }
        }

        private static DemoGameConfig DeserializeConfig(string json)
        {
            if (!(SimpleJsonParser.Parse(json) is Dictionary<string, object> root))
            {
                return null;
            }

            return new DemoGameConfig
            {
                SystemConstants = ReadObjectList(root, "systemConstants", ParseSystemConstantEntry),
                Styles = ReadObjectList(root, "styles", ParseStyleConfig),
                Opening = ParseOpeningConfig(GetObject(root, "opening")),
                Demo = ParseRuntimeConfig(GetObject(root, "demo"))
            };
        }

        private static DemoOpeningConfig ParseOpeningConfig(Dictionary<string, object> raw)
        {
            DemoOpeningConfig opening = new DemoOpeningConfig();
            if (raw == null)
            {
                return opening;
            }

            opening.Roots = ReadObjectMap(raw, "roots", ParseRootConfig);
            opening.Regions = ReadObjectMap(raw, "regions", ParseRegionConfig);
            opening.JourneyLines = ReadObjectMap(raw, "journeyLines", ParseJourneyLineConfig);
            return opening;
        }

        private static DemoRuntimeConfig ParseRuntimeConfig(Dictionary<string, object> raw)
        {
            DemoRuntimeConfig runtime = new DemoRuntimeConfig();
            if (raw == null)
            {
                return runtime;
            }

            runtime.Cards = ReadObjectMap(raw, "cards", ParseCardConfig);
            runtime.CardPools = ReadObjectMapOfLists(raw, "cardPools", ParseCardPoolEntryConfig);
            runtime.Gongfas = ReadObjectMap(raw, "gongfas", ParseGongfaConfig);
            runtime.Artifacts = ReadObjectMap(raw, "artifacts", ParseArtifactConfig);
            runtime.Relics = ReadObjectMap(raw, "relics", ParseRelicConfig);
            runtime.RoutePlans = ReadObjectMap(raw, "routePlans", ParseRoutePlanConfig);
            runtime.Enemies = ReadObjectMap(raw, "enemies", ParseEnemyConfig);
            runtime.RewardPriorities = ParseRewardPriorities(GetObject(raw, "rewardPriorities"));
            return runtime;
        }

        private static DemoSystemConstantEntry ParseSystemConstantEntry(Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoSystemConstantEntry
            {
                Group = GetString(raw, "group"),
                Key = GetString(raw, "key"),
                Value = GetValue(raw, "value"),
                ValueType = GetString(raw, "valueType"),
                Notes = GetString(raw, "notes")
            };
        }

        private static DemoStyleConfig ParseStyleConfig(Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoStyleConfig
            {
                StyleId = GetString(raw, "styleId"),
                Name = GetString(raw, "name"),
                FocusText = GetString(raw, "focusText"),
                BasicPathCardId = GetString(raw, "basicPathCardId"),
                ThemeTags = GetString(raw, "themeTags")
            };
        }

        private static DemoRootConfig ParseRootConfig(string key, Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoRootConfig
            {
                Id = GetString(raw, "id", key),
                Name = GetString(raw, "name"),
                Rarity = GetString(raw, "rarity"),
                UnlockCondition = GetString(raw, "unlockCondition"),
                IsDefaultPool = GetBool(raw, "isDefaultPool"),
                Summary = GetString(raw, "summary")
            };
        }

        private static DemoRegionConfig ParseRegionConfig(string key, Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoRegionConfig
            {
                Id = GetString(raw, "id", key),
                Name = GetString(raw, "name"),
                RewardFocus = GetString(raw, "rewardFocus"),
                Description = GetString(raw, "description"),
                NodeWeights = ReadObjectList(raw, "nodeWeights", ParseRegionNodeWeightConfig)
            };
        }

        private static DemoRegionNodeWeightConfig ParseRegionNodeWeightConfig(Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoRegionNodeWeightConfig
            {
                NodeType = GetString(raw, "nodeType"),
                Weight = GetInt(raw, "weight")
            };
        }

        private static DemoJourneyLineConfig ParseJourneyLineConfig(string key, Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoJourneyLineConfig
            {
                Id = GetString(raw, "id", key),
                RootId = GetString(raw, "rootId"),
                Title = GetString(raw, "title"),
                OriginText = GetString(raw, "originText"),
                CarryItemName = GetString(raw, "carryItemName"),
                CarryItemEffect = GetString(raw, "carryItemEffect"),
                FirstRegionId = GetString(raw, "firstRegionId"),
                RiskLevel = GetString(raw, "riskLevel"),
                SummaryTags = ReadStringList(GetArray(raw, "summaryTags")),
                NodeBiases = ReadObjectList(raw, "nodeBiases", ParseJourneyNodeBiasConfig),
                RewardBiases = ReadObjectList(raw, "rewardBiases", ParseJourneyRewardBiasConfig)
            };
        }

        private static DemoJourneyNodeBiasConfig ParseJourneyNodeBiasConfig(Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoJourneyNodeBiasConfig
            {
                NodeType = GetString(raw, "nodeType"),
                DeltaPercent = GetInt(raw, "deltaPercent")
            };
        }

        private static DemoJourneyRewardBiasConfig ParseJourneyRewardBiasConfig(Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoJourneyRewardBiasConfig
            {
                TagId = GetString(raw, "tagId"),
                DeltaPercent = GetInt(raw, "deltaPercent"),
                Priority = GetString(raw, "priority")
            };
        }

        private static DemoCardConfig ParseCardConfig(string key, Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoCardConfig
            {
                Id = GetString(raw, "id", key),
                Name = GetString(raw, "name"),
                IconGlyph = GetString(raw, "iconGlyph"),
                Type = GetString(raw, "type"),
                Style = GetString(raw, "style"),
                Quality = GetString(raw, "quality"),
                Cost = GetInt(raw, "cost"),
                Damage = GetInt(raw, "damage"),
                Block = GetInt(raw, "block"),
                Draw = GetInt(raw, "draw"),
                EnergyGain = GetInt(raw, "energyGain"),
                SwordIntent = GetInt(raw, "swordIntent"),
                Shock = GetInt(raw, "shock"),
                Bleed = GetInt(raw, "bleed"),
                TemporarySwords = GetInt(raw, "temporarySwords"),
                PermanentSword = GetBool(raw, "permanentSword"),
                ConsumeAllSwordIntent = GetBool(raw, "consumeAllSwordIntent"),
                SelfDamage = GetInt(raw, "selfDamage"),
                SpecialEffect = GetString(raw, "specialEffect"),
                RulesOverride = GetString(raw, "rulesOverride")
            };
        }

        private static DemoCardPoolEntryConfig ParseCardPoolEntryConfig(Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoCardPoolEntryConfig
            {
                EntryType = GetString(raw, "entryType"),
                RefId = GetString(raw, "refId"),
                Count = GetInt(raw, "count", 1),
                Notes = GetString(raw, "notes")
            };
        }

        private static DemoGongfaConfig ParseGongfaConfig(string key, Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoGongfaConfig
            {
                Id = GetString(raw, "id", key),
                RuntimeEnum = GetString(raw, "runtimeEnum"),
                Slot = GetString(raw, "slot"),
                Style = GetString(raw, "style"),
                Name = GetString(raw, "name"),
                IconGlyph = GetString(raw, "iconGlyph"),
                Title = GetString(raw, "title"),
                Quality = GetString(raw, "quality"),
                Description = GetString(raw, "description")
            };
        }

        private static DemoArtifactConfig ParseArtifactConfig(string key, Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoArtifactConfig
            {
                Id = GetString(raw, "id", key),
                RuntimeEnum = GetString(raw, "runtimeEnum"),
                Name = GetString(raw, "name"),
                IconGlyph = GetString(raw, "iconGlyph"),
                Style = GetString(raw, "style"),
                Quality = GetString(raw, "quality"),
                Description = GetString(raw, "description")
            };
        }

        private static DemoRelicConfig ParseRelicConfig(string key, Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoRelicConfig
            {
                Id = GetString(raw, "id", key),
                Name = GetString(raw, "name"),
                IconGlyph = GetString(raw, "iconGlyph"),
                Style = GetString(raw, "style"),
                Quality = GetString(raw, "quality"),
                Description = GetString(raw, "description")
            };
        }

        private static DemoRoutePlanConfig ParseRoutePlanConfig(string key, Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoRoutePlanConfig
            {
                Id = GetString(raw, "id", key),
                Layer = GetInt(raw, "layer"),
                Name = GetString(raw, "name"),
                Description = GetString(raw, "description"),
                RouteStyle = GetString(raw, "routeStyle"),
                RouteQuality = GetString(raw, "routeQuality"),
                RouteGlyph = GetString(raw, "routeGlyph"),
                RouteTag = GetString(raw, "routeTag"),
                Nodes = ReadObjectList(raw, "nodes", ParseRoutePlanNodeConfig)
            };
        }

        private static DemoRoutePlanNodeConfig ParseRoutePlanNodeConfig(Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoRoutePlanNodeConfig
            {
                Seq = GetInt(raw, "seq"),
                Layer = GetInt(raw, "layer"),
                NodeType = GetString(raw, "nodeType"),
                NodeName = GetString(raw, "nodeName")
            };
        }

        private static DemoEnemyConfig ParseEnemyConfig(string key, Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoEnemyConfig
            {
                Id = GetString(raw, "id", key),
                Name = GetString(raw, "name"),
                BattleRole = GetString(raw, "battleRole"),
                IsBoss = GetBool(raw, "isBoss"),
                MaxHealth = GetInt(raw, "maxHealth"),
                BaseDamageProfile = GetString(raw, "baseDamageProfile"),
                Notes = GetString(raw, "notes"),
                BossPhases = ReadObjectList(raw, "bossPhases", ParseBossPhaseConfig)
            };
        }

        private static DemoBossPhaseConfig ParseBossPhaseConfig(Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoBossPhaseConfig
            {
                PhaseId = GetString(raw, "phaseId"),
                PhaseOrder = GetInt(raw, "phaseOrder"),
                Name = GetString(raw, "name"),
                HealthRatioMax = GetDouble(raw, "healthRatioMax"),
                IntentText = GetString(raw, "intentText"),
                ChargeTurn = GetBool(raw, "chargeTurn"),
                BaseDamage = GetInt(raw, "baseDamage"),
                ShockApply = GetInt(raw, "shockApply"),
                Notes = GetString(raw, "notes")
            };
        }

        private static DemoRewardPriorityEntryConfig ParseRewardPriorityEntryConfig(Dictionary<string, object> raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new DemoRewardPriorityEntryConfig
            {
                PriorityGroup = GetString(raw, "priorityGroup"),
                Seq = GetInt(raw, "seq"),
                RefType = GetString(raw, "refType"),
                RefId = GetString(raw, "refId"),
                Notes = GetString(raw, "notes")
            };
        }

        private static Dictionary<string, Dictionary<string, List<DemoRewardPriorityEntryConfig>>> ParseRewardPriorities(Dictionary<string, object> raw)
        {
            Dictionary<string, Dictionary<string, List<DemoRewardPriorityEntryConfig>>> result
                = new Dictionary<string, Dictionary<string, List<DemoRewardPriorityEntryConfig>>>(StringComparer.OrdinalIgnoreCase);

            if (raw == null)
            {
                return result;
            }

            foreach (KeyValuePair<string, object> service in raw)
            {
                if (!(service.Value is Dictionary<string, object> styleMap))
                {
                    continue;
                }

                Dictionary<string, List<DemoRewardPriorityEntryConfig>> styles
                    = new Dictionary<string, List<DemoRewardPriorityEntryConfig>>(StringComparer.OrdinalIgnoreCase);

                foreach (KeyValuePair<string, object> style in styleMap)
                {
                    List<object> entries = style.Value as List<object>;
                    styles[style.Key] = ReadObjectList(entries, ParseRewardPriorityEntryConfig);
                }

                result[service.Key] = styles;
            }

            return result;
        }

        private static Dictionary<string, object> GetObject(Dictionary<string, object> parent, string key)
        {
            if (parent != null && parent.TryGetValue(key, out object value) && value is Dictionary<string, object> obj)
            {
                return obj;
            }

            return null;
        }

        private static List<object> GetArray(Dictionary<string, object> parent, string key)
        {
            if (parent != null && parent.TryGetValue(key, out object value) && value is List<object> list)
            {
                return list;
            }

            return null;
        }

        private static object GetValue(Dictionary<string, object> parent, string key)
        {
            if (parent != null && parent.TryGetValue(key, out object value))
            {
                return value;
            }

            return null;
        }

        private static string GetString(Dictionary<string, object> parent, string key, string fallback = null)
        {
            object value = GetValue(parent, key);
            if (value == null)
            {
                return fallback;
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
        }

        private static bool GetBool(Dictionary<string, object> parent, string key, bool fallback = false)
        {
            object value = GetValue(parent, key);
            return TryConvertBool(value, out bool parsed) ? parsed : fallback;
        }

        private static int GetInt(Dictionary<string, object> parent, string key, int fallback = 0)
        {
            object value = GetValue(parent, key);
            return TryConvertInt(value, out int parsed) ? parsed : fallback;
        }

        private static double GetDouble(Dictionary<string, object> parent, string key, double fallback = 0d)
        {
            object value = GetValue(parent, key);
            return TryConvertDouble(value, out double parsed) ? parsed : fallback;
        }

        private static List<string> ReadStringList(List<object> source)
        {
            List<string> result = new List<string>();
            if (source == null)
            {
                return result;
            }

            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] == null)
                {
                    continue;
                }

                result.Add(Convert.ToString(source[i], CultureInfo.InvariantCulture));
            }

            return result;
        }

        private static List<T> ReadObjectList<T>(Dictionary<string, object> parent, string key, Func<Dictionary<string, object>, T> factory)
            where T : class
        {
            return ReadObjectList(GetArray(parent, key), factory);
        }

        private static List<T> ReadObjectList<T>(List<object> source, Func<Dictionary<string, object>, T> factory)
            where T : class
        {
            List<T> result = new List<T>();
            if (source == null)
            {
                return result;
            }

            for (int i = 0; i < source.Count; i++)
            {
                if (!(source[i] is Dictionary<string, object> raw))
                {
                    continue;
                }

                T item = factory(raw);
                if (item != null)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private static Dictionary<string, T> ReadObjectMap<T>(
            Dictionary<string, object> parent,
            string key,
            Func<string, Dictionary<string, object>, T> factory)
            where T : class
        {
            Dictionary<string, T> result = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, object> rawMap = GetObject(parent, key);
            if (rawMap == null)
            {
                return result;
            }

            foreach (KeyValuePair<string, object> pair in rawMap)
            {
                if (!(pair.Value is Dictionary<string, object> raw))
                {
                    continue;
                }

                T item = factory(pair.Key, raw);
                if (item != null)
                {
                    result[pair.Key] = item;
                }
            }

            return result;
        }

        private static Dictionary<string, List<T>> ReadObjectMapOfLists<T>(
            Dictionary<string, object> parent,
            string key,
            Func<Dictionary<string, object>, T> factory)
            where T : class
        {
            Dictionary<string, List<T>> result = new Dictionary<string, List<T>>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, object> rawMap = GetObject(parent, key);
            if (rawMap == null)
            {
                return result;
            }

            foreach (KeyValuePair<string, object> pair in rawMap)
            {
                result[pair.Key] = ReadObjectList(pair.Value as List<object>, factory);
            }

            return result;
        }

        private static bool TryConvertBool(object value, out bool parsed)
        {
            switch (value)
            {
                case bool boolValue:
                    parsed = boolValue;
                    return true;
                case string stringValue when bool.TryParse(stringValue, out bool fromString):
                    parsed = fromString;
                    return true;
                case string stringValue when stringValue == "1":
                    parsed = true;
                    return true;
                case string stringValue when stringValue == "0":
                    parsed = false;
                    return true;
                default:
                    parsed = default;
                    return false;
            }
        }

        private static bool TryConvertInt(object value, out int parsed)
        {
            switch (value)
            {
                case int intValue:
                    parsed = intValue;
                    return true;
                case long longValue when longValue >= int.MinValue && longValue <= int.MaxValue:
                    parsed = (int)longValue;
                    return true;
                case float floatValue:
                    parsed = (int)floatValue;
                    return true;
                case double doubleValue:
                    parsed = (int)doubleValue;
                    return true;
                case decimal decimalValue:
                    parsed = (int)decimalValue;
                    return true;
                case string stringValue:
                    return int.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed);
                default:
                    parsed = default;
                    return false;
            }
        }

        private static bool TryConvertFloat(object value, out float parsed)
        {
            switch (value)
            {
                case float floatValue:
                    parsed = floatValue;
                    return true;
                case double doubleValue:
                    parsed = (float)doubleValue;
                    return true;
                case int intValue:
                    parsed = intValue;
                    return true;
                case long longValue:
                    parsed = longValue;
                    return true;
                case decimal decimalValue:
                    parsed = (float)decimalValue;
                    return true;
                case string stringValue:
                    return float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed);
                default:
                    parsed = default;
                    return false;
            }
        }

        private static bool TryConvertDouble(object value, out double parsed)
        {
            switch (value)
            {
                case double doubleValue:
                    parsed = doubleValue;
                    return true;
                case float floatValue:
                    parsed = floatValue;
                    return true;
                case int intValue:
                    parsed = intValue;
                    return true;
                case long longValue:
                    parsed = longValue;
                    return true;
                case decimal decimalValue:
                    parsed = (double)decimalValue;
                    return true;
                case string stringValue:
                    return double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed);
                default:
                    parsed = default;
                    return false;
            }
        }

        private static bool TryGetConstant(string group, string key, out DemoSystemConstantEntry entry)
        {
            return constantsByKey.TryGetValue(GetConstantKey(group, key), out entry);
        }

        private static string GetConstantKey(string group, string key)
        {
            return string.Concat(group ?? string.Empty, "::", key ?? string.Empty);
        }

        private static string GetStyleKey(DemoSwordStyle style)
        {
            return style.ToString().ToLowerInvariant();
        }

        private static T ParseEnum<T>(string rawValue, T fallback)
            where T : struct
        {
            return TryParseEnum(rawValue, out T value) ? value : fallback;
        }

        private static bool TryParseEnum<T>(string rawValue, out T value)
            where T : struct
        {
            if (!string.IsNullOrEmpty(rawValue))
            {
                string normalized = NormalizeEnumToken(rawValue);
                if (Enum.TryParse(normalized, true, out value))
                {
                    return true;
                }
            }

            value = default;
            return false;
        }

        private static string NormalizeEnumToken(string rawValue)
        {
            string[] parts = rawValue
                .Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (part.Length == 0)
                {
                    continue;
                }

                builder.Append(char.ToUpperInvariant(part[0]));
                if (part.Length > 1)
                {
                    builder.Append(part.Substring(1));
                }
            }

            return builder.ToString();
        }

        private static DemoGongfaDefinition Clone(DemoGongfaDefinition definition)
        {
            return new DemoGongfaDefinition
            {
                Type = definition.Type,
                Slot = definition.Slot,
                Style = definition.Style,
                Name = definition.Name,
                IconGlyph = definition.IconGlyph,
                Title = definition.Title,
                Description = definition.Description,
                Quality = definition.Quality
            };
        }

        private static DemoArtifactDefinition Clone(DemoArtifactDefinition definition)
        {
            return new DemoArtifactDefinition
            {
                Type = definition.Type,
                Name = definition.Name,
                IconGlyph = definition.IconGlyph,
                Description = definition.Description,
                Style = definition.Style,
                Quality = definition.Quality
            };
        }

        private static DemoRelicDefinition Clone(DemoRelicDefinition definition)
        {
            return new DemoRelicDefinition
            {
                Name = definition.Name,
                IconGlyph = definition.IconGlyph,
                Description = definition.Description,
                Style = definition.Style,
                Quality = definition.Quality
            };
        }

        private static DemoRoutePlanDefinition Clone(DemoRoutePlanDefinition definition)
        {
            DemoMapNode[] clonedNodes = definition.Plan.Nodes
                .Select(node => node.Clone())
                .ToArray();

            return new DemoRoutePlanDefinition
            {
                Id = definition.Id,
                Plan = new DemoMapRoutePlan(definition.Plan.Name, definition.Plan.Description, clonedNodes),
                RouteStyle = definition.RouteStyle,
                RouteQuality = definition.RouteQuality,
                RouteGlyph = definition.RouteGlyph,
                RouteTag = definition.RouteTag
            };
        }

        private static DemoEnemyDefinition Clone(DemoEnemyDefinition definition)
        {
            return new DemoEnemyDefinition
            {
                Id = definition.Id,
                Name = definition.Name,
                BattleRole = definition.BattleRole,
                IsBoss = definition.IsBoss,
                MaxHealth = definition.MaxHealth,
                BaseDamageProfile = definition.BaseDamageProfile,
                Notes = definition.Notes,
                BossPhases = definition.BossPhases != null
                    ? new List<DemoBossPhaseConfig>(definition.BossPhases)
                    : new List<DemoBossPhaseConfig>()
            };
        }

        private static DemoRootDefinition Clone(DemoRootDefinition definition)
        {
            return new DemoRootDefinition
            {
                Id = definition.Id,
                Name = definition.Name,
                Rarity = definition.Rarity,
                UnlockCondition = definition.UnlockCondition,
                IsDefaultPool = definition.IsDefaultPool,
                Summary = definition.Summary
            };
        }

        private static DemoRegionDefinition Clone(DemoRegionDefinition definition)
        {
            return new DemoRegionDefinition
            {
                Id = definition.Id,
                Name = definition.Name,
                RewardFocus = definition.RewardFocus,
                Description = definition.Description,
                NodeWeights = new Dictionary<string, int>(definition.NodeWeights, StringComparer.OrdinalIgnoreCase)
            };
        }

        private static DemoJourneyLineDefinition Clone(DemoJourneyLineDefinition definition)
        {
            return new DemoJourneyLineDefinition
            {
                Id = definition.Id,
                RootId = definition.RootId,
                Title = definition.Title,
                OriginText = definition.OriginText,
                CarryItemName = definition.CarryItemName,
                CarryItemEffect = definition.CarryItemEffect,
                FirstRegionId = definition.FirstRegionId,
                RiskLevel = definition.RiskLevel,
                SummaryTags = definition.SummaryTags != null ? new List<string>(definition.SummaryTags) : new List<string>(),
                NodeBiases = definition.NodeBiases != null ? new List<DemoJourneyNodeBiasConfig>(definition.NodeBiases) : new List<DemoJourneyNodeBiasConfig>(),
                RewardBiases = definition.RewardBiases != null ? new List<DemoJourneyRewardBiasConfig>(definition.RewardBiases) : new List<DemoJourneyRewardBiasConfig>()
            };
        }
    }
}

param(
    [string]$CsvDir = "CSV",
    [string]$JsonDir = "JSON"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Read-CsvFile {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "CSV file not found: $Path"
    }

    return Import-Csv -LiteralPath $Path -Encoding UTF8
}

function Convert-TypedValue {
    param(
        [string]$RawValue,
        [string]$ValueType
    )

    switch ($ValueType) {
        "int" { return [int]$RawValue }
        "float" { return [double]$RawValue }
        "bool" { return [System.Convert]::ToBoolean($RawValue) }
        default { return $RawValue }
    }
}

function Assert-UniqueColumn {
    param(
        [object[]]$Rows,
        [string]$Column,
        [string]$TableName
    )

    $duplicates = @($Rows |
        Group-Object -Property $Column |
        Where-Object { [string]::IsNullOrWhiteSpace($_.Name) -or $_.Count -gt 1 })

    if ($duplicates.Count -gt 0) {
        throw "$TableName has blank or duplicate $Column values: $($duplicates.Name -join ', ')"
    }
}

function Assert-Reference {
    param(
        [string]$Value,
        [object[]]$ValidValues,
        [string]$Context,
        [switch]$AllowEmpty
    )

    if ([string]::IsNullOrWhiteSpace($Value)) {
        if ($AllowEmpty) {
            return
        }

        throw "$Context is required."
    }

    if ($ValidValues -notcontains $Value) {
        throw "$Context references unknown id '$Value'."
    }
}

function Build-Config {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RootDir
    )

    $csv = @{
        systemConstants = Read-CsvFile (Join-Path $RootDir "system_constants.csv")
        systemFormulas = Read-CsvFile (Join-Path $RootDir "system_formulas.csv")
        styles = Read-CsvFile (Join-Path $RootDir "styles.csv")
        roots = Read-CsvFile (Join-Path $RootDir "roots.csv")
        rootModifiers = Read-CsvFile (Join-Path $RootDir "root_modifiers.csv")
        traces = Read-CsvFile (Join-Path $RootDir "traces.csv")
        traceModifiers = Read-CsvFile (Join-Path $RootDir "trace_modifiers.csv")
        regions = Read-CsvFile (Join-Path $RootDir "regions.csv")
        regionNodeWeights = Read-CsvFile (Join-Path $RootDir "region_node_weights.csv")
        journeyLines = Read-CsvFile (Join-Path $RootDir "journey_lines.csv")
        journeyVessels = Read-CsvFile (Join-Path $RootDir "journey_vessels.csv")
        journeyLineNodeBiases = Read-CsvFile (Join-Path $RootDir "journey_line_node_biases.csv")
        journeyLineRewardBiases = Read-CsvFile (Join-Path $RootDir "journey_line_reward_biases.csv")
        cards = Read-CsvFile (Join-Path $RootDir "cards.csv")
        cardPools = Read-CsvFile (Join-Path $RootDir "card_pools.csv")
        corePractices = Read-CsvFile (Join-Path $RootDir "core_practices.csv")
        techniques = Read-CsvFile (Join-Path $RootDir "techniques.csv")
        bearers = Read-CsvFile (Join-Path $RootDir "bearers.csv")
        startingPracticePackages = Read-CsvFile (Join-Path $RootDir "starting_practice_packages.csv")
        gongfas = Read-CsvFile (Join-Path $RootDir "gongfas.csv")
        artifacts = Read-CsvFile (Join-Path $RootDir "artifacts.csv")
        relics = Read-CsvFile (Join-Path $RootDir "relics.csv")
        routePlans = Read-CsvFile (Join-Path $RootDir "route_plans.csv")
        routePlanNodes = Read-CsvFile (Join-Path $RootDir "route_plan_nodes.csv")
        rewardProfiles = Read-CsvFile (Join-Path $RootDir "reward_profiles.csv")
        nodeActionProfiles = Read-CsvFile (Join-Path $RootDir "node_action_profiles.csv")
        enemies = Read-CsvFile (Join-Path $RootDir "enemies.csv")
        bossPhases = Read-CsvFile (Join-Path $RootDir "boss_phases.csv")
        rewardServicePriorities = Read-CsvFile (Join-Path $RootDir "reward_service_priorities.csv")
    }

    $rootById = @{}
    foreach ($root in $csv.roots) {
        $rootById[$root.root_id] = [ordered]@{
            id = $root.root_id
            name = $root.name
            rarity = $root.rarity
            unlockCondition = $root.unlock_condition
            isDefaultPool = [System.Convert]::ToBoolean($root.is_default_pool)
            isAvailable = [System.Convert]::ToBoolean($root.is_available)
            summary = $root.summary
            modifiers = @($csv.rootModifiers | Where-Object { $_.root_id -eq $root.root_id } | ForEach-Object {
                [ordered]@{
                    targetSystem = $_.target_system
                    targetKey = $_.target_key
                    operation = $_.operation
                    value = $_.value
                    isVisible = [System.Convert]::ToBoolean($_.is_visible)
                    notes = $_.notes
                }
            })
        }
    }

    $traceById = @{}
    foreach ($trace in $csv.traces) {
        $traceById[$trace.trace_id] = [ordered]@{
            id = $trace.trace_id
            name = $trace.name
            traceType = $trace.trace_type
            summary = $trace.summary
            modifiers = @($csv.traceModifiers | Where-Object { $_.trace_id -eq $trace.trace_id } | ForEach-Object {
                [ordered]@{
                    targetSystem = $_.target_system
                    targetKey = $_.target_key
                    operation = $_.operation
                    value = $_.value
                    isVisible = [System.Convert]::ToBoolean($_.is_visible)
                    notes = $_.notes
                }
            })
        }
    }

    $regionById = @{}
    foreach ($region in $csv.regions) {
        $regionById[$region.region_id] = [ordered]@{
            id = $region.region_id
            name = $region.name
            rewardFocus = $region.reward_focus
            description = $region.description
            isAvailable = [System.Convert]::ToBoolean($region.is_available)
            nodeWeights = @($csv.regionNodeWeights | Where-Object { $_.region_id -eq $region.region_id } | ForEach-Object {
                [ordered]@{
                    nodeType = $_.node_type
                    weight = [int]$_.weight
                }
            })
        }
    }

    $journeyLineById = @{}
    foreach ($line in $csv.journeyLines) {
        $journeyLineById[$line.line_id] = [ordered]@{
            id = $line.line_id
            rootId = $line.root_id
            title = $line.title
            originText = $line.origin_text
            carryItemName = $line.carry_item_name
            carryItemEffect = $line.carry_item_effect
            firstRegionId = $line.first_region_id
            regionCandidateIds = @($line.region_candidate_ids -split "\|" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.Trim() })
            riskLevel = $line.risk_level
            summaryTags = $line.summary_tags -split "\|"
            nodeBiases = @($csv.journeyLineNodeBiases | Where-Object { $_.line_id -eq $line.line_id } | ForEach-Object {
                [ordered]@{
                    nodeType = $_.node_type
                    deltaPercent = [int]$_.delta_percent
                }
            })
            rewardBiases = @($csv.journeyLineRewardBiases | Where-Object { $_.line_id -eq $line.line_id } | ForEach-Object {
                [ordered]@{
                    tagId = $_.tag_id
                    deltaPercent = [int]$_.delta_percent
                    priority = $_.priority
                }
            })
        }
    }

    $journeyVesselById = @{}
    foreach ($vessel in $csv.journeyVessels) {
        $journeyVesselById[$vessel.vessel_id] = [ordered]@{
            id = $vessel.vessel_id
            rootId = $vessel.root_id
            name = $vessel.name
            originText = $vessel.origin_text
            vesselType = $vessel.vessel_type
            starterPoolId = $vessel.starter_pool_id
            startingPracticePackageId = $vessel.starting_practice_package_id
            baseStyle = $vessel.base_style
            startingEffectText = $vessel.starting_effect_text
            firstRegionId = $vessel.first_region_id
            regionCandidateIds = @($vessel.region_candidate_ids -split "\|" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.Trim() })
            riskLevel = $vessel.risk_level
            summaryTags = @($vessel.summary_tags -split "\|" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.Trim() })
            isAvailable = [System.Convert]::ToBoolean($vessel.is_available)
        }
    }
    # Runtime opening still uses the legacy journey-line shape during the UI migration.
    # Its data source is now the available journey vessels, not journey_lines.csv.
    $journeyLineById = @{}
    foreach ($vessel in $csv.journeyVessels | Where-Object { [System.Convert]::ToBoolean($_.is_available) }) {
        $journeyLineById[$vessel.vessel_id] = [ordered]@{
            id = $vessel.vessel_id
            rootId = $vessel.root_id
            title = $vessel.name
            originText = $vessel.origin_text
            carryItemName = $vessel.name
            carryItemEffect = $vessel.starting_effect_text
            vesselType = $vessel.vessel_type
            starterPoolId = $vessel.starter_pool_id
            startingPracticePackageId = $vessel.starting_practice_package_id
            baseStyle = $vessel.base_style
            isAvailable = $true
            firstRegionId = $vessel.first_region_id
            regionCandidateIds = @($vessel.region_candidate_ids -split "\|" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.Trim() })
            riskLevel = $vessel.risk_level
            summaryTags = @($vessel.summary_tags -split "\|" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.Trim() })
            nodeBiases = @()
            rewardBiases = @()
        }
    }
    $cardById = @{}
    foreach ($card in $csv.cards) {
        $cardById[$card.card_id] = [ordered]@{
            id = $card.card_id
            name = $card.name
            iconGlyph = $card.icon_glyph
            type = $card.type
            style = $card.style
            quality = $card.quality
            cost = [int]$card.cost
            damage = [int]$card.damage
            block = [int]$card.block
            draw = [int]$card.draw
            energyGain = [int]$card.energy_gain
            swordIntent = [int]$card.sword_intent
            shock = [int]$card.shock
            bleed = [int]$card.bleed
            temporarySwords = [int]$card.temporary_swords
            permanentSword = [System.Convert]::ToBoolean($card.permanent_sword)
            consumeAllSwordIntent = [System.Convert]::ToBoolean($card.consume_all_sword_intent)
            selfDamage = [int]$card.self_damage
            specialEffect = $card.special_effect
            rulesOverride = $card.rules_override
        }
    }

    $cardPools = @{}
    foreach ($poolName in ($csv.cardPools.pool_id | Sort-Object -Unique)) {
        $cardPools[$poolName] = @($csv.cardPools |
            Where-Object { $_.pool_id -eq $poolName } |
            Sort-Object { [int]$_.entry_order } |
            ForEach-Object {
                [ordered]@{
                    entryType = $_.entry_type
                    refId = $_.ref_id
                    count = [int]$_.count
                    notes = $_.notes
                }
            })
    }

    $corePracticeById = @{}
    foreach ($practice in $csv.corePractices) {
        $corePracticeById[$practice.practice_id] = [ordered]@{
            id = $practice.practice_id
            name = $practice.name
            practiceType = $practice.practice_type
            passiveRuleText = $practice.passive_rule_text
            grantedTechniqueId = $practice.granted_technique_id
            sourceStoryId = $practice.source_story_id
        }
    }

    $techniqueById = @{}
    foreach ($technique in $csv.techniques) {
        $techniqueById[$technique.technique_id] = [ordered]@{
            id = $technique.technique_id
            name = $technique.name
            kind = $technique.kind
            sourceStoryId = $technique.source_story_id
            rulesText = $technique.rules_text
            visualTag = $technique.visual_tag
        }
    }

    $bearerById = @{}
    foreach ($bearer in $csv.bearers) {
        $bearerById[$bearer.bearer_id] = [ordered]@{
            id = $bearer.bearer_id
            name = $bearer.name
            mode = $bearer.mode
            resourceKey = $bearer.resource_key
            isRequired = [System.Convert]::ToBoolean($bearer.is_required)
        }
    }

    $startingPracticePackageById = @{}
    foreach ($package in $csv.startingPracticePackages) {
        $startingPracticePackageById[$package.package_id] = [ordered]@{
            id = $package.package_id
            rootId = $package.root_id
            sourceStoryId = $package.source_story_id
            innateArtifactId = $package.innate_artifact_id
            corePracticeId = $package.core_practice_id
            primaryTechniqueId = $package.primary_technique_id
            bearerDefinitionId = $package.bearer_definition_id
            isAvailable = [System.Convert]::ToBoolean($package.is_available)
            activeTechniqueIds = @($package.active_technique_ids -split "\|" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.Trim() })
        }
    }

    $gongfaById = @{}
    foreach ($gongfa in $csv.gongfas) {
        $gongfaById[$gongfa.gongfa_id] = [ordered]@{
            id = $gongfa.gongfa_id
            runtimeEnum = $gongfa.runtime_enum
            slot = $gongfa.slot
            style = $gongfa.style
            name = $gongfa.name
            iconGlyph = $gongfa.icon_glyph
            title = $gongfa.title
            quality = $gongfa.quality
            description = $gongfa.description
        }
    }

    $artifactById = @{}
    foreach ($artifact in $csv.artifacts) {
        $artifactById[$artifact.artifact_id] = [ordered]@{
            id = $artifact.artifact_id
            runtimeEnum = $artifact.runtime_enum
            name = $artifact.name
            iconGlyph = $artifact.icon_glyph
            style = $artifact.style
            quality = $artifact.quality
            description = $artifact.description
        }
    }

    $relicById = @{}
    foreach ($relic in $csv.relics) {
        $relicById[$relic.relic_id] = [ordered]@{
            id = $relic.relic_id
            name = $relic.name
            iconGlyph = $relic.icon_glyph
            style = $relic.style
            quality = $relic.quality
            description = $relic.description
        }
    }

    $routePlanById = @{}
    foreach ($route in $csv.routePlans) {
        $routePlanById[$route.route_plan_id] = [ordered]@{
            id = $route.route_plan_id
            layer = [int]$route.layer
            name = $route.name
            description = $route.description
            routeStyle = $route.route_style
            routeQuality = $route.route_quality
            routeGlyph = $route.route_glyph
            routeTag = $route.route_tag
            nodes = @($csv.routePlanNodes |
                Where-Object { $_.route_plan_id -eq $route.route_plan_id } |
                Sort-Object { [int]$_.seq } |
                ForEach-Object {
                    [ordered]@{
                        seq = [int]$_.seq
                        layer = [int]$_.layer
                        nodeType = $_.node_type
                        nodeName = $_.node_name
                        nodeId = $_.node_id
                        encounterId = $_.encounter_id
                        rewardProfileId = $_.reward_profile_id
                        actionProfileId = $_.action_profile_id
                    }
                })
        }
    }

    $rewardProfileById = @{}
    foreach ($profile in $csv.rewardProfiles) {
        $rewardProfileById[$profile.reward_profile_id] = [ordered]@{
            id = $profile.reward_profile_id
            tier = $profile.tier
            source = $profile.source
            routeRisk = $profile.route_risk
            allowsFinisher = [System.Convert]::ToBoolean($profile.allows_finisher)
            allowsDivine = [System.Convert]::ToBoolean($profile.allows_divine)
            description = $profile.description
        }
    }

    $nodeActionProfileById = @{}
    foreach ($profile in $csv.nodeActionProfiles) {
        $nodeActionProfileById[$profile.action_profile_id] = [ordered]@{
            id = $profile.action_profile_id
            actionType = $profile.action_type
            rewardProfileId = $profile.reward_profile_id
            guaranteedComponentId = $profile.guaranteed_component_id
            healAmount = if ([string]::IsNullOrWhiteSpace($profile.heal_amount)) { 0 } else { [int]$profile.heal_amount }
            description = $profile.description
        }
    }

    $enemyById = @{}
    foreach ($enemy in $csv.enemies) {
        $enemyById[$enemy.enemy_id] = [ordered]@{
            id = $enemy.enemy_id
            name = $enemy.name
            battleRole = $enemy.battle_role
            isBoss = [System.Convert]::ToBoolean($enemy.is_boss)
            maxHealth = [int]$enemy.max_health
            baseDamageProfile = $enemy.base_damage_profile
            notes = $enemy.notes
            bossPhases = @($csv.bossPhases | Where-Object { $_.boss_id -eq $enemy.enemy_id } | Sort-Object { [int]$_.phase_order } | ForEach-Object {
                [ordered]@{
                    phaseId = $_.phase_id
                    phaseOrder = [int]$_.phase_order
                    name = $_.name
                    healthRatioMax = [double]$_.health_ratio_max
                    intentText = $_.intent_text
                    chargeTurn = [System.Convert]::ToBoolean($_.charge_turn)
                    baseDamage = [int]$_.base_damage
                    shockApply = [int]$_.shock_apply
                    notes = $_.notes
                }
            })
        }
    }

    Assert-UniqueColumn $csv.roots "root_id" "roots.csv"
    Assert-UniqueColumn $csv.traces "trace_id" "traces.csv"
    Assert-UniqueColumn $csv.regions "region_id" "regions.csv"
    Assert-UniqueColumn $csv.journeyVessels "vessel_id" "journey_vessels.csv"
    Assert-UniqueColumn $csv.cards "card_id" "cards.csv"
    Assert-UniqueColumn $csv.corePractices "practice_id" "core_practices.csv"
    Assert-UniqueColumn $csv.techniques "technique_id" "techniques.csv"
    Assert-UniqueColumn $csv.bearers "bearer_id" "bearers.csv"
    Assert-UniqueColumn $csv.startingPracticePackages "package_id" "starting_practice_packages.csv"
    Assert-UniqueColumn $csv.gongfas "gongfa_id" "gongfas.csv"
    Assert-UniqueColumn $csv.artifacts "artifact_id" "artifacts.csv"
    Assert-UniqueColumn $csv.relics "relic_id" "relics.csv"
    Assert-UniqueColumn $csv.routePlans "route_plan_id" "route_plans.csv"
    Assert-UniqueColumn $csv.routePlanNodes "node_id" "route_plan_nodes.csv"
    Assert-UniqueColumn $csv.rewardProfiles "reward_profile_id" "reward_profiles.csv"
    Assert-UniqueColumn $csv.nodeActionProfiles "action_profile_id" "node_action_profiles.csv"
    Assert-UniqueColumn $csv.enemies "enemy_id" "enemies.csv"

    $rootIds = @($csv.roots.root_id)
    $regionIds = @($csv.regions.region_id)
    $poolIds = @($csv.cardPools.pool_id | Sort-Object -Unique)
    $cardIds = @($csv.cards.card_id)
    $practiceIds = @($csv.corePractices.practice_id)
    $techniqueIds = @($csv.techniques.technique_id)
    $bearerIds = @($csv.bearers.bearer_id)
    $startingPracticePackageIds = @($csv.startingPracticePackages.package_id)
    $gongfaIds = @($csv.gongfas.gongfa_id)
    $artifactIds = @($csv.artifacts.artifact_id)
    $relicIds = @($csv.relics.relic_id)
    $routePlanIds = @($csv.routePlans.route_plan_id)
    $rewardProfileIds = @($csv.rewardProfiles.reward_profile_id)
    $actionProfileIds = @($csv.nodeActionProfiles.action_profile_id)
    $enemyIds = @($csv.enemies.enemy_id)

    foreach ($vessel in $csv.journeyVessels) {
        Assert-Reference $vessel.root_id $rootIds "journey_vessels[$($vessel.vessel_id)].root_id"
        Assert-Reference $vessel.starter_pool_id $poolIds "journey_vessels[$($vessel.vessel_id)].starter_pool_id"
        Assert-Reference $vessel.starting_practice_package_id $startingPracticePackageIds "journey_vessels[$($vessel.vessel_id)].starting_practice_package_id" -AllowEmpty
        Assert-Reference $vessel.first_region_id $regionIds "journey_vessels[$($vessel.vessel_id)].first_region_id"
        foreach ($regionId in @($vessel.region_candidate_ids.Split([char]124) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })) {
            Assert-Reference $regionId.Trim() $regionIds "journey_vessels[$($vessel.vessel_id)].region_candidate_ids"
        }
    }

    foreach ($entry in $csv.cardPools) {
        if ($entry.entry_type -eq "card") {
            Assert-Reference $entry.ref_id $cardIds "card_pools[$($entry.pool_id)].ref_id"
        }
    }

    foreach ($practice in $csv.corePractices) {
        Assert-Reference $practice.granted_technique_id $techniqueIds "core_practices[$($practice.practice_id)].granted_technique_id"
    }

    foreach ($package in $csv.startingPracticePackages) {
        Assert-Reference $package.root_id $rootIds "starting_practice_packages[$($package.package_id)].root_id"
        Assert-Reference $package.core_practice_id $practiceIds "starting_practice_packages[$($package.package_id)].core_practice_id"
        Assert-Reference $package.primary_technique_id $techniqueIds "starting_practice_packages[$($package.package_id)].primary_technique_id"
        Assert-Reference $package.bearer_definition_id $bearerIds "starting_practice_packages[$($package.package_id)].bearer_definition_id"

        $activeTechniqueIds = @($package.active_technique_ids -split "\|" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { $_.Trim() })
        if ($activeTechniqueIds.Count -ne 2) {
            throw "starting_practice_packages[$($package.package_id)] must expose exactly two active techniques."
        }

        foreach ($techniqueId in $activeTechniqueIds) {
            Assert-Reference $techniqueId $techniqueIds "starting_practice_packages[$($package.package_id)].active_technique_ids"
        }

        $practice = $csv.corePractices | Where-Object { $_.practice_id -eq $package.core_practice_id } | Select-Object -First 1
        if ($activeTechniqueIds -notcontains $practice.granted_technique_id -or $activeTechniqueIds -notcontains $package.primary_technique_id) {
            throw "starting_practice_packages[$($package.package_id)] must contain the practice-granted and primary techniques."
        }
    }

    foreach ($node in $csv.routePlanNodes) {
        Assert-Reference $node.route_plan_id $routePlanIds "route_plan_nodes[$($node.node_id)].route_plan_id"
        Assert-Reference $node.encounter_id $enemyIds "route_plan_nodes[$($node.node_id)].encounter_id" -AllowEmpty
        Assert-Reference $node.reward_profile_id $rewardProfileIds "route_plan_nodes[$($node.node_id)].reward_profile_id" -AllowEmpty
        Assert-Reference $node.action_profile_id $actionProfileIds "route_plan_nodes[$($node.node_id)].action_profile_id" -AllowEmpty

        if ($node.node_type -in @("battle", "boss")) {
            Assert-Reference $node.encounter_id $enemyIds "route_plan_nodes[$($node.node_id)].encounter_id"
            Assert-Reference $node.reward_profile_id $rewardProfileIds "route_plan_nodes[$($node.node_id)].reward_profile_id"
        }

        if ($node.node_type -eq "boss") {
            $enemy = $csv.enemies | Where-Object { $_.enemy_id -eq $node.encounter_id } | Select-Object -First 1
            if ($null -eq $enemy -or -not [System.Convert]::ToBoolean($enemy.is_boss)) {
                throw "route_plan_nodes[$($node.node_id)] must reference a boss encounter."
            }
        }
    }

    foreach ($profile in $csv.nodeActionProfiles) {
        Assert-Reference $profile.reward_profile_id $rewardProfileIds "node_action_profiles[$($profile.action_profile_id)].reward_profile_id" -AllowEmpty
        if (-not [string]::IsNullOrWhiteSpace($profile.guaranteed_component_id) -and $profile.guaranteed_component_id -notin @("engine_wanjian", "survival_boss")) {
            $componentIds = @($cardIds + $gongfaIds + $artifactIds)
            Assert-Reference $profile.guaranteed_component_id $componentIds "node_action_profiles[$($profile.action_profile_id)].guaranteed_component_id"
        }
    }

    foreach ($priority in $csv.rewardServicePriorities) {
        switch ($priority.ref_type) {
            "card" { Assert-Reference $priority.ref_id $cardIds "reward_service_priorities[$($priority.service)].ref_id" }
            "gongfa" { Assert-Reference $priority.ref_id $gongfaIds "reward_service_priorities[$($priority.service)].ref_id" }
            "artifact" {
                $runtimeEnums = @($csv.artifacts.runtime_enum)
                Assert-Reference $priority.ref_id $runtimeEnums "reward_service_priorities[$($priority.service)].ref_id"
            }
            "relic" {
                $relicNames = @($csv.relics.name)
                Assert-Reference $priority.ref_id $relicNames "reward_service_priorities[$($priority.service)].ref_id"
            }
        }
    }
    $rewardPriorities = @{}
    foreach ($serviceName in ($csv.rewardServicePriorities.service | Sort-Object -Unique)) {
        $rewardPriorities[$serviceName] = @{}
        foreach ($styleName in (($csv.rewardServicePriorities | Where-Object { $_.service -eq $serviceName }).style | Sort-Object -Unique)) {
            $rewardPriorities[$serviceName][$styleName] = @($csv.rewardServicePriorities |
                Where-Object { $_.service -eq $serviceName -and $_.style -eq $styleName } |
                Sort-Object { [int]$_.seq } |
                ForEach-Object {
                    [ordered]@{
                        priorityGroup = $_.priority_group
                        seq = [int]$_.seq
                        refType = $_.ref_type
                        refId = $_.ref_id
                        notes = $_.notes
                    }
                })
        }
    }

    return [ordered]@{
        meta = [ordered]@{
            generatedAt = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssK")
            source = "UNITY/Assets/Data/CSV"
            purpose = "Program-readable complete config tables for opening system and current Unity DEMO runtime content."
        }
        systemConstants = @($csv.systemConstants | ForEach-Object {
            [ordered]@{
                group = $_.group
                key = $_.key
                value = Convert-TypedValue -RawValue $_.value -ValueType $_.value_type
                valueType = $_.value_type
                notes = $_.notes
            }
        })
        systemFormulas = @($csv.systemFormulas)
        styles = @($csv.styles)
        opening = [ordered]@{
            roots = $rootById
            traces = $traceById
            regions = $regionById
            journeyLines = $journeyLineById
            journeyVessels = $journeyVesselById
        }
        demo = [ordered]@{
            cards = $cardById
            cardPools = $cardPools
            corePractices = $corePracticeById
            techniques = $techniqueById
            bearers = $bearerById
            startingPracticePackages = $startingPracticePackageById
            gongfas = $gongfaById
            artifacts = $artifactById
            relics = $relicById
            routePlans = $routePlanById
            rewardProfiles = $rewardProfileById
            nodeActionProfiles = $nodeActionProfileById
            enemies = $enemyById
            rewardPriorities = $rewardPriorities
        }
    }
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$resolvedCsvDir = Join-Path $scriptDir $CsvDir
$resolvedJsonDir = Join-Path $scriptDir $JsonDir

if (-not (Test-Path -LiteralPath $resolvedJsonDir)) {
    New-Item -ItemType Directory -Path $resolvedJsonDir | Out-Null
}

$config = Build-Config -RootDir $resolvedCsvDir

$jsonText = $config | ConvertTo-Json -Depth 100
$jsonPath = Join-Path $resolvedJsonDir "game_config.json"
[System.IO.File]::WriteAllText($jsonPath, $jsonText, [System.Text.UTF8Encoding]::new($false))

Write-Output "Generated: $jsonPath"

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
        journeyLineNodeBiases = Read-CsvFile (Join-Path $RootDir "journey_line_node_biases.csv")
        journeyLineRewardBiases = Read-CsvFile (Join-Path $RootDir "journey_line_reward_biases.csv")
        cards = Read-CsvFile (Join-Path $RootDir "cards.csv")
        cardPools = Read-CsvFile (Join-Path $RootDir "card_pools.csv")
        gongfas = Read-CsvFile (Join-Path $RootDir "gongfas.csv")
        artifacts = Read-CsvFile (Join-Path $RootDir "artifacts.csv")
        relics = Read-CsvFile (Join-Path $RootDir "relics.csv")
        routePlans = Read-CsvFile (Join-Path $RootDir "route_plans.csv")
        routePlanNodes = Read-CsvFile (Join-Path $RootDir "route_plan_nodes.csv")
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
                    }
                })
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
        }
        demo = [ordered]@{
            cards = $cardById
            cardPools = $cardPools
            gongfas = $gongfaById
            artifacts = $artifactById
            relics = $relicById
            routePlans = $routePlanById
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

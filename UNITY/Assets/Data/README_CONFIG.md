# Path of Ten Thousand Ways 配置目录

本目录把原本写在设计文档和 Demo 硬编码里的核心数据拆成程序可读配置。

当前覆盖两层：

1. `27` 对应的完整开局系统：
   - 根脚
   - 传承道痕
   - 境域模板
   - 启程线
   - 显化阈值与开局公式
2. 当前 Unity DEMO 已经落地的核心运行数据：
   - 卡牌
   - 起始牌组
   - 功法
   - 法器
   - 遗珍
   - 路线模板
   - 敌人与 Boss
   - 奖励优先级
   - 战斗常量

目录约定：

- `CSV/`：策划主编辑表，尽量保持扁平和可 diff。
- `JSON/`：从 CSV 聚合出的程序消费版本。

字段约定：

- 所有主键统一使用 `snake_case`。
- 需要对接现有 Demo 代码的位置，额外保留 `runtime_key` 或 `runtime_enum`。
- 百分比修正统一存裸值，例如 `12` 表示 `+12%`，`-6` 表示 `-6%`。
- 布尔值统一使用 `true / false`。

本轮刻意没有只做“开局表导出”，而是把当前 DEMO 核心闭环能配置化的部分一并拆出来，避免再次出现“入口有表、核心玩法埋代码里”的情况。

当前 P0 开局候选由 `journey_vessels.csv` 中 `is_available=true` 的记录生成；旧 `journey_lines.csv` 仅保留迁移兼容。
## P0 纵切契约

- 新开局主表为 journey_vessels.csv；运行时使用 DemoJourneyVesselDefinition、OpeningSelection.Vessel 与 SetVessel。旧 journey_lines.csv、DemoJourneyLineDefinition、GetJourneyLinesForRoot 和 SetJourneyLine 仅作迁移兼容。
- GetRootsForOpening、GetJourneyVesselsForRoot(includeUnavailable: true) 会返回锁定候选供 UI 展示；is_available=false 的根脚、器物和区域不可进入。旧 GetDefaultRoots、GetJourneyLinesForRoot 仍只返回当前可玩项。
- route_plan_nodes.csv 的每个节点必须提供全局稳定的 node_id。战斗与 Boss 节点还必须按 ID 引用 encounter_id 和 reward_profile_id；修炼、整备、路线与结算行为使用 action_profile_id。
- reward_profiles.csv 定义奖励来源、档位、路线风险以及终结牌/神通开放边界；node_action_profiles.csv 定义恢复、定向保底和页面动作。P0 奖励只生成卡牌、功法、法器或资源，relics.csv 仅保留旧存档兼容。
- generate_config_json.ps1 会在写 JSON 前校验主键唯一性，以及 vessel、region、starter pool、route node、enemy、reward profile、action profile 和奖励优先级的跨表引用；无效引用会直接终止生成。
- P0 路线没有通用 Reward 节点。控制器在每个 Battle/Boss 结束时读取该战斗节点的 reward_profile_id；普通战/精英战先展示奖励再推进地图，Boss 直接进入一世结算。

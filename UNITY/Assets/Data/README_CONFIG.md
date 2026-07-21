# Path of Ten Thousand Ways 配置目录

本目录承载 Unity DEMO 的策划配置。设计和数据契约以 [`Doc/07_配置数据与存档契约.md`](../../../Doc/07_配置数据与存档契约.md) 为准，迁移顺序以 [`Doc/10_DEMO重构实施计划.md`](../../../Doc/10_DEMO重构实施计划.md) 的阶段 A/B 为准。

## 真源与生成物

- `CSV/`：人工维护真源，保持扁平、稳定 ID 和可 diff。
- `JSON/game_config.json`：运行时聚合产物，不手工编辑。
- `generate_config_json.ps1`：从 CSV 读取、校验并确定性生成 JSON。

同一个字段不能同时手改 CSV 和 JSON。生成前必须检查 ID 唯一、跨表引用、类型、枚举、地图连通和关键节点可达；校验失败时停止写入，不静默回退到旧硬编码。

## 当前实现基线

现有 CSV 已覆盖：

- 根脚、道痕、所携/启程兼容数据和境域。
- 卡牌、卡池、功法、法器、遗珍和系统常量。
- 固定路线、节点行为、战斗所得与奖励优先级。
- 敌人、Boss 阶段和基础战斗参数。

这些表支撑当前可运行的旧版固定路线原型。`journey_vessels.csv`、`route_plans.csv`、`route_plan_nodes.csv`、`reward_profiles.csv`、`cards.csv` 等仍可能被运行时代码引用，不能在新结构落地前直接删除。

## 目标配置

目标纵切在现有 CSV -> JSON 链上新增：

- `core_practices.csv`
- `techniques.csv`
- `bearers.csv`
- `starting_practice_packages.csv`
- `innate_artifacts.csv`
- `innate_artifact_stages.csv`
- `mind_methods.csv`
- `mind_method_levels.csv`
- `techniques.csv`
- `realm_breakthroughs.csv`
- `foundation_rules.csv`
- `encounter_groups.csv`
- `encounter_group_members.csv`
- `map_templates.csv`
- `map_template_nodes.csv`
- `map_template_edges.csv`
- `events.csv`
- `event_choices.csv`
- `story_flags.csv`

首批四张开局表已经接入生成链：根本修行法可以是心法或炼体术，主动法诀可以是心诀、剑诀、炼体术或拳法；`starting_practice_packages.csv`固定声明故事来源、两门主动法诀、本命器物和承载物。当前只开放剑修配置，炼体/拳法只作为契约测试，不进入玩家内容池。

`journey_vessels.csv.starting_practice_package_id` 将可选所携与故事起步包绑定；运行时选中所携后解析该包，后续路线只需替换 ID 即可切换到炼体术 + 拳法等分支。

目标数据必须支持三幕手工地图模板加种子变化、1-3 名敌人编组、场景化所得、经历标记、故事筑基和节点间存档。新流程全部通过后，再停止生成固定路线、通用战后奖励和天劫 Boss 配置。

## 字段约定

- 主键使用稳定 `snake_case` ID；进入存档后的 ID 不复用。
- 百分比存裸值，例如 `12` 表示 `+12%`，字段名说明百分比或倍率语义。
- 布尔值只使用 `true / false`。
- 运行时枚举可以保留 `runtime_enum`，但跨表和存档身份始终使用稳定 ID。
- 条件、效果、目标与来源使用结构化字段或明确引用，不从展示文案解析规则。
- 聚合结果记录 `schema_version`、`content_version` 和地图生成算法版本。

## 迁移边界

- `cards.csv` 在迁移期可继续作为法诀物理表名，前台语义统一为法诀。
- `gongfas.csv`、`artifacts.csv`、`route_plans.csv`、`route_plan_nodes.csv` 和 `reward_profiles.csv` 暂保留兼容读取。
- `journey_lines.csv`、通用三选一、固定路线奖励和单一 `enemy_id` 结构不得继续决定新系统边界。
- CSV/JSON、运行时聚合类型、CompileCheck 配置镜像和测试数据必须在同一个迁移阶段同步修改。

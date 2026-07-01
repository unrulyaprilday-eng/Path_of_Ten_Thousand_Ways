# Path of Ten Thousand Ways - P0 UI Asset List

这份清单只列当前 DEMO 最缺、且会立刻影响首屏卖相与战斗观感的正式资源。

目标：

- 停止继续用程序矩形假装正式美术。
- 先把首屏、道途选择、战斗主舞台这三处门面资源补齐。
- 资源一旦落地，就能立即替换当前运行时占位表现。
- 让下个具备出图接口的会话可以不重新讨论规则，直接开始产图。
- 若当前会话可用 `imagegen`，默认直接进入生成与迭代，不把它误判为只能继续保留程序占位。

## 当前统一视觉锚点

- 主页面：参考黑白水墨巨物压境的品牌首屏关系，主角与巨物 / 天威同屏。
- 开局前台：参考纵向展示海报式器物卡与场景图卡结构，但整体改成写意水墨风，不做西式卡牌海报。
- 战斗舞台：参考大角度空中对峙镜头，玩家可以偏背身，敌方抬高，中间演算区更开阔。
- 整体色彩：黑白水墨为主，局部点翠青 / 石青 / 微朱砂点色，避免持续压黑的整屏压色。

## P0 总表

| 优先级 | 文件名 | 用途 | 接入点 | 状态 |
| --- | --- | --- | --- | --- |
| P0 | `ui_home_hero_ink_001.png` | 主页面首屏背景，定义整体品牌气质 | 主页面 / 非战斗总底图 | 待生成 |
| P0 | `ui_opening_item_swordcase_001.png` | 携行之物页：旧剑匣器物大牌 | 开局物品选择 | 待生成 |
| P0 | `ui_opening_item_thunderbone_001.png` | 携行之物页：雷骨短钉器物大牌 | 开局物品选择 | 待生成 |
| P0 | `ui_opening_item_bloodjade_001.png` | 携行之物页：温养血玉器物大牌 | 开局物品选择 | 待生成 |
| P0 | `ui_opening_scene_trade_road_001.png` | 首境页：商路荒谷场景图卡 | 开局场景选择 | 待生成 |
| P0 | `ui_opening_scene_old_mine_001.png` | 首境页：旧矿地窟场景图卡 | 开局场景选择 | 待生成 |
| P0 | `ui_opening_scene_thunder_marsh_001.png` | 首境页：雷泽险地场景图卡 | 开局场景选择 | 待生成 |
| P0 | `scene_cloudsea_far_001.png` | 共用远景层：天穹、远山、云海 | 战斗 / 非战斗共享 far layer | 待生成 |
| P0 | `scene_battle_cloudsea_mid_001.png` | 战斗中景层：浮空山门、裂隙、对峙结构 | 战斗舞台 mid layer | 待生成 |
| P0 | `scene_battle_cloudsea_near_001.png` | 战斗近景层：前景断崖、云雾、包边 | 战斗舞台 near layer | 待生成 |
| P0 | `boss_tianjie_halfbody_002.png` | 更贴近黑白水墨的 Boss 半身立绘 | Boss 战压迫视觉锚点 | 待生成 |

## 资源要求

### 1. 主页面首屏

参考方向：

- 用户给的图 1
- 黑白水墨为主
- 主角与巨物 / 天威同屏
- 不是纯 UI 底纹，而是能立住品牌气质的首屏主图

要求：

- 留出顶部标题区和中部功能入口区
- 不做大面积纯黑压暗
- 允许局部朱砂或青灰点色

建议文件：

- `UNITY/Assets/Art/UI/ui_home_hero_ink_001.png`
- 接入优先级：高

### 2. 携行之物页

参考方向：

- 用户给的图 1、图 3、图 4
- 纵向海报式器物展示牌，而不是战斗卡牌外框
- 三张牌各自独立美术，不共用同一底版换色

要求：

- 黑白水墨为主，局部点翠青 / 微朱砂点染
- 每张牌保留清晰器物主视觉中心
- 文字压到最少，只保留器物名、1 句来历、1 条效果

建议文件：

- `UNITY/Assets/Art/UI/ui_opening_item_swordcase_001.png`
- `UNITY/Assets/Art/UI/ui_opening_item_thunderbone_001.png`
- `UNITY/Assets/Art/UI/ui_opening_item_bloodjade_001.png`
- 接入优先级：最高

### 3. 首境页

参考方向：

- 用户给的图 2
- 图多字少的场景图卡
- 山势、云海、断崖、古塔、矿窟先成立，再补极短提示

要求：

- 同屏 2-4 张卡，必须有大留白和明确轮廓差
- 场景名之外，页面上只保留风险词或 1 句短提示
- 不要把节点权重、奖励标签、大段文案堆在卡面

建议文件：

- `UNITY/Assets/Art/UI/ui_opening_scene_trade_road_001.png`
- `UNITY/Assets/Art/UI/ui_opening_scene_old_mine_001.png`
- `UNITY/Assets/Art/UI/ui_opening_scene_thunder_marsh_001.png`
- 接入优先级：最高

### 4. 战斗主舞台三层场景

参考方向：

- 用户给的图 3 的大角度对峙关系
- 但画风改为黑白水墨
- 玩家可偏背身，敌方悬空更高
- 中间演算区要足够开阔

要求：

- 远景：负责大气和天穹，不抢戏
- 中景：负责战斗结构和空间深度
- 近景：负责 framing，但不能吞掉中场

建议文件：

- `UNITY/Assets/Art/Scenes/scene_cloudsea_far_001.png`
- `UNITY/Assets/Art/Scenes/scene_battle_cloudsea_mid_001.png`
- `UNITY/Assets/Art/Scenes/scene_battle_cloudsea_near_001.png`
- 接入优先级：最高

### 5. Boss 半身

当前已有：

- `boss_tianjie_halfbody_001.png`

问题：

- 现在这张更偏暗蓝概念稿
- 和当前要求的黑白水墨门面风格不完全统一

建议新增：

- `UNITY/Assets/Art/Boss/boss_tianjie_halfbody_002.png`
- 接入优先级：中

## 接入顺序

1. 先生成三张携行之物图卡
2. 接入开局物品选择页
3. 再生成 2-4 张首境场景图卡
4. 接入开局场景选择页
5. 再生成战斗三层场景并接入 `DemoBattleSceneView`
6. 最后替换主页面首屏图和 Boss 半身

## 当前代码准备情况

当前工程已经完成以下准备，可直接承接正式资源：

- 开局前台已经可以承接分层链路：启动页、根脚页、携行之物页、首境页。
- 开局核心选择页已经不再依赖 Hover 说明。
- 战斗舞台的镜头关系、角色站位和中场留白已先用代码占位确定。
- 战斗场景资源需求已拆成远景 / 中景 / 近景三层。
- 现阶段继续靠程序矩形细修的收益很低，应立即切回资源生成主线。

## 验收标准

- 开局页截图拿出去时，不再像程序拼出来的占位 UI
- 战斗截图第一眼就有“空中斗法舞台”的空间感
- 主页面截图第一眼能传达黑白水墨修仙品牌气质
- 不再依赖大段说明文字去补视觉表达

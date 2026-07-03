# DEMO 当前进度交接

> 用于会话切换时快速接手。当前重点仍然是把 `启动页 -> 根脚 -> 携行之物 -> 首境 -> 路口 -> 战斗` 这条 DEMO 门面链路继续补完整。

## 1. 当前结论

- 开局链路已经统一为：

```text
启动页 -> 根脚 -> 携行之物 -> 首境 -> 路口 -> 战斗
```

- `启程` 这类旧式合并方案不再作为当前执行路径。
- 当前视觉方向已统一为：
  - 高留白写意水墨
  - 黑白灰主调
  - 点缀青 / 石青 / 微朱砂
- 当前战斗场景不再只做单一压迫感，而是按节点分级：
  - 普通战：小怪
  - 中节点：精英
  - 大节点：小Boss
  - 最终节点：大Boss

## 2. 已完成

### 2.1 文档与资源规范同步

已阅读并对齐：

- `Doc/README.md`
- `Doc/21_美术资源生成规范.md`
- `Doc/22_项目资源生产手册.md`
- `Doc/26_开局结构与启程系统设计.md`
- `Doc/27_开局系统配置表.md`
- `UNITY/README_DEMO.md`
- `UNITY/Assets/Art/AI_BATCH_01_PROMPTS.md`
- `UNITY/Assets/Art/AI_BATCH_02_SCENE_LAYER_REQUIREMENTS.md`
- `UNITY/Assets/Art/P0_HOME_UI_SPEC.md`
- `UNITY/Assets/Art/P0_UI_ASSET_LIST.md`

### 2.2 Unity Connect 干扰处理

已处理编辑器侧 UnityConnect 相关干扰，相关缓解脚本已存在于工程中。

### 2.3 开局流程拆分完成

已完成运行时流程拆分，涉及：

- `根脚`
- `携行之物`
- `首境`

关键文件：

- `UNITY/Assets/Scripts/Systems/DemoGameController.cs`
- `UNITY/Assets/Scripts/Rewards/DemoReward.cs`

当前开局领取流程：

```text
根脚 -> 携行之物 -> 首境 -> 进入地图 -> 路口 -> 战斗
```

### 2.4 开局页与路口页 UI 收口

已完成：

- `携行之物` 页与 `首境` 页拆开显示
- `首境` 页改成更偏图像、少文字的场景卡
- `路口` 页改成图卡式路线选择
- 路口页在不新增图片的前提下，已强化“节点流程优先”的读法
- 路口卡现在显示节点序号、节点类型、短节点名、战斗 / 补强 / Boss 数量摘要
- 路口卡底部不再重复节点顺序，而是改为“压力取舍 + 收益重点”提示
- 路口页隐藏持续构筑摘要侧栏，避免视觉上像重复选主流派
- 开局三页与路口页关闭 Hover 详情弹窗，减少大段文字干扰

关键文件：

- `UNITY/Assets/Scripts/UI/DemoRuntimeCanvasUI.cs`

### 2.5 战斗场景分层优化已接入

已完成：

- 战斗场景视觉分层入口已接上
- 战斗舞台不再只按单一 Boss 压迫感表现
- 已加入按节点层级区分的战斗压迫感逻辑
- 已修正战斗层级 HUD 文案中的乱码
- 已补充常规战 / 精英战 / 守关小 Boss / 终局 Boss 的敌方锚点、舞台调色和意图提示
- 非战斗节点会重置舞台层级表现，避免继承上一场 Boss 氛围

关键文件：

- `UNITY/Assets/Scripts/UI/DemoBattleSceneView.cs`

当前效果目标：

- 普通战：克制、留白多
- 精英战：冷色压迫感略升，HUD 明确提示“精英压制”
- 守关小 Boss：镜头更靠前，敌方更重，前景与暖色压迫更明显
- 终局 Boss：只在真正天劫战里给完整压迫感和 Boss 半身压场

### 2.6 编译验证

最近一次编译结果：

```text
dotnet build UNITY/CompileCheck/CompileCheck.csproj
```

结果：

- `0 warnings`
- `0 errors`

### 2.7 首页 P0 资源补齐与接入

已完成：

- 生成并入库首页主视觉 `ui_home_hero_ink_001.png` 与正式启动页二稿 `ui_home_hero_ink_002.png`
- 生成并入库首页主按钮、次按钮、Logo 题签、右下信息签
- 本地绘制并入库首页图鉴 / 设置 / 退出三枚临时正式图标
- `启动页` 已改成全屏独立首页，不再塞在节点页大框内
- 当前优先加载 `ui_home_hero_ink_002.png`，`ui_home_hero_ink_001.png` 仅作为兜底
- 首页入口按钮已使用首页主按钮底板，次级入口使用次按钮底板
- 已去掉旧版启动页里的大框、硬遮罩、旧剑匣预览和漂浮小卡片
- 右下信息签已缩小、下沉并贴右侧，避免遮挡人物与崖边主体

说明：

- 首页主视觉、按钮底板、Logo 题签、信息签使用 `imagegen2` 生成后裁切 / 缩放落库。
- 首页二稿 Prompt 要点：正式启动页全屏背景、左侧 42% 留白、右侧天门 / 雷劫 / 云海山门、下方崖边孤身修士、无 UI、无边框、无卡片拼贴、无文字。
- 首页 UI 件 Prompt 要点：水墨卷轴器面、旧绢纸、描金、玉石点缀、无文字、中心留白，分别用于主按钮、次按钮、Logo 题签和右下信息签。
- 三枚小图标因生图接口多次断连，先用本地绘制版占住正式文件名，后续可按同名替换为 AI 二稿。

关键文件：

- `UNITY/Assets/Scripts/UI/DemoRuntimeCanvasUI.cs`

## 3. 已落地的正式资源

### 3.1 开局三张携行之物

已生成并入库：

- `UNITY/Assets/Art/UI/ui_opening_item_swordcase_001.png`
- `UNITY/Assets/Art/UI/ui_opening_item_thunderbone_001.png`
- `UNITY/Assets/Art/UI/ui_opening_item_bloodjade_001.png`

### 3.2 开局三张首境图

已生成并入库：

- `UNITY/Assets/Art/UI/ui_opening_scene_trade_road_001.png`
- `UNITY/Assets/Art/UI/ui_opening_scene_old_mine_001.png`
- `UNITY/Assets/Art/UI/ui_opening_scene_thunder_marsh_001.png`

### 3.3 战斗相关现有资源

已入库并可被运行时加载：

- `UNITY/Assets/Art/Scenes/scene_battle_cloudsea_001.png`
- `UNITY/Assets/Art/Scenes/scene_cloudsea_far_001.png`
- `UNITY/Assets/Art/Boss/boss_tianjie_halfbody_001.png`

已在 `DemoBattleSceneView` 中预留接入位，但当前仍待生成 / 入库：

- `UNITY/Assets/Art/Scenes/scene_battle_cloudsea_mid_001.png`
- `UNITY/Assets/Art/Scenes/scene_battle_cloudsea_near_001.png`
- `UNITY/Assets/Art/Boss/boss_tianjie_halfbody_002.png`

### 3.4 首页 P0 资源

已入库并可被运行时加载：

- `UNITY/Assets/Art/UI/ui_home_hero_ink_001.png`
- `UNITY/Assets/Art/UI/ui_home_hero_ink_002.png`
- `UNITY/Assets/Art/UI/ui_btn_home_primary_001.png`
- `UNITY/Assets/Art/UI/ui_btn_home_secondary_001.png`
- `UNITY/Assets/Art/UI/ui_home_logo_seal_001.png`
- `UNITY/Assets/Art/UI/ui_home_info_tag_001.png`
- `UNITY/Assets/Art/UI/ui_icon_codex_001.png`
- `UNITY/Assets/Art/UI/ui_icon_settings_001.png`
- `UNITY/Assets/Art/UI/ui_icon_exit_001.png`

同名文件也已同步到：

- `UNITY/Assets/Resources/Art/UI/`

## 4. 还没做完

### 4.1 需要继续补的正式美术

- 战斗中景层 `scene_battle_cloudsea_mid_001.png`
- 战斗近景层 `scene_battle_cloudsea_near_001.png`
- 更贴近黑白水墨方向的 Boss 二稿 `boss_tianjie_halfbody_002.png`
- 路口页正式路线图卡；当前已有非生图流程版，可先继续验证交互读感
- 首页图标可在生图接口稳定后换成 AI 二稿；当前本地绘制版可先用于 DEMO
- 现有首境页图片可继续二次校色，但当前已有基础可用版本

### 4.2 还可继续优化的路口与奖励页

- 需要进 Unity 实际看 3 张路口卡的高度、文字是否溢出、节点芯片是否太小
- 路口流程芯片目前最多展示 4 个节点，若后续路线更长，需要加折叠 / 省略态
- 奖励页普通三选一仍可继续向“少文字、强识别、明确取舍”靠拢

### 4.3 还可继续优化的战斗表现

- 需要实际进 Unity 截图检查各层级是否有遮挡、过暗或文字拥挤
- Boss 半身二稿生成后，需要验证终局 Boss 的压迫感是否统一到黑白水墨主线
- 若后续接入真实中景 / 近景图层，需要重新校准程序雾层透明度，避免画面过重

## 5. 下次继续做什么

建议顺序：

1. 非生图继续：进 Unity 走一遍 `启动页 -> 根脚 -> 携行之物 -> 首境 -> 路口 -> 战斗`，优先检查路口卡流程芯片和战斗 HUD 是否拥挤
2. 非生图继续：把普通奖励三选一也改成更明确的“取舍提示 + 构筑收益”结构
3. 生图恢复后：生成并入库 `scene_battle_cloudsea_mid_001.png`、`scene_battle_cloudsea_near_001.png` 和 `boss_tianjie_halfbody_002.png`
4. 生图恢复后：补路口页正式路线图卡，再根据截图校准程序雾层、敌方锚点和 HUD 密度

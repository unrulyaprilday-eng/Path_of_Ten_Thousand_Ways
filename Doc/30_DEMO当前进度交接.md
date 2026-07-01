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
- 路口页隐藏持续构筑摘要侧栏，避免视觉上像重复选主流派
- 开局三页与路口页关闭 Hover 详情弹窗，减少大段文字干扰

关键文件：

- `UNITY/Assets/Scripts/UI/DemoRuntimeCanvasUI.cs`

### 2.5 战斗场景分层优化已接入

已完成：

- 战斗场景视觉分层入口已接上
- 战斗舞台不再只按单一 Boss 压迫感表现
- 已加入按节点层级区分的战斗压迫感逻辑

关键文件：

- `UNITY/Assets/Scripts/UI/DemoBattleSceneView.cs`

当前效果目标：

- 普通战：克制、留白多
- 精英战：压迫感略升
- 小Boss：镜头更靠前，敌方更重
- 大Boss：只在真正 Boss 战里给完整压迫感

### 2.6 编译验证

最近一次编译结果：

```text
dotnet build UNITY/CompileCheck/CompileCheck.csproj
```

结果：

- `0 warnings`
- `0 errors`

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

已对接资源位：

- `UNITY/Assets/Art/Scenes/scene_battle_cloudsea_001.png`
- `UNITY/Assets/Art/Scenes/scene_cloudsea_far_001.png`
- `UNITY/Assets/Art/Scenes/scene_battle_cloudsea_mid_001.png`
- `UNITY/Assets/Art/Scenes/scene_battle_cloudsea_near_001.png`
- `UNITY/Assets/Art/Boss/boss_tianjie_halfbody_001.png`
- `UNITY/Assets/Art/Boss/boss_tianjie_halfbody_002.png`

## 4. 还没做完

### 4.1 需要继续补的正式美术

- 主首页首屏背景
- 战斗场景层进一步校色
- 路口页与首境页的正式补图
- 更完整的 Boss 立绘二稿

### 4.2 还可继续优化的战斗表现

- 战斗中精英 / 小Boss 的读感还可以再拉开
- 敌方锚点、镜头压迫和色调可继续细化
- 战斗 HUD 还能再进一步区分“常规战”和“高压战”

## 5. 下次继续做什么

建议顺序：

1. 继续补战斗场景层和 Boss 二稿
2. 补首境页正式场景图
3. 补路口页正式路线图卡
4. 再统一走一遍 `启动页 -> 根脚 -> 携行之物 -> 首境 -> 路口 -> 战斗` 的完整视觉节奏

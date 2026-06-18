# Path of Ten Thousand Ways - Scene Layer Requirements

这份清单用于补齐当前 DEMO 最缺的场景层资源。

当前问题：

- `DemoBattleSceneView` 仍以整张背景图 + 程序色块为主。
- 远景 / 中景 / 近景缺少可单独漂移的真实图层。
- 战斗场景与奖励 / 整备场景共用同一舞台气质，节点切换时辨识度不够。

本批次目标：

- 先做可直接接入 Unity 的三层场景资源。
- 优先解决战斗主舞台的景深问题。
- 同时为奖励 / 整备节点预留独立的中近景资源，让两个场景在气质上拉开。
- 若当前 Codex 会话可直接使用 `imagegen`，默认直接进入生成，不把这批图层错误延后为“等工具环境补齐再说”。

当前新增镜头要求：

- 战斗舞台中间演算区要比上一版更开阔，左右 framing 更靠边，不要挤压中场。
- 玩家侧镜头更低、更近，允许偏背身视角；敌方位置更高，更像空中压制。

## 资源优先级

| 优先级 | 文件名 | 用途 | 接入点 | 状态 |
| --- | --- | --- | --- | --- |
| P0 | `scene_cloudsea_far_001.png` | 共用远景层，负责天穹、远山、云海地平线 | 战斗 / 奖励共用 far layer | 待生成 |
| P0 | `scene_battle_cloudsea_mid_001.png` | 战斗中景，负责浮空山门、裂隙、远处战场结构 | 战斗 scene set mid layer | 待生成 |
| P0 | `scene_battle_cloudsea_near_001.png` | 战斗近景，负责左右断崖、前景雾带、压迫式 framing | 战斗 scene set near layer | 待生成 |
| P1 | `scene_reward_cloudaltar_mid_001.png` | 奖励 / 修炼 / 商店中景，负责云台、古剑台、静态器面感 | 非战斗 scene set mid layer | 待生成 |
| P1 | `scene_reward_cloudaltar_near_001.png` | 奖励 / 修炼 / 商店近景，负责前景卷雾、石台边缘、引导视线 | 非战斗 scene set near layer | 待生成 |

## 统一风格约束

```text
暗黑水墨修仙 + 卷轴式商业化 UI + 可读性优先的 2.5D 横版斗法舞台
```

统一要求：

- 国风、修仙、水墨、暗黑奇幻。
- 不做二次元萌系，不做页游金闪闪。
- 中央演出区必须留空，不能堵住飞剑轨迹和伤害字。
- 图层应服务景深和舞台 framing，不抢 UI。
- 同一批资源要保持同一时辰、同一气候体系和同一色温逻辑。
- 整体画风以黑白水墨和淡墨灰阶为主，雷电或血煞只做局部点色。
- 不做高饱和蓝绿大光效覆盖整场。

## 透明输出要求

除 `scene_cloudsea_far_001.png` 外，其余图层默认按“可去底图层”处理：

- 生成时使用纯色抠图底。
- 导出为带 alpha 的 PNG。
- 图层主体尽量集中在本层负责的区域，避免无意义的大面积残留。

推荐抠图底色：

- 默认 `#00ff00`
- 若主体含明显绿色，改用 `#ff00ff`

## Prompt 01

### 文件

`UNITY/Assets/Art/Scenes/scene_cloudsea_far_001.png`

### 说明

- 战斗和奖励场景共用远景。
- 负责天空、月光、远山和云海地平线。
- 应是最慢漂移的一层。

### 建议尺寸

`1920x1080`

### Prompt

```text
Use case: stylized-concept
Asset type: game parallax background far layer
Primary request: create the far background layer for a Chinese cultivation air-duel game scene
Scene/backdrop: high-altitude moonlit cloud sea, distant mountains, storm sky, open horizon
Subject: only the far layer elements, including sky, far cloud sea, distant mountain silhouettes, remote floating landforms, no foreground cliffs
Style/medium: dark Chinese fantasy environment art, ink wash painting blended with premium 2.5D game background rendering
Composition/framing: wide 16:9 side-view layer for parallax, center kept open, strong readable horizon line, no large foreground silhouettes
Lighting/mood: cold moonlit storm atmosphere, mysterious, solemn, slightly oppressive
Color palette: black, deep blue-gray, ink green, moon silver, restrained warm mist near the lower horizon
Materials/textures: layered ink clouds, distant ridges, atmospheric haze, subtle lightning glow, brush-textured sky
Constraints: no characters, no UI, no text, no watermark, no near objects, no giant structures covering the center
Avoid: bright daytime landscape, photoreal western fantasy look, heavy foreground framing, cluttered composition
```

当前补充要求：

- 中央必须给飞剑轨迹、伤害字和结算特效留出更大的空场。

## Prompt 02

### 文件

`UNITY/Assets/Art/Scenes/scene_battle_cloudsea_mid_001.png`

### 说明

- 战斗场景专用中景。
- 负责浮空山门、裂隙、破碎平台、法术飞行带。
- 应能明显区分于奖励节点的静态氛围。

### 建议尺寸

`1920x1080`

### Prompt

```text
Use case: stylized-concept
Asset type: game parallax background mid layer
Primary request: create the combat midground layer for a Chinese cultivation air-duel battle scene on a removable chroma-key background
Scene/backdrop: perfectly flat solid #00ff00 chroma-key background for background removal
Subject: only midground elements such as floating mountain shelves, broken immortal platforms, storm-torn cloud bands, distant ruined pavilions, and aerial duel structures
Style/medium: dark Chinese fantasy environment layer art, ink wash painting blended with premium game scene rendering
Composition/framing: wide 16:9 side-view parallax layer, leave the center combat lane readable, stronger structures on the left and right, enough negative space for sword trails
Lighting/mood: cold storm light, dangerous, tense, active combat atmosphere
Color palette: charcoal, storm blue, ink cyan, muted stone gray, tiny restrained gold traces
Materials/textures: cracked stone platforms, ink clouds, Taoist ruins, torn mist ribbons, subtle lightning illumination
Constraints: perfectly flat solid #00ff00 background only, no gradients in the background, no characters, no UI, no text, no watermark
Avoid: full scenic painting, near-camera cliffs, giant centered palace, green elements that match the key color
```

## Prompt 03

### 文件

`UNITY/Assets/Art/Scenes/scene_battle_cloudsea_near_001.png`

### 说明

- 战斗场景专用近景。
- 负责左右前景断崖、前景雾、舞台包边。
- 这一层要把“斗法舞台”框出来。

### 建议尺寸

`1920x1080`

### Prompt

```text
Use case: stylized-concept
Asset type: game parallax background near layer
Primary request: create the foreground framing layer for a Chinese cultivation air-duel battle scene on a removable chroma-key background
Scene/backdrop: perfectly flat solid #00ff00 chroma-key background for background removal
Subject: only near foreground elements, including dark cliff lips, hanging stone ledges, dense foreground mist, torn ink fog, and subtle platform edge glow framing the left and right sides
Style/medium: premium dark Chinese fantasy layer art, ink wash plus atmospheric 2.5D game foreground
Composition/framing: wide 16:9 side-view layer, heavy framing on left and right lower corners, center lower area partially open for units and damage numbers
Lighting/mood: oppressive, heavy, cinematic, foreground silhouettes with slight rim lighting
Color palette: near-black, deep blue-gray, ash gray mist, tiny warm gold ember traces
Materials/textures: wet stone, brush-ink fog, broken cliff edges, worn ancient platform surfaces
Constraints: perfectly flat solid #00ff00 background only, no characters, no UI, no text, no watermark
Avoid: full environment background, bright saturation, symmetrical toy-like staging, green subject matter
```

## Prompt 04

### 文件

`UNITY/Assets/Art/Scenes/scene_reward_cloudaltar_mid_001.png`

### 说明

- 奖励 / 修炼 / 商店场景专用中景。
- 负责让“战后补强”从战斗舞台切换到云台、剑冢、古修整备空间。

### 建议尺寸

`1920x1080`

### Prompt

```text
Use case: stylized-concept
Asset type: game parallax background mid layer
Primary request: create the utility-node midground layer for a Chinese cultivation reward and preparation scene on a removable chroma-key background
Scene/backdrop: perfectly flat solid #00ff00 chroma-key background for background removal
Subject: only midground elements such as suspended stone altars, sword tomb silhouettes, ritual platforms, incense haze, distant Taoist pavilion fragments, calmer than battle but still solemn
Style/medium: dark Chinese fantasy environment layer art, ink wash painting with premium UI scene sensibility
Composition/framing: wide 16:9 side-view parallax layer, center readable for reward cards and text, structures slightly lower and calmer than the combat version
Lighting/mood: quieter, contemplative, post-battle, mysterious, restrained sacred atmosphere
Color palette: charcoal, ink blue, desaturated jade, muted warm lantern gold, ash mist
Materials/textures: carved stone altars, ancient sword racks, cloud wisps, faint talisman smoke, worn ritual architecture
Constraints: perfectly flat solid #00ff00 background only, no characters, no UI, no text, no watermark
Avoid: active combat lightning, giant centered building, crowded scene, green subject matter
```

## Prompt 05

### 文件

`UNITY/Assets/Art/Scenes/scene_reward_cloudaltar_near_001.png`

### 说明

- 奖励 / 修炼 / 商店场景专用近景。
- 负责前景云台边缘、卷雾、器面包边。

### 建议尺寸

`1920x1080`

### Prompt

```text
Use case: stylized-concept
Asset type: game parallax background near layer
Primary request: create the foreground framing layer for a Chinese cultivation reward and preparation scene on a removable chroma-key background
Scene/backdrop: perfectly flat solid #00ff00 chroma-key background for background removal
Subject: only near foreground elements such as ritual stone edges, low cloud rolls, incense smoke, hanging talisman fragments, and soft framing masses along the lower left and lower right
Style/medium: premium dark Chinese fantasy foreground layer, ink wash plus atmospheric game rendering
Composition/framing: wide 16:9 side-view layer, center kept open for reward cards, quieter and cleaner than the battle foreground
Lighting/mood: contemplative, dim, sacred, after-battle stillness
Color palette: near-black, smoke gray, muted jade, old bronze, tiny warm paper-gold accents
Materials/textures: aged stone, scroll smoke, talisman ash, carved ritual edges
Constraints: perfectly flat solid #00ff00 background only, no characters, no UI, no text, no watermark
Avoid: battle cliffs, huge weapon silhouettes in center, bright glow overload, green subject matter
```

## 接入顺序

1. 先接 `scene_cloudsea_far_001.png` + `scene_battle_cloudsea_mid_001.png` + `scene_battle_cloudsea_near_001.png`
2. 用它们替掉 `DemoBattleSceneView` 中的大部分程序矩形层
3. 再接 `scene_reward_cloudaltar_mid_001.png` + `scene_reward_cloudaltar_near_001.png`
4. 让奖励 / 修炼 / 商店节点切到独立 scene set

## 验收标准

- 玩家一眼能看出远景 / 中景 / 近景不是一张图压黑。
- 战斗节点与奖励 / 整备节点切换时，舞台气质明显变化。
- 中央演出区和卡牌区仍然保持可读。
- 近景不会吞掉角色和飞剑轨迹。

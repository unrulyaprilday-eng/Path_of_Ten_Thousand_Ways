# Path of Ten Thousand Ways - AI Batch 01

这是一批面向 DEMO 当前版本的第一轮真实资源生成包。

目标：

- 优先替换最影响卖相的 20% 表皮资源。
- 服务当前运行时 Canvas UI 和战斗主舞台。
- 不追求全量正式资产，只追求“先把原型味压下去”。
- 若当前 Codex 会话可直接使用 `imagegen`，默认优先用它生成这批资源，不等待外部模型站点或额外配置。

当前建议先生成 8 张：

1. `ui_panel_scroll_dark_001.png` - 已完成并入库
2. `ui_frame_card_001.png` - 已完成并入库
3. `scene_battle_cloudsea_001.png` - 已完成并入库（2026-06-12）
4. `boss_tianjie_halfbody_001.png` - 已完成并入库（2026-06-12）
5. `ui_home_hero_ink_001.png` - 待生成
6. `ui_path_wanjian_001.png` - 已入库（2026-06-15）
7. `ui_path_thunder_001.png` - 已入库（2026-06-15）
8. `ui_path_blood_001.png` - 已入库（2026-06-15）

起手道途选择页新增 P0 候选：

5. `ui_path_wanjian_001.png`
6. `ui_path_thunder_001.png`
7. `ui_path_blood_001.png`

## 通用风格约束

核心风格：

```text
暗黑水墨修仙 + 卷轴 UI + 雷劫压迫感 + 高级商业化游戏界面质感
```

统一要求：

- 国风、修仙、水墨、暗黑奇幻
- 避免二次元萌系、Q 版、页游感
- 画面应偏神秘、肃杀、孤绝、飞升压迫感
- 细节尽量高级克制，不要廉价金边堆砌
- 尽量给出明确材质层次：旧绢纸、墨染、鎏金、玉石、雷光
- 所有 UI 类资源都不要出现英文、数字、水印、签名
- 所有资源都不要出现现代图标风格、扁平商务插画风格
- 当前新增方向：黑白水墨主调优先于纯暗黑压色
- 当前新增方向：起手道途牌按纵向展示海报做，不按通用卡框做

推荐英文风格锚点：

```text
Dark Chinese Fantasy, Ink Wash Painting, Ancient Scroll UI, Cultivation World, Taoist Mystic, premium game interface art, cinematic atmospheric lighting
```

## 资源 01

### 文件

`UNITY/Assets/Art/UI/ui_panel_scroll_dark_001.png`

### 用途

- 左侧道途栏
- 右侧构筑栏
- 中部信息面板
- 通用深色卷轴底板

### 建议尺寸

`1024x1024`

### Prompt

```text
Use case: stylized-concept
Asset type: game UI panel background
Primary request: create a premium dark Chinese cultivation scroll panel background for a game interface
Scene/backdrop: flat UI asset, no environment, no perspective scene
Subject: ancient scroll-like panel surface with layered dark paper, subtle ink wash, restrained gold trim, faint Taoist talisman texture
Style/medium: premium game UI concept art, dark ink wash painting, Chinese fantasy interface material study
Composition/framing: centered square texture sheet, suitable for slicing and cropping into multiple UI panels, symmetrical visual balance, generous clean center area for overlaid text
Lighting/mood: low-key cinematic ambient lighting, moody, mystical, controlled contrast
Color palette: black, deep gray, ink blue, muted warm brown, restrained gold accents
Materials/textures: aged silk paper, brushed ink stains, worn lacquer, subtle gilded edge dust, faint cloud motif
Constraints: no text, no symbols that dominate the center, no watermark, no character, no weapons, no dramatic perspective
Avoid: flat mobile UI look, glossy sci-fi interface, bright saturation, ornamental overload, beige parchment cartoon look
```

### 期望结果

- 中间区域相对干净，方便叠字和按钮
- 四周边缘有层次，但不能太抢戏
- 更像“器面底板”，不是完整插画

## 资源 02

### 文件

`UNITY/Assets/Art/UI/ui_frame_card_001.png`

### 用途

- 手牌卡面外框
- 奖励卡框变体参考
- 功法 / 神通 / 法宝卡片的基础框体

### 建议尺寸

`512x768`

### Prompt

```text
Use case: stylized-concept
Asset type: collectible card frame
Primary request: create a premium Chinese cultivation card frame for a dark fantasy deckbuilding game
Scene/backdrop: isolated UI asset on neutral dark background
Subject: vertical card frame with ancient scroll structure, polished but restrained gold edging, ink-wash paper core, corners shaped like Taoist artifact hardware
Style/medium: premium collectible game UI art, dark Chinese fantasy, ink and metal hybrid styling
Composition/framing: portrait 5:7 card frame, clear top title zone, mid illustration window, lower rules area, top-left cost area, top-right rarity mark area
Lighting/mood: refined, premium, mystical, elegant but dangerous
Color palette: dark charcoal, warm paper tan, muted gold, traces of cyan and crimson allowed only as tiny accent hints
Materials/textures: old silk scroll, brushed bronze, talisman lacquer, carved edge details, subtle cloud motif embossing
Constraints: no text, no iconography in the illustration window, keep central illustration area readable, no watermark
Avoid: bright fantasy gacha style, anime card frame, mobile RPG candy colors, toy-like proportions, overdecorated clutter
```

### 期望结果

- 上中下区域结构非常明确
- 中间插画区要留够空间
- 边框看起来像“可量产的统一框”，不是一次性海报

## 资源 03

### 文件

`UNITY/Assets/Art/Scenes/scene_battle_cloudsea_001.png`

### 用途

- 普通战斗主背景
- 当前 `DemoBattleSceneView` 的正式替代底图

### 建议尺寸

`1920x1080`

### Prompt

```text
Use case: stylized-concept
Asset type: game battle background
Primary request: create a side-view battle background for a Chinese cultivation air duel above a dark ink-wash cloud sea
Scene/backdrop: high-altitude cloud sea, broken mountain silhouettes, distant storm light, open midair duel space
Subject: empty battle stage environment for two floating cultivators, with clear foreground, midground, and background separation
Style/medium: dark Chinese fantasy environment art, ink wash painting blended with premium 2.5D game background rendering
Composition/framing: wide 16:9 side-view game background, left and right combat lanes readable, center kept visually open for sword trails and VFX
Lighting/mood: cold moonlit storm atmosphere with subtle warm undertone near the lower mist, mysterious and oppressive
Color palette: black, deep blue-gray, ink green, storm cyan, tiny muted gold warmth in the lower haze
Materials/textures: layered ink clouds, distant mountain ridges, drifting mist, subtle calligraphic brush texture, soft lightning glow in the far sky
Constraints: no characters, no UI, no text, no watermark, leave enough negative space for combat readability
Avoid: bright daytime sky, photoreal western fantasy landscape, cluttered composition, huge foreground objects blocking the center
```

### 期望结果

- 中央必须留空，方便飞剑和伤害字
- 左右两边要有站位感
- 不要让背景比战斗演出更抢

## 资源 04

### 文件

`UNITY/Assets/Art/Boss/boss_tianjie_halfbody_001.png`

### 用途

- 天劫化身 Boss 立绘
- Boss 战阶段切换和预警区域的视觉锚点

### 建议尺寸

`1024x1024`

### Prompt

```text
Use case: stylized-concept
Asset type: boss portrait
Primary request: create a half-body portrait of the final boss Tianjie Incarnation for a dark Chinese cultivation roguelike
Scene/backdrop: abstract storm void with ink thunder clouds, no concrete environment details
Subject: a terrifying heavenly tribulation incarnation, humanoid but not fully human, composed of Taoist wrath, thunder, cloud, judgment, and ancient celestial authority
Style/medium: premium Chinese fantasy boss concept art, ink wash plus high-detail game key art
Composition/framing: centered half-body portrait, readable silhouette, suitable for UI placement and boss warning scenes
Lighting/mood: divine oppression, cold thunder light, ominous sacred violence, majestic and terrifying
Color palette: black, storm blue, silver-white lightning, dark crimson trace, restrained gold-celestial highlights
Materials/textures: storm robes, talisman fragments, heavenly seals, cloud smoke, crackling lightning veins, aged celestial armor accents
Constraints: no extra characters, no full body action pose, no watermark, no readable text, keep silhouette strong and iconic
Avoid: anime villain face, generic demon king, western armor fantasy, oversexualized design, cartoon thunder effects
```

### 期望结果

- 要有“天道化身”的压迫感，不是普通妖魔
- 轮廓必须强，远看就有识别性
- 后面方便裁进 UI，而不是只能当海报

## 导入建议

生成完后建议按以下方式处理：

1. UI 框类资源先保留原图，再做一份切片/裁切版本。
2. 背景图先不要压太狠，先看 Unity 里实际显示效果。
3. Boss 图优先保留透明感背景版本，哪怕暂时不是透明 PNG，也尽量让背景足够干净。

## 第二批候选

第一批确认风格成立后，再进入：

- `ui_frame_reward_001.png`
- `ui_frame_boss_warning_001.png`
- `icon_energy_001.png`
- `icon_sword_intent_001.png`
- `icon_shock_001.png`
- `icon_bleed_001.png`
- `scene_battle_thundercloud_001.png`

## 起手道途三联牌

这组三联牌服务当前 DEMO 的首屏门面，不是普通奖励卡换色。

统一要求：

- 竖版大幅展示卡。
- 黑白水墨为主，留白明显。
- 不做持续压暗的暗黑整屏。
- 每张牌只保留流派大字印、气质主视觉和 3 个特点承载区。
- 适合在 Unity 中叠加轻微飘带、粒子、雷屑、墨点动效。

补充要求：

- 当前这组三联牌是对外展示 DEMO 的首屏门面资源，优先级高于继续微调程序占位 UI。
- 纵向感必须强，第一眼更像“章节门面 / 道途海报”，而不是战斗手牌。
- 三张图生成后，建议优先进入 `Resources/Art/UI/` 或其他可直接被运行时加载的目录。

## 主页面首屏图

### 文件

`UNITY/Assets/Art/UI/ui_home_hero_ink_001.png`

### Prompt

```text
Use case: stylized-concept
Asset type: game home screen hero background
Primary request: create a premium black-and-white ink wash hero image for the home screen of a Chinese cultivation roguelike deckbuilding game
Scene/backdrop: vast misty mountain realm, monumental heavenly presence, open cloud sea, dramatic empty atmosphere
Subject: a lone cultivator in the foreground and a colossal heavenly beast or dragon-like tribulation presence above the mountains, conveying destiny and pressure rather than direct combat
Style/medium: premium Chinese fantasy key art, black-and-white ink wash painting with restrained cinnabar and pale jade accent traces
Composition/framing: wide 16:9 composition, strong left-top brand area, open center space for menu/UI, clear foreground-midground-background separation
Lighting/mood: solemn, mythic, elegant, high-end, mysterious, not muddy dark
Color palette: white mist, charcoal ink, soft stone gray, tiny restrained cinnabar accents
Materials/textures: xuan paper bloom, brush fog, mountain wash, calligraphic edge breakup
Constraints: no watermark, no readable text, no modern props, no anime poster layout, no UI baked into the art
Avoid: over-dark muddy fantasy splash art, bright gacha colors, full combat explosion scene
```

补充说明：

- 这张图是品牌首屏，不是战斗截图。
- 重点是世界观压迫感、门面气质和留白，不是动作数量。

### 文件

`UNITY/Assets/Art/UI/ui_path_wanjian_001.png`

### Prompt

```text
Use case: stylized-concept
Asset type: large path selection card for a game start screen
Primary request: create a premium black-and-white ink wash cultivation path card for the Wanjian sword path
Scene/backdrop: isolated vertical UI showcase card, no environment perspective
Subject: flowing ink composition centered on flying swords, sword marks, blade arcs, and a bold calligraphic sword seal, designed as a distinct start-path selection panel
Style/medium: premium Chinese fantasy game UI art, black-and-white ink wash with restrained gold accents
Composition/framing: tall vertical composition with clear title zone, central hero ink motif, lower area reserved for three short feature callouts
Lighting/mood: elegant, sharp, disciplined, rising momentum
Color palette: black, white, soft paper gray, restrained antique gold
Materials/textures: xuan paper, brushed ink edges, sword trail scratches, lacquered seal hints
Constraints: no readable UI text, no watermark, no anime character portrait, no clutter
Avoid: dark muddy full-card shading, generic card frame look, bright fantasy gacha colors
```

### 文件

`UNITY/Assets/Art/UI/ui_path_thunder_001.png`

### Prompt

```text
Use case: stylized-concept
Asset type: large path selection card for a game start screen
Primary request: create a premium black-and-white ink wash cultivation path card for the Thunder sword path, but the visual identity must read as thunder talisman and heavenly lightning first, sword second
Scene/backdrop: isolated vertical UI showcase card, no environment perspective
Subject: storm ink composition with a symbolic upper-middle thunder sigil inspired by Taoist thunder talisman glyph strokes, natural branching lightning, celestial crack lines, and sky-splitting energy, designed as a distinct start-path selection panel with minimal sword reading
Style/medium: premium Chinese fantasy game UI art, black-and-white ink wash with restrained cyan-blue accents
Composition/framing: tall vertical composition with clear title zone, upper-middle symbolic thunder sigil as the hero motif, lower area reserved for three short feature callouts, do not center the composition on a full sword body
Lighting/mood: tense, explosive, heavenly pressure, volatile
Color palette: black, white, cold gray, restrained cyan-blue
Materials/textures: xuan paper, talisman brush strokes, lightning scratches, misted ink bloom, celestial crack lines
Constraints: no readable UI text, no watermark, no anime character portrait, no clutter, no dominant central sword silhouette
Avoid: neon sci-fi lightning, overpainted purple fantasy style, muddy dark card, repeated blade shapes, weapon-poster composition, sword array replacing the thunder sigil
```

### 文件

`UNITY/Assets/Art/UI/ui_path_blood_001.png`

### Prompt

```text
Use case: stylized-concept
Asset type: large path selection card for a game start screen
Primary request: create a premium black-and-white ink wash cultivation path card for the Blood sword path
Scene/backdrop: isolated vertical UI showcase card, no environment perspective
Subject: violent but elegant ink composition with blood ribbons, fractured sword traces, crimson mist, and a bold calligraphic blood seal, designed as a distinct start-path selection panel
Style/medium: premium Chinese fantasy game UI art, black-and-white ink wash with restrained cinnabar red accents
Composition/framing: tall vertical composition with clear title zone, central blood path hero motif, lower area reserved for three short feature callouts
Lighting/mood: dangerous, sacrificial, intense, seductive but harsh
Color palette: black, white, paper gray, restrained cinnabar red
Materials/textures: xuan paper, torn ink edges, drifting ribbon streaks, blood sand specks
Constraints: no readable UI text, no watermark, no anime character portrait, no gore realism, no clutter
Avoid: horror gore poster, bright red overfill, generic demon card look
```

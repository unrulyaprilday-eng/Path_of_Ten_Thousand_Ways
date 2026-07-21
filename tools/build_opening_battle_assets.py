from __future__ import annotations

import math
import random
from pathlib import Path

from PIL import Image, ImageChops, ImageDraw, ImageEnhance, ImageFilter, ImageOps


ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / "UNITY" / "Assets" / "Art"
RESOURCES = ROOT / "UNITY" / "Assets" / "Resources" / "Art"


def cover(image: Image.Image, size: tuple[int, int]) -> Image.Image:
    source = image.copy()
    ratio = max(size[0] / source.width, size[1] / source.height)
    resized = source.resize(
        (round(source.width * ratio), round(source.height * ratio)),
        Image.Resampling.LANCZOS,
    )
    left = (resized.width - size[0]) // 2
    top = (resized.height - size[1]) // 2
    return resized.crop((left, top, left + size[0], top + size[1]))


def save_pair(image: Image.Image, relative_path: str) -> None:
    for root in (ART, RESOURCES):
        path = root / relative_path
        path.parent.mkdir(parents=True, exist_ok=True)
        image.save(path, optimize=True)
        print(path.relative_to(ROOT).as_posix())


def build_background() -> None:
    source = Image.open(ART / "Scenes" / "scene_battle_old_mine_entry_001.png").convert("RGB")
    base = cover(source, (1920, 1080))
    base = ImageOps.autocontrast(base, cutoff=0.6)
    base = ImageEnhance.Color(base).enhance(0.72)
    base = ImageEnhance.Contrast(base).enhance(0.91)
    base = ImageEnhance.Brightness(base).enhance(1.10)

    warm = Image.new("RGB", base.size, (240, 234, 220))
    base = Image.blend(base, warm, 0.055)

    glow_mask = Image.new("L", base.size, 0)
    glow_pixels = glow_mask.load()
    cx, cy = 960.0, 410.0
    rx, ry = 540.0, 680.0
    for y in range(base.height):
        for x in range(base.width):
            distance = math.sqrt(((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2)
            glow_pixels[x, y] = round(max(0.0, 1.0 - distance) ** 2 * 52)

    glow = Image.new("RGB", base.size, (250, 246, 235))
    base = Image.composite(glow, base, glow_mask.filter(ImageFilter.GaussianBlur(46)))
    save_pair(base, "Scenes/scene_battle_old_mine_opening_far_002.png")

    mid_source = Image.open(ART / "Scenes" / "scene_battle_cloudsea_mid_001.png").convert("RGBA")
    mid = cover(mid_source, (1920, 1080))
    mid = tint_rgba(mid, (162, 157, 143), alpha_scale=0.20).filter(ImageFilter.GaussianBlur(1.2))
    save_pair(mid, "Scenes/scene_battle_old_mine_opening_mid_002.png")

    near_source = Image.open(ART / "Scenes" / "scene_battle_cloudsea_near_001.png").convert("RGBA")
    near = cover(near_source, (1920, 1080))
    near = tint_rgba(near, (51, 49, 45), alpha_scale=0.34).filter(ImageFilter.GaussianBlur(0.7))
    save_pair(near, "Scenes/scene_battle_old_mine_opening_near_002.png")


def tint_rgba(image: Image.Image, color: tuple[int, int, int], alpha_scale: float = 1.0) -> Image.Image:
    rgba = image.convert("RGBA")
    luminance = ImageOps.grayscale(rgba.convert("RGB"))
    low = tuple(max(0, round(channel * 0.24)) for channel in color)
    rgb = ImageOps.colorize(luminance, black=low, white=color)
    alpha = rgba.getchannel("A").point(lambda value: round(value * alpha_scale))
    rgb.putalpha(alpha)
    return rgb


def prepare_character(source_path: Path, tint: tuple[float, float, float]) -> Image.Image:
    image = Image.open(source_path).convert("RGBA")
    red, green, blue, alpha = image.split()
    red = red.point(lambda value: min(255, round(value * tint[0])))
    green = green.point(lambda value: min(255, round(value * tint[1])))
    blue = blue.point(lambda value: min(255, round(value * tint[2])))

    # The source portraits touch the lower crop. Dissolve the final section
    # into the generated ink wash so the original horizontal cut disappears.
    edge_blend = Image.new("L", image.size, 255)
    edge_pixels = edge_blend.load()
    start = max(0, image.height - 72)
    length = max(1, image.height - start - 1)
    for y in range(start, image.height):
        t = (y - start) / length
        smooth = t * t * (3.0 - 2.0 * t)
        value = round(255 * (1.0 - smooth))
        for x in range(image.width):
            edge_pixels[x, y] = value

    alpha = ImageChops.multiply(alpha, edge_blend.filter(ImageFilter.GaussianBlur(1.1)))
    return Image.merge("RGBA", (red, green, blue, alpha))


def polygon_mask(size: tuple[int, int], points: list[tuple[int, int]], blur: float = 0.0) -> Image.Image:
    scale = 3
    mask = Image.new("L", (size[0] * scale, size[1] * scale), 0)
    draw = ImageDraw.Draw(mask)
    draw.polygon([(x * scale, y * scale) for x, y in points], fill=255)
    mask = mask.resize(size, Image.Resampling.LANCZOS)
    return mask.filter(ImageFilter.GaussianBlur(blur)) if blur > 0.0 else mask


def textured_ink_layer(
    size: tuple[int, int],
    color: tuple[int, int, int],
    mask: Image.Image,
    seed: int,
    opacity: float = 1.0,
) -> Image.Image:
    randomizer = random.Random(seed)
    noise_size = (max(2, size[0] // 24), max(2, size[1] // 24))
    noise = Image.new("L", noise_size)
    noise.putdata([randomizer.randint(104, 160) for _ in range(noise_size[0] * noise_size[1])])
    noise = noise.resize(size, Image.Resampling.BICUBIC).filter(ImageFilter.GaussianBlur(4.5))

    low = tuple(max(0, round(channel * 0.68)) for channel in color)
    high = tuple(min(255, round(channel * 1.08)) for channel in color)
    layer = ImageOps.colorize(noise, black=low, white=high).convert("RGBA")
    layer.putalpha(mask.point(lambda value: round(value * opacity)))
    return layer


def add_ink_wash(
    canvas: Image.Image,
    points: list[tuple[int, int]],
    color: tuple[int, int, int],
    seed: int,
    opacity: float,
    source_height: int,
    guide_alpha: Image.Image,
    fade_in: int = 86,
) -> None:
    mask = polygon_mask(canvas.size, points, blur=4.2)
    envelope = Image.new("L", canvas.size, 0)
    envelope_pixels = envelope.load()
    fade_start = source_height - fade_in
    fade_peak = source_height - 8
    fade_out_start = source_height + 14
    fade_out_end = canvas.height - 6
    randomizer = random.Random(seed)
    row_noise_image = Image.new("L", (1, canvas.height))
    row_noise_image.putdata([randomizer.randint(188, 244) for _ in range(canvas.height)])
    row_noise_image = row_noise_image.filter(ImageFilter.GaussianBlur(9.0))
    row_noise = [0.82 + value / 255.0 * 0.22 for value in row_noise_image.getdata()]
    for y in range(canvas.height):
        if y < fade_start:
            strength = 0.0
        elif y < fade_peak:
            t = (y - fade_start) / max(1, fade_peak - fade_start)
            strength = t * t * (3.0 - 2.0 * t)
        elif y <= fade_out_start:
            strength = 1.0
        else:
            t = (y - fade_out_start) / max(1, fade_out_end - fade_out_start)
            t = max(0.0, min(1.0, t))
            strength = 1.0 - t * t * (3.0 - 2.0 * t)
        value = round(255 * strength * row_noise[y])
        for x in range(canvas.width):
            envelope_pixels[x, y] = value

    mask = ImageChops.multiply(mask, envelope.filter(ImageFilter.GaussianBlur(1.4)))

    guide = Image.new("L", canvas.size, 255)
    upper_guide = guide_alpha.filter(ImageFilter.MaxFilter(15)).filter(ImageFilter.GaussianBlur(4.0))
    guide.paste(upper_guide, (0, 0))
    guide_pixels = guide.load()
    blend_start = max(0, source_height - 28)
    for y in range(blend_start, min(source_height, canvas.height)):
        t = (y - blend_start) / max(1, source_height - blend_start - 1)
        smooth = t * t * (3.0 - 2.0 * t)
        for x in range(canvas.width):
            guide_pixels[x, y] = round(guide_pixels[x, y] * (1.0 - smooth) + 255 * smooth)
    mask = ImageChops.multiply(mask, guide)
    low_frequency = Image.new("L", (max(2, canvas.width // 42), max(2, canvas.height // 42)))
    low_frequency.putdata(
        [randomizer.randint(108, 255) for _ in range(low_frequency.width * low_frequency.height)]
    )
    low_frequency = low_frequency.resize(canvas.size, Image.Resampling.BICUBIC).filter(
        ImageFilter.GaussianBlur(6.0)
    )
    mask = ImageChops.multiply(mask, low_frequency)
    canvas.alpha_composite(textured_ink_layer(canvas.size, color, mask, seed + 1, opacity))


def extend_player_character(source_path: Path) -> Image.Image:
    guide_alpha = Image.open(source_path).convert("RGBA").getchannel("A")
    character = prepare_character(source_path, (1.02, 1.03, 1.08))
    canvas = Image.new("RGBA", (character.width, 465), (0, 0, 0, 0))
    add_ink_wash(
        canvas,
        [
            (184, 218), (421, 216), (425, 290), (418, 330), (410, 367),
            (397, 408), (380, 448), (361, 432), (343, 456), (324, 437),
            (305, 452), (286, 430), (266, 448), (246, 420), (226, 399),
            (210, 366), (198, 326),
        ],
        (29, 45, 70),
        seed=101,
        opacity=0.54,
        source_height=character.height,
        guide_alpha=guide_alpha,
    )
    add_ink_wash(
        canvas,
        [
            (240, 224), (388, 220), (382, 333), (361, 386), (342, 451),
            (322, 429), (301, 459), (282, 408), (260, 432),
        ],
        (155, 162, 164),
        seed=103,
        opacity=0.16,
        source_height=character.height,
        guide_alpha=guide_alpha,
        fade_in=74,
    )
    add_ink_wash(
        canvas,
        [
            (32, 235), (112, 232), (120, 287), (115, 326), (125, 365),
            (116, 402), (101, 425), (87, 397), (77, 361), (62, 329), (47, 286),
        ],
        (31, 49, 73),
        seed=109,
        opacity=0.30,
        source_height=character.height,
        guide_alpha=guide_alpha,
        fade_in=72,
    )
    add_ink_wash(
        canvas,
        [
            (106, 238), (180, 236), (188, 289), (181, 329), (190, 367),
            (181, 403), (165, 427), (150, 397), (142, 361), (127, 319),
        ],
        (34, 53, 78),
        seed=113,
        opacity=0.28,
        source_height=character.height,
        guide_alpha=guide_alpha,
        fade_in=70,
    )
    add_ink_wash(
        canvas,
        [
            (405, 229), (474, 225), (470, 283), (454, 322), (461, 357),
            (451, 392), (435, 418), (421, 390), (423, 354), (411, 307),
        ],
        (36, 55, 78),
        seed=127,
        opacity=0.27,
        source_height=character.height,
        guide_alpha=guide_alpha,
        fade_in=68,
    )

    canvas.alpha_composite(character, (0, 0))
    return canvas


def extend_enemy_character(source_path: Path) -> Image.Image:
    guide_alpha = Image.open(source_path).convert("RGBA").getchannel("A")
    character = prepare_character(source_path, (1.02, 0.99, 0.94))
    canvas = Image.new("RGBA", (character.width, 515), (0, 0, 0, 0))
    add_ink_wash(
        canvas,
        [
            (168, 268), (421, 264), (423, 350), (414, 387), (404, 428),
            (391, 471), (374, 502), (355, 482), (337, 507), (318, 486),
            (300, 502), (280, 477), (260, 498), (240, 467), (219, 442),
            (200, 407), (183, 365),
        ],
        (53, 57, 56),
        seed=151,
        opacity=0.55,
        source_height=character.height,
        guide_alpha=guide_alpha,
    )
    add_ink_wash(
        canvas,
        [
            (232, 278), (394, 270), (396, 354), (388, 397), (376, 444),
            (363, 485), (347, 466), (332, 491), (315, 464), (297, 482),
            (278, 450), (259, 470), (245, 427), (238, 382),
        ],
        (175, 174, 164),
        seed=157,
        opacity=0.17,
        source_height=character.height,
        guide_alpha=guide_alpha,
        fade_in=78,
    )
    add_ink_wash(
        canvas,
        [
            (118, 292), (187, 286), (192, 348), (183, 385), (190, 421),
            (181, 455), (166, 478), (153, 449), (145, 414), (130, 370),
        ],
        (49, 53, 52),
        seed=167,
        opacity=0.28,
        source_height=character.height,
        guide_alpha=guide_alpha,
        fade_in=72,
    )
    add_ink_wash(
        canvas,
        [
            (399, 270), (455, 266), (458, 326), (448, 363), (454, 399),
            (446, 432), (432, 456), (420, 427), (418, 392), (406, 348),
        ],
        (58, 64, 61),
        seed=173,
        opacity=0.27,
        source_height=character.height,
        guide_alpha=guide_alpha,
        fade_in=68,
    )
    add_ink_wash(
        canvas,
        [
            (438, 261), (492, 257), (493, 316), (483, 351), (489, 385),
            (479, 416), (465, 438), (453, 411), (456, 376), (445, 334),
        ],
        (67, 73, 68),
        seed=179,
        opacity=0.25,
        source_height=character.height,
        guide_alpha=guide_alpha,
        fade_in=64,
    )

    canvas.alpha_composite(character, (0, 0))
    return canvas


def build_characters() -> None:
    player = extend_player_character(
        ART / "Characters" / "char_player_sword_cultivator_battle_002.png"
    )
    save_pair(player, "Characters/char_player_sword_cultivator_battle_003.png")

    enemy = extend_enemy_character(
        ART / "Characters" / "char_enemy_tribulation_wraith_battle_002.png"
    )
    save_pair(enemy, "Characters/char_enemy_old_mine_wraith_battle_003.png")


def build_vfx() -> dict[str, Image.Image]:
    sword = tint_rgba(
        Image.open(ART / "VFX" / "vfx_flying_sword_001.png"),
        (250, 232, 181),
        1.0,
    )
    save_pair(sword, "VFX/vfx_opening_flying_sword_002.png")

    impact = tint_rgba(
        Image.open(ART / "VFX" / "vfx_sword_slash_001.png"),
        (246, 225, 172),
        0.84,
    )
    save_pair(impact, "VFX/vfx_opening_sword_impact_002.png")

    guard = tint_rgba(
        Image.open(ART / "VFX" / "vfx_impact_ink_burst_001.png"),
        (91, 157, 148),
        0.66,
    )
    save_pair(guard, "VFX/vfx_opening_guard_ink_001.png")

    smoke_path = ART / "UI" / "ui_root_smoke_wisp_001.png"
    cloud = tint_rgba(Image.open(smoke_path), (224, 229, 218), 0.74)
    save_pair(cloud, "VFX/vfx_opening_cloud_draw_001.png")
    return {"sword": sword, "impact": impact, "guard": guard, "cloud": cloud}


def paper_canvas(size: tuple[int, int], seed: int) -> Image.Image:
    randomizer = random.Random(seed)
    image = Image.new("RGBA", size, (226, 218, 199, 255))
    pixels = image.load()
    for y in range(size[1]):
        vertical = y / max(1, size[1] - 1)
        for x in range(size[0]):
            noise = randomizer.randint(-5, 5)
            pixels[x, y] = (
                max(0, min(255, round(235 - vertical * 18 + noise))),
                max(0, min(255, round(229 - vertical * 17 + noise))),
                max(0, min(255, round(214 - vertical * 15 + noise))),
                255,
            )
    return image.filter(ImageFilter.GaussianBlur(0.35))


def paste_center(canvas: Image.Image, source: Image.Image, center: tuple[int, int], scale: float, rotation: float) -> None:
    width = max(1, round(source.width * scale))
    height = max(1, round(source.height * scale))
    layer = source.resize((width, height), Image.Resampling.LANCZOS).rotate(
        rotation,
        Image.Resampling.BICUBIC,
        expand=True,
    )
    position = (round(center[0] - layer.width / 2), round(center[1] - layer.height / 2))
    canvas.alpha_composite(layer, position)


def build_card_art(vfx: dict[str, Image.Image]) -> None:
    size = (640, 420)

    slash = paper_canvas(size, 11)
    draw = ImageDraw.Draw(slash, "RGBA")
    draw.line((58, 348, 578, 76), fill=(92, 78, 52, 72), width=4)
    paste_center(slash, vfx["impact"], (342, 204), 0.82, -17)
    paste_center(slash, vfx["sword"], (358, 192), 0.62, -17)
    save_pair(slash, "Cards/card_art_sword_slash_001.png")

    guard = paper_canvas(size, 23)
    guard_draw = ImageDraw.Draw(guard, "RGBA")
    guard_draw.ellipse((176, 56, 468, 348), outline=(69, 122, 116, 105), width=7)
    guard_draw.ellipse((207, 86, 438, 318), outline=(197, 177, 122, 68), width=3)
    paste_center(guard, vfx["guard"], (322, 210), 0.66, 0)
    paste_center(guard, vfx["sword"], (322, 222), 0.42, 0)
    save_pair(guard, "Cards/card_art_guard_step_001.png")

    cloud = paper_canvas(size, 37)
    cloud_draw = ImageDraw.Draw(cloud, "RGBA")
    for index in range(5):
        y = 278 - index * 34
        cloud_draw.arc((54 + index * 22, y - 54, 594 - index * 16, y + 64), 196, 350, fill=(89, 125, 121, 66), width=3)
    paste_center(cloud, vfx["cloud"], (330, 218), 0.82, -3)
    paste_center(cloud, vfx["sword"], (388, 176), 0.34, -9)
    save_pair(cloud, "Cards/card_art_cloud_step_001.png")


def main() -> None:
    build_background()
    build_characters()
    vfx = build_vfx()
    build_card_art(vfx)


if __name__ == "__main__":
    main()

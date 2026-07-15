using System;
using System.IO;
using System.Linq;
using System.Reflection;
using PathOfTenThousandWays.Demo.Battle;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Rewards;
using PathOfTenThousandWays.Demo.Systems;
using UnityEditor;
using UnityEngine;

namespace PathOfTenThousandWays.Demo.EditorTools
{
    [InitializeOnLoad]
    public static class DemoPostBattleScreenshotTool
    {
        private const string ActiveKey = "PathOfTenThousandWays.PostBattleCapture.Active";
        private const string StageKey = "PathOfTenThousandWays.PostBattleCapture.Stage";
        private const int SettleFrames = 18;
        private const int CaptureWidth = 1920;
        private const int CaptureHeight = 1080;

        private static int waitFrames;
        private static CaptureSession captureSession;

        static DemoPostBattleScreenshotTool()
        {
            if (EditorPrefs.GetBool(ActiveKey, false))
            {
                EditorApplication.update -= UpdateCapture;
                EditorApplication.update += UpdateCapture;
            }
        }

        public static void Capture()
        {
            EditorPrefs.SetBool(ActiveKey, true);
            EditorPrefs.SetInt(StageKey, 0);
            waitFrames = 0;
            EditorApplication.update -= UpdateCapture;
            EditorApplication.update += UpdateCapture;
            EditorApplication.isPlaying = true;
        }

        private static void UpdateCapture()
        {
            if (!EditorPrefs.GetBool(ActiveKey, false))
            {
                EditorApplication.update -= UpdateCapture;
                return;
            }

            if (!EditorApplication.isPlaying)
            {
                CancelCaptureSession();
                if (EditorPrefs.GetInt(StageKey, 0) >= 99)
                {
                    Finish(0);
                }

                return;
            }

            try
            {
                if (captureSession != null)
                {
                    if (captureSession.Tick())
                    {
                        captureSession.Dispose();
                        captureSession = null;
                        AdvanceStage();
                        waitFrames = 0;
                    }

                    return;
                }

                DemoGameController controller = UnityEngine.Object.FindAnyObjectByType<DemoGameController>();
                if (controller == null)
                {
                    return;
                }

                Screen.SetResolution(CaptureWidth, CaptureHeight, false);
                if (++waitFrames < SettleFrames)
                {
                    return;
                }

                waitFrames = 0;
                int stage = EditorPrefs.GetInt(StageKey, 0);
                switch (stage)
                {
                    case 0:
                        AdvanceOpeningToBattle(controller);
                        AdvanceStage();
                        break;
                    case 1:
                        BeginCapture("battle_opening_realtime.png");
                        break;
                    case 2:
                        BeginCapture("battle_opening_realtime_1280x720.png", 1280, 720);
                        break;
                    case 3:
                        SkipOpeningBattle(controller);
                        AdvanceStage();
                        break;
                    case 4:
                        BeginCapture("post_battle_reward_commercial.png");
                        break;
                    case 5:
                        BeginCapture("post_battle_reward_commercial_1280x720.png", 1280, 720);
                        break;
                    case 6:
                        ClaimFirstAvailable(controller);
                        AdvanceStage();
                        break;
                    case 7:
                        BeginCapture("post_battle_route_commercial.png");
                        break;
                    case 8:
                        BeginCapture("post_battle_route_commercial_1280x720.png", 1280, 720);
                        break;
                    case 9:
                        StartMultiSwordPreview(controller);
                        AdvanceStage();
                        break;
                    case 10:
                        BeginCapture("battle_mid_multi_sword.png");
                        break;
                    case 11:
                        StartBossIntentPreview(controller);
                        AdvanceStage();
                        break;
                    case 12:
                        BeginCapture("battle_boss_intent.png");
                        break;
                    case 13:
                        controller.Battle.ClearBattle();
                        ClaimUtilityFirstRoute(controller);
                        AdvanceStage();
                        break;
                    case 14:
                        BeginCapture("post_battle_node_commercial.png");
                        break;
                    case 15:
                        ForceRunResult(controller, true);
                        AdvanceStage();
                        break;
                    case 16:
                        BeginCapture("run_result_victory.png");
                        break;
                    case 17:
                        controller.AdvanceUtilityNode();
                        ForceRunResult(controller, false);
                        AdvanceStage();
                        break;
                    case 18:
                        BeginCapture("run_result_defeat.png");
                        break;
                    case 19:
                        EditorPrefs.SetInt(StageKey, 99);
                        EditorApplication.isPlaying = false;
                        break;
                }
            }
            catch (Exception exception)
            {
                CancelCaptureSession();
                Debug.LogException(exception);
                Finish(1);
            }
        }

        private static void ClaimFirstAvailable(DemoGameController controller)
        {
            if (controller.CurrentRewards.Count == 0)
            {
                throw new InvalidOperationException("Expected at least one reward choice.");
            }

            controller.ClaimRewardAt(0);
        }


        private static void AdvanceOpeningToBattle(DemoGameController controller)
        {
            if (controller.Run.Map.CurrentNode.Type == DemoNodeType.Start && controller.CurrentRewards.Count == 0)
            {
                controller.AdvanceUtilityNode();
            }

            int guard = 8;
            while (!controller.HasBattle && guard-- > 0)
            {
                if (controller.CurrentRewards.Count == 0)
                {
                    throw new InvalidOperationException("Opening flow stopped before a selectable choice.");
                }

                int index = Enumerable.Range(0, controller.CurrentRewards.Count)
                    .FirstOrDefault(i => IsOpeningRewardAvailable(controller.CurrentRewards[i]));
                controller.ClaimRewardAt(index);
            }

            if (!controller.HasBattle)
            {
                throw new InvalidOperationException("Opening selections did not reach the realtime entry battle.");
            }
        }

        private static bool IsOpeningRewardAvailable(DemoReward reward)
        {
            if (reward == null)
            {
                return false;
            }

            if (reward.Type == DemoRewardType.Root)
            {
                return reward.Root != null && reward.Root.IsAvailable;
            }

            if (reward.Type == DemoRewardType.Vessel || reward.Type == DemoRewardType.Journey)
            {
                return reward.Vessel != null
                    ? reward.Vessel.IsAvailable
                    : reward.JourneyLine != null && reward.JourneyLine.IsAvailable;
            }

            if (reward.Type == DemoRewardType.OpeningScene)
            {
                return reward.Region != null && reward.Region.IsAvailable;
            }

            return true;
        }

        private static void SkipOpeningBattle(DemoGameController controller)
        {
            if (!controller.HasBattle)
            {
                throw new InvalidOperationException("Opening battle was not active before capture skip.");
            }

            InvokePrivate(controller, "HandleBattleWon", null);
            if (controller.CurrentRewards.Count != 3)
            {
                throw new InvalidOperationException("Opening battle did not enter the three-choice reward state.");
            }
        }

        private static void StartMultiSwordPreview(DemoGameController controller)
        {
            DemoCard summon = new DemoCard
            {
                Id = "capture_multi_sword",
                Name = "剑影分光",
                Cost = 0,
                Type = DemoCardType.FlyingSword,
                Style = DemoSwordStyle.Wanjian,
                Quality = DemoQuality.Earth,
                TemporarySwords = 6
            };
            controller.Battle.StartBattle(new DemoBattleSetup
            {
                Deck = new[] { summon },
                EnemyName = "劫云守卫",
                EnemyHealth = 9999,
                PlayerHealth = 72,
                PlayerMaxHealth = 72,
                InitialEnergy = 2,
                InitialHandSize = 1,
                DrawIntervalSeconds = 100f,
                FlyingSwordIntervalSeconds = 100f,
                EnemyIntentMinSeconds = 100f,
                EnemyIntentMaxSeconds = 100f,
                IntroSeconds = 0f,
                RandomSeed = 23
            });
            controller.Battle.TryPlayCard(0);
        }

        private static void StartBossIntentPreview(DemoGameController controller)
        {
            controller.Battle.ClearBattle();
            DemoCard phaseBreaker = new DemoCard
            {
                Id = "capture_phase_breaker",
                Name = "万剑破劫",
                Cost = 0,
                Damage = 2500,
                Type = DemoCardType.Finisher,
                Style = DemoSwordStyle.Wanjian,
                Quality = DemoQuality.Immortal
            };
            controller.Battle.StartBattle(new DemoBattleSetup
            {
                Deck = new[] { phaseBreaker },
                EnemyName = "天劫化身",
                EnemyHealth = 3800,
                IsBoss = true,
                PlayerHealth = 72,
                PlayerMaxHealth = 72,
                InitialEnergy = 2,
                InitialHandSize = 1,
                DrawIntervalSeconds = 100f,
                FlyingSwordIntervalSeconds = 100f,
                IntroSeconds = 0f,
                RandomSeed = 29
            });
            controller.Battle.TryPlayCard(0);
        }

        private static void ForceRunResult(DemoGameController controller, bool victory)
        {
            controller.Battle.ClearBattle();
            controller.Run.Map.CompleteWithResult(victory);
            SetPrivateField(controller, "lastReachedLayer", victory ? 3 : 2);
            InvokePrivate(controller, "FinishRun", new object[] { victory, victory });
        }

        private static void SetPrivateField(DemoGameController controller, string fieldName, object value)
        {
            FieldInfo field = typeof(DemoGameController).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new MissingFieldException(typeof(DemoGameController).FullName, fieldName);
            }

            field.SetValue(controller, value);
        }

        private static void InvokePrivate(DemoGameController controller, string methodName, object[] arguments)
        {
            MethodInfo method = typeof(DemoGameController).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
            {
                throw new MissingMethodException(typeof(DemoGameController).FullName, methodName);
            }

            method.Invoke(controller, arguments);
        }

        private static void ClaimUtilityFirstRoute(DemoGameController controller)
        {
            int index = Enumerable.Range(0, controller.CurrentRewards.Count)
                .FirstOrDefault(i =>
                {
                    DemoReward reward = controller.CurrentRewards[i];
                    return reward.RoutePlan != null
                        && reward.RoutePlan.Nodes.Count > 0
                        && reward.RoutePlan.Nodes[0].Type != DemoNodeType.Battle
                        && reward.RoutePlan.Nodes[0].Type != DemoNodeType.Boss;
                });
            controller.ClaimRewardAt(index);

            if (controller.HasBattle)
            {
                throw new InvalidOperationException("No utility-first route was available for the node screenshot.");
            }
        }


        private static void BeginCapture(string fileName)
        {
            BeginCapture(fileName, CaptureWidth, CaptureHeight);
        }

        private static void BeginCapture(string fileName, int width, int height)
        {
            if (captureSession != null)
            {
                throw new InvalidOperationException("A commercial UI capture is already in progress.");
            }

            Canvas canvas = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None)
                .FirstOrDefault(candidate => candidate.name == "DEMO_RuntimeCanvas");
            if (canvas == null)
            {
                throw new InvalidOperationException("Runtime canvas was not available for capture.");
            }

            string outputDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "tmp"));
            Directory.CreateDirectory(outputDirectory);
            string outputPath = Path.Combine(outputDirectory, fileName);
            captureSession = new CaptureSession(canvas, outputPath, width, height);
        }

        private static void CancelCaptureSession()
        {
            if (captureSession == null)
            {
                return;
            }

            captureSession.Dispose();
            captureSession = null;
        }

        private sealed class CaptureSession : IDisposable
        {
            private const int RequiredStableFrames = 8;
            private const int MaximumLayoutFrames = 120;
            private const int PixelSampleStride = 4;
            private const float DimensionTolerance = 1f;
            private const double MinimumRegionVariance = 4d;
            private const double MinimumRegionCoverage = 0.02d;
            private const int ClearColorDistanceSquared = 324;

            private readonly Canvas canvas;
            private readonly RectTransform canvasRect;
            private readonly string outputPath;
            private readonly int width;
            private readonly int height;
            private readonly RenderMode originalRenderMode;
            private readonly Camera originalCamera;
            private readonly float originalPlaneDistance;
            private readonly GameObject cameraObject;
            private readonly Camera captureCamera;
            private readonly RenderTexture renderTexture;
            private readonly Color clearColor = new Color(0.96f, 0.94f, 0.88f, 1f);

            private CapturePhase phase = CapturePhase.Prepare;
            private int layoutFrames;
            private int stableFrames;
            private int lastObservedFrame = -1;
            private bool disposed;

            public CaptureSession(Canvas canvas, string outputPath, int width, int height)
            {
                this.canvas = canvas;
                canvasRect = canvas.transform as RectTransform;
                this.outputPath = outputPath;
                this.width = width;
                this.height = height;
                originalRenderMode = canvas.renderMode;
                originalCamera = canvas.worldCamera;
                originalPlaneDistance = canvas.planeDistance;

                if (canvasRect == null)
                {
                    throw new InvalidOperationException("Runtime canvas did not have a RectTransform.");
                }

                cameraObject = new GameObject("DemoCommercialCaptureCamera");
                cameraObject.hideFlags = HideFlags.HideAndDontSave;
                captureCamera = cameraObject.AddComponent<Camera>();
                captureCamera.enabled = true;
                captureCamera.clearFlags = CameraClearFlags.SolidColor;
                captureCamera.backgroundColor = clearColor;
                captureCamera.orthographic = true;
                captureCamera.orthographicSize = 5f;
                captureCamera.nearClipPlane = 0.01f;
                captureCamera.farClipPlane = 100f;
                captureCamera.transform.position = new Vector3(0f, 0f, -10f);
                captureCamera.rect = new Rect(0f, 0f, 1f, 1f);
                captureCamera.aspect = width / (float)height;

                renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
                {
                    name = "DemoCommercialCaptureTexture",
                    antiAliasing = 1,
                    useMipMap = false,
                    autoGenerateMips = false,
                    hideFlags = HideFlags.HideAndDontSave
                };
                renderTexture.Create();
                captureCamera.targetTexture = renderTexture;
            }

            public bool Tick()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(nameof(CaptureSession));
                }

                switch (phase)
                {
                    case CapturePhase.Prepare:
                        Prepare();
                        phase = CapturePhase.WaitForLayout;
                        return false;
                    case CapturePhase.WaitForLayout:
                        WaitForLayout();
                        return false;
                    case CapturePhase.Capture:
                        Capture();
                        phase = CapturePhase.Restore;
                        return false;
                    case CapturePhase.Restore:
                        Dispose();
                        Debug.Log("Captured commercial UI screenshot: " + outputPath);
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                if (canvas != null)
                {
                    canvas.renderMode = originalRenderMode;
                    canvas.worldCamera = originalCamera;
                    canvas.planeDistance = originalPlaneDistance;
                    Canvas.ForceUpdateCanvases();
                }

                if (captureCamera != null)
                {
                    captureCamera.targetTexture = null;
                }

                if (renderTexture != null)
                {
                    renderTexture.Release();
                    UnityEngine.Object.DestroyImmediate(renderTexture);
                }

                if (cameraObject != null)
                {
                    UnityEngine.Object.DestroyImmediate(cameraObject);
                }
            }

            private void Prepare()
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = captureCamera;
                canvas.planeDistance = 1f;
                Canvas.ForceUpdateCanvases();
            }

            private void WaitForLayout()
            {
                if (Time.frameCount == lastObservedFrame)
                {
                    return;
                }

                lastObservedFrame = Time.frameCount;
                layoutFrames++;
                Canvas.ForceUpdateCanvases();

                string layoutIssue = GetLayoutIssue();
                if (layoutIssue == null)
                {
                    stableFrames++;
                    if (stableFrames >= RequiredStableFrames)
                    {
                        phase = CapturePhase.Capture;
                    }

                    return;
                }

                stableFrames = 0;
                if (layoutFrames >= MaximumLayoutFrames)
                {
                    throw new InvalidOperationException(
                        $"Capture layout did not stabilize at {width}x{height} after {layoutFrames} frames: {layoutIssue}");
                }
            }

            private void Capture()
            {
                string layoutIssue = GetLayoutIssue();
                if (layoutIssue != null)
                {
                    throw new InvalidOperationException("Capture layout changed before render: " + layoutIssue);
                }

                captureCamera.Render();
                Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
                RenderTexture previous = RenderTexture.active;
                try
                {
                    RenderTexture.active = renderTexture;
                    screenshot.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, false);
                    screenshot.Apply(false, false);
                    ValidatePixels(screenshot);
                    File.WriteAllBytes(outputPath, screenshot.EncodeToPNG());
                }
                finally
                {
                    RenderTexture.active = previous;
                    UnityEngine.Object.DestroyImmediate(screenshot);
                }
            }

            private string GetLayoutIssue()
            {
                if (canvas == null || canvasRect == null)
                {
                    return "runtime canvas was destroyed";
                }

                if (canvas.renderMode != RenderMode.ScreenSpaceCamera || canvas.worldCamera != captureCamera)
                {
                    return "runtime canvas was not bound to the capture camera";
                }

                if (captureCamera.targetTexture != renderTexture)
                {
                    return "capture camera lost its target texture";
                }

                if (captureCamera.pixelWidth != width || captureCamera.pixelHeight != height)
                {
                    return $"camera pixel size was {captureCamera.pixelWidth}x{captureCamera.pixelHeight}";
                }

                Rect pixelRect = canvas.pixelRect;
                if (!Approximately(pixelRect.width, width) || !Approximately(pixelRect.height, height))
                {
                    return $"canvas pixel rect was {pixelRect.width:0.##}x{pixelRect.height:0.##}";
                }

                Rect rootRect = canvasRect.rect;
                float expectedAspect = width / (float)height;
                float rootAspect = rootRect.height <= 0f ? 0f : rootRect.width / rootRect.height;
                if (rootRect.width <= 0f || rootRect.height <= 0f || Mathf.Abs(rootAspect - expectedAspect) > 0.01f)
                {
                    return $"canvas root rect was {rootRect.width:0.##}x{rootRect.height:0.##} with an unexpected aspect";
                }

                return null;
            }

            private void ValidatePixels(Texture2D screenshot)
            {
                Color32[] pixels = screenshot.GetPixels32();
                Color32 clear = clearColor;
                RegionStats lower = MeasureRegion(pixels, 0, height / 3, clear);
                RegionStats middle = MeasureRegion(pixels, height / 3, height * 2 / 3, clear);
                RegionStats upper = MeasureRegion(pixels, height * 2 / 3, height, clear);

                Debug.Log(
                    $"Capture pixel check {Path.GetFileName(outputPath)}: "
                    + $"upper variance={upper.Variance:0.00}, coverage={upper.Coverage:P1}; "
                    + $"middle variance={middle.Variance:0.00}, coverage={middle.Coverage:P1}; "
                    + $"lower variance={lower.Variance:0.00}, coverage={lower.Coverage:P1}.");

                ValidateRegion("upper", upper);
                ValidateRegion("middle", middle);
                ValidateRegion("lower", lower);
            }

            private RegionStats MeasureRegion(Color32[] pixels, int startY, int endY, Color32 clear)
            {
                double red = 0d;
                double green = 0d;
                double blue = 0d;
                double redSquared = 0d;
                double greenSquared = 0d;
                double blueSquared = 0d;
                int covered = 0;
                int samples = 0;

                for (int y = startY; y < endY; y += PixelSampleStride)
                {
                    int row = y * width;
                    for (int x = 0; x < width; x += PixelSampleStride)
                    {
                        Color32 pixel = pixels[row + x];
                        red += pixel.r;
                        green += pixel.g;
                        blue += pixel.b;
                        redSquared += pixel.r * pixel.r;
                        greenSquared += pixel.g * pixel.g;
                        blueSquared += pixel.b * pixel.b;

                        int deltaRed = pixel.r - clear.r;
                        int deltaGreen = pixel.g - clear.g;
                        int deltaBlue = pixel.b - clear.b;
                        if (deltaRed * deltaRed + deltaGreen * deltaGreen + deltaBlue * deltaBlue >= ClearColorDistanceSquared)
                        {
                            covered++;
                        }

                        samples++;
                    }
                }

                double inverseSamples = 1d / samples;
                double variance = (
                    redSquared * inverseSamples - Math.Pow(red * inverseSamples, 2d)
                    + greenSquared * inverseSamples - Math.Pow(green * inverseSamples, 2d)
                    + blueSquared * inverseSamples - Math.Pow(blue * inverseSamples, 2d)) / 3d;
                return new RegionStats(Math.Max(0d, variance), covered * inverseSamples);
            }

            private static void ValidateRegion(string name, RegionStats stats)
            {
                if (stats.Coverage < MinimumRegionCoverage)
                {
                    throw new InvalidOperationException(
                        $"Capture {name} third was effectively clear ({stats.Coverage:P1} coverage); the Canvas was likely only partially rendered.");
                }

                if (stats.Variance < MinimumRegionVariance)
                {
                    throw new InvalidOperationException(
                        $"Capture {name} third was effectively flat ({stats.Variance:0.00} variance); refusing a gray or blank screenshot.");
                }
            }

            private static bool Approximately(float actual, float expected)
            {
                return Mathf.Abs(actual - expected) <= DimensionTolerance;
            }

            private enum CapturePhase
            {
                Prepare,
                WaitForLayout,
                Capture,
                Restore
            }

            private readonly struct RegionStats
            {
                public RegionStats(double variance, double coverage)
                {
                    Variance = variance;
                    Coverage = coverage;
                }

                public double Variance { get; }

                public double Coverage { get; }
            }
        }

        private static void AdvanceStage()
        {
            EditorPrefs.SetInt(StageKey, EditorPrefs.GetInt(StageKey, 0) + 1);
        }

        private static void Finish(int exitCode)
        {
            CancelCaptureSession();
            EditorPrefs.DeleteKey(ActiveKey);
            EditorPrefs.DeleteKey(StageKey);
            EditorApplication.update -= UpdateCapture;
            EditorApplication.Exit(exitCode);
        }
    }
}

using UnityEngine;
using PathOfTenThousandWays.Demo.UI;

namespace PathOfTenThousandWays.Demo.Systems
{
    public sealed class DemoBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateController()
        {
            if (Object.FindAnyObjectByType<DemoGameController>() != null)
            {
                return;
            }

            GameObject controller = new GameObject("DEMO_GameController");
            controller.AddComponent<DemoGameController>();
            controller.AddComponent<DemoRuntimeCanvasUI>();
            DontDestroyOnLoad(controller);
        }
    }
}

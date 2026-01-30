using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ali
{
    /// <summary>
    /// Forces VR Controllers to be active immediately on start.
    /// Fixes issue where controllers are hidden until button press or scene load glitches.
    /// </summary>
    public class ForceVRControllers : MonoBehaviour
    {
        [Header("Controller Objects")]
        [Tooltip("The GameObject holding the Left Hand Controller (LeftHand Controller)")]
        public GameObject leftHandObject;

        [Tooltip("The GameObject holding the Right Hand Controller (RightHand Controller)")]
        public GameObject rightHandObject;

        [Header("Optional Components")]
        [Tooltip("Assign if you want to force enabling inputs specifically")]
        public ActionBasedController leftControllerComp;
        public ActionBasedController rightControllerComp;

        void Start()
        {
            ForceActive();
        }

        void OnEnable()
        {
            ForceActive();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            ForceActive();
        }

        public void ForceActive()
        {
            // 1. Force GameObjects Active
            if (leftHandObject && !leftHandObject.activeSelf)
            {
                Debug.Log("[ForceVRControllers] Activating Left Hand Object");
                leftHandObject.SetActive(true);
            }

            if (rightHandObject && !rightHandObject.activeSelf)
            {
                Debug.Log("[ForceVRControllers] Activating Right Hand Object");
                rightHandObject.SetActive(true);
            }

            // 2. Force Components Enabled (Rarely needed but safe)
            if (leftControllerComp && !leftControllerComp.enabled) leftControllerComp.enabled = true;
            if (rightControllerComp && !rightControllerComp.enabled) rightControllerComp.enabled = true;
        }

        void Update()
        {
            // Watchdog: If for some reason they get disabled by Unity (e.g. tracking lost), force them back
            // Only do this if you REALLY want them visible even when tracking is lost.
            if (leftHandObject && !leftHandObject.activeSelf) leftHandObject.SetActive(true);
            if (rightHandObject && !rightHandObject.activeSelf) rightHandObject.SetActive(true);
        }
    }
}
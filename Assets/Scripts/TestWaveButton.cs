using UnityEngine;
using UnityEngine.InputSystem;
using AnutoTD.Core;

namespace AnutoTD
{
    public class TestWaveButton : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.StartNextWave();
                }
            }
        }
    }
}
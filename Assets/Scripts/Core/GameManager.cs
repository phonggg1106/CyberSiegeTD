using UnityEngine;

namespace AnutoTD.Core
{
    public enum GameState
    {
        Preparing,  // Đang chuẩn bị (đặt tháp trước khi gọi quái)
        Playing,    // Đang chiến đấu
        Won,        // Thắng
        Lost        // Thua
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private GameState currentState = GameState.Preparing;
        public GameState CurrentState => currentState;

        public static event System.Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            BaseHealth.OnBaseDead += HandleBaseDead;
            WaveSpawner.OnAllWavesCleared += HandleAllWavesCleared;
        }

        private void OnDisable()
        {
            BaseHealth.OnBaseDead -= HandleBaseDead;
            WaveSpawner.OnAllWavesCleared -= HandleAllWavesCleared;
        }

        private void Start()
        {
            ChangeState(GameState.Preparing);
        }

        // Người chơi bấm nút "Gọi Wave" hoặc "Bắt đầu"
        public void StartNextWave()
        {
            if (currentState == GameState.Won || currentState == GameState.Lost) return;

            if (currentState == GameState.Preparing)
            {
                ChangeState(GameState.Playing);
            }

            if (WaveSpawner.Instance != null)
            {
                WaveSpawner.Instance.SpawnNextWave();
            }
        }

        private void HandleBaseDead()
        {
            ChangeState(GameState.Lost);
        }

        private void HandleAllWavesCleared()
        {
            ChangeState(GameState.Won);
        }

        private void ChangeState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged?.Invoke(newState);
            Debug.Log($"[Game] Trạng thái: {newState}");
        }
    }
}
using UnityEngine;

namespace PMDM.Core
{
    public enum GameState { Menu, Playing, Won, GameOver, Paused }

    /// <summary>
    /// Singleton que controla el flujo global del juego 2548.
    /// Gestiona estados, puntuación y coordina BoardManager con los PowerUps.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private BoardManager _boardManager;
        [SerializeField] private PowerUps.PowerUpManager _powerUpManager;

        private GameState _currentState = GameState.Menu;
        private long _highScore;
        private bool _continueAfterWin;

        public GameState CurrentState => _currentState;
        public long HighScore => _highScore;
        public BoardManager Board => _boardManager;

        // Eventos para que la UI reaccione
        public System.Action<GameState> OnStateChanged;
        public System.Action<long> OnHighScoreChanged;

        private const string HighScoreKey = "2548_HighScore";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            LoadHighScore();
        }

        private void OnEnable()
        {
            if (_boardManager != null)
            {
                _boardManager.OnScoreChanged += HandleScoreChanged;
                _boardManager.OnWin += HandleWin;
                _boardManager.OnGameOver += HandleGameOver;
            }
        }

        private void OnDisable()
        {
            if (_boardManager != null)
            {
                _boardManager.OnScoreChanged -= HandleScoreChanged;
                _boardManager.OnWin -= HandleWin;
                _boardManager.OnGameOver -= HandleGameOver;
            }
        }

        public void StartNewGame()
        {
            _continueAfterWin = false;
            _boardManager.InitializeBoard();
            SetState(GameState.Playing);

            if (_powerUpManager != null)
                _powerUpManager.ResetPowerUps();
        }

        public void PauseGame()
        {
            if (_currentState != GameState.Playing) return;
            SetState(GameState.Paused);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (_currentState != GameState.Paused) return;
            Time.timeScale = 1f;
            SetState(GameState.Playing);
        }

        public void ContinueAfterWin()
        {
            _continueAfterWin = true;
            SetState(GameState.Playing);
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            _boardManager.ClearBoard();
            SetState(GameState.Menu);
        }

        /// <summary>
        /// Intenta ejecutar un movimiento. Solo si el estado es Playing.
        /// </summary>
        public bool TryMove(MoveDirection direction)
        {
            if (_currentState != GameState.Playing) return false;
            return _boardManager.ExecuteMove(direction);
        }

        public void TryUndo()
        {
            if (_currentState != GameState.Playing) return;
            if (_boardManager.CanUndo())
                _boardManager.Undo();
        }

        private void HandleScoreChanged(long newScore)
        {
            if (newScore > _highScore)
            {
                _highScore = newScore;
                SaveHighScore();
                OnHighScoreChanged?.Invoke(_highScore);
            }
        }

        private void HandleWin()
        {
            if (_continueAfterWin) return;
            SetState(GameState.Won);
        }

        private void HandleGameOver()
        {
            SetState(GameState.GameOver);
        }

        private void SetState(GameState newState)
        {
            _currentState = newState;
            OnStateChanged?.Invoke(_currentState);
        }

        private void LoadHighScore()
        {
            _highScore = long.Parse(PlayerPrefs.GetString(HighScoreKey, "0"));
            OnHighScoreChanged?.Invoke(_highScore);
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetString(HighScoreKey, _highScore.ToString());
            PlayerPrefs.Save();
        }
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameOver : MonoBehaviour
    {
        private VisualElement _root;
        private Button _playAgainButton;
        private Button _mainMenuButton;
        private Label _winningTitleLabel;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            RegisterButtonsAndEvents();
        }
        
        private void RegisterButtonsAndEvents()
        {
            _playAgainButton = _root.Q<Button>("play-again-button");
            _mainMenuButton = _root.Q<Button>("main-menu-button");
            _winningTitleLabel = _root.Q<Label>("winning-title");

            _playAgainButton.clicked += HandlePlayAgainButtonClicked;
            _mainMenuButton.clicked += HandleMainMenuButtonClicked;
        }

        private void HandleMainMenuButtonClicked()
        {
            gameObject.SetActive(false);
            SceneManager.LoadScene("Main Menu");
        }

        private void HandlePlayAgainButtonClicked()
        {
            gameObject.SetActive(false);
            SceneManager.LoadScene("Chess Game Scene");
        }

        public void SetWinningPlayerLabelText(string winner)
        {
            _winningTitleLabel.text = winner + " Wins";
        }
    }
}

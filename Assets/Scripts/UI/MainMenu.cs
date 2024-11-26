using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        private VisualElement _root;
        private Button _startGameButton;
        private Button _quitGameButton;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            RegisterButtonsAndEvents();
        }
        
        private void RegisterButtonsAndEvents()
        {
            _startGameButton = _root.Q<Button>("start-button");
            _quitGameButton = _root.Q<Button>("quit-button");

            _startGameButton.clicked += HandleStartGameButtonClicked;
            _quitGameButton.clicked += HandleQuitGameButtonClicked;
        }

        private void HandleQuitGameButtonClicked()
        {
            Application.Quit();
        }

        private void HandleStartGameButtonClicked()
        {
            gameObject.SetActive(false);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("Chess Game Scene", LoadSceneMode.Additive);
        }
    }
}

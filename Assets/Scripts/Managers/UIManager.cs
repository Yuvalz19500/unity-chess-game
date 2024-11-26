using System;
using Core;
using Enums;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private PromotionPanel promotionPanel;
        [SerializeField] private GameOver gameOverScreen;
        
        public static event Action<PieceType> OnPlayerPickPromotion;
        
        private void Awake()
        {
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            Board.OnPawnPromotion += HandlePawnPromotion;
            PromotionPanel.OnPlayerPickPromotion += HandlePlayerPickPromotion;
            GameManager.OnGameOver += HandleGameOver;
        }

        private void HandleGameOver(string winner)
        {
            gameOverScreen.gameObject.SetActive(true);
            gameOverScreen.SetWinningPlayerLabelText(winner);
        }

        private void UnregisterEvents()
        {
            Board.OnPawnPromotion -= HandlePawnPromotion;
            PromotionPanel.OnPlayerPickPromotion -= HandlePlayerPickPromotion;
        }
        
        private void HandlePlayerPickPromotion(PieceType piece)
        {
            OnPlayerPickPromotion?.Invoke(piece);
        }

        private void HandlePawnPromotion()
        {
            promotionPanel.TogglePromotionPanel(true);
        }

        public static void LoadUISceneIfNotLoaded()
        {
            if (!SceneManager.GetSceneByName("UI").isLoaded)
            {
                SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
            }
        }
    }
}

using System;
using Enums;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class PromotionPanel : MonoBehaviour
    {
        private VisualElement _root;
        private Button _knightButton;
        private Button _bishopButton;
        private Button _rookButton;
        private Button _queenButton;
        
        public static event Action<PieceType> OnPlayerPickPromotion;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            RegisterButtonsAndEvents();
        }

        private void RegisterButtonsAndEvents()
        {
            _knightButton = _root.Q<Button>("knight-button");
            _bishopButton = _root.Q<Button>("bishop-button");
            _rookButton = _root.Q<Button>("rook-button");
            _queenButton = _root.Q<Button>("queen-button");

            _knightButton.clicked += HandleKnightButtonClicked;
            _bishopButton.clicked += HandleBishopButtonClicked;
            _rookButton.clicked += HandleRookButtonClicked;
            _queenButton.clicked += HandleQueenButtonClicked;
        }

        private void OnDestroy()
        {
            _knightButton.clicked -= HandleKnightButtonClicked;
            _bishopButton.clicked -= HandleBishopButtonClicked;
            _rookButton.clicked -= HandleRookButtonClicked;
            _queenButton.clicked -= HandleQueenButtonClicked;
        }

        private void HandleQueenButtonClicked()
        {
            OnPlayerPickPromotion?.Invoke(PieceType.Queen);
            TogglePromotionPanel(false);
        }

        private void HandleRookButtonClicked()
        {
            OnPlayerPickPromotion?.Invoke(PieceType.Rook);
            TogglePromotionPanel(false);
        }

        private void HandleBishopButtonClicked()
        {
            OnPlayerPickPromotion?.Invoke(PieceType.Bishop);
            TogglePromotionPanel(false);
        }

        private void HandleKnightButtonClicked()
        {
            OnPlayerPickPromotion?.Invoke(PieceType.Knight);
            TogglePromotionPanel(false);
        }

        public void TogglePromotionPanel(bool toggle)
        {
            gameObject.SetActive(toggle);
        }
    }
}

using System;
using System.Linq;
using Core;
using Enums;
using Pieces;
using Scriptable_Objects;
using UnityEngine;
using Utils;
using UnityEngine.SceneManagement;

namespace Managers
{
    [RequireComponent(typeof(PieceCreator))]
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private BoardStartingLayout boardStartingLayout;
        [SerializeField] private Board board;

        private PieceCreator _pieceCreator;
        private Player _whitePlayer;
        private Player _blackPlayer;
        private Player _activePlayer;
        private bool _isGameOver = false;

        public static event Action<string> OnGameOver;

        protected override void Awake()
        {
            base.Awake();
            SetupDependencies();
        }

        private void Start()
        {
            StartNewGame();
        }
        
        private void SetupDependencies()
        {
            #if !UNITY_EDITOR
                UIManager.LoadUISceneIfNotLoaded();
            #endif
            _pieceCreator = GetComponent<PieceCreator>();
            CreatePlayers();
        }
        
        private void StartNewGame()
        {
            InitBoardLayout();
            GenerateActiveMovesForPlayer(_activePlayer);
        }

        private void CreatePlayers()
        {
            _whitePlayer = new Player(TeamColor.White, board);
            _blackPlayer = new Player(TeamColor.Black, board);
            _activePlayer = _whitePlayer;
        }
 
        private void InitBoardLayout()
        {
            for (int i = 0; i < boardStartingLayout.GetPiecesCount(); i++)
            {
                Vector2Int piecePos = boardStartingLayout.GetPiecePositionAtIndex(i);
                string pieceName = boardStartingLayout.GetPieceNameAtIndex(i);
                TeamColor pieceColor = boardStartingLayout.GetPieceTeamColorAtIndex(i);
                
                CreatePieceFromLayout(piecePos, pieceName, pieceColor);
            }
        }

        private string GetActivePlayerAsString()
        {
            return _activePlayer == _whitePlayer ? "White Player" : "Black Player";
        }

        private void CreatePieceFromLayout(Vector2Int piecePos, string pieceName, TeamColor pieceTeamColor)
        {
            Piece piece = _pieceCreator.CreatePiece(pieceName).GetComponent<Piece>();
            piece.SetPieceMaterial(_pieceCreator.GetTeamMaterial(pieceTeamColor));
            piece.SetPieceData(piecePos, pieceTeamColor, board);

            Player currentPlayer = pieceTeamColor == TeamColor.Black ? _blackPlayer : _whitePlayer;
            currentPlayer.AddPiece(piece);
            
            board.SetPieceOnBoard(piece, piecePos);
        }
        
        private Player GetOpponentToActivePlayer()
        {
            return _activePlayer == _whitePlayer ? _blackPlayer : _whitePlayer;
        }
        
        private void GenerateActiveMovesForPlayer(Player player)
        {
            Player opponent = GetOpponentToActivePlayer();
            
            player.GeneratePossibleMoves();
            opponent.GeneratePossibleMoves();
            
             foreach (Piece piece in player.ActivePieces)
             {
                 player.RemoveMovesEnablingAttackOnPieceOfType<King>(opponent, piece);
             }
        }

        private void ChangeActiveTeam()
        {
            _activePlayer = _activePlayer == _whitePlayer ? _blackPlayer : _whitePlayer;
            GenerateActiveMovesForPlayer(_activePlayer);
        }

        public void EndTurn()
        {
            GenerateActiveMovesForPlayer(_activePlayer);
            GenerateActiveMovesForPlayer(GetOpponentToActivePlayer());

            if (IsGameFinished())
            {
                EndGame();
            }
            else
            {
                ChangeActiveTeam();
            }
        }

        private void EndGame()
        {
            _isGameOver = true;
            OnGameOver?.Invoke(GetActivePlayerAsString());
        }

        private bool IsGameFinished()
        {
            board.ClearActiveCheckSquares();
            
            Piece[] piecesAttackingOppositeKing = _activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
            if (piecesAttackingOppositeKing.Length <= 0) return false;

            Player opponent = GetOpponentToActivePlayer();
            Piece attackedKing = opponent.GetPiecesOfType<King>().FirstOrDefault();
            
            if (!attackedKing) return true;
            foreach (Piece piece in piecesAttackingOppositeKing.Append(attackedKing))
            {
                board.CreateCheckSquare(piece.SquarePosition);
            }
            
            opponent.RemoveMovesEnablingAttackOnPieceOfType<King>(_activePlayer, attackedKing);
            if (attackedKing.MovesDict.Count != 0) return false;
            
            return !opponent.CanCoverPieceFromAttack<King>(_activePlayer);
        }

        public TeamColor GetActiveTeamColorTurn()
        {
            return _activePlayer == _whitePlayer ? TeamColor.White : TeamColor.Black;
        }

        public void OnPieceTaken(Piece piece)
        {
            GetOpponentToActivePlayer().RemovePiece(piece);
        }

        public Piece CreatePieceForPromotion(Piece oldPawn, PieceType type, Vector2Int position, TeamColor teamColor)
        {
            _activePlayer.RemovePiece(oldPawn);
            
            Piece piece = _pieceCreator.CreatePiece(type.ToString()).GetComponent<Piece>();
            piece.SetPieceMaterial(_pieceCreator.GetTeamMaterial(teamColor));
            piece.SetPieceData(position, teamColor, board);
            
            _activePlayer.AddPiece(piece);
            return piece;
        }

        public bool IsGameOver()
        {
            return _isGameOver;
        }
    }
}
using System;

namespace Game
{
    public enum GameState
    {
        Connecting,
        MatchMaking,
        InGame
    }
    
    public static class GameStateController
    {
        private static GameState _state = GameState.Connecting;
        
        public static GameState State
        {
            get => _state;
            set
            {
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }
        
        public static event Action<GameState> StateChanged;
    }
}
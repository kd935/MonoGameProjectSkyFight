using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyFight
{
    public struct ScoreManager
    {
        public ScoreManager(GameStates initialGameState)
        {
            PlayerScore = 0;
            EnemyScore = 0;
            GameState = initialGameState;
            Winner = null;
        }
        private GameStates GameState { get; set; }
        private Winners? Winner { get; set; }
        public int PlayerScore { get; private set; }
        public int EnemyScore { get; private set; }
        public (GameStates, Winners?) CheckPlayersScore()
        {
            if (PlayerScore == MainGame.SCORE_TO_END_THE_GAME)
            {
                Winner = Winners.Player;
                GameState = GameStates.Ended;
                return (GameState, Winner);
            }
            if (EnemyScore == MainGame.SCORE_TO_END_THE_GAME)
            {
                Winner = Winners.EnemyPlayer;
                GameState = GameStates.Ended;
                return (GameState, Winner);
            }
            return (GameState, Winner);
        }
        public void IncreasePlayerScore()
        {
            PlayerScore++;
        }
        public void IncreaseEnemyScore()
        {
            EnemyScore++;
        }
    }
}

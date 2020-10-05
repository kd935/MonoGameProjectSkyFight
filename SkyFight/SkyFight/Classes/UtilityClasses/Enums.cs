using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyFight
{
    public enum Rotations
    {
        Clockwise,
        Counterclockwise
    }
    public enum GameModes
    {
        PlayerVersusPlayer,
        PlayerVersusAI
    }
    public enum Winners
    {
        Player,
        EnemyPlayer
    }
    public enum GameStates
    {
        NotStarted,
        Started,
        Ended
    }
    public enum Bounds
    {
        Left,
        Top,
        Right
    }
    public enum SpriteType
    {
        LeftSidedOrigin,
        RightSidedOrigin
    }
}

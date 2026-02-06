using Unity.Mathematics;
using UnityEngine;

namespace Core.Commander.Input
{
    public readonly struct CommanderInputSnapshot{
        public readonly Vector2 Move;
        public readonly bool DashPressed;

        public CommanderInputSnapshot(Vector2 move, bool dashPressed)
        {
            Move = move;
            DashPressed = dashPressed;
        }
    }

}
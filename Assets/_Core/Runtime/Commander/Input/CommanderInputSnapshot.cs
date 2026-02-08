using Unity.Mathematics;
using UnityEngine;

namespace Core.Commander.Input
{
    public readonly struct CommanderInputSnapshot{
        public readonly Vector2 Move;
        public readonly bool DashPressed;
        public readonly bool AttackPressed;

        public readonly bool BuildPressed;
        public readonly bool ConfirmPressed;
        public readonly bool CancelPressed;

        public readonly Vector2 PointerScreen;

        public CommanderInputSnapshot(Vector2 move, bool dashPressed, 
        bool attackPressed, bool buildPressed, 
        bool cancelPressed, bool confirmPressed,
        Vector2 pointerScreen)
        {
            Move = move;
            DashPressed = dashPressed;
            AttackPressed = attackPressed;
            BuildPressed = buildPressed;
            ConfirmPressed = confirmPressed;
            CancelPressed = cancelPressed;
            PointerScreen = pointerScreen;
        }
    }

}
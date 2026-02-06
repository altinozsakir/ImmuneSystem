using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Commander.Input
{
    


    public class CommanderInputReader : MonoBehaviour
    {
        
        private InputActionReference move;
        private InputActionReference dash;

        private Vector2 moveValue;
        private bool dashPressedEdge;

        private void OnEnable()
        {
            if(move != null)
            {
                move.action.Enable();
                move.action.performed += OnMove;
                move.action.canceled += OnMove;
            }

            if(dash != null)
            {
                dash.action.Enable();
                dash.action.performed += OnDashPerformed;
            }
        }

        private void OnDisable()
        {
            if(move!= null)
            {
                
                move.action.performed -= OnMove;
                move.action.canceled -= OnMove;
                move.action.Disable();
            }

            if(dash != null)
            {
                dash.action.performed -= OnDashPerformed;
                dash.action.Disable();
                
            }


        }
        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveValue = ctx.ReadValue<Vector2>();
        }

        private void OnDashPerformed(InputAction.CallbackContext ctx)
        {
            dashPressedEdge = true;
        }


        public CommanderInputSnapshot ConsumeSnapshot()
        {
            var snap = new CommanderInputSnapshot(moveValue, dashPressedEdge);
            dashPressedEdge = false;
            return snap;
        }
    }



}
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Commander.Input
{
    


    public class CommanderInputReader : MonoBehaviour
    {
        
        [SerializeField] private InputActionReference move;
        [SerializeField] private InputActionReference dash;
        [SerializeField] private InputActionReference attack;
        [SerializeField] private InputActionReference build;
        [SerializeField] private InputActionReference confirm;
        [SerializeField] private InputActionReference cancel;
        [SerializeField] private InputActionReference pointer;

        private Vector2 moveValue;
        private Vector2 pointerValue;
        private bool dashPressedEdge;
        private bool attackPressedEdge;
        private bool buildPressedEdge;
        private bool confirmPressedEdge;
        private bool cancelPressedEdge;

        private void OnEnable()
        {
            Hook(move, OnMove);
            Hook(dash, OnDashPerformed);
            Hook(attack, OnAttackPerformed);

            Hook(build, OnBuildPerformed);
            Hook(confirm, OnConfirmPerformed);
            Hook(cancel, OnCancelPerformed);

            if (pointer != null)
            {
                pointer.action.Enable();
                pointer.action.performed += OnPointer;
                pointer.action.canceled += OnPointer;
            }
        }

        private void OnDisable()
        {
            Unhook(move, OnMove);
            Unhook(dash, OnDashPerformed);
            Unhook(attack, OnAttackPerformed);

            Unhook(build, OnBuildPerformed);
            Unhook(confirm, OnConfirmPerformed);
            Unhook(cancel, OnCancelPerformed);

            if (pointer != null)
            {
                pointer.action.performed -= OnPointer;
                pointer.action.canceled -= OnPointer;
                pointer.action.Disable();
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

        private void OnAttackPerformed(InputAction.CallbackContext ctx)
        {
            attackPressedEdge = true;
        }
        private void OnPointer(InputAction.CallbackContext ctx) => pointerValue = ctx.ReadValue<Vector2>();

        private void OnBuildPerformed(InputAction.CallbackContext ctx) { if (ctx.performed) buildPressedEdge = true; }
        private void OnConfirmPerformed(InputAction.CallbackContext ctx) { if (ctx.performed) confirmPressedEdge = true; }
        private void OnCancelPerformed(InputAction.CallbackContext ctx) { if (ctx.performed) cancelPressedEdge = true; }
        private static void Unhook(InputActionReference a, System.Action<InputAction.CallbackContext> cb)
        {
            if (a == null) return;
            a.action.performed -= cb;
            a.action.canceled -= cb;
            a.action.Disable();
        }

        private static void Hook(InputActionReference a, System.Action<InputAction.CallbackContext> cb)
        {
            if (a == null) return;
            a.action.Enable();
            a.action.performed += cb;
            a.action.canceled += cb;
        }


        public CommanderInputSnapshot ConsumeSnapshot()
        {
            var snap = new CommanderInputSnapshot(moveValue, 
            dashPressedEdge, 
            attackPressedEdge,
            buildPressedEdge,
            confirmPressedEdge,
            cancelPressedEdge,
            pointerValue);
            dashPressedEdge = attackPressedEdge = buildPressedEdge = confirmPressedEdge = cancelPressedEdge = false;
            return snap;
        }
    }



}
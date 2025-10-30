using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputCustom : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    private bool jumpPressed;

    private void Awake()
    {
        inputActions = new InputSystem_Actions(); // Твоё сгенерированное имя класса, скорее всего именно так или похожее
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => jumpPressed = true;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    public Vector2 GetMoveDirection => moveInput;

    public bool IsJumpPressed
    {
        get
        {
            if (jumpPressed)
            {
                jumpPressed = false;
                return true;
            }
            return false;
        }
    }
}

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovements : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInputCustom _input;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;

    [Header("Movement Config")]
    [SerializeField] private float moveSpeed = 5f;
    [Range(1f, 20f)] [SerializeField] private float groundSmoothSpeed = 15f;
    [Range(1f, 20f)] [SerializeField] private float airSmoothSpeed = 8f;
    private float currentSmoothSpeed;
    private Vector3 smoothInput = Vector3.zero;

    [Header("Gravity / Jump")]
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float jumpForce = 7f;
    private Vector3 velocity = Vector3.zero;

    [Header("Scale Flip")]
    [SerializeField] private float rangeScale = 1f;

    [Header("Dialog/Notif")]
    [SerializeField] private Transform dialogBox;
    [SerializeField] private Transform notifBox;

    private readonly int moveHash = Animator.StringToHash("Move");

    [Header("Pickup Settings")]
    [SerializeField] private float pickupRange = 2f; // радиус подбора
    [SerializeField] private LayerMask pickupLayer;  // слой, на котором лежат предметы

    [SerializeField] private HeroineCode heroine;

    public bool Food = false;
    public bool Water = false;

    private void Awake()
    {
        if (_input == null) _input = GetComponent<PlayerInputCustom>();
        if (_controller == null) _controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyGravity();
        UpdateAnimation();
        UpdateDirection();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    private void TryPickup()
    {
        // Ищем все коллайдеры вокруг игрока
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Food"))
            {
                Debug.Log($"Picked up: {hit.name}");

                Water = true;

                // 🔹 Здесь можно добавить, например:
                // - увеличение энергии
                // - добавление в инвентарь
                // - воспроизведение звука
                // А потом уничтожаем предмет
                Destroy(hit.gameObject);

                return; // нашли один — выходим
            }

            else if (hit.CompareTag("Water"))
            {
                Debug.Log($"Picked up: {hit.name}");

                Food = true;

                // 🔹 Здесь можно добавить, например:
                // - увеличение энергии
                // - добавление в инвентарь
                // - воспроизведение звука
                // А потом уничтожаем предмет
                Destroy(hit.gameObject);

                return; // нашли один — выходим
            }

            if (hit.CompareTag("Give") && Food == true && Water == true)
            {
                Debug.Log($"Picked up: {hit.name}");

                heroine.dialog.StartDialogue(1);

                heroine.Invoke(nameof(heroine.PlayAnim), 3f);
                Destroy(hit.gameObject);

                return; // нашли один — выходим
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Switch"))
        {
            SwitchCode tile = other.GetComponent<SwitchCode>();
            if (tile != null)
            {
                tile.ActivateDrop();
                Debug.Log("FEFE");
            }
        }
    }


    private void HandleMovement()
    {
        Vector2 input = _input.GetMoveDirection;
        Vector3 targetInput = new Vector3(input.x, 0f, input.y);

        currentSmoothSpeed = _controller.isGrounded ? groundSmoothSpeed : airSmoothSpeed;
        smoothInput = Vector3.Lerp(smoothInput, targetInput, Time.fixedDeltaTime * currentSmoothSpeed);

        Vector3 move = transform.TransformDirection(smoothInput) * moveSpeed * Time.fixedDeltaTime;
        move += velocity * Time.fixedDeltaTime;

        _controller.Move(move);
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded)
        {
            if (velocity.y < 0f) velocity.y = -2f; // "прилипание" к земле
            if (_input.IsJumpPressed) velocity.y = jumpForce;
        }
        else
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
        }
    }

    private void UpdateAnimation()
    {
        bool isMoving = smoothInput.magnitude > 0.1f;
        _animator.SetBool(moveHash, isMoving);
    }

    private void UpdateDirection()
    {
        if (smoothInput.x > 0.1f)
            transform.localScale = new Vector3(rangeScale, rangeScale, rangeScale);
        else if (smoothInput.x < -0.1f)
            transform.localScale = new Vector3(-rangeScale, rangeScale, rangeScale);

        //UpdateDialogDirection();
    }

    private void UpdateDialogDirection()
    {
        if (dialogBox != null)
        {
            Vector3 scale = dialogBox.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(transform.localScale.x);
            dialogBox.localScale = scale;
        }

        if (notifBox != null)
        {
            Vector3 scale = notifBox.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(transform.localScale.x);
            notifBox.localScale = scale;
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PCMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float lookSpeed = 2f;

    [Header("Physics")]
    [Tooltip("Ângulo máximo de rampa que o player consegue subir (45° é padrão Unity)")]
    public float slopeLimit  = 45f;
    [Tooltip("Altura máxima de degrau que o player consegue subir")]
    public float stepOffset  = 0.3f;

    private float _rotX           = 0f;
    private float _rotY           = 0f;
    private float _verticalVelocity = 0f;
    private CharacterController _cc;

    private void Start()
    {
        _rotY = transform.eulerAngles.y;
        _cc   = GetComponent<CharacterController>();

        ApplyPhysicsLimits();
    }

    // Chamado pelo Unity quando o script é reativado (ex: ao levantar do banco)
    private void OnEnable()
    {
        _verticalVelocity = 0f; // zera gravidade acumulada para não cair ao reativar

        // Reaplica os limites caso o BenchInteraction os tenha restaurado
        // (garante consistência se houver delay entre RestoreCCSlope e OnEnable)
        if (_cc == null) _cc = GetComponent<CharacterController>();
        ApplyPhysicsLimits();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            _rotX -= mouseDelta.y * lookSpeed * 0.1f;
            _rotY += mouseDelta.x * lookSpeed * 0.1f;
            _rotX  = Mathf.Clamp(_rotX, -80f, 80f);
            transform.rotation = Quaternion.Euler(_rotX, _rotY, 0f);
        }

        if (Keyboard.current == null) return;

        float h = 0f;
        float v = 0f;

        if (Keyboard.current.aKey.isPressed     || Keyboard.current.leftArrowKey.isPressed)  h = -1f;
        if (Keyboard.current.dKey.isPressed     || Keyboard.current.rightArrowKey.isPressed) h =  1f;
        if (Keyboard.current.sKey.isPressed     || Keyboard.current.downArrowKey.isPressed)  v = -1f;
        if (Keyboard.current.wKey.isPressed     || Keyboard.current.upArrowKey.isPressed)    v =  1f;

        // Gravidade acumulada corretamente
        if (_cc.isGrounded)
            _verticalVelocity = -2f; // valor negativo pequeno para manter colado ao chão
        else
            _verticalVelocity -= 9.8f * Time.deltaTime;

        Vector3 move = transform.right * h + transform.forward * v;
        move.y = _verticalVelocity * Time.deltaTime;

        _cc.Move(move * moveSpeed * Time.deltaTime);
    }

    // ─────────────────────────────────────────────────────────────
    //  Aplica os limites de física ao CharacterController
    // ─────────────────────────────────────────────────────────────
    private void ApplyPhysicsLimits()
    {
        if (_cc == null) return;
        _cc.slopeLimit = slopeLimit;
        _cc.stepOffset = stepOffset;
    }
}
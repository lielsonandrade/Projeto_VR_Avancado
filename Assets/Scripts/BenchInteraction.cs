using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Script de interação com bancos.
/// Compatível com CharacterController e Rigidbody.
/// Suporta Novo Input System e Legacy Input Manager.
/// </summary>
public class BenchInteraction : MonoBehaviour
{
    private enum State { Standing, Sitting, Transitioning }
    private State _state = State.Standing;

    [Header("Configurações de Assento")]
    [Tooltip("Transform onde o player ficará posicionado ao sentar")]
    public Transform sitPoint;
    public float interactionRadius = 2.5f;
    [Tooltip("Offset para onde o player vai ao levantar (relativo ao sitPoint)")]
    public Vector3 standOffset = new Vector3(0f, 0f, 1.0f);

    [Header("Duração")]
    public float sitDuration = 0.5f;
    public float standDuration = 0.4f;

    [Header("Câmera")]
    public Vector3 sittingCameraOffset = new Vector3(0f, 1.5f, 0.2f);
    public float cameraLerpSpeed = 8f;

    [Header("Animação")]
    public string sitBoolName = "IsSitting";

    [Header("Interface (UI)")]
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string sitMessage = "Pressione [E] para Sentar";
    public string standMessage = "Pressione [E] para Levantar";
    public bool billboardPrompt = true;

    private Transform _player;
    private CharacterController _cc;
    private Rigidbody _rb;
    private Animator _animator;
    private Camera _cam;

    private Vector3 _camDefaultLocalPos;
    private Quaternion _camDefaultLocalRot;
    private Vector3 _camTargetPos;
    private Quaternion _camTargetRot;

    private Coroutine _activeRoutine;

    private void Start()
    {
        _cam = Camera.main;
        if (_cam != null)
        {
            _camDefaultLocalPos = _cam.transform.localPosition;
            _camDefaultLocalRot = _cam.transform.localRotation;
        }
        _camTargetPos = _camDefaultLocalPos;
        _camTargetRot = _camDefaultLocalRot;

        if (promptUI != null)
            promptUI.SetActive(false);

        UpdatePromptText(sitMessage);
    }

    private void Update()
    {
        if (_player == null)
        {
            FindPlayer();
            return;
        }

        // Lógica de distância e exibição do prompt
        float dist = Vector3.Distance(sitPoint != null ? sitPoint.position : transform.position, _player.position);
        bool inRange = dist <= interactionRadius;
        bool shouldShowPrompt = inRange && _state != State.Transitioning;

        if (promptUI != null)
        {
            if (promptUI.activeSelf != shouldShowPrompt)
                promptUI.SetActive(shouldShowPrompt);

            if (shouldShowPrompt && billboardPrompt && _cam != null)
            {
                promptUI.transform.LookAt(promptUI.transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
            }
        }

        // Detecção de Input (E) - Corrigido para evitar conflito entre sistemas
        bool interactPressed = false;

#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            interactPressed = UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        // Só tenta usar o Input antigo se o novo não disparou ou não está ativo
        if (!interactPressed)
        {
            interactPressed = Input.GetKeyDown(KeyCode.E);
        }
#endif

        // Fallback caso nenhuma diretiva esteja ativa (garante funcionamento básico)
        if (!interactPressed && !Application.isEditor)
        {
            try { interactPressed = Input.GetKeyDown(KeyCode.E); } catch { }
        }

        if (shouldShowPrompt && interactPressed)
        {
            if (_state == State.Standing)
                ExecuteTransition(SitDownRoutine());
            else if (_state == State.Sitting)
                ExecuteTransition(StandUpRoutine());
        }

        // Suavização da Câmera
        if (_cam != null && _cam.transform.parent != null)
        {
            float t = 1f - Mathf.Exp(-cameraLerpSpeed * Time.deltaTime);
            _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, _camTargetPos, t);
            _cam.transform.localRotation = Quaternion.Slerp(_cam.transform.localRotation, _camTargetRot, t);
        }
    }

    private void ExecuteTransition(IEnumerator routine)
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(routine);
    }

    private IEnumerator SitDownRoutine()
    {
        if (sitPoint == null) { Debug.LogError("Sit Point não definido!"); yield break; }

        _state = State.Transitioning;
        if (promptUI != null) promptUI.SetActive(false);

        TogglePlayerPhysics(false);
        
        Vector3 startPos = _player.position;
        Quaternion startRot = _player.rotation;

        _camTargetPos = sittingCameraOffset;
        _camTargetRot = Quaternion.identity;

        float elapsed = 0;
        while (elapsed < sitDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / sitDuration);
            
            _player.position = Vector3.Lerp(startPos, sitPoint.position, t);
            _player.rotation = Quaternion.Slerp(startRot, sitPoint.rotation, t);
            yield return null;
        }

        _player.position = sitPoint.position;
        _player.rotation = sitPoint.rotation;

        SetAnimation(true);
        _state = State.Sitting;
        UpdatePromptText(standMessage);
        _activeRoutine = null;
    }

    private IEnumerator StandUpRoutine()
    {
        _state = State.Transitioning;
        if (promptUI != null) promptUI.SetActive(false);
        SetAnimation(false);

        Vector3 targetPos = sitPoint.TransformPoint(standOffset);
        Vector3 startPos = _player.position;
        Quaternion startRot = _player.rotation;
        Quaternion targetRot = Quaternion.LookRotation(sitPoint.forward, Vector3.up);

        _camTargetPos = _camDefaultLocalPos;
        _camTargetRot = _camDefaultLocalRot;

        float elapsed = 0;
        while (elapsed < standDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / standDuration);
            _player.position = Vector3.Lerp(startPos, targetPos, t);
            _player.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        _player.position = targetPos;
        _player.rotation = targetRot;

        TogglePlayerPhysics(true);
        _state = State.Standing;
        UpdatePromptText(sitMessage);
        _activeRoutine = null;
    }

    private void TogglePlayerPhysics(bool enable)
    {
        if (_cc != null) _cc.enabled = enable;
        if (_rb != null) _rb.isKinematic = !enable;

        MonoBehaviour[] scripts = _player.GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
        {
            if (s == this || s == null) continue;
            string n = s.GetType().Name.ToLower();
            if (n.Contains("movement") || n.Contains("controller") || n.Contains("motor") || n.Contains("fps"))
            {
                s.enabled = enable;
            }
        }
    }

    private void SetAnimation(bool sitting)
    {
        if (_animator != null && !string.IsNullOrEmpty(sitBoolName))
            _animator.SetBool(sitBoolName, sitting);
    }

    private void UpdatePromptText(string text)
    {
        if (promptText != null) promptText.text = text;
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        _player = playerObj.transform;
        _cc = playerObj.GetComponent<CharacterController>();
        _rb = playerObj.GetComponent<Rigidbody>();
        _animator = playerObj.GetComponentInChildren<Animator>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = sitPoint != null ? sitPoint.position : transform.position;
        Gizmos.DrawWireSphere(pos, interactionRadius);
        
        if (sitPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(sitPoint.TransformPoint(standOffset), 0.1f);
        }
    }
}

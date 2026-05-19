using UnityEngine;
using TMPro;

/// <summary>
/// Script auxiliar para exibir prompts de interação em objetos genéricos.
/// </summary>
public class InteractionPrompt : MonoBehaviour
{
    [Header("Referências")]
    public GameObject promptVisual;
    public TextMeshProUGUI promptText;
    public string message = "Pressione [E] para Interagir";

    [Header("Configurações")]
    public float displayDistance = 3f;
    public bool billboardCamera = true;

    private Transform _player;
    private Camera _cam;
    private bool _forceHidden = false;

    private void Start()
    {
        _cam = Camera.main;
        FindPlayer();

        if (promptVisual != null) promptVisual.SetActive(false);
        if (promptText != null) promptText.text = message;
    }

    private void Update()
    {
        if (_player == null)
        {
            FindPlayer();
            return;
        }

        if (promptVisual == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool show = dist <= displayDistance && !_forceHidden;

        if (promptVisual.activeSelf != show)
            promptVisual.SetActive(show);

        if (show && billboardCamera && _cam != null)
        {
            promptVisual.transform.LookAt(
                promptVisual.transform.position + _cam.transform.rotation * Vector3.forward,
                _cam.transform.rotation * Vector3.up);
        }
    }

    private void FindPlayer()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) _player = go.transform;
    }

    public void SetMessage(string newMessage)
    {
        message = newMessage;
        if (promptText != null) promptText.text = newMessage;
    }

    public void Hide() 
    { 
        _forceHidden = true; 
        if (promptVisual != null) promptVisual.SetActive(false); 
    }

    public void Show() 
    { 
        _forceHidden = false; 
    }
}

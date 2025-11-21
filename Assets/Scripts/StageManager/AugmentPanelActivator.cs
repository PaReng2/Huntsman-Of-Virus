using UnityEngine;

[DisallowMultipleComponent]
public class AugmentPanelActivator : MonoBehaviour
{
    [SerializeField] private AugmentSystem augmentSystem;
    [SerializeField] private GameObject panelRoot;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Reset()
    {
        if (panelRoot == null) panelRoot = gameObject;
        if (augmentSystem == null) augmentSystem = FindObjectOfType<AugmentSystem>();
    }

    private void OnEnable()
    {
        if (Application.isPlaying && panelRoot != null && augmentSystem != null)
        {
            bool opened = augmentSystem.TryOpenPanel();
            gameManager.isInteracting = opened;
            if (!opened && panelRoot.activeSelf)
            {
                panelRoot.SetActive(false);
            }
        }
    }

    public void OpenFromCode()
    {
        if (augmentSystem != null)
        {
            bool opened = augmentSystem.TryOpenPanel();
            if (!opened && panelRoot.activeSelf)
                panelRoot.SetActive(false);
        }
    }
}
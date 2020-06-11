using UnityEngine;

/// <summary>Assigns the GameObject that functions as an ImageHolder to the <see cref="ImageManager"/>.</summary>
public class ImageHolderPasser : MonoBehaviour
{
    [SerializeField]
    private bool m_LoadAndSetSceneImages = true;

    void Awake()
    {
        ImageManager.Instance.ImageHolder = gameObject;
        ImageManager.Instance.PrepareForFirstLoad(m_LoadAndSetSceneImages);
    }

    void Update()
    {
        if (Game.Mode == GameMode.Singleplayer)
        {
            ImageManager.Instance.UpdateAlphaValue();
        }
    }

    void OnDestroy()
    {
        ImageManager.Instance.ImageHolder = null;
    }
}

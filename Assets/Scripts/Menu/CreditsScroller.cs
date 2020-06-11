using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Handles the behaviour of the credits in the menu.</summary>
public class CreditsScroller : MonoBehaviour
{
    [SerializeField]
    private GameObject m_credits;
    [SerializeField]
    private GameObject m_mainMenu;

    private Vector3 m_originalPos;

    private const float m_SCROLL_SPEED = 0.05f;
    private const int m_CREDITS_SCREEN_BORDER_Y = 15;
    
    void Awake()
    {
        // Save the original position of credits for reset.
        m_originalPos = gameObject.transform.position;
    }

    void OnEnable()
    {
        gameObject.transform.position = m_originalPos;
    }

    void Update()
    {
        // Scroll the credits.
        gameObject.transform.Translate(Vector3.up * m_SCROLL_SPEED);

        if (gameObject.transform.position.y >= m_CREDITS_SCREEN_BORDER_Y)
        {
            m_mainMenu.SetActive(true);
            m_credits.SetActive(false);
        }
    }

    void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
    }
}

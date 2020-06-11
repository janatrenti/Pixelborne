using System;
using System.Diagnostics;
using UnityEngine;

/// <summary>Implements the fading and hudsymbol swapping for cameras of the game.</summary>
public abstract class GameCamera : MonoBehaviour, ICamera
{
    /// <summary>The time how long a fade takes.</summary>
    [SerializeField]
    protected float m_fadeTime = 1500;
    /// <summary>The fade image.</summary>
    [SerializeField]
    protected GameObject m_fadeImage;

    /// <summary>The fade mode.</summary>
    protected FadeMode m_fadeMode = FadeMode.NoFade;
    /// <summary>The fade stopwatch which is used to determine when the fading is finished.</summary>
    protected Stopwatch m_fadeStopwatch = new Stopwatch();

    /// <summary>Describes the fade mode of the game.</summary>
    protected enum FadeMode
    {
        /// <summary>The fade in mode.</summary>
        FadeIn,
        /// <summary>The fade out mode.</summary>
        FadeOut,
        /// <summary>The no fade mode.</summary>
        NoFade
    }

    /// <summary>Updates this instance by executing the fade function.</summary>
    protected virtual void Update()
    {
        Fade();
    }

    // Implements the fade in and fade out logic.
    private void Fade()
    {
        // Skip function if fade in / fade out was successfully completed and has not been triggered again.
        if (m_fadeMode == FadeMode.NoFade)
        {
            return;
        }

        long elapsedTime = m_fadeStopwatch.ElapsedMilliseconds;
        // Complete the fade when enough time has passed.
        bool isFadeComplete = elapsedTime >= m_fadeTime;

        Color color = m_fadeImage.GetComponent<SpriteRenderer>().color;

        if (m_fadeMode == FadeMode.FadeOut)
        {
            // Lower the opacity of the fade to black canvas object based on the time passed since the fade to black trigger.
            float percentage = elapsedTime / m_fadeTime;
            color.a = isFadeComplete ? 1.0f : percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = color;

            if (isFadeComplete)
            {
                FadeCompleted();
                FadedOut();
            }
        }
        else if (m_fadeMode == FadeMode.FadeIn)
        {
            // Increase the opacity of the fade to black canvas object based on the time passed since fade was triggered. 
            float percentage = 1 - elapsedTime / m_fadeTime;
            color.a = isFadeComplete ? 0.0f : percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = color;

            if (isFadeComplete)
            {
                FadeCompleted();
                FadedIn();
            }
        }
        else
        {
            throw new Exception("Error: Wrong fade mode was triggered.");
        }
    }

    private void FadeCompleted()
    {
        m_fadeMode = FadeMode.NoFade;
        m_fadeStopwatch.Reset();
    }

    /// <summary>Triggers the fade to black animation.</summary>
    public void FadeOut()
    {
        m_fadeStopwatch.Start();
        m_fadeMode = FadeMode.FadeOut;
    }

    /// <summary>Triggers the fade in animation.</summary>
    public void FadeIn()
    {
        m_fadeStopwatch.Start();
        m_fadeMode = FadeMode.FadeIn;
    }

    /// <summary>Is invoked when the fade out has finished.</summary>
    protected abstract void FadedOut();

    /// <summary>Is invoked when the fade in has finished.</summary>
    protected abstract void FadedIn();

    /// <summary>Swaps the hud symbol.</summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="sprite">The sprite.</param>
    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        GameObject hudObject = transform.Find($"{gameObject.name}HudSymbol").gameObject;
        SpriteRenderer spriteRenderer = hudObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}

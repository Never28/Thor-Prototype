using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Slider health;
    public Slider healthVisualizer;
    public Slider focus;
    public Slider focusVisualizer;
    public Slider stamina;
    public Slider staminaVisualizer;

    public float sizeMultiplier = 2;
    public float visualizerSpeed = 2;

    public void InitSlider(StatSlider slider, int value) {

        Slider s = null;
        Slider v = null;
        switch (slider)
        {
            case StatSlider.health:
                s = health;
                v = healthVisualizer;
                break;
            case StatSlider.focus:
                s = focus;
                v = focusVisualizer;
                break;
            case StatSlider.stamina:
                s = stamina;
                v = staminaVisualizer;
                break;
            default:
                break;
        }
        s.maxValue = value;
        v.maxValue = value;
        RectTransform r = s.GetComponent<RectTransform>();
        RectTransform r_v = v.GetComponent<RectTransform>();

        float actualValue = value * sizeMultiplier;
        actualValue = Mathf.Clamp(actualValue, 0, 1000);

        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, actualValue);
        r_v.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, actualValue);
    }

    public void Tick(CharacterStats stats, float delta) {
        health.value = stats._health;
        focus.value = stats._focus;
        stamina.value = stats._stamina;

        healthVisualizer.value = Mathf.Lerp(healthVisualizer.value, stats._health, delta * visualizerSpeed);
        focusVisualizer.value = Mathf.Lerp(focusVisualizer.value, stats._focus, delta * visualizerSpeed);
        staminaVisualizer.value = Mathf.Lerp(staminaVisualizer.value, stats._stamina, delta * visualizerSpeed);
    }

    public void AffectAll(int h, int f, int s) {
        InitSlider(StatSlider.health, h);
        InitSlider(StatSlider.focus, f);
        InitSlider(StatSlider.stamina, s);
    }

    public enum StatSlider { health, focus, stamina}

    public static UIManager singleton;
    void Awake()
    {
        singleton = this;
    }
}

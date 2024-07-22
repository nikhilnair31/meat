using UnityEngine;

[CreateAssetMenu(fileName = "UITransitionData", menuName = "ScriptableObjects/UITransitionData", order = 1)]
public class UITransitionData : ScriptableObject
{
    [Header("UI Related")]
    public float transitionDuration = 1f;
    public float transitionAmount = 0.3f;
    public float canvasOffsetY = 3f;
    public float displayDuration = 2f;
    public bool allowShrinkingOfUI = true;
}
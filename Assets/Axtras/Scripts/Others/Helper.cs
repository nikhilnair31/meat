using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.UI;

public static class Helper 
{
    public static void CameraShake(float amplitude, float duration, float amplitudemultiplier = 1f) {
        // Get the Cinemachine Basic Multi Channel Perlin component from the virtual camera
        CinemachineVirtualCamera virtualCamera = GameObject.Find("Game Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null) {
            Debug.LogError("No CinemachineBasicMultiChannelPerlin component found on the virtual camera.");
            return;
        }

        // Set the initial amplitude and frequency
        noise.m_AmplitudeGain = amplitude * amplitudemultiplier;

        // Create a tween to gradually reduce the amplitude and frequency over the duration
        DOTween.To(() => noise.m_AmplitudeGain, x => noise.m_AmplitudeGain = x, 0f, duration).SetEase(Ease.OutQuad);
    }
    public static Vector3 CameraCenterTargetPoint() {
        // FIXME: Make thrown item go to center of camera
        Camera mainCamera = Camera.main;
        Vector3 screenCenter = new(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane + 1f);
        Vector3 screenCenterWorldPoint = mainCamera.ScreenToWorldPoint(screenCenter);
        Vector3 throwDirection = (screenCenterWorldPoint - mainCamera.transform.position).normalized;

        // Define the maximum throw distance
        float maxThrowDistance = 100f;

        // Perform a raycast from the camera's position in the throw direction
        Ray ray = new(mainCamera.transform.position, throwDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, maxThrowDistance)) {
            return hit.point;
        }
        else {
            return mainCamera.transform.position + throwDirection * maxThrowDistance;
        }
    }

    public static T GetComponentInParentByName<T>(Transform currentTransform, string parentName) where T : Component {
        if (currentTransform == null) {
            // Debug.LogWarning("Current transform is null.");
            return null;
        }
        // Debug.Log($"Checking transform: {currentTransform.name}");

        if (currentTransform.name == parentName) {
            // Debug.Log($"Found parent with name: {parentName}");
            return currentTransform.GetComponent<T>();
        }

        return GetComponentInParentByName<T>(currentTransform.parent, parentName);
    }
    public static T GetComponentInParentByTag<T>(Transform currentTransform, string tagName) where T : Component {
        if (currentTransform == null) {
            // Debug.LogWarning("Current transform is null.");
            return null;
        }
        // Debug.Log($"Checking transform: {currentTransform.name}");

        if (currentTransform.CompareTag(tagName)) {
            // Debug.Log($"Found parent with tag: {tagName}");
            return currentTransform.GetComponent<T>();
        }

        return GetComponentInParentByTag<T>(currentTransform.parent, tagName);
    }

    public static void MoveUpAndDownUI(Transform uiTransform, float moveDistance, float moveDuration) {
        Vector3 originalPosition = uiTransform.position;
        Vector3 targetPosition = new(originalPosition.x, originalPosition.y + moveDistance, originalPosition.z);
        uiTransform.DOMoveY(targetPosition.y, moveDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
    public static void ScaleInAndOutUI(Transform uiTransform, float scaleTo, float scaleInTime) {
        uiTransform.DOScale(scaleTo, scaleInTime);
    }
    public static void UIShake(RectTransform uiElement, float strength, float duration, int vibrato = 100, float randomness = 90, bool fadeOut = true) {
        if (uiElement == null) {
            Debug.LogError("No RectTransform component found on the UI element.");
            return;
        }

        // Perform the shake animation on the UI element
        uiElement.DOShakeAnchorPos(duration, strength, vibrato, randomness, false, fadeOut);
    }
    public static void TransitionMenu(Image transitionImage, GameObject menu, float transitionDuration, bool enable) {
        if (enable) {
            transitionImage.DOFade(1f, transitionDuration)
                .OnStart(() => {
                    transitionImage.DOFade(0f, 0f);
                    menu.SetActive(true);
                })
                .SetUpdate(true);
        }
        else {
            transitionImage.DOFade(0f, transitionDuration)
                .OnComplete(() => {
                    menu.SetActive(false);
                })
                .SetUpdate(true);
        }
    }

    public static Vector3 GetClosestPointOnNavMesh(Vector3 targetPosition, float maxDistance) {
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, maxDistance, NavMesh.AllAreas)) {
            return hit.position;
        }
        else {
            Debug.LogWarning("No valid NavMesh point found within the specified distance.");
            return targetPosition; // Return the original position if no valid point is found.
        }
    }

    public static bool IsRelevantCollider(Collider collider, List<string> tags) {
        foreach (string tag in tags) {
            if (collider.CompareTag(tag)) {
                return true;
            }
        }

        return false;
    }
}
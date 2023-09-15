using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class Helper
{
    private static Camera _camera;
    private static readonly Dictionary<float, WaitForSeconds> waitDictionary = new();
    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;

    public static Camera Camera
    {
        get
        {
            if (_camera == null) { _camera = Camera.main; }
            return _camera;
        }
    }

    public static WaitForSeconds GetWait(float time)
    {
        if (waitDictionary.TryGetValue(time, out WaitForSeconds wait)) { return wait; }

        waitDictionary[time] = new WaitForSeconds(time);
        return waitDictionary[time];
    }

    public static bool IsOverUI()
    {
        _eventDataCurrentPosition = new(EventSystem.current) { position = Input.mousePosition };
        _results = new();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }

    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
        return result;
    }
}

public static class AlphaExtensions
{
    public static void Fade(this SpriteRenderer renderer, float alpha)
    {
        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }

    public static T GetRandom<T>(this IList<T> ts, int initialInclusive = 0, int finalExclusive = 0)
    {
        if (finalExclusive == 0) { finalExclusive = ts.Count; }
        return ts[UnityEngine.Random.Range(initialInclusive, finalExclusive)];
    }

    public static T GetRandom<T>(this T[] ts, int initialInclusive = 0, int finalExclusive = 0)
    {
        if (finalExclusive == 0) { finalExclusive = ts.Length; }
        return ts[UnityEngine.Random.Range(initialInclusive, finalExclusive)];
    }

    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t) { UnityEngine.Object.Destroy(child.gameObject); }
    }

    public static void SetLayersRecursively(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform t in gameObject.transform)
        {
            t.gameObject.SetLayersRecursively(layer);
        }
    }

    public static Vector2 ToV2(this Vector3 input) => new(input.x, input.y);

    public static Vector3 Flat(this Vector3 input) => new(input.x, 0, input.z);

    public static Vector3Int ToVector3Int(this Vector3 vec3) => new((int)vec3.x, (int)vec3.y, (int)vec3.z);
}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioMixerGroup output;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;
    public bool playOnAwake;
    public bool loop;
}

[Serializable]
public class TransformSet1
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformSet1(Vector3 position = new(), Quaternion rotation = new(), Vector3 scale = new())
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
}

[Serializable]
public struct TransformSet
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformSet(Vector3 position = default, Quaternion rotation = default, Vector3 scale = default)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
}

[Serializable]
public class EventAndResponse
{
    public string name;
    public GameEvent gameEvent;
    public UnityEvent genericResponse;
    public UnityEvent<int> sentIntResponse;
    public UnityEvent<bool> sentBoolResponse;
    public UnityEvent<float> sentFloatResponse;
    public UnityEvent<string> sentStringResponse;

    public void EventRaised()
    {
        if (genericResponse.GetPersistentEventCount() >= 1) { genericResponse.Invoke(); }

        if (sentIntResponse.GetPersistentEventCount() >= 1) { sentIntResponse.Invoke(gameEvent.sentInt); }

        if (sentBoolResponse.GetPersistentEventCount() >= 1) { sentBoolResponse.Invoke(gameEvent.sentBool); }

        if (sentFloatResponse.GetPersistentEventCount() >= 1) { sentFloatResponse.Invoke(gameEvent.sentFloat); }

        if (sentStringResponse.GetPersistentEventCount() >= 1) { sentStringResponse.Invoke(gameEvent.sentString); }
    }
}

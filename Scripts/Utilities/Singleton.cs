using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Singleton<T> : MonoBehaviour where T : Component {
    private static T _instance;
    //Lock is so we don't try to change the same singleton on multithreaded behaviors
    private static object _lock = new object();

    public static T Instance {
        get {
            if (applicationIsQuitting)
                return null;
            lock (_lock) {
                if (_instance == null) {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null) {
                        GameObject obj = new GameObject();
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
    }

    public virtual void Awake() {
        DontDestroyOnLoad(this.gameObject);
        if (_instance == null)
            _instance = this as T;
        else
            Destroy(gameObject);
    }

    private static bool applicationIsQuitting = false;

    // Stop object ghosting
    void OnDestroy() {
        applicationIsQuitting = true;
    }
}

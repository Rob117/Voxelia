using UnityEngine;
using System.Collections;

public class ProtoModify : MonoBehaviour {
    Vector2 rot;
    [SerializeField]
    LayerMask validLayers;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out hit, 5, validLayers)) {
                EditTerrain.SetBlock(hit, new BlockAir());
            }
        }
    }

}

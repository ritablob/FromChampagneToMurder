using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MapPins : MonoBehaviour
{
    public RectTransform render_sprite_obj_ref;
    public GameObject map_camera_obj_ref;
    public GameObject pin_button;
    public GameObject pins;

    Camera map_camera_ref;
    GameObject current_pin_prefab;

    // Start is called before the first frame update
    void Start()
    {
        map_camera_ref = map_camera_obj_ref.GetComponent<Camera>();
        DeactivatePlacingPin();
    }

    // Update is called once per frame
    void Update()
    {
        map_camera_ref = map_camera_obj_ref.GetComponent<Camera>();
    }

    public void PlacePin()
    {
        Vector2 mousePos = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(render_sprite_obj_ref, mousePos, null, out Vector2 localPos);

        Vector2 uv = new Vector2(
            (localPos.x + render_sprite_obj_ref.rect.width / 2) / render_sprite_obj_ref.rect.width,
            (localPos.y + render_sprite_obj_ref.rect.height / 2) / render_sprite_obj_ref.rect.height
        );

        RenderTexture renderTexture = render_sprite_obj_ref.GetComponent<RawImage>().texture as RenderTexture;

        Vector3 worldPos = map_camera_ref.ViewportToWorldPoint(new Vector3(uv.x, uv.y, 0));

        GameObject pin = Instantiate(current_pin_prefab, worldPos, Quaternion.identity, transform);
        pin.transform.localPosition = new Vector3(pin.transform.localPosition.x, pin.transform.localPosition.y, 0);

        DeactivatePlacingPin();
    }

    public void ActivatePlacingPin(GameObject pin_prefab)
    {
        current_pin_prefab = pin_prefab;
        pin_button.SetActive(true);
        pins.SetActive(false);
    }

    public void DeactivatePlacingPin()
    {
        pin_button.SetActive(false);
        pins.SetActive(true);
    }
}

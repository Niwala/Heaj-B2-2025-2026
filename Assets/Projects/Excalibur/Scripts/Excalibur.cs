using UnityEngine;
using UnityEngine.InputSystem;

public class Excalibur : MonoBehaviour
{
    //Exposed
    [SerializeField]
    private InputAction mouseClick;

    [SerializeField]
    private new Collider collider;

    [SerializeField]
    private GameObject sparks;

    [SerializeField]
    private Animator animator;


    //Hidden
    private int hitCount = 0;

    const int hitCountTarget = 3;
    const float raycastMaxDistance = 100;

    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MouseClick_performed;
    }

    private void OnDisable()
    {
        mouseClick.Disable();
        mouseClick.performed -= MouseClick_performed;
    }

    void Update()
    {
        
    }

    private void MouseClick_performed(InputAction.CallbackContext obj)
    {
        Camera camera = Camera.main;
        Vector2 screenPos = Mouse.current.position.value;
        Ray ray = camera.ScreenPointToRay(screenPos);

        if (collider.Raycast(ray, out RaycastHit hitInfo, raycastMaxDistance))
        {
            hitCount++;
            SpawnSparks(hitInfo.point, hitInfo.normal);

            if (hitCount < hitCountTarget)
            {
                animator.SetTrigger("Hit");
            }
            else
            {
                animator.SetBool("Fly", true);
            }
        }
    }

    private void SpawnSparks(Vector3 position, Vector3 normal)
    {
        Quaternion rotation = Quaternion.LookRotation(normal);
        GameObject.Instantiate(sparks, position, rotation, transform);
    }
}

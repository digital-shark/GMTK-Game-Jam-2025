using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
	public float Sensitivity
	{
		get { return sensitivity; }
		set { sensitivity = value; }
	}
	[Range(0.1f, 9f)] [SerializeField] float sensitivity = 2f;
	[Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
	[Range(0f, 90f)] [SerializeField] float yRotationLimit = 88f;

	Vector2 rotation = Vector2.zero;
	public Vector2 mouseinput = Vector2.zero;

    private void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }

    void Update()
	{
		rotation.x += mouseinput.x * sensitivity;
		rotation.y += mouseinput.y * sensitivity;
		rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
		var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

		transform.localRotation = xQuat * yQuat; //Quaternions seem to rotate more consistently than EulerAngles. Sensitivity seemed to change slightly at certain degrees using Euler. transform.localEulerAngles = new Vector3(-rotation.y, rotation.x, 0);
	}

	private Interactable lastHit;

	public void interactWithInteractable(InputAction.CallbackContext context) 
	{

		Debug.DrawRay(transform.position, transform.forward, Color.red, 10);
		if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 3, LayerMask.GetMask("Buttons")) && context.started)
		{
			lastHit = hit.collider.transform.GetComponent<Interactable>();
			lastHit.Interact();
		}

		if (context.canceled && lastHit != null) 
		{
			lastHit.stopInteract();
		}
	}
}

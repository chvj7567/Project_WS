using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody rbPlayer;
    [SerializeField] Camera camPlayer;
    [SerializeField] UnitHand leftHand;
    [SerializeField] UnitHand rightHand;

    [SerializeField] float maxSpeed;
    [SerializeField] float mouseSensitivity = 200f;
    [SerializeField] float limitUpperY = 45f;
    [SerializeField] float limitUnderY = 45f;
    float rotationY = 0f;
    float rotationX = 0f;
    
    public UnitHand GetLeftHand() { return leftHand; }
    public UnitHand GetRightHand() { return rightHand; }

    private void Start()
    {
        //CHMMain.UI.ShowUI(EUI.UIAlarm, new UIArg());
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);
        Init();
    }

    void Init()
    {
        rbPlayer.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        leftHand.UpdateState();
        rightHand.UpdateState();
    }

    void Look()
    {
        float lookY = Input.GetAxisRaw("Mouse Y");
        float lookX = Input.GetAxisRaw("Mouse X");

        rotationX = 0f;

        rotationY += lookY * mouseSensitivity * Time.deltaTime;
        rotationX += lookX * mouseSensitivity * Time.deltaTime;

        rotationY = Mathf.Clamp(rotationY, -limitUnderY, limitUpperY);

        if (lookY != 0)
        {
            camPlayer.transform.localEulerAngles = new Vector3(-rotationY, 0f, 0f);
        }
        if (lookX != 0)
        {
            rbPlayer.MoveRotation(rbPlayer.rotation * Quaternion.Euler(new Vector3(0f, rotationX, 0f)));
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            rbPlayer.MovePosition(rbPlayer.position + transform.right * maxSpeed * horizontal * Time.deltaTime);
        }

        if (vertical != 0)
        {
            rbPlayer.MovePosition(rbPlayer.position + transform.forward * maxSpeed * vertical * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        Look();
        Move();
    }
}

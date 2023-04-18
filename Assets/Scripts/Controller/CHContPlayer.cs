using UnityEngine;

public class CHContPlayer : MonoBehaviour
{
    [SerializeField] Rigidbody rbPlayer;
    [SerializeField] Camera camPlayer;
    [SerializeField] CHUnitHand csLeftHand;
    [SerializeField] CHUnitHand csRightHand;

    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float mouseSensitivity = 200f;
    [SerializeField] public bool canSee = true;
    [SerializeField] public bool canMove = true;
    [SerializeField] float limitUpperY = 45f;
    [SerializeField] float limitUnderY = 45f;
    float rotationY = 0f;
    float rotationX = 0f;

    [SerializeField] public float tempHp = 100f;
    

    public CHUnitHand GetLeftHand() { return csLeftHand; }
    public CHUnitHand GetRightHand() { return csRightHand; }

    private void Start()
    {
        //CHMMain.UI.ShowUI(EUI.UIAlarm, new UIArg());
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);
        Init();
    }

    void Init()
    {
        if (rbPlayer) rbPlayer.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (csLeftHand) csLeftHand.UpdateState();
        if (csRightHand) csRightHand.UpdateState();
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
        if (canSee) Look(); 
        if (canMove) Move();
    }
}

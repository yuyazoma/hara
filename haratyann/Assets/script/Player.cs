using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{
    private float moveValue;
    private float jumpValue;
    public TextMeshProUGUI debugText;
    public int[] sw = new int[3];
    public int[] vol = new int[2];
    public int[] swPre = new int[3];
    public VolToSw LRStick = new VolToSw(2200, 2500);
    public VolToSw UDStick = new VolToSw(2400, 2700);
    public bool[] jklPress = new bool[3];
    public bool[] jklToggle = new bool[3];
    public float startTime;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveValue = 1f; // Rolling movement speed
        jumpValue = 1f;
        debugText.text = "debug";
        sw[0] = 1; sw[1] = 1; sw[2] = 1;
        vol[0] = 2300; vol[1] = 2300;
        jklPress[0] = false;
        jklPress[1] = false;
        jklPress[2] = false;
        jklToggle[0] = false;
        jklToggle[1] = false;
        jklToggle[2] = false;
        startTime = -1;
    }

    void FixedUpdate()
    {
        var current = Keyboard.current;
        if (current == null) return;

        // Get Camera Direction
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 movement = Vector3.zero;

        // Stop movement smoothly if within threshold
        if (vol[0] >= 1800 && vol[0] <= 3000 && vol[1] >= 1800 && vol[1] <= 3000)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.001f);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, 0.1f);
            return;
        }

        // Calculate movement input
        if (current.rightArrowKey.isPressed || LRStick.GetKeyH() || vol[0] > 3000)
            movement += cameraRight;
        if (current.leftArrowKey.isPressed || LRStick.GetKeyL() || vol[0] < 1800)
            movement -= cameraRight;
        if (current.upArrowKey.isPressed || UDStick.GetKeyH() || vol[1] > 3000)
            movement += cameraForward;
        if (current.downArrowKey.isPressed || UDStick.GetKeyL() || vol[1] < 1800)
            movement -= cameraForward;

        // Normalize movement
        if (movement.magnitude > 1)
            movement = movement.normalized;

        // Reduce speed when moving backward
        if (Vector3.Dot(cameraForward, movement) < 0)
            movement *= 0.8f;

        // Apply movement force
        rb.AddForce(movement * moveValue, ForceMode.Force);

        // Jump
        if (current.zKey.wasPressedThisFrame || (sw[2] == 0 && swPre[2] == 1))
        {
            rb.AddForce(Vector3.up * jumpValue, ForceMode.Impulse);
        }

        // JKL key toggles
        if (current.jKey.wasPressedThisFrame)
        {
            jklPress[0] = true;
            jklToggle[0] = !jklToggle[0];
        }
        if (current.kKey.wasPressedThisFrame)
        {
            jklPress[1] = true;
            jklToggle[1] = true;
        }
        if (current.kKey.wasReleasedThisFrame)
        {
            jklPress[1] = true;
            jklToggle[1] = false;
        }
        if (current.lKey.wasPressedThisFrame)
        {
            jklPress[2] = true;
            jklToggle[2] = true;
            startTime = 0;
        }

        if (startTime >= 0f)
        {
            startTime += Time.deltaTime;
            if (startTime > 3f)
            {
                jklPress[2] = true;
                jklToggle[2] = false;
                startTime = -1;
            }
        }

        // Update debug text
        debugText.text = string.Format("vol: {0}, {1}, sw: {2}, {3}, {4}", vol[0], vol[1], sw[0], sw[1], sw[2]);

        for (int i = 0; i < swPre.Length; i++)
            swPre[i] = sw[i];
    }
    public class VolToSw
    {
        int vol;
        int previousValue;
        int previousSwStateL;
        int currentSwStateL;
        int previousSwStateH;
        int currentSwStateH;
        int thresholdLowValue;
        int thresholdHighValue;

        bool preparation;

        public VolToSw(int thresholdLow, int thresholdHigh)
        {
            thresholdLowValue = thresholdLow;
            thresholdHighValue = thresholdHigh;
            previousValue = -1;
            preparation = false;
        }

        public int Vol
        {
            set
            {
                this.vol = value;
                if (previousValue > -1)
                {
                    preparation = true;
                }

                if (value > thresholdHighValue)
                {
                    currentSwStateH = 0;
                }
                else
                {
                    currentSwStateH = 1;
                }

                if (value < thresholdLowValue)
                {
                    currentSwStateL = 0;
                }
                else
                {
                    currentSwStateL = 1;
                }
            }

            get { return vol; }
        }

        public void Update()
        {
            previousValue = vol;
            previousSwStateH = currentSwStateH;
            previousSwStateL = currentSwStateL;
        }

        public bool GetKeyDownH() => preparation && previousSwStateH == 1 && currentSwStateH == 0;
        public bool GetKeyUpH() => preparation && previousSwStateH == 0 && currentSwStateH == 1;
        public bool GetKeyH() => currentSwStateH == 0;
        public bool GetKeyDownL() => preparation && currentSwStateL == 0 && previousSwStateL == 1;
        public bool GetKeyUpL() => preparation && previousSwStateL == 1 && currentSwStateL == 0;
        public bool GetKeyL() => currentSwStateL == 0;
    }

}
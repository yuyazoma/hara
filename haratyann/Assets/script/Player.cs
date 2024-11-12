using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{
    private float moveValue;
    private float jumpValue; // Declare jumpValue here
    public TextMeshProUGUI debugText;
    public int[] sw = new int[3];
    public int[] vol = new int[2];
    int[] swPre = new int[3]; 

    // Start is called before the first frame update
    void Start()
    {
        moveValue = 0.01f;
        jumpValue = 5f; // Initialize jumpValue here
        debugText.text = "debug";
        sw[0] = 1; sw[1] = 1; sw[2] = 1;
        vol[0] = 2000; vol[1] = 2000;
    }

    // Update is called once per frame
    void Update()
    {
        var current = Keyboard.current;

        if (current == null)
            return;

        // Move right
        if (current.rightArrowKey.isPressed || sw[0] == 0 || vol[0] > 3400)
            this.transform.position += new Vector3(moveValue, 0f, 0f);

        // Move left
        if (current.leftArrowKey.isPressed || sw[1] == 0 || vol[0] < 4000)
            this.transform.position += new Vector3(-moveValue, 0f, 0f);

        // Jump
        if (current.zKey.wasPressedThisFrame || sw[2] == 0 && swPre[2] == 1)
        {
            GetComponent<Rigidbody>().velocity = Vector3.up * jumpValue;
        }

        // Update debug text
        string str = string.Format("vol: {0}, {1}, sw: {2}, {3}, {4}", vol[0], vol[1], sw[0], sw[1], sw[2]);
        debugText.text = str;

        for (int i = 0; i < swPre.Length; i++)
            swPre[i] = sw[i];
    }
}

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
    int[] swPre = new int[3];

    void Start()
    {
        moveValue = 0.01f;
        jumpValue = 5f;
        debugText.text = "debug";
        sw[0] = 1; sw[1] = 1; sw[2] = 1;
        vol[0] = 2300; vol[1] = 2300;
    }

    void Update()
    {
        var current = Keyboard.current;

        if (current == null)
            return;

        // Right movement
        if (current.rightArrowKey.isPressed || sw[0] == 0 || vol[0] > 2600)
        {
            this.transform.position += new Vector3(moveValue, 0f, 0f);
            if (current.rightArrowKey.isPressed || sw[0] == 0 || vol[0] > 4090)
                this.transform.position += new Vector3(2 * moveValue, 0f, 0f);
        }

        // Left movement
        if (current.leftArrowKey.isPressed || sw[1] == 0 || vol[0] < 2200)
        {
            this.transform.position += new Vector3(-moveValue, 0f, 0f);
            if (current.leftArrowKey.isPressed || sw[1] == 0 || vol[0] < 1000)
                this.transform.position += new Vector3(-2 * moveValue, 0f, 0f);
        }

        // Forward (up) movement along Z-axis
        if (current.upArrowKey.isPressed || sw[0] == 0 || vol[1] > 2600)
        {
            this.transform.position += new Vector3(0f, 0f, moveValue); // ZŽ²•ûŒü‚É‘Oi
            if (current.upArrowKey.isPressed || sw[0] == 0 || vol[1] > 4090)
                this.transform.position += new Vector3(0f, 0f, 2 * moveValue);
        }

        // Backward (down) movement along Z-axis
        if (current.downArrowKey.isPressed || sw[1] == 0 || vol[1] < 2200)
        {
            this.transform.position += new Vector3(0f, 0f, -moveValue); // ZŽ²•ûŒü‚ÉŒã‘Þ
            if (current.downArrowKey.isPressed || sw[1] == 0 || vol[1] < 1000)
                this.transform.position += new Vector3(0f, 0f, -2 * moveValue);
        }

        // Jump
        if (current.zKey.wasPressedThisFrame || (sw[2] == 0 && swPre[2] == 1))
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

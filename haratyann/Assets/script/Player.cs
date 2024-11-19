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

    void Start()
    {
        moveValue = 0.01f;
        jumpValue = 5f;
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
            this.transform.position += new Vector3(0f, 0f, moveValue); // Z軸方向に前進
            if (current.upArrowKey.isPressed || sw[0] == 0 || vol[1] > 4090)
                this.transform.position += new Vector3(0f, 0f, 2 * moveValue);
        }

        // Backward (down) movement along Z-axis
        if (current.downArrowKey.isPressed || sw[1] == 0 || vol[1] < 2200)
        {
            this.transform.position += new Vector3(0f, 0f, -moveValue); // Z軸方向に後退
            if (current.downArrowKey.isPressed || sw[1] == 0 || vol[1] < 1000)
                this.transform.position += new Vector3(0f, 0f, -2 * moveValue);
        }

        // Jump
        if (current.zKey.wasPressedThisFrame || (sw[2] == 0 && swPre[2] == 1))
        {
            GetComponent<Rigidbody>().velocity = Vector3.up * jumpValue;
        }

        if(current.jKey.wasPressedThisFrame)
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
        string str = string.Format("vol: {0}, {1}, sw: {2}, {3}, {4}", vol[0], vol[1], sw[0], sw[1], sw[2]);
        debugText.text = str;

        for (int i = 0; i < swPre.Length; i++)
            swPre[i] = sw[i];
    }
}


// アナログ値をデジタル値に変換するクラス
// HIGH側しきい値を超えたときに1、LOW側しきい値を下回ったときに-1、その他は0に変換する。
// GetKeyDown* で押したとき、GetKeyUp* で離したとき、GetKey* で連射対策前のデータが取得できる。
// コンストラクタでしきい値を設定、ループ処理の最後でupdateを呼ぶ。

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

    public bool GetKeyDownH()
    {
        if (preparation && previousSwStateH == 1 && currentSwStateH == 0)
        {
            string str = string.Format("val:{0}, DownH", Vol);
            Debug.Log(str);

            return true;
        }
        else
            return false;
    }

    public bool GetKeyUpH()
    {
        if (preparation && previousSwStateH == 0 && currentSwStateH == 1)
        {
            string str = string.Format("val:{0}, UpH", Vol);
            Debug.Log(str);
            return true;
        }
        else
            return false;
    }

    public bool GetKeyH()
    {
        if (currentSwStateH == 0)
            return true;
        else
            return false;
    }
    public bool GetKeyDownL()
    {
        if (preparation && currentSwStateL == 0 && previousSwStateL == 1)
            return true;
        else
            return false;
    }

    public bool GetKeyUpL()
    {
        if (preparation && currentSwStateL == 1 && previousSwStateL == 0)
            return true;
        else
            return false;
    }

    public bool GetKeyL()
    {
        if (currentSwStateL == 0)
            return true;
        else
            return false;
    }
}
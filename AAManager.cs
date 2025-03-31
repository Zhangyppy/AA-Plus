using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class AAManager : MonoBehaviour
{
    public static int serialNumber = 0;
    public int MenuColumn;

    public bool isMusicMenu = false;

    public GameObject rotateObj;
    public GameObject triggerZone;
    public GameObject head;
    public GameObject canvas;
    public GameObject gazePointer;
    public Sprite[] image;
    public GameObject[] appIcon;
    public GameObject[] btnCollider;
    public Button[] leftGrow;
    public Button[] rightGrow;
    public GameObject[] leftArrow;
    public GameObject[] rightArrow;
    public GameObject sign;
    public GameObject sceneManager;

    public GameObject leftOrUpArrow;
    public GameObject rightOrDownArrow;
    public Button leftOrUpGrowBody;
    public Button rightOrDownGrowBody;

    public Text errorNumber;
    public Text overSign;
    public Text timer;

    private Vector3 normalVector;
    private Vector3 enterButtonRowForward;
    private Vector3 enterButtonRowRight;
    private Vector3 canvasForward;
    private Vector3 oriHeadP;
    private Vector3 newHeadP;
    private Vector3 difHeadP;
    private Vector3 oriV;
    private Vector3 nowV;
    private Vector3 addV;
    private Vector3 oriForward;
    private Vector3 nowForward;
    private float angleV = 0f;

    private float time = 0;
    private float leftAmount;
    private float rightAmount;
    private float arrowRange;
    private float angleTurn;
    private float difHeadD = 0f;
    private float plusMinus;

    private int errorNum = 0;
    private int numUI = 16;
    private int orderIndex;
    private int targetNum;
    private int beginNum = 0;

    private bool enterRow = false;
    private bool isBack = false;
    private bool getCanvasForward = false;
    private bool resetArrowP = false;
    private bool over;

    private string path;
    private string menuColumn;

    private int[] orderTarget4 = new int[] { 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11, 3, 12, 4, 11 };
    private int[] orderTarget8 = new int[] { 4, 12, 2, 10, 5, 3, 13, 11, 4, 12, 2, 10, 5, 12, 13, 11, 4, 12, 2, 10, 5, 3, 13, 11, 4, 12, 2, 10, 5, 12, 13, 11, 4, 12, 2, 10, 5, 3, 13, 11, 4, 12, 2, 10, 5, 12, 13, 11, 4, 12, 2, 10, 5, 3, 13, 11, 4, 12, 2, 10, 5, 12, 13, 11 };
    private int[] orderTarget12 = new int[] { 2, 4, 3, 5, 10, 12, 13, 3, 5, 10, 9, 11, 10, 2, 4, 9, 2, 4, 11, 5, 10, 12, 13, 3, 5, 10, 9, 11, 10, 2, 4, 9, 2, 4, 9, 5, 10, 12, 13, 3, 5, 10, 9, 11, 10, 2, 4, 9, 2, 4, 11, 5, 10, 12, 13, 3, 5, 10, 9, 11, 10, 2, 4, 9 };
    private int[] orderTarget16 = new int[] { 0, 4, 12, 10, 1, 8, 6, 9, 6, 10, 0, 12, 1, 4, 9, 8, 0, 4, 12, 10, 1, 8, 6, 9, 6, 10, 0, 12, 1, 4, 9, 8, 0, 4, 12, 10, 1, 8, 6, 9, 6, 10, 0, 12, 1, 4, 9, 8, 0, 4, 12, 10, 1, 8, 6, 9, 6, 10, 0, 12, 1, 4, 9, 8 };
    private int[] orderTarget;

    // Start is called before the first frame update
    void Start()
    {
        leftAmount = 0f;
        rightAmount = 0f;
        arrowRange = 160f;
        serialNumber = targetNum;
        Hashtable hashtable1 = new Hashtable();

        //���Ŀ����
        if (MenuColumn == 2)
        {
            orderTarget = orderTarget4;
            menuColumn = "2 Column";
        }
        else if (MenuColumn == 4)
        {
            orderTarget = orderTarget8;
            menuColumn = "4 Column";
        }
        else if (MenuColumn == 6)
        {
            orderTarget = orderTarget12;
            menuColumn = "6 Column";
        }
        else
        {
            orderTarget = orderTarget16;
            menuColumn = "8 Column";
        }

        over = false;
        targetNum = orderTarget[0];
        orderIndex = 1;
        appIcon[targetNum].GetComponent<Image>().sprite = image[0];
        IEnumerable<int> list = GenerateNoDuplicateRandom(1, 15);
        foreach (int item in list)
        {
            if (beginNum != targetNum) 
            {
                appIcon[beginNum].GetComponent<Image>().sprite = image[item];
                beginNum++;
            }
            else
            {
                beginNum++;
                appIcon[beginNum].GetComponent<Image>().sprite = image[item];
                beginNum++;
            }
        }
        beginNum = 0;
        path = Application.persistentDataPath;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(head.transform.position, head.transform.forward);
        RaycastHit hit;

        if (canvas.activeSelf)
        {
            time += Time.deltaTime;

            if (!getCanvasForward)
            {
                canvas.transform.position = new Vector3(rotateObj.transform.position.x, canvas.transform.position.y, rotateObj.transform.position.z);
                oriHeadP = head.transform.position;
                getCanvasForward = true;
            }
            canvasForward = canvas.transform.forward;

            newHeadP = head.transform.position;
            difHeadP = newHeadP - oriHeadP;
            difHeadP = Vector3.Project(difHeadP, canvasForward);
            if (Vector3.Dot(difHeadP, canvasForward) < 0)
            {
                plusMinus = -1;
            }
            else
            {
                plusMinus = 1;
            }
            difHeadD = difHeadP.magnitude;
            canvas.transform.position += canvasForward * difHeadD * plusMinus;
        }

        if (Physics.Raycast(ray, out hit) && canvas.activeSelf)
        {
            gazePointer.transform.position = hit.point;

            if (hit.transform.name == "collider")
            {
                triggerZone.GetComponent<BoxCollider>().enabled = false;
                gazePointer.SetActive(false);
            }
            if (!triggerZone.GetComponent<BoxCollider>().enabled)
            {
                for (int i = 0; i < 8; i++)
                {
                    btnCollider[i].SetActive(true);
                }
            }

            if (hit.transform.name.StartsWith("Collider"))
            {
                if (int.TryParse(hit.transform.name.Substring(8), out int index))
                {
                    ResetOtherColumn(index);
                    ColorGrow(index);
                }
            }


            if (hit.transform.name == "Groundback")
            {
                leftOrUpArrow.SetActive(false);
                rightOrDownArrow.SetActive(false);

                leftOrUpGrowBody.image.fillAmount = 0;
                rightOrDownGrowBody.image.fillAmount = 0;

                enterRow = false;
                rightAmount = 0;
                leftAmount = 0;
                isBack = false;
            }
        }

        {
            oriHeadP = newHeadP;
        }
    }

    //If not finish, target appear randomly
    private void RandomTarget()
    {
        if (orderIndex < 48)
        {
            targetNum = orderTarget[orderIndex];
            appIcon[targetNum].GetComponent<Image>().sprite = image[0];
            IEnumerable<int> list = GenerateNoDuplicateRandom(1, 15);

            foreach (int item in list)
            {
                if (beginNum != targetNum)
                {
                    appIcon[beginNum].GetComponent<Image>().sprite = image[item];
                    beginNum++;
                }
                else
                {
                    beginNum++;
                    appIcon[beginNum].GetComponent<Image>().sprite = image[item];
                    beginNum++;
                }
            }

            // Reset the begin number for the next round
            beginNum = 0;
            orderIndex++;

            // Store the serial number of the target
            serialNumber = targetNum;

            // Show the sign and hide unnecessary UI elements
            sign.SetActive(true);
            canvas.SetActive(false);
            gazePointer.SetActive(false);
        }
        else
        {
            targetNum = -1;
            overSign.text = "Over!";
            timer.text = time.ToString();

            errorNumber.text = ((float)((errorNum * 1.00f / 48) * 100)).ToString("0.000000") + "%";
            timer.text = ((float)((time * 1.00f / 48))).ToString("0.000000");

            // Log the results to a file
            Createfile(path, "MenuAA.txt", menuColumn + "Time : " + timer.text + "\n" + "Error : " + errorNumber.text + "\n");

            // Hide UI elements and show the scene manager
            canvas.SetActive(false);
            gazePointer.SetActive(false);
            sceneManager.SetActive(true);
        }
    }

    private void ResetOtherColumn(int n)
    {
        for (int i = 0; i < 7; i++) 
        {
            if (i != n)
            {
                rightGrow[i].image.fillAmount = 0;
                leftGrow[i].image.fillAmount = 0;
                rightArrow[i].SetActive(false);
                leftArrow[i].SetActive(false);

                rightArrow[i].transform.localPosition = new Vector3(0, arrowRange / 2 - rightGrow[i].image.fillAmount * arrowRange - 17, 0);
                leftArrow[i].transform.localPosition = new Vector3(0, leftGrow[i].image.fillAmount * arrowRange - arrowRange / 2 + 17, 0);
            }
        }
    }

    // private void RepositionArrowandBody
    // {
        
    // }

    private void ColorGrow(int num)
    {
        if (!enterRow)
        {
            enterButtonRowForward = head.transform.forward;
            enterButtonRowRight = head.transform.up;
            oriV = head.transform.up;
            addV = head.transform.right;
            oriForward = head.transform.forward;
            enterRow = true;
        }

        
        leftOrUpArrow.SetActive(true);
        rightOrDownArrow.SetActive(true);
        leftOrUpArrow.GetComponent<Image>().color = new Color(1f, 1f, 1f, 180 / 255f);
        rightOrDownArrow.GetComponent<Image>().color = new Color(1f, 1f, 1f, 180 / 255f);


        angleV = Vector3.SignedAngle(addV, head.transform.right, Vector3.up);
        nowV = Quaternion.AngleAxis(angleV, Vector3.up) * oriV;
        nowForward = Quaternion.AngleAxis(angleV, Vector3.up) * oriForward;
        normalVector = Vector3.ProjectOnPlane(head.transform.forward, head.transform.right);
        enterButtonRowForward = Vector3.ProjectOnPlane(nowForward, head.transform.right);
        float result = Vector3.Dot(head.transform.forward, nowV);
        angleTurn = Vector3.Angle(normalVector, enterButtonRowForward);



        if (!isMusicMenu)
        {
            if (result < 0)
            {
                leftOrUpGrowBody.image.fillAmount = 0;
                rightAmount = angleTurn / 9.648f;
                rightOrDownGrowBody.image.fillAmount = rightAmount;

                rightOrDownArrow.transform.localPosition = new Vector3(0, arrowRange / 2 - rightOrDownGrowBody.image.fillAmount * arrowRange - 17, 0);


                if (rightAmount >= 1 && !isBack)
                {
                    if (num + 8 == targetNum)
                    {
                        RandomTarget();
                        enterRow = false;
                    }
                    else
                    {
                        errorNum++;
                        RandomTarget();
                        enterRow = false;
                    }
                    leftOrUpArrow.SetActive(false);
                    rightOrDownArrow.SetActive(false);
                    isBack = true;
                    canvas.SetActive(false);
                    getCanvasForward = false;
                    for (int i = 0; i < 8; i++)
                    {
                        btnCollider[i].SetActive(false);
                    }
                    triggerZone.GetComponent<BoxCollider>().enabled = true;
                }

                if (rightAmount < 1)
                {
                    isBack = false;
                }
            }
            else
            {
                rightOrDownGrowBody.image.fillAmount = 0;
                leftAmount = angleTurn / 6.783f;
                leftOrUpGrowBody.image.fillAmount = leftAmount;

                leftOrUpArrow.transform.localPosition = new Vector3(0, leftOrUpGrowBody.image.fillAmount * arrowRange - arrowRange / 2 + 17, 0);

                if (leftAmount >= 1 && !isBack)
                {
                    if (num == targetNum)
                    {
                        leftOrUpGrowBody.image.fillAmount = 0;
                        RandomTarget();
                        enterRow = false;
                    }
                    else
                    {
                        errorNum++;
                        RandomTarget();
                        enterRow = false;
                    }
                    leftOrUpArrow.SetActive(false);
                    rightOrDownArrow.SetActive(false);
                    isBack = true;
                    canvas.SetActive(false);
                    getCanvasForward = false;
                    for (int i = 0; i < 8; i++)
                    {
                        btnCollider[i].SetActive(false);
                    }
                    triggerZone.GetComponent<BoxCollider>().enabled = true;
                }
                if (leftAmount < 1)
                {
                    isBack = false;
                }
            }
        }
        else
        {
            if (result < 0)
            {
                leftOrUpGrowBody.image.fillAmount = 0;
                rightAmount = angleTurn / 9.648f;
                rightOrDownGrowBody.image.fillAmount = rightAmount;

                rightOrDownArrow.transform.localPosition = new Vector3(0, arrowRange / 2 - rightOrDownGrowBody.image.fillAmount * arrowRange - 17, 0);


                if (rightAmount >= 1 && !isBack)
                {
                    if (num + 8 == 13)
                    {
                        rightGrow[num].image.fillAmount = 0;
                        RandomTarget();
                        SceneManager.LoadScene("MusicMenu");
                        isBack = true;
                        canvas.SetActive(false);
                        getCanvasForward = false;
                        for (int i = 0; i < 8; i++)
                        {
                            btnCollider[i].SetActive(false);
                        }
                        triggerZone.GetComponent<BoxCollider>().enabled = true;
                        SceneManager.LoadScene("AAMenu");
                        leftOrUpArrow.SetActive(false);
                        rightOrDownArrow.SetActive(false);
                    }
                }

                if (rightAmount < 1)
                {
                    isBack = false;
                }
            }
        }
    }

    public static void Createfile(string path, string name, string info)
    {
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists)
        {
            sw = t.CreateText();
        }
        else
        {
            sw = t.AppendText();
        }
        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();
    }

    public IEnumerable<int>
      GenerateNoDuplicateRandom(int minValue, int maxValue)
    {
        return Enumerable.Range(minValue, maxValue).OrderBy(g => System.Guid.NewGuid());
    }
}

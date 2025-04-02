using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class AAExtension : MonoBehaviour
{
    public static int serialNumber = 0;
    public int MenuColumn;

    public bool isMusicMenu = false;

    public enum MappingOption
    {
        Low = 0,    // 0.5 (Small head movement -> Large cursor movement)
        Medium = 1, // 1.0 (1:1 head-to-cursor mapping)
        High = 2    // 1.5 (Large head movement -> Small cursor movement)
    }

    [Header("Cursor Movement Mapping Options")]
    [Tooltip("Adjust how head movement translates to cursor movement.")]
    public MappingOption cursorMapping = MappingOption.Medium;

    // Adjust cursor speed based on the selected mapping option
    private float cursorSpeed;

    public GameObject upButton;
    public GameObject downButton;
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject functionButton;
    // public GameObject menuIndicatorCursor;

    public int columnNumber = 9;
    public int rawNumber = 2; 

    private float smallBlock = 2f;
    private float largeBlock = 6f;
    private float yawthreshold = 15f;

    private float lastYaw = 0f;
    private float currentYaw = 0f;
    private float deltaYaw = 0f;
    private float yawSpeed = 0f;
    private float allYawAngle = 0f;
    private float menuDistance = 3f;
    private float scaleX = 0f;
    private float scaleUI = 0f;
    private float horizontal = 21.42f;
    private float rowSpaceFOV = 0f;
    private float rowSpaceDistance = 0f;
    private float UISize = 0f;

    private Vector3 currentHeadRotation = Vector3.zero;
    private Vector3 lastHeadRotation = Vector3.zero;
    private Vector3 deltaHeadRotation = Vector3.zero;
    private Vector3 yawDirectionIndex = Vector3.zero;

    public Sprite[] image;
    public GameObject[] appIcon;
    private GameObject[] upButtonArray;
    private GameObject[] downButtonArray;
    private GameObject[] leftButtonArray;
    private GameObject[] rightButtonArray;
    private GameObject[] functionButtonArray;
    private GameObject[] allButtonArray;
    private GameObject[,] buttonArray;
    private GameObject[] upBtnGrowBody;
    private GameObject[] upBtnArrow;
    private GameObject[] upBtnImage;
    private GameObject[] downBtnGrowBody;
    private GameObject[] downBtnArrow;
    private GameObject[] downBtnImage;
    private Transform[] functionButtons;

    private float lastPointerX = 0f;
    private float currentPointerX = 0f;
    private float adjustedX = 0f;


    // 1 means yaw left, -1 means yaw right
    private int yawLeftOrRight = 1; 
    private int columnIndex = 4;
    private int lastIndex = -1;

    private bool getFastYawDirectionIndex = false;
    private bool getSlowYawDirectionIndex = false;

    public GameObject rotateObj;
    public GameObject triggerZone;
    public GameObject head;
    public GameObject canvas;
    public GameObject gazePointer;
    public GameObject[] btnCollider;
    public Button[] leftGrow;
    public Button[] rightGrow;
    public GameObject[] leftArrow;
    public GameObject[] rightArrow;
    public GameObject sign;
    public GameObject sceneManager;
    public GameObject cursor;

    // public Text errorNumber;
    // public Text overSign;
    // public Text timer;

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
    private int[] appIconNum = new int[16];
    private int[] imageNum = new int[16];
    private int beginNum = 0;

    private bool enterRow = false;
    private bool isBack = false;
    private bool getCanvasForward = false;
    private bool resetArrowP = false;
    private bool over;

    private string path;
    private string menuColumn;

    private int[] orderTarget16 = new int[] { 0, 4, 12, 10, 1, 8, 6, 9, 6, 10, 0, 12, 1, 4, 9, 8, 0, 4, 12, 10, 1, 8, 6, 9, 6, 10, 0, 12, 1, 4, 9, 8, 0, 4, 12, 10, 1, 8, 6, 9, 6, 10, 0, 12, 1, 4, 9, 8, 0, 4, 12, 10, 1, 8, 6, 9, 6, 10, 0, 12, 1, 4, 9, 8 };
    private int[] orderTarget;

    // Start is called before the first frame update
    void Start()
    {
    // Initialize yaw and button arrays
        lastYaw = head.transform.eulerAngles.y;

        upButtonArray = new GameObject[columnNumber];
        downButtonArray = new GameObject[columnNumber];
        functionButtonArray = new GameObject[rawNumber];
        buttonArray = new GameObject[columnNumber, 2];
        upBtnGrowBody = new GameObject[columnNumber];
        upBtnArrow = new GameObject[columnNumber];
        upBtnImage = new GameObject[columnNumber];
        downBtnGrowBody = new GameObject[columnNumber];
        downBtnArrow = new GameObject[columnNumber];
        downBtnImage = new GameObject[columnNumber];

        switch (cursorMapping)
        {
            case MappingOption.Low:
                cursorSpeed = 0.5f; // Small head movement -> Large cursor movement (fast)
                break;
            case MappingOption.Medium:
                cursorSpeed = 1.0f; // 1:1 mapping
                break;
            case MappingOption.High:
                cursorSpeed = 1.5f; // Large head movement -> Small cursor movement (slow)
                break;
        }

        scaleX = 2 * menuDistance * Mathf.Tan(horizontal * Mathf.PI / 180);
        rowSpaceDistance = menuDistance * Mathf.Tan(rowSpaceFOV * Mathf.PI / 180);
        scaleUI = scaleX / columnNumber;



        for (int i = 0; i < rawNumber; i++)
        {
            
            GameObject functionInstance = Instantiate
            (
                functionButton, 
                new Vector3
                (
                    canvas.transform.position.x, 
                    canvas.transform.position.y + ((rawNumber - 2 * i - 1f) / 2f) * (rowSpaceDistance + scaleUI), 
                    canvas.transform.position.z
                ), 
                Quaternion.identity
            );

            functionInstance.name = "functionButton_" + i;  
            functionButtonArray[i] = functionInstance;

            functionInstance.transform.SetParent(canvas.transform, false);

            functionInstance.transform.localPosition = new Vector3(functionInstance.transform.position.x, functionInstance.transform.position.y, 0);
        }

        for (int i = 0; i < columnNumber; i++)
        {
            if (i != (int)(columnNumber / 2))
            {
                GameObject upInstance = Instantiate
                (
                    upButton, 
                    new Vector3
                    (
                        functionButtonArray[0].transform.position.x + scaleUI * (i - columnNumber / 2), 
                        functionButtonArray[0].transform.position.y, 
                        functionButtonArray[0].transform.position.z
                    ), 
                    Quaternion.identity
                );
                upInstance.name = "upButton_" + i;

                upInstance.transform.SetParent(canvas.transform);


                if (i < (int)(columnNumber / 2))
                {
                    upButtonArray[i] = upInstance;
                    upBtnGrowBody[i] = upButtonArray[i].transform.Find("GrowBody")?.gameObject;
                    upBtnGrowBody[i].SetActive(false);
                    upBtnArrow[i] = upButtonArray[i].transform.Find("Arrow")?.gameObject;
                    upBtnArrow[i].SetActive(false);
                    upBtnImage[i] = upButtonArray[i].transform.Find("Image")?.gameObject;
                }
                else
                {
                    upButtonArray[i - 1] = upInstance;
                    upBtnGrowBody[i - 1] = upButtonArray[i - 1].transform.Find("GrowBody")?.gameObject;
                    upBtnGrowBody[i - 1].SetActive(false);
                    upBtnArrow[i - 1] = upButtonArray[i - 1].transform.Find("Arrow")?.gameObject;
                    upBtnArrow[i - 1].SetActive(false);
                    upBtnImage[i - 1] = upButtonArray[i - 1].transform.Find("Image")?.gameObject;
                }

                GameObject downInstance = Instantiate
                (
                    downButton, 
                    new Vector3
                    (
                        functionButtonArray[1].transform.position.x + scaleUI * (i - columnNumber / 2), 
                        functionButtonArray[1].transform.position.y, 
                        functionButtonArray[1].transform.position.z
                    ), 
                    Quaternion.identity
                );
                downInstance.name = "downButton_" + i;  

                downInstance.transform.SetParent(canvas.transform);
 
                if (i < (int)(columnNumber / 2))
                {
                    downButtonArray[i] = downInstance;
                    downBtnGrowBody[i] = downButtonArray[i].transform.Find("GrowBody")?.gameObject;
                    downBtnGrowBody[i].SetActive(false);
                    downBtnArrow[i] = downButtonArray[i].transform.Find("Arrow")?.gameObject;
                    downBtnArrow[i].SetActive(false);
                    downBtnImage[i] = downButtonArray[i].transform.Find("Image")?.gameObject;
                }
                else
                {
                    downButtonArray[i - 1] = downInstance;
                    downBtnGrowBody[i - 1] = downButtonArray[i - 1].transform.Find("GrowBody")?.gameObject;
                    downBtnGrowBody[i - 1].SetActive(false);
                    downBtnArrow[i - 1] = downButtonArray[i - 1].transform.Find("Arrow")?.gameObject;
                    downBtnArrow[i - 1].SetActive(false);
                    downBtnImage[i - 1] = downButtonArray[i - 1].transform.Find("Image")?.gameObject;
                }

                if (i < (int)(columnNumber / 2))
                {
                    buttonArray[i, 0] = upInstance;
                    buttonArray[i, 1] = downInstance;
                }
                else
                {
                    buttonArray[i-1, 0] = upInstance;
                    buttonArray[i-1, 1] = downInstance;
                }
                
            }
        }
        lastPointerX = currentPointerX;
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(head.transform.position, head.transform.forward);

        // canvas follows user
        if (canvas.activeSelf)
        {
            time += Time.deltaTime;

            if (!getCanvasForward)
            {
                canvas.transform.position = new Vector3(rotateObj.transform.position.x, canvas.transform.position.y, rotateObj.transform.position.z);
                oriHeadP = head.transform.position;
                getCanvasForward = true;
                
                //------------------------------------------------------------------------------------------------------
                lastHeadRotation = head.transform.eulerAngles;
                yawDirectionIndex = head.transform.right; 
                
                lastYaw = head.transform.eulerAngles.y;
                currentYaw = head.transform.eulerAngles.y;
                //------------------------------------------------------------------------------------------------------
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
                // gazePointer.SetActive(false);
            }
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            float minDist = Mathf.Infinity;
            int targetIndex = -1;

            Vector3 buttonPos = functionButtonArray[0].transform.position;
            Vector3 buttonScale = functionButtonArray[0].transform.localScale;
            float buttonWidth = buttonScale.x;

            // 处理左右超出范围，光标跟随头部
            if (hit.point.x < buttonPos.x - scaleUI / 2 || hit.point.x > buttonPos.x + scaleUI / 2)
            {
                // 左右超出范围，光标跟随头部
                //gazePointer.transform.position = hit.point;
                // gazePointer.SetActive(false);
                // Adjust cursor position based on the current cursorSpeed
                Vector3 hitPoint = hit.point;
                currentPointerX = hitPoint.x;
                float moveSpeed;
                if ((currentPointerX - lastPointerX) / Time.deltaTime > 100)
                {
                    moveSpeed = 3f;
                }
                else
                {
                    moveSpeed = cursorSpeed;
                }
                adjustedX += (currentPointerX - lastPointerX) * moveSpeed; // Example of adjusting X based on cursorSpeed
                cursor.transform.position = new Vector3(adjustedX, 0, 0);

                // Additional logic for up/down button visibility based on hit point
                UpdateButtonVisibility(cursor.transform.position);

                lastPointerX = currentPointerX;
                // return;
            }
            // else
            // {
            //     gazePointer.SetActive(false);
            //     gazePointer.transform.position = hit.point;
            // }

            // 处理上下界情况
            if (hit.point.y < functionButtonArray[functionButtonArray.Length - 1].transform.position.y)
            {
                // 低于第一个按钮，光标跟随头部
                gazePointer.transform.position = hit.point;
                return;
            }
            if (hit.point.y > functionButtonArray[0].transform.position.y)
            {
                // higher than the first button, follow user
                gazePointer.transform.position = hit.point;
                return;
            }

            // 找到光标最近的 functionButtonArray[i] 和 functionButtonArray[i+1]
            for (int i = 0; i < functionButtonArray.Length - 1; i++)
            {
                float yMin = functionButtonArray[i].transform.position.y;
                float yMax = functionButtonArray[i + 1].transform.position.y;

                if (hit.point.y >= Mathf.Min(yMin, yMax) && hit.point.y <= Mathf.Max(yMin, yMax))
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex != -1)
            {
                // 计算 Y 轴中间位置
                float middleY = (functionButtonArray[targetIndex].transform.position.y + functionButtonArray[targetIndex + 1].transform.position.y) / 2;

                // 让 gazePointer 固定在中间位置，并且 X 和 Z 轴不变
                gazePointer.transform.position = new Vector3
                (
                    functionButtonArray[targetIndex].transform.position.x, // X 轴锁定
                    middleY,
                    functionButtonArray[targetIndex].transform.position.z  // Z 轴锁定
                );
            }
            else
            {
                // 没找到合适的位置，光标跟随头部
                gazePointer.transform.position = hit.point;
            }

            UpdateUpDownButtons(targetIndex);
        }

        {
            oriHeadP = newHeadP;
        }

        // HandleGazePointer();
    }

    void UpdateUpDownButtons(int targetIndex)
    {
        // 先隐藏所有按钮
        HideAllUpDownButtons();

        for (int i = 0; i < columnNumber; i++)
        {
            // 获取左右列的偏移量
            float xOffset = scaleUI * (i - columnNumber / 2);

            // 获取 functionButtonArray[i] 和 functionButtonArray[i+1] 的位置
            Vector3 upButtonPos = new Vector3(
                functionButtonArray[targetIndex].transform.position.x + xOffset,
                functionButtonArray[targetIndex].transform.position.y,
                functionButtonArray[targetIndex].transform.position.z
            );

            Vector3 downButtonPos = new Vector3(
                functionButtonArray[targetIndex + 1].transform.position.x + xOffset,
                functionButtonArray[targetIndex + 1].transform.position.y,
                functionButtonArray[targetIndex + 1].transform.position.z
            );

            if (i < (int)(columnNumber / 2))
            {
                upButtonArray[i].transform.position = upButtonPos;
                downButtonArray[i].transform.position = downButtonPos;
                upButtonArray[i].SetActive(true);
                downButtonArray[i].SetActive(true);
            }
            if (i > (int)(columnNumber / 2))
            {
                upButtonArray[i-1].transform.position = upButtonPos;
                downButtonArray[i-1].transform.position = downButtonPos;
                upButtonArray[i-1].SetActive(true);
                downButtonArray[i-1].SetActive(true);
            }
        }
    }

    void HideAllUpDownButtons()
    {
        for (int i = 0; i < columnNumber - 1; i++)
        {
            upButtonArray[i]?.SetActive(false);
            downButtonArray[i]?.SetActive(false);
        }
    }

    void UpdateButtonVisibility(Vector3 hitPoint)
    {
        // Loop through your upButtonArray and downButtonArray, checking if the hit point is within their range
        for (int i = 0; i < upButtonArray.Length; i++)
        {
            if (upButtonArray[i] != null && downButtonArray[i] != null)
            {
                bool insideUpRange = hitPoint.x >= upButtonArray[i].transform.position.x - scaleUI / 2 &&
                                    hitPoint.x <= upButtonArray[i].transform.position.x + scaleUI / 2;
                bool insideDownRange = hitPoint.x >= downButtonArray[i].transform.position.x - scaleUI / 2 &&
                                    hitPoint.x <= downButtonArray[i].transform.position.x + scaleUI / 2;

                // Enable/Disable GrowBody and Arrow for buttons based on the cursor's position
                upButtonArray[i].transform.Find("GrowBody").gameObject.SetActive(insideUpRange);
                upButtonArray[i].transform.Find("Arrow").gameObject.SetActive(insideUpRange);
                downButtonArray[i].transform.Find("GrowBody").gameObject.SetActive(insideDownRange);
                downButtonArray[i].transform.Find("Arrow").gameObject.SetActive(insideDownRange);
            }
        }
    }
        //------------------------------------------------------------------------------------------------------

        // leftAmount = 0f;
        // rightAmount = 0f;
        // arrowRange = 160f;
        // serialNumber = targetNum;
        // Hashtable hashtable1 = new Hashtable();

        // orderTarget = orderTarget16;
        // menuColumn = "8 Column";

        // over = false;
        // targetNum = orderTarget[0];
        // orderIndex = 1;
        // appIcon[targetNum].GetComponent<Image>().sprite = image[0];
        // IEnumerable<int> list = GenerateNoDuplicateRandom(1, 15);
        // foreach (int item in list)
        // {
        //     if (beginNum != targetNum)
        //     {
        //         appIcon[beginNum].GetComponent<Image>().sprite = image[item];
        //         beginNum++;
        //     }
        //     else
        //     {
        //         beginNum++;
        //         appIcon[beginNum].GetComponent<Image>().sprite = image[item];
        //         beginNum++;
        //     }
        // }
        // beginNum = 0;
        // path = Application.persistentDataPath;
    

    // // Update is called once per frame
    // void Update()
    // {
    //     Ray ray = new Ray(head.transform.position, head.transform.forward);
    //     RaycastHit hit;

    //     if (canvas.activeSelf)
    //     {
    //         time += Time.deltaTime;

    //         if (!getCanvasForward)
    //         {
    //             canvas.transform.position = new Vector3(rotateObj.transform.position.x, canvas.transform.position.y, rotateObj.transform.position.z);
    //             oriHeadP = head.transform.position;
    //             getCanvasForward = true;

                
    //             //------------------------------------------------------------------------------------------------------
    //             lastHeadRotation = head.transform.eulerAngles;
    //             yawDirectionIndex = head.transform.right; 
                
    //             lastYaw = head.transform.eulerAngles.y;
    //             currentYaw = head.transform.eulerAngles.y;
    //             //------------------------------------------------------------------------------------------------------
    //         }
    //         canvasForward = canvas.transform.forward;
 
    //         newHeadP = head.transform.position;
    //         difHeadP = newHeadP - oriHeadP;
    //         difHeadP = Vector3.Project(difHeadP, canvasForward);
    //         if (Vector3.Dot(difHeadP, canvasForward) < 0)
    //         {
    //             plusMinus = -1;
    //         }
    //         else
    //         {
    //             plusMinus = 1;
    //         }
    //         difHeadD = difHeadP.magnitude;
    //         canvas.transform.position += canvasForward * difHeadD * plusMinus;

           
    //     }

    //     if (Physics.Raycast(ray, out hit) && canvas.activeSelf)
    //     {
    //         gazePointer.transform.position = hit.point;

    //         if (hit.transform.name == "collider")
    //         {
    //             triggerZone.GetComponent<BoxCollider>().enabled = false;
    //             gazePointer.SetActive(false);
    //         }

    //         //------------------------------------------------------------------------------------------------------
            
    //         if (!gazePointer.activeSelf)
    //         {
    //             currentHeadRotation = head.transform.eulerAngles;

    //             // calculate the speed of yaw
    //             currentYaw = head.transform.eulerAngles.y;
    //             deltaYaw = Mathf.DeltaAngle(lastYaw, currentYaw);
    //             yawSpeed = Mathf.Abs(deltaYaw) / Time.deltaTime;

    //             if (Vector3.Dot(head.transform.forward, yawDirectionIndex) < 0)
    //             {
    //                 yawLeftOrRight = 1; // turn left
    //             }
    //             else
    //             {
    //                 yawLeftOrRight = -1; // turn right
    //             }

    //             if (yawSpeed > yawthreshold)
    //             {
    //                 if (!getFastYawDirectionIndex)
    //                 {
    //                     lastHeadRotation = head.transform.eulerAngles;
    //                     yawDirectionIndex = head.transform.right;
    //                     allYawAngle = 0f;
    //                     getFastYawDirectionIndex = true;
    //                 }

    //                 allYawAngle += yawLeftOrRight * Mathf.Abs(deltaYaw);

    //                 if (columnIndex < 7 && columnIndex >=0 )
    //                 {
    //                     columnIndex -= yawLeftOrRight * (int)(allYawAngle % smallBlock);
    //                     ResetOtherColumn(columnIndex);
    //                     ColorGrow(columnIndex);
    //                 }
    //                 else
    //                 {
    //                     allYawAngle = 0;
    //                     ResetOtherColumn(8);
    //                 }
    //             }
    //             else
    //             {
    //                 if (!getSlowYawDirectionIndex)
    //                 {
    //                     lastHeadRotation = head.transform.eulerAngles;
    //                     yawDirectionIndex = head.transform.right;
    //                     allYawAngle = 0f;
    //                     getSlowYawDirectionIndex = true;
    //                 }

    //                 allYawAngle += yawLeftOrRight * Mathf.Abs(deltaYaw);

    //                 if (columnIndex < 7 && columnIndex >=0 )
    //                 {
    //                     columnIndex -= yawLeftOrRight * (int)(allYawAngle % largeBlock);
    //                     ResetOtherColumn(columnIndex);
    //                     ColorGrow(columnIndex);
    //                 }
    //                 else
    //                 {
    //                     allYawAngle = 0;
    //                     ResetOtherColumn(8);
    //                 }
    //             }

    //             if (yawSpeed < yawthreshold / 5)
    //             {
    //                 getFastYawDirectionIndex = false;
    //                 getSlowYawDirectionIndex = false;
    //             }


    //         }
            
            

    //         lastYaw = currentYaw;

    //         // if (hit.transform.name == "Groundback")
    //         // {
    //         //     for (int i = 0; i < numUI / 2; i++)
    //         //     {
    //         //         rightGrow[i].image.fillAmount = 0;
    //         //         leftGrow[i].image.fillAmount = 0;

    //         //         rightArrow[i].SetActive(false);
    //         //         leftArrow[i].SetActive(false);
    //         //     }

    //         //     enterRow = false;
    //         //     rightAmount = 0;
    //         //     leftAmount = 0;
    //         //     isBack = false;
    //         // }
    //     }

    //     {
    //         oriHeadP = newHeadP;
    //     }
    // }

    // //����任��ɫ
    // private void RamdomTarget()
    // {
    //     if (orderIndex < 48)
    //     {
    //         targetNum = orderTarget[orderIndex];
    //         appIcon[targetNum].GetComponent<Image>().sprite = image[0];
    //         IEnumerable<int> list = GenerateNoDuplicateRandom(1, 15);
    //         foreach (int item in list)
    //         {
    //             if (beginNum != targetNum)
    //             {
    //                 appIcon[beginNum].GetComponent<Image>().sprite = image[item];
    //                 beginNum++;
    //             }
    //             else
    //             {
    //                 beginNum++;
    //                 appIcon[beginNum].GetComponent<Image>().sprite = image[item];
    //                 beginNum++;
    //             }
    //         }
    //         beginNum = 0;
    //         orderIndex++;
    //         serialNumber = targetNum;
    //         sign.SetActive(true);
    //         canvas.SetActive(false);
    //         gazePointer.SetActive(false);
    //     }
    //     // else
    //     // {
    //     //     targetNum = -1;
    //     //     overSign.text = "Over!";
    //     //     timer.text = time.ToString();
    //     //     errorNumber.text = ((float)((errorNum * 1.00f / 48) * 100)).ToString("0.000000") + "%";
    //     //     timer.text = ((float)((time * 1.00f / 48))).ToString("0.000000");
    //     //     Createfile(path, "MenuAA.txt", menuColumn + "Time : " + timer.text);
    //     //     Createfile(path, "MenuAA.txt", menuColumn + "Error : " + errorNumber.text + "\n");
    //     //     canvas.SetActive(false);
    //     //     gazePointer.SetActive(false);
    //     //     sceneManager.SetActive(true);
    //     // }
    // }

    // private void ColorGrow(int num)
    // {
    //     if (!enterRow)
    //     {
    //         enterButtonRowForward = head.transform.forward;
    //         enterButtonRowRight = head.transform.up;
    //         oriV = head.transform.up;
    //         addV = head.transform.right;
    //         oriForward = head.transform.forward;
    //         enterRow = true;
    //     }

    //     rightArrow[num].SetActive(true);
    //     leftArrow[num].SetActive(true);
    //     rightArrow[num].GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 180 / 255f);
    //     leftArrow[num].GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 180 / 255f);


    //     angleV = Vector3.SignedAngle(addV, head.transform.right, Vector3.up);
    //     nowV = Quaternion.AngleAxis(angleV, Vector3.up) * oriV;
    //     nowForward = Quaternion.AngleAxis(angleV, Vector3.up) * oriForward;
    //     normalVector = Vector3.ProjectOnPlane(head.transform.forward, head.transform.right);
    //     enterButtonRowForward = Vector3.ProjectOnPlane(nowForward, head.transform.right);
    //     float result = Vector3.Dot(head.transform.forward, nowV);
    //     angleTurn = Vector3.Angle(normalVector, enterButtonRowForward);


    //     if (!isMusicMenu)
    //     {
    //         if (result < 0)
    //         {
    //             leftGrow[num].image.fillAmount = 0;
    //             rightAmount = angleTurn / 9.648f;
    //             rightGrow[num].image.fillAmount = rightAmount;

    //             rightArrow[num].transform.localPosition = new Vector3(0, arrowRange / 2 - rightGrow[num].image.fillAmount * arrowRange - 17, 0);


    //             if (rightAmount >= 1 && !isBack)
    //             {
    //                 if (num + 8 == targetNum)
    //                 {
    //                     rightGrow[num].image.fillAmount = 0;
    //                     RamdomTarget();
    //                     rightArrow[num].SetActive(false);
    //                     leftArrow[num].SetActive(false);
    //                     // SceneManager.LoadScene("MusicMenu");
    //                     enterRow = false;
    //                 }
    //                 else
    //                 {
    //                     errorNum++;
    //                     RamdomTarget();
    //                     rightArrow[num].SetActive(false);
    //                     leftArrow[num].SetActive(false);
    //                     enterRow = false;
    //                 }
    //                 isBack = true;
    //                 canvas.SetActive(false);
    //                 getCanvasForward = false;
    //                 for (int i = 0; i < 8; i++)
    //                 {
    //                     btnCollider[i].SetActive(false);
    //                 }
    //                 triggerZone.GetComponent<BoxCollider>().enabled = true;
    //             }

    //             if (rightAmount < 1)
    //             {
    //                 isBack = false;
    //             }
    //         }
    //         else
    //         {
    //             rightGrow[num].image.fillAmount = 0;
    //             leftAmount = angleTurn / 6.783f;
    //             leftGrow[num].image.fillAmount = leftAmount;

    //             leftArrow[num].transform.localPosition = new Vector3(0, leftGrow[num].image.fillAmount * arrowRange - arrowRange / 2 + 17, 0);

    //             if (leftAmount >= 1 && !isBack)
    //             {
    //                 if (num == targetNum)
    //                 {
    //                     leftGrow[num].image.fillAmount = 0;
    //                     RamdomTarget();
    //                     rightArrow[num].SetActive(false);
    //                     leftArrow[num].SetActive(false);
    //                     // SceneManager.LoadScene("MusicMenu");
    //                     enterRow = false;
    //                 }
    //                 else
    //                 {
    //                     errorNum++;
    //                     RamdomTarget();
    //                     rightArrow[num].SetActive(false);
    //                     leftArrow[num].SetActive(false);
    //                     enterRow = false;
    //                 }
    //                 isBack = true;
    //                 canvas.SetActive(false);
    //                 getCanvasForward = false;
    //                 for (int i = 0; i < 8; i++)
    //                 {
    //                     btnCollider[i].SetActive(false);
    //                 }
    //                 triggerZone.GetComponent<BoxCollider>().enabled = true;
    //             }
    //             if (leftAmount < 1)
    //             {
    //                 isBack = false;
    //             }
    //         }
    //     }
    //     else
    //     {
    //         if (result < 0)
    //         {
    //             leftGrow[num].image.fillAmount = 0;
    //             rightAmount = angleTurn / 9.648f;
    //             rightGrow[num].image.fillAmount = rightAmount;

    //             rightArrow[num].transform.localPosition = new Vector3(0, arrowRange / 2 - rightGrow[num].image.fillAmount * arrowRange - 17, 0);


    //             if (rightAmount >= 1 && !isBack)
    //             {
    //                 if (num + 8 == 13)
    //                 {
    //                     rightGrow[num].image.fillAmount = 0;
    //                     RamdomTarget();
    //                     rightArrow[num].SetActive(false);
    //                     leftArrow[num].SetActive(false);
    //                     // SceneManager.LoadScene("MusicMenu");
    //                     isBack = true;
    //                     canvas.SetActive(false);
    //                     getCanvasForward = false;
    //                     for (int i = 0; i < 8; i++)
    //                     {
    //                         btnCollider[i].SetActive(false);
    //                     }
    //                     triggerZone.GetComponent<BoxCollider>().enabled = true;
    //                     // SceneManager.LoadScene("AAMenu");
    //                 }
    //             }

    //             if (rightAmount < 1)
    //             {
    //                 isBack = false;
    //             }
    //         }
    //     }
    // }

    // private void ResetOtherColumn(int n)
    // {
    //     for (int i = 0; i < 7; i++) 
    //     {
    //         if (i != n)
    //         {
    //             rightGrow[i].image.fillAmount = 0;
    //             leftGrow[i].image.fillAmount = 0;
    //             rightArrow[i].SetActive(false);
    //             leftArrow[i].SetActive(false);

    //             rightArrow[i].transform.localPosition = new Vector3(0, arrowRange / 2 - rightGrow[i].image.fillAmount * arrowRange - 17, 0);
    //             leftArrow[i].transform.localPosition = new Vector3(0, leftGrow[i].image.fillAmount * arrowRange - arrowRange / 2 + 17, 0);
    //         }
    //     }
    // }

    // public static void Createfile(string path, string name, string info)
    // {
    //     StreamWriter sw;
    //     FileInfo t = new FileInfo(path + "//" + name);
    //     if (!t.Exists)
    //     {
    //         sw = t.CreateText();
    //     }
    //     else
    //     {
    //         sw = t.AppendText();
    //     }
    //     sw.WriteLine(info);
    //     sw.Close();
    //     sw.Dispose();
    // }

    public IEnumerable<int> GenerateNoDuplicateRandom(int minValue, int maxValue)
    {
        return Enumerable.Range(minValue, maxValue).OrderBy(g => System.Guid.NewGuid());
    }
}

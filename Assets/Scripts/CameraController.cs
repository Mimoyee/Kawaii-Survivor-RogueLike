using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float Min_X, Max_X, Min_Y, Max_Y;
    [SerializeField] float cameraSpeed = 1f;
    void Awake()
    {
        //通过标签找到Player对象, ?? 运算符表示如果playerTransform为空, 则使用默认值playerTransform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {

    }


    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, playerTransform.position, cameraSpeed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, Min_X, Max_X), Mathf.Clamp(transform.position.y, Min_Y, Max_Y), -10);
    }
}

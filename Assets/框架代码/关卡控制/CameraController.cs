using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    
    private float rolSpeed = 0.1f;
    private float posSpeed = 0.5f;
    
    private Quaternion tarRol;
    public Quaternion slowRol;
    public Quaternion baseRol;
    private Vector3 tarPos;
    public Vector3 basePos;
    public Vector3 rightPos;
    
    void Awake()
    {
        InitManager.Register(this);
        camera = GetComponent<Camera>();
        
        baseRol = camera.transform.rotation;
        Vector3 rol = new Vector3(58.872f, -14.79f, -17.14f);
        slowRol = Quaternion.Euler(rol);
        basePos = rightPos = camera.transform.position;
        rightPos.x -= 2f;

        tarRol = baseRol;
        tarPos = basePos;
        
        
    }
    
    void Update()
    {
        camera.transform.rotation= Quaternion.Slerp(camera.transform.rotation,tarRol , rolSpeed);
        camera.transform.position = Vector3.MoveTowards(camera.transform.position, tarPos, posSpeed);
    }

    /// <summary>
    /// 改变目标位置和目标旋转
    /// </summary>
    /// <param name="tarPos_"></param>
    /// <param name="tarRol_"></param>
    public void ChangeTar(Vector3 tarPos_, Quaternion tarRol_)
    {
        tarPos = tarPos_;
        tarRol = tarRol_;
        posSpeed = Vector3.Distance(camera.transform.position, tarPos) / 10;
    }
    
    
}

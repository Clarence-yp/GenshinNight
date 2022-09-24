using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slash : MonoBehaviour
{

    public Vector3 Init(Transform oper, Vector3 rol)
    {
        Vector3 pos = oper.position;
        switch (rol.y)
        {
            case 0:
                pos.x += 0.2f;
                pos.z += 0.2f;
                break;
            case 90:
                break;
            case 180:
                pos.x -= 0.2f;
                pos.z += 0.2f;
                break;
            case -90:
                pos.z += 0.4f;
                break;
        }

        rol.y += 45;
        transform.position = pos;
        transform.eulerAngles = rol;
        
        
        switch (rol.y)
        {
            case 0:
                pos.x += 0.3f;
                break;
            case 90:
                pos.z -= 0.3f;
                break;
            case 180:
                pos.x -= 0.3f;
                break;
            case -90:
                pos.z += 0.3f;
                break;
        }

        return pos;
    }
    
}

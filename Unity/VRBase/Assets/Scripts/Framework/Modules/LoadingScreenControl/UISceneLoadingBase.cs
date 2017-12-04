using UnityEngine;
using System.Collections;
namespace HDJ
{
    public class UISceneLoadingBase : MonoBehaviour
    {
       public virtual  void UpdateLoadingUI(float progress)
        {
           // Debug.Log("LoadingProgress--> " +progress);
        }
    }
}

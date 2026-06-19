using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigEdit : MonoBehaviour
{
    [SerializeField] private RigBuilder rigBuilder;

    void Update()
    {

    }

    public void Do()
    {
        if (rigBuilder != null && !Application.isPlaying)
        {
            rigBuilder.Build();
        } 
    }

}

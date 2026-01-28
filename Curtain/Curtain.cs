using UnityEngine;

namespace Yuha
{
    /// <summary>
    /// <c>Curtain<c> Responsible for descending or ascending the main curtain.
    /// </summary>
    public class Curtain : MonoBehaviour
    {
        private Animator _anim;

        private void Awake() => _anim = GetComponent<Animator>();

        private void CurtainFall() => _anim.SetTrigger("Fall");
        
        private void CurtainRise() => _anim.SetTrigger("Rise");
    }
}
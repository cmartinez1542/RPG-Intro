using UnityEngine;

public class AutoDestroyAfterAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        float animTime = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animTime);
    }
}

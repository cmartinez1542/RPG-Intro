using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void AwakenBoss()
    {
        animator.SetTrigger("isAwake");
    }
    public void PlayAttack()
    {
        animator.SetTrigger("isAttacking");
    }
    public void PlaySummon()
    {
        animator.SetBool("isSummoning", true);
    }
    public void EndSummon()
    {
        animator.SetBool("isSummoning", false);
    }
    public void ShortSummon()
    {
        animator.SetTrigger("otherSummoningTrigger");
    }
    public void PlayHurt()
    {
        animator.SetTrigger("gotHit");
    }
    public void PlayDeath()
    {
        animator.SetTrigger("isDead");
    }
}

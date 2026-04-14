using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteHandler : MonoBehaviour
{
    [SerializeField] Sprite currentSprite;
    [SerializeField] SpriteRenderer characterRenderer;
    
    public CharacterAnimationState animationState;
    
    [Header("Animations")]
    [SerializeField] ObjectAnimation idle;
    [SerializeField] ObjectAnimation talkingAnimation;
    [SerializeField] ObjectAnimation walkCycleAnimation;
    [SerializeField] ObjectAnimation meleeAnimation;
    [SerializeField] List<ObjectAnimation> spAttackAnimations;

    public IEnumerator playOneshotAnimation(ObjectAnimation animation)
    {
        animationState = CharacterAnimationState.OneshotAnimation;

        float elapsedTime = 0f;
        float spriteTime = 0f;
        int spriteIndex = 0;

        characterRenderer.sprite = animation.animationSprites[spriteIndex];

        while(elapsedTime < animation.animationTime)
        {
            elapsedTime += Time.deltaTime;
            spriteTime += Time.deltaTime;
            if(spriteTime > animation.animationDelay && spriteIndex < animation.animationSprites.Count)
            {
                //increment the index and update the sprite
                spriteIndex++;
                characterRenderer.sprite = animation.animationSprites[spriteIndex];
                spriteTime = 0f;
            }

            yield return null;
        }
    }

    public IEnumerator loopAnimation(ObjectAnimation animation)
    {
        yield return null;
    }
}
public enum CharacterAnimationState
{
    Idle,
    LoopingAnimation,
    OneshotAnimation
}
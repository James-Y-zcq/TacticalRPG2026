using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteHandler : MonoBehaviour
{
    private Sprite currentSprite;
    private SpriteRenderer characterRenderer;
    
    public CharacterAnimationState animationState;
    
    ObjectAnimation idle;
    ObjectAnimation talkingAnimation;
    ObjectAnimation walkCycleAnimation;
    ObjectAnimation meleeAnimation;
    ObjectAnimation rangedAnimation;

    //init function to be used by the FieldCharacter
    public void Setup(CombatSpriteDepot depot)
    {
        idle = depot.Idle;
        walkCycleAnimation = depot.Walking;
        meleeAnimation = depot.MeleeBase;
        rangedAnimation = depot.RangedBase;

        animationState = CharacterAnimationState.Idle;
    }

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
    
    //TODO: make this logic functional
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
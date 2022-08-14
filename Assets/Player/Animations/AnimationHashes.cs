using UnityEngine;

namespace Animations
{
    public struct AnimationHashes
    {
        // Floats
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int HangMovement = Animator.StringToHash("HangMovement");
        
        // Bools
        public static readonly int isGrounded = Animator.StringToHash("isGrounded");
        public static readonly int isJumping = Animator.StringToHash("isJumping");
        public static readonly int isHanging = Animator.StringToHash("isHanging");
        public static readonly int isClimbingUp = Animator.StringToHash("isClimbingUp");
        public static readonly int isVaulting = Animator.StringToHash("isVaulting");
    }
}
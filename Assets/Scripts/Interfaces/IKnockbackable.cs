using UnityEngine;

// Anything that can be pushed implements this
public interface IKnockbackable
{
    void ApplyKnockback(Vector2 irection, float force, float duration);

    bool IsBeingKnockedBack { get; }
}

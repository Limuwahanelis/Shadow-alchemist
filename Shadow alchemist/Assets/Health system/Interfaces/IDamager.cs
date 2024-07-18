using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamager
{
    public Vector3 Position { get; }
    public void ResumeCollisons(Collider2D[] playerCols);
    public void PreventCollisions(Collider2D[] playerCols);
}

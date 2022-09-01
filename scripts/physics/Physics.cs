using System;
using System.Collections.Generic;
using Godot;

public class Physics : Node
{
    private static Physics sg;
    
    private List<AABB> boxes = new List<AABB>();
    private List<AABB> hurtboxes = new List<AABB>();
    public override void _Ready ()
    {
        sg = this;
    }

    public static void Register (Entity entity)
    {
        if (entity is AABB box)
        {
            if (sg != null)
            {
                if(box.Type == (int)Boxes.Pushbox)
                    if(!sg.boxes.Contains(box))
                        sg.boxes.Add(box);
                if(box.Type == (int)Boxes.Hurtbox)
                    if(!sg.hurtboxes.Contains(box))
                        sg.hurtboxes.Add(box);
            }
        }
    }

    public static void Unregister (Entity entity)
    {
        if(entity is AABB box)
            if (sg != null)
            {
                if(box.Type == (int)Boxes.Pushbox)
                    if(sg.boxes.Contains(box))
                        sg.boxes.Remove(box);
                if(box.Type == (int)Boxes.Hurtbox)
                    if(sg.hurtboxes.Contains(box))
                        sg.hurtboxes.Remove(box);
            }
    }
    
    public static IEnumerable<AABB> QueryBoxes ()
    {
        return sg.boxes;
    }
    
    public static IEnumerable<AABB> QueryHurtboxes ()
    {
        return sg.hurtboxes;
    }

    /// <summary>
    /// Cast a Ray and return every colliders hit in its way
    /// </summary>
    /// <param name="position">Starting position of the Ray</param>
    /// <param name="direction">Normalized direction the Ray</param>
    /// <param name="length">Length of the Ray</param>
    /// <returns>Return a list of hit boxes</returns>
    public static IEnumerable<Hit> CastRay (sfloat2 position, sfloat2 direction, sfloat2 length)
    {
        List<Hit> hits = new List<Hit>();
        foreach (AABB box in QueryBoxes())
        {
            Hit hit = box.IntersectSegment(position, direction * length);
            if(hit != null)
                hits.Add(hit);
        }
        
        return hits;
    }

    /// <summary>
    /// Cast a Ray and return every colliders hit in its way
    /// </summary>
    /// <param name="position">Starting position of the Ray</param>
    /// <param name="delta">Traveled distance + direction</param>
    /// <returns>Return a list of hit boxes</returns>
    public static IEnumerable<Hit> CastRay (sfloat2 position, sfloat2 delta)
    {
        List<Hit> hits = new List<Hit>();
        foreach (AABB box in QueryBoxes())
        {
            Hit hit = box.IntersectSegment(position, delta);
            if(hit != null)
                hits.Add(hit);
        }

        return hits;
    }

    /// <summary>
    /// Cast an AABB and return every colliders overlapping it
    /// </summary>
    /// <param name="position">Center of the AABB</param>
    /// <param name="halfExtents">Half Extents of the AABB</param>
    /// <param name="targets">List of AABBs to cast against</param>
    /// <param name="breakOnFirst">Should we stop on the first hit ?</param>
    /// <param name="conditions">A list of condition that a hit must respect to be valid</param>
    /// <returns>Return a list of hit boxes</returns>
    public static List<(AABB, Hit)> CastAABB (sfloat2 position, sfloat2 halfExtents, IEnumerable<AABB> targets, bool breakOnFirst = false, List<Predicate<AABB>> conditions = null)
    {
        List<(AABB, Hit)> hits = new List<(AABB, Hit)>();
        AABB castedBox = new AABB(position, halfExtents);

        foreach (AABB box in targets)
        {
            if(conditions != null)
                foreach(Predicate<AABB> condition in conditions)
                    if (!condition.Invoke(box))
                        goto next;

            Hit hit = box.IntersectAABB(castedBox);

            if (hit != null)
            {
                hits.Add((box, hit));
                if (breakOnFirst)
                    return hits;
            }
            
            next:;
        }

        return hits;
    }
}

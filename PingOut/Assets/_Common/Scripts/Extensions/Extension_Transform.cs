using UnityEngine;

public static class Extension_Transform
{
    /// <summary>
    /// Call each frame for smoothly rotating to the beneath surface.
    /// Best used with a transform which is rotated by nothing else.
    /// The used transform must be the child of a parent, which may be rotated for all needs, and which will be used as a reference for directions.
    /// </summary>
    /// <param name="surfaceMask">LayerMask used for checking surface</param>
    /// <param name="smooth">When approaching 0: rotation takes a long time. When approaching 1: rotation is instantaneous</param>
    /// <returns>Returns true if a surface was found beneath transform</returns>
    public static bool SmoothRotateToSurface(this Transform transform, int surfaceMask, float smooth = 0.15f)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, surfaceMask))
        {
            Vector3 lProjectedNormal = Vector3.ProjectOnPlane(hit.normal, transform.parent.right);

            // Calculate the rotation angle around the X axis based on the surface normal. 90 degrees are removed in order to get the surface slope value instead of normal
            float lTargetXRotation = Vector3.SignedAngle(transform.parent.forward, lProjectedNormal, -transform.parent.right) - 90f;
            lTargetXRotation *= -1f;

            //Make rotation
            Quaternion lTargetRotation = Quaternion.Euler(lTargetXRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);

            // Apply smoothed rotation to the transform
            transform.localRotation = Quaternion.Lerp(transform.localRotation, lTargetRotation, smooth);

            return true;
        }

        return false;
    }

    public static void DestroyChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }
}
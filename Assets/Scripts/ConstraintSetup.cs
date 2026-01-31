using UnityEngine;
using UnityEngine.Animations; // Required for Constraints

public class ConstraintSetup : MonoBehaviour
{
    void Start()
    {
        // 1. Get the Constraint component
        RotationConstraint constraint = GetComponent<RotationConstraint>();
        
        // 2. Create the source data pointing to the Main Camera
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = Camera.main.transform;
        source.weight = 1f;

        // 3. Add the source and activate
        constraint.AddSource(source);
        constraint.rotationAxis = Axis.X | Axis.Z; // Lock X and Z (only rotate Y)
        constraint.constraintActive = true;
    }
}
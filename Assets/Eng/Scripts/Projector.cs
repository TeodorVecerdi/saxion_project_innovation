using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class Projector : MonoBehaviour {
    public LayerMask TargetLayerMask;
    [Tooltip("[First Stage Calibration] Step distance in degrees")]
    public float FirstStageResolution = 15f;
    [Tooltip("[Second Stage Calibration] Step distance in degrees")]
    public float SecondStageResolution = 1f;
    [Tooltip("[Third Stage Calibration] Step distance in degrees")]
    public float ThirdStageResolution = 0.05f;
    [Range(0f, 1f)] public float SkipRayChance = 0.5f;
    public float InteractiveStepSpeed = 1f;

    public bool ShowPoints;
    public float PointRadius = 0.01f;
    public Color FirstStageRayColor;
    public Color SecondStageRayColor;
    public Color ThirdStageRayColor;
    public Color InteractiveRayColor;

    private List<Vector3> firstStageHits = new List<Vector3>();
    private List<Vector3> secondStageHits = new List<Vector3>();
    private List<Vector3> finalStageHits = new List<Vector3>();

    private Vector3 average;
    private float firstStageMinHeading;
    private float firstStageMaxHeading;
    private float secondStageMinHeading;
    private float secondStageMaxHeading;
    private Vector3 min;
    private Vector3 max;
    private int secondStageDirection;
    private int thirdStageDirection;

    // Interactive
    private bool doingFirstStageInteractive;
    private bool doingSecondStageInteractive;
    private bool secondStageInteractiveDoneMin;
    private bool doingThirdStageInteractive;
    private bool thirdStageInteractiveDoneLTR;
    private float currentInteractiveHeading;
    private float targetInteractiveHeading;
    private float sourceInteractiveHeading;
    private decimal time;
#if UNITY_EDITOR
    private void OnEnable() {
        EditorApplication.update += Update;
    }

    private void OnDisable() {
        EditorApplication.update -= Update;
    }
#endif
    public void ResetCalibration() {
        average = transform.position;
        min = max = Vector3.zero;
        firstStageMinHeading = Mathf.Infinity;
        firstStageMaxHeading = Mathf.NegativeInfinity;
        firstStageHits.Clear();
        secondStageHits.Clear();
        finalStageHits.Clear();
    }

    public void FirstStageCalibration() {
        for (var heading = 0f; heading <= 360f; heading += FirstStageResolution) {
            for (var pitch = 0f; pitch <= 180f; pitch += FirstStageResolution) {
                var direction = MathHelper.SphericalToCartesian(1, heading * Mathf.Deg2Rad, pitch * Mathf.Deg2Rad).normalized;
                var ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out var hitInfo, 1000f, TargetLayerMask)) {
                    firstStageHits.Add(hitInfo.point);
                }
            }
        }

        average = Vector3.zero;
        foreach (var hit in firstStageHits) {
            average += hit;
        }

        average /= firstStageHits.Count;

        var minHeading = firstStageMinHeading;
        var maxPitch = Mathf.NegativeInfinity;
        var maxHeading = firstStageMaxHeading;

        // Calculate Min Max
        foreach (var hit in firstStageHits) {
            var (radius, heading, pitch) = MathHelper.CartesianToSpherical((hit - transform.position).normalized);
            if (pitch > maxPitch) maxPitch = pitch;
            if (heading < minHeading) minHeading = heading;
            if (heading > maxHeading) maxHeading = heading;
        }

        secondStageMinHeading = firstStageMinHeading = minHeading;
        secondStageMaxHeading = firstStageMaxHeading = maxHeading;
        min = MathHelper.SphericalToCartesian(25, secondStageMinHeading, maxPitch);
        max = MathHelper.SphericalToCartesian(25, secondStageMaxHeading, maxPitch);
    }

    public void SecondStageCalibration() {
        for (var pitch = 0f; pitch <= 180f; pitch += SecondStageResolution) {
            for (var heading = firstStageMinHeading * Mathf.Rad2Deg; heading >= firstStageMinHeading * Mathf.Rad2Deg - FirstStageResolution - 1; heading -= SecondStageResolution) {
                var direction = MathHelper.SphericalToCartesian(1, heading * Mathf.Deg2Rad, pitch * Mathf.Deg2Rad).normalized;
                var ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out var hitInfo, 1000f, TargetLayerMask)) {
                    secondStageHits.Add(hitInfo.point);
                }
            }

            for (var heading = firstStageMaxHeading * Mathf.Rad2Deg; heading <= firstStageMaxHeading * Mathf.Rad2Deg + FirstStageResolution + 1; heading += SecondStageResolution) {
                var direction = MathHelper.SphericalToCartesian(1, heading * Mathf.Deg2Rad, pitch * Mathf.Deg2Rad).normalized;
                var ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out var hitInfo, 1000f, TargetLayerMask)) {
                    secondStageHits.Add(hitInfo.point);
                }
            }
        }

        average = Vector3.zero;
        foreach (var hit in firstStageHits) {
            average += hit;
        }

        average /= firstStageHits.Count;

        var minHeading = firstStageMinHeading;
        var maxPitch = Mathf.NegativeInfinity;
        var maxHeading = firstStageMaxHeading;

        // Calculate Min Max
        foreach (var hit in firstStageHits) {
            var (radius, heading, pitch) = MathHelper.CartesianToSpherical((hit - transform.position).normalized);
            if (pitch > maxPitch) maxPitch = pitch;
            if (heading < minHeading) minHeading = heading;
            if (heading > maxHeading) maxHeading = heading;
        }

        secondStageMinHeading = minHeading;
        secondStageMaxHeading = maxHeading;
        min = MathHelper.SphericalToCartesian(25, secondStageMinHeading, maxPitch);
        max = MathHelper.SphericalToCartesian(25, secondStageMaxHeading, maxPitch);
    }

    public void FirstStageInteractive() {
        doingFirstStageInteractive = true;
        sourceInteractiveHeading = 0f;
        targetInteractiveHeading = sourceInteractiveHeading + FirstStageResolution;
        time = 0m;
    }

    public void SecondStageInteractive() {
        doingSecondStageInteractive = true;
        secondStageInteractiveDoneMin = false;
        sourceInteractiveHeading = Mathf.Rad2Deg * firstStageMinHeading;
        targetInteractiveHeading = sourceInteractiveHeading - SecondStageResolution;
        secondStageDirection = -1;
        time = 0m;
    }

    public void ThirdStageInteractive() {
        doingThirdStageInteractive = true;
        thirdStageInteractiveDoneLTR = false;
        sourceInteractiveHeading = Mathf.Rad2Deg * secondStageMinHeading;
        targetInteractiveHeading = sourceInteractiveHeading + ThirdStageResolution;
        thirdStageDirection = 1;
        time = 0m;

        firstStageHits.Clear();
        secondStageHits.Clear();
    }

    private void Update() {
        if (doingFirstStageInteractive) {
            currentInteractiveHeading = Mathf.LerpAngle(sourceInteractiveHeading, targetInteractiveHeading, (float) time);
            time += (decimal) InteractiveStepSpeed * (decimal) Time.deltaTime;
            if (time >= 1m) {
                sourceInteractiveHeading += FirstStageResolution;
                targetInteractiveHeading = sourceInteractiveHeading + FirstStageResolution;
                time = 0m;

                for (var pitch = 0f; pitch <= 180f; pitch += FirstStageResolution) {
                    var direction = MathHelper.SphericalToCartesian(1, currentInteractiveHeading * Mathf.Deg2Rad, pitch * Mathf.Deg2Rad).normalized;
                    var ray = new Ray(transform.position, direction);
                    if (Physics.Raycast(ray, out var hitInfo, 1000f, TargetLayerMask)) {
                        firstStageHits.Add(hitInfo.point);
                    }
                }

                average = Vector3.zero;
                foreach (var hit in firstStageHits) {
                    average += hit;
                }

                average /= firstStageHits.Count;

                var minHeading = firstStageMinHeading;
                var maxPitch = Mathf.NegativeInfinity;
                var maxHeading = firstStageMaxHeading;

                // Calculate Min Max
                foreach (var hit in firstStageHits) {
                    var (radius, heading, pitch) = MathHelper.CartesianToSpherical((hit - transform.position).normalized);
                    if (pitch > maxPitch) maxPitch = pitch;
                    if (heading < minHeading) minHeading = heading;
                    if (heading > maxHeading) maxHeading = heading;
                }

                secondStageMinHeading = firstStageMinHeading = minHeading;
                secondStageMaxHeading = firstStageMaxHeading = maxHeading;
                min = MathHelper.SphericalToCartesian(25, secondStageMinHeading, maxPitch);
                max = MathHelper.SphericalToCartesian(25, secondStageMaxHeading, maxPitch);
            }

            if (sourceInteractiveHeading >= 360f) {
                doingFirstStageInteractive = false;
            }
        }

        if (doingSecondStageInteractive) {
            currentInteractiveHeading = Mathf.LerpAngle(sourceInteractiveHeading, targetInteractiveHeading, (float) time);
            time += (decimal) InteractiveStepSpeed * (decimal) Time.deltaTime;
            if (time >= 1m) {
                sourceInteractiveHeading += secondStageDirection * SecondStageResolution;
                targetInteractiveHeading = sourceInteractiveHeading + secondStageDirection * SecondStageResolution;
                time = 0m;

                for (var pitch = 0f; pitch <= 180f; pitch += SecondStageResolution) {
                    var direction = MathHelper.SphericalToCartesian(1, currentInteractiveHeading * Mathf.Deg2Rad, pitch * Mathf.Deg2Rad).normalized;
                    var ray = new Ray(transform.position, direction);
                    if (Physics.Raycast(ray, out var hitInfo, 1000f, TargetLayerMask)) {
                        secondStageHits.Add(hitInfo.point);
                    }
                }

                var minHeading = secondStageMinHeading;
                var maxPitch = Mathf.NegativeInfinity;
                var maxHeading = secondStageMaxHeading;

                // Calculate Min Max
                foreach (var hit in firstStageHits) {
                    var (radius, heading, pitch) = MathHelper.CartesianToSpherical((hit - transform.position).normalized);
                    if (pitch > maxPitch) maxPitch = pitch;
                    if (heading < minHeading) minHeading = heading;
                    if (heading > maxHeading) maxHeading = heading;
                }

                foreach (var hit in secondStageHits) {
                    var (radius, heading, pitch) = MathHelper.CartesianToSpherical((hit - transform.position).normalized);
                    if (pitch > maxPitch) maxPitch = pitch;
                    if (heading < minHeading) minHeading = heading;
                    if (heading > maxHeading) maxHeading = heading;
                }

                secondStageMinHeading = minHeading;
                secondStageMaxHeading = maxHeading;
                min = MathHelper.SphericalToCartesian(25, secondStageMinHeading, maxPitch);
                max = MathHelper.SphericalToCartesian(25, secondStageMaxHeading, maxPitch);
            }

            if (!secondStageInteractiveDoneMin && sourceInteractiveHeading <= Mathf.Rad2Deg * firstStageMinHeading - (FirstStageResolution + 1)) {
                secondStageInteractiveDoneMin = true;
                sourceInteractiveHeading = Mathf.Rad2Deg * firstStageMaxHeading;
                targetInteractiveHeading = sourceInteractiveHeading + SecondStageResolution;
                secondStageDirection = 1;
            } else if (secondStageInteractiveDoneMin && sourceInteractiveHeading >= Mathf.Rad2Deg * firstStageMaxHeading + (FirstStageResolution + 1)) {
                doingSecondStageInteractive = false;
            }
        }

        if (doingThirdStageInteractive) {
            currentInteractiveHeading = Mathf.LerpAngle(sourceInteractiveHeading, targetInteractiveHeading, (float) time);
            time += (decimal) InteractiveStepSpeed * (decimal) Time.deltaTime;
            if (time >= 1m) {
                sourceInteractiveHeading += thirdStageDirection * ThirdStageResolution;
                targetInteractiveHeading = sourceInteractiveHeading + thirdStageDirection * ThirdStageResolution;
                time = 0m;

                for (var pitch = 0f; pitch <= 180f; pitch += ThirdStageResolution) {
                    var direction = MathHelper.SphericalToCartesian(1, currentInteractiveHeading * Mathf.Deg2Rad, pitch * Mathf.Deg2Rad).normalized;
                    var ray = new Ray(transform.position, direction);
                    if (Physics.Raycast(ray, out var hitInfo, 1000f, TargetLayerMask)) {
                        finalStageHits.Add(hitInfo.point);
                    }
                }
            }

            if (!thirdStageInteractiveDoneLTR && sourceInteractiveHeading >= Mathf.Rad2Deg * secondStageMaxHeading) {
                thirdStageInteractiveDoneLTR = true;
                sourceInteractiveHeading = Mathf.Rad2Deg * secondStageMaxHeading;
                targetInteractiveHeading = sourceInteractiveHeading - ThirdStageResolution;
                thirdStageDirection = -1;
            } else if (thirdStageInteractiveDoneLTR && sourceInteractiveHeading <= Mathf.Rad2Deg * secondStageMinHeading) {
                doingThirdStageInteractive = false;
                Debug.Log($"Final stage number of points: {finalStageHits.Count}");
            }
        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = FirstStageRayColor;
        foreach (var hit in firstStageHits) {
            if (ShowPoints) Gizmos.DrawWireSphere(hit, PointRadius);
            else Gizmos.DrawLine(transform.position, hit);
        }

        Gizmos.color = SecondStageRayColor;
        foreach (var hit in secondStageHits) {
            if (ShowPoints) Gizmos.DrawWireSphere(hit, PointRadius);
            else Gizmos.DrawLine(transform.position, hit);
        }

        Gizmos.color = ThirdStageRayColor;
        foreach (var hit in finalStageHits) {
            if (Random.value <= SkipRayChance) continue;

            if (ShowPoints) Gizmos.DrawWireSphere(hit, PointRadius);
            else Gizmos.DrawLine(transform.position, hit);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, average);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + min);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + max);

        if (!doingFirstStageInteractive && !doingSecondStageInteractive && !doingThirdStageInteractive) {
            return;
        }

        Gizmos.color = InteractiveRayColor;
        for (var pitch = 0f; pitch <= 180f; pitch += (doingFirstStageInteractive ? FirstStageResolution : doingSecondStageInteractive ? SecondStageResolution : ThirdStageResolution)) {
            var direction = MathHelper.SphericalToCartesian(25, currentInteractiveHeading * Mathf.Deg2Rad, pitch * Mathf.Deg2Rad);
            Gizmos.DrawLine(transform.position, transform.position + direction);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using System.Collections;
using System.Drawing;
using Color = UnityEngine.Color;

public static class MathPlump
{

    /// <summary>Given a value between a and b, returns its normalized location in that range, as a t-value (interpolant) from 0 to 1</summary>
    /// <param name="a">The start of the range, where it would return 0</param>
    /// <param name="b">The end of the range, where it would return 1</param>
    /// <param name="value">A value between a and b. Note: values outside this range are still valid, and will be extrapolated</param>
    public static float InverseLerp(float a, float b, float value) => (value - a) / (b - a);


    /*/// <summary>Given a value between a and b, returns its normalized location in that range, as a t-value (interpolant) from 0 to 1</summary>
    /// <param name="a">The start of the range, where it would return 0</param>
    /// <param name="b">The end of the range, where it would return 1</param>
    /// <param name="value">A value between a and b. Note: values outside this range are still valid, and will be extrapolated</param>
    public static float CalculateKnockback(float victimPercent,float baseKnockback, float victimWeight)
    {

        // weight to range .5f - 2f
        float weightAsKnockBackFactor = (Mathf.InverseLerp(1f, 100f, victimWeight) * 1.5f) + .5f;

        // victim percent is X value in graph
        return knockbackGrowth * ((victimPercent * weightAsKnockBackFactor) + baseKnockback);
    }*/

    private const float A = .5f;
    private const float L = 100f;
    private const float G = 1.75f;
    //private const float U = 1000f;

    /// <summary> Patented Bungo Bash Knockback Formula </summary>
    /// <param name="victimPercent"> becomes x in equation </param>
    /// <param name="attackPower"> value 0 - 1 </param>
    /// <param name="baseAttackPower">value 0 - 1 </param>
    /// /// <param name="victimWeight">value 0 - 1 </param>
    public static float KnockbackFormula(float victimPercent,float attackPower,float baseAttackPower,float attackGrowthFactor, float victimWeight,float knockbackCap = 1000f)
    {
        float x = victimPercent;
        float g = attackGrowthFactor;
        float p = attackPower;
        float b = baseAttackPower;
        float w = victimWeight;

        float M = L / Mathf.Pow(L, (G + g));

        float knockback = Mathf.Min( ( (Mathf.Pow(x,(G+g))*M*p*w) * A ) + b , knockbackCap);

        return knockback;
    }  
    public static int GetAdditionalFreezeFramesFromKnockback(BungoCurve freezeFrameCurve, float knockback)
    {
        int freezeFrames = Mathf.RoundToInt(freezeFrameCurve.Evaluate(knockback));
        return freezeFrames;
    }
    public static float GetAreaUnderCurve(AnimationCurve curve)
    {
        // only evaluates area under time 0f-1f
        float stepsize = 0.001f;
        float sum = 0;
        for (int i = 0; i < 1 / stepsize; i++)
        {
            sum += IntegralOnStep(stepsize * i, curve.Evaluate(stepsize * i), stepsize * (i + 1), curve.Evaluate(stepsize * (i + 1)));
        }

        return sum;
    }

    public static Vector3 FindClosestPointOnCinemachinePath(CinemachineSmoothPath path, Vector3 position)
    {
        float closestLedgePoint = path.FindClosestPoint(position, 0, -1, 1);
        Vector3 closestLedgePointVector = path.EvaluatePosition(closestLedgePoint);

        return closestLedgePointVector;
    }
    public static float GetAreaUnderCurve(BungoCurve curve)
    {
        //float stepsize = 0.01f;
        float sum = 0;

        float steps = 100f;
        float steppy = (curve.timeRange.y - curve.timeRange.x) / steps;

        for (float i = curve.timeRange.x; i < curve.timeRange.y; i += steppy)
        {
            sum += IntegralOnStep(i, curve.Evaluate(i), (i + steppy), curve.Evaluate( (i + steppy)));
        }

        float curveTimeDifference = curve.timeRange.y - curve.timeRange.x;
        float curveValuesDifference = curve.valueRange.y - curve.valueRange.x;

        return sum * curveTimeDifference * curveValuesDifference;
    }
    private static float IntegralOnStep(float x0, float y0, float x1, float y1)
    {
        float a = (y1 - y0) / (x1 - x0);
        float b = y0 - a * x0;
        float area = (a / 2 * x1 * x1 + b * x1) - (a / 2 * x0 * x0 + b * x0);

        return area;
    }
    public static int GetAdditionalFreezeFramesFromPercent(float victimPercent)
    {
        return Mathf.RoundToInt(Mathf.InverseLerp(0f, 100f, victimPercent) * 5f);
    }

    /// <summary>Given a value between a and b, returns its normalized location in that range, as a t-value (interpolant) from 0 to 1</summary>
    public static float CalculateEndLagFromKnockback(float knockback)
    {
        float endlag;

        endlag = knockback / 100f;

        return endlag;
    }

    public static float EvaluateAO(this AnimationCurve curve,Vector2 AO,float lerpPos)
    {
        return (curve.Evaluate(lerpPos)*AO.x) + AO.y;
    }

    /// <summary> lerp pos needs to be within time range ie not 0 - 1</summary>

    public static float Evaluate(this BungoCurve bungoCurve, float lerpPos)
    {
        float stretchedTime = bungoCurve.curve.Evaluate((lerpPos - bungoCurve.timeRange.x) * (1f / (-bungoCurve.timeRange.x + bungoCurve.timeRange.y)));
        float stretchedAmplitude = (stretchedTime * (bungoCurve.valueRange.y - bungoCurve.valueRange.x) ) + bungoCurve.valueRange.x;
        
        return stretchedAmplitude;
    }
    /// <summary> lerp pos needs to be within 0 - 1</summary>

    public static float Evaluate(this AmplitudeCurve amplitudeCurve, float lerpPos)
    {
        float ampLerpPosFromTime = amplitudeCurve.curve.Evaluate(lerpPos);
        float stretchedAmplitude = (ampLerpPosFromTime * (amplitudeCurve.valueRange.y - amplitudeCurve.valueRange.x)) + amplitudeCurve.valueRange.x;

        return stretchedAmplitude;
    }

    public static T[] GetComponentsInDirectChildren<T>(this Component parent) where T : Component
    {
        return parent.GetComponentsInDirectChildren<T>(false);
    }

    public static T[] GetComponentsInDirectChildren<T>(this Component parent, bool includeInactive) where T : Component
    {
        List<T> tmpList = new List<T>();
        foreach (Transform transform in parent.transform)
        {
            if (includeInactive || transform.gameObject.activeInHierarchy)
            {
                tmpList.AddRange(transform.GetComponents<T>());
            }
        }
        return tmpList.ToArray();
    }

    // eventually should make this rely on the variable framerate  
    public static float ConvertFramesToSeconds(this int frames) => (float)frames / 60f;


    public static bool IsPointWithinCube(Vector3 point, Vector3 bounds)
    {
        return Mathf.Abs(point.x) <= bounds.x &&
               Mathf.Abs(point.y) <= bounds.y &&
               Mathf.Abs(point.z) <= bounds.z;
    }
}

public static class BungoTweener
{
    public class MyStaticMB : MonoBehaviour { }

    //Variable reference for the class
    private static MyStaticMB myStaticMB;

    //Now Initialize the variable (instance)
    private static void Init()
    {
        //If the instance not exit the first time we call the static class
        if (myStaticMB == null)
        {
            //Create an empty object called MyStatic
            GameObject gameObject = new GameObject("MyStatic");


            //Add this script to the object
            myStaticMB = gameObject.AddComponent<MyStaticMB>();
        }
    }
    public static void BTMove(this Transform transform,Vector3 startPosition, Vector3 newPosition, float duration)
    {
        Init();
        myStaticMB.StartCoroutine(BTMoveRoutine(transform,startPosition, newPosition,duration));
    }
    private static IEnumerator BTMoveRoutine(Transform transform, Vector3 startPosition, Vector3 newPosition, float duration)
    {
        float timer = 0f;
        float x;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            x = timer / duration;

            transform.position = Vector3.Lerp(startPosition, newPosition, x);
            yield return null;
        }
    }

    public static void BTSpriteColorMove(this SpriteRenderer sr, Color startColor, Color endColor, float duration)
    {
        Init();
        myStaticMB.StartCoroutine(BTColorMoveRoutine(sr,startColor, endColor, duration));
    }

    private static IEnumerator BTColorMoveRoutine(SpriteRenderer sr, Color startColor, Color endColor, float duration)
    {
        float timer = 0f;
        float x;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            x = timer / duration;

            sr.color = Color.Lerp(startColor, endColor, x);
            yield return null;
        }
    }

    public static void BTSpriteEnable(this SpriteRenderer spriteRenderer,bool enable,float duration)
    {
        Init();
        myStaticMB.StartCoroutine(BTSpriteEnableRoutine(spriteRenderer,enable, duration));
    }

    private static IEnumerator BTSpriteEnableRoutine(SpriteRenderer spriteRenderer, bool enable,float duration)
    {
        yield return new WaitForSeconds(duration);
        spriteRenderer.enabled = enable;
    }

    //Create a class that actually inheritance from MonoBehaviour
}




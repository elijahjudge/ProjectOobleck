using System.Collections.Generic;
using UnityEngine;

public class RateOfThing
{
    private float perSecond;
    private List<float> timesThingsHappened;

    public RateOfThing(float perSecond)
    {
        this.perSecond = perSecond;
        timesThingsHappened = new List<float>();
    }

    public void AddThing()
    {
        timesThingsHappened.Add(Time.time);
    }

    public void UpdateRateTiming()
    {

        if (timesThingsHappened.Count == 0)
            return;

        if (Time.time - timesThingsHappened[0] >= perSecond)
        {
            timesThingsHappened.RemoveAt(0);
        }

    }
    public float GetRate()
    {
        return ((float)timesThingsHappened.Count)/perSecond;
    }

    public void Reset()
    {
        timesThingsHappened.Clear();
    }
}
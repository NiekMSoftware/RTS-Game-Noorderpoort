using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class TestMultiTheading : MonoBehaviour
{
    [SerializeField] private bool useJobs;

    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;

        if (useJobs)
        {
            NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.Temp);

            for (int i = 0; i < 10; i++)
            {
                JobHandle jobHandle = ReallyToughTaskJob();
                handles.Add(jobHandle);
            }

            JobHandle.CompleteAll(handles);
            handles.Dispose();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                ReallyToughTask();
            }
        }

        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f + "ms");
    }

    private void ReallyToughTask()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }

    private JobHandle ReallyToughTaskJob()
    {
        ReallyToughJob job = new();
        return job.Schedule();
    }
}

[BurstCompile]
public struct ReallyToughJob : IJob
{
    public void Execute()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}
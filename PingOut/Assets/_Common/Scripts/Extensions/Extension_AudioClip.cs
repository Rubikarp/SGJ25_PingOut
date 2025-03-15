using UnityEngine;

public static class Extension_AudioClip
{
    public static float GetLoudnessAt(this AudioClip clip, int clipPosition, int sampleSize = 64)
    {
        int lStartPosition = clipPosition - sampleSize;

        if (lStartPosition < 0)
            return -1f;

        float[] lWaveData = new float[sampleSize];
        clip.GetData(lWaveData, lStartPosition);

        //Compute loudness
        float lTotalLoudness = 0f;

        for (int i = 0; i < sampleSize; i++)
            lTotalLoudness += Mathf.Abs(lWaveData[i]);

        return lTotalLoudness / sampleSize;
    }
}
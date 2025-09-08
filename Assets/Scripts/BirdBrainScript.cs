// BirdBrainScript.cs, controls the brain of the AI bird.

using UnityEngine;

// Wrapper to save as json
[System.Serializable]
public class SerializableFloatArray
{
    public float[] values;
}

// Wrapper to save as json
[System.Serializable]
public class LayerData
{
    public SerializableFloatArray[] weights;
    public float[] biases;
}

[System.Serializable]
public class BirdBrain
{
    public int[] layerSizes; // structure, 4 input, 5 hidden, 1 output
    public LayerData[] layers; // data of layers

    public BirdBrain(int[] layerSizes)
    {
        this.layerSizes = layerSizes;
        layers = new LayerData[layerSizes.Length - 1];

        // Initilizes random weights/biases
        for (int l = 0; l < layers.Length; l++)
        {
            int inCount = layerSizes[l];
            int outCount = layerSizes[l + 1];

            layers[l] = new LayerData();
            layers[l].weights = new SerializableFloatArray[inCount];
            for (int i = 0; i < inCount; i++)
            {
                layers[l].weights[i] = new SerializableFloatArray();
                layers[l].weights[i].values = new float[outCount];
                for (int j = 0; j < outCount; j++)
                    layers[l].weights[i].values[j] = Random.Range(-1f, 1f);
            }

            layers[l].biases = new float[outCount];
            for (int j = 0; j < outCount; j++)
                layers[l].biases[j] = Random.Range(-1f, 1f);
        }
    }

    // forward pass
    public float[] FeedForward(float[] inputs)
    {
        float[] activations = inputs;

        for (int l = 0; l < layers.Length; l++)
        {
            float[] next = new float[layerSizes[l + 1]];
            for (int j = 0; j < layerSizes[l + 1]; j++)
            {
                float sum = layers[l].biases[j];
                for (int i = 0; i < layerSizes[l]; i++)
                    sum += activations[i] * layers[l].weights[i].values[j];

                next[j] = Tanh(sum); // activation
            }
            activations = next;
        }

        return activations;
    }

    // clone for elitism
    public BirdBrain Clone()
    {
        BirdBrain clone = new BirdBrain(layerSizes);
        for (int l = 0; l < layers.Length; l++)
        {
            for (int i = 0; i < layers[l].weights.Length; i++)
                layers[l].weights[i].values.CopyTo(clone.layers[l].weights[i].values, 0);

            layers[l].biases.CopyTo(clone.layers[l].biases, 0);
        }
        return clone;
    }

    public static float Tanh(float x)
    {
        float ePos = Mathf.Exp(x);
        float eNeg = Mathf.Exp(-x);
        return (ePos - eNeg) / (ePos + eNeg);
    }
}



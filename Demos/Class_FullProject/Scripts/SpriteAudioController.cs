/*

A series of instructions to chatGPT - I made some minor changes to the script with each iteration and pasted back into the chat for context.

please write a unity C# script that does the following: It has 3 public spriterenderers with 2 audiosources attached to each. I want to use a coroutine to randomly choose one of the 3 to fade up from black, play a clip in one audio source, and in the other audio source, fade it up and back down again, don't choose the same spriterenderer unless it has fully faded out, i suggest using an array of floats to control the volume of the second audiosource and the color fading.

ok great but i want the tones to play for longer so they overlap - expose the timetoplay variable public, they should fade up, hold for a time, and then fade down - don't wait until they fade out to play the next one - but don't play one if it hasn't faded down yet

expose maxvolume of samples, and also use a sine wave to subtly jiggle the scale of the spriterenderers, use a counter float and time.deltatime so that the sine wave slows down from 5 hertz in the beginning to 0 hertz at the end of when the audio is playing, publicly expose the min and max scales:

parented to each sprite is another quad with a meshrenderer and a material attached that has a "_RingSize" float - whenever the primaryaudio plays - as the sinescale is animating, scale up the ring from a public min and max value:

*/

using UnityEngine;
using System.Collections;

public class SpriteAudioController : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    private AudioSource[][] audioSources;
    private Material[] ringMaterials; // Array to store materials of the rings

    private bool[] isFading;
    private float[] fadeVolumes;
    public float timeToPlay = 5.0f; // Time to play each tone
    public float playTime = 5.0f;
    public float maxVolume = 1.0f; // Maximum volume for the samples
    public Vector3 minScale = new Vector3(0.9f, 0.9f, 0.9f); // Minimum scale
    public Vector3 maxScale = new Vector3(1.1f, 1.1f, 1.1f); // Maximum scale
    public float minRingSize = 0.5f; // Minimum ring size
    public float maxRingSize = 2.0f; // Maximum ring size

    void OnEnable()
    {
        // Initialize arrays
        audioSources = new AudioSource[spriteRenderers.Length][];
        ringMaterials = new Material[spriteRenderers.Length];
        isFading = new bool[spriteRenderers.Length];
        fadeVolumes = new float[spriteRenderers.Length];

        // Attach two AudioSources to each SpriteRenderer and initialize them
        // Also get the materials of the child quads
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            audioSources[i] = spriteRenderers[i].GetComponents<AudioSource>();
            ringMaterials[i] = spriteRenderers[i].transform.GetChild(0).GetComponent<MeshRenderer>().material;
            if (audioSources[i].Length < 2)
            {
                Debug.LogError("Not enough AudioSources attached to SpriteRenderer");
            }
            Color spriteColor = spriteRenderers[i].color;
            spriteRenderers[i].color = new Color(spriteColor.r,spriteColor.g,spriteColor.b, 0); // Start with sprite fully transparent
            isFading[i] = false;
            fadeVolumes[i] = 0.0f;
        }

        StartCoroutine(RandomSpriteAudioRoutine());
    }

    IEnumerator RandomSpriteAudioRoutine()
    {

        yield return new WaitForSeconds(1);

        while (true)
        {
            int chosenIndex = Random.Range(0, spriteRenderers.Length);

            // Ensure the same SpriteRenderer isn't chosen if it's still fading
            while (isFading[chosenIndex])
            {
                chosenIndex = Random.Range(0, spriteRenderers.Length);
                yield return null; // Wait a frame before re-checking
            }

            StartCoroutine(FadeSpriteAndAudio(chosenIndex));
            yield return new WaitForSeconds(timeToPlay); // Wait for 'timeToPlay' before choosing the next one
        }
    }

    IEnumerator FadeSpriteAndAudio(int index)
    {
        isFading[index] = true;
        SpriteRenderer sr = spriteRenderers[index];
        AudioSource primaryAudio = audioSources[index][0];
        AudioSource fadingAudio = audioSources[index][1];

        Color spriteColor = sr.color;
        // Fade in the sprite
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            sr.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, t);
            sr.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_ColorInner", sr.color);
            sr.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_ColorOuter", sr.color);
            yield return null;
        }

        fadingAudio.pitch = Random.Range(.8f, 1.2f);
        
        // Fade up the secondary audio
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            fadeVolumes[index] = Mathf.Lerp(0, maxVolume, t);
            fadingAudio.volume = fadeVolumes[index];
            yield return null;
        }

        float counter = 0f;
        float counter2 = 0f;

        primaryAudio.time = .5f;
        primaryAudio.Play();
        
        // Hold for the duration of timeToPlay
        while(counter2 < playTime)
        {
            float scale = Mathf.Lerp(5f, 0f, counter / playTime);
            counter += Time.deltaTime * scale;
            counter2 += Time.deltaTime;
            float sineValue = (Mathf.Sin(2 * Mathf.PI * counter) + 1) * 0.5f;
            Vector3 newScale = Vector3.Lerp(minScale, maxScale, sineValue);
            sr.transform.localScale = newScale;

            // Scale up the ring size based on the sine wave
            float ringSize = Mathf.Lerp(minRingSize, maxRingSize, counter2);
            ringMaterials[index].SetFloat("_RingSize", ringSize);

            yield return null;
        }

        // Fade down the secondary audio
        for (float t = 1f; t > 0; t -= Time.deltaTime)
        {
            fadeVolumes[index] = Mathf.Lerp(0, maxVolume, t);
            fadingAudio.volume = fadeVolumes[index];
            yield return null;
        }
        
        // Fade out the sprite
        for (float t = 1f; t > 0; t -= Time.deltaTime)
        {
            sr.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, t);
            sr.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_ColorInner", sr.color);
            sr.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_ColorOuter", sr.color);
            yield return null;
        }

        sr.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0);

        isFading[index] = false;
    }
}
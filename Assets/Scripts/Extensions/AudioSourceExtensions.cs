using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class AudioSourceExtensions
    {
        /// <summary>
        /// Instantiates an object at position (0, 0, 0) with an AudioSource component attached to it and plays the passed clip upon instantiation.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="clipToPlay">The clip that will be played by the AudioSource.</param>

        public static void InstantiateAudioSource(this AudioSource audioSource, AudioClip clipToPlay)
        {
            GameObject soundPlayerObject = new GameObject($"{clipToPlay.name}_sound_object");
            soundPlayerObject.transform.position = Vector3.zero;

            AudioSource audioSourceComponent = soundPlayerObject.AddComponent<AudioSource>();

            audioSourceComponent.clip = clipToPlay ?? null;

            if (audioSourceComponent.clip != null)
            {
                audioSourceComponent.Play();
            }

            //Destroy the sound player object after the end of the clip.

            Object.Destroy(soundPlayerObject, audioSourceComponent.clip.length);
        }

        /// <summary>
        /// Instantiates an object at a defined position with an AudioSource component attached and plays the passed clip upon instantiation.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="clipToPlay">The clip that will be played by the AudioSource.</param>
        /// <param name="position">The position where the GameObject should be created (this will be the source of the sound).</param>

        public static void InstantiateAudioSource(this AudioSource audioSource, AudioClip clipToPlay, Vector3 position)
        {
            GameObject soundPlayerObject = new GameObject($"{clipToPlay.name}_sound_object");
            soundPlayerObject.transform.position = position;

            AudioSource audioSourceComponent = soundPlayerObject.AddComponent<AudioSource>();

            audioSourceComponent.clip = clipToPlay ?? null;

            if (audioSourceComponent.clip != null)
            {
                audioSourceComponent.Play();
            }

            //Destroy the sound player object after the end of the clip.

            Object.Destroy(soundPlayerObject, audioSourceComponent.clip.length);
        }
    }
}
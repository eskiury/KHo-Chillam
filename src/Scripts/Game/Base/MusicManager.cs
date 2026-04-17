using Godot;
using System;

public partial class MusicManager : AudioStreamPlayer
{
    public static MusicManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void PlayMusic(AudioStream cancion)
    {
        if (cancion == null) return;

        Stream = cancion;
        Play();
    }

    public void StopMusic()
    {
        Stop();
    }

    public void PauseMusic()
    {
        StreamPaused = true;
    }

    public void ResumeMusic()
    {
        StreamPaused = false;
    }


    public void PlaySFX(AudioStream sfx)
    {
        if (sfx == null) return;

        var sfxPlayer = new AudioStreamPlayer();
        sfxPlayer.Stream = sfx;
        AddChild(sfxPlayer);
        sfxPlayer.Play();
        sfxPlayer.Connect("finished", Callable.From(() => sfxPlayer.QueueFree()));
        //GD.Print("Playing SFX");
    }

    public void SetVolume(float linearValue)
    {
        VolumeDb = Mathf.LinearToDb(linearValue);
    }
}
public readonly struct SoundInfo {
    public readonly string name;
    public readonly float volume;
    public readonly float pitch;
    public SoundInfo(string name, float volume = 1f, float pitch = 1f) {
        this.name = name;
        this.volume = volume;
        this.pitch = pitch;
    }
    
    public static implicit operator SoundInfo(string soundName) {
        return new SoundInfo(soundName);
    }
}

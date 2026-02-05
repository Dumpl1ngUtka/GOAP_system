namespace Services.SaveLoad.Interfaces
{
    public interface ISaveWithTime
    {
        long LastSaveTime { get; set; }
    }
}
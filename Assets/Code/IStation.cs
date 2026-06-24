public interface IStation
{
    public int StationId { get; }

    public void OnOpen();
    public void OnClose();
}
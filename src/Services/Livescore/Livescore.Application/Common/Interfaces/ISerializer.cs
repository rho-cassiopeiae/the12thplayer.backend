namespace Livescore.Application.Common.Interfaces {
    public interface ISerializer {
        string Serialize<T>(T @object);
    }
}

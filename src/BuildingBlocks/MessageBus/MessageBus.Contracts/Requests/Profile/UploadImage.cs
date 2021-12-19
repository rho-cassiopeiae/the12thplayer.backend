namespace MessageBus.Contracts.Requests.Profile {
    public class UploadImage : Message {
        public string FilePath { get; set; }
    }
}

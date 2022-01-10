namespace MessageBus.Contracts.Requests.Feed {
    public class UploadImage : Message {
        public string FilePath { get; set; }
    }
}

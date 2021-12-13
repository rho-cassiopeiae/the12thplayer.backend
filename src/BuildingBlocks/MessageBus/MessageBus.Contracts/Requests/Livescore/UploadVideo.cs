namespace MessageBus.Contracts.Requests.Livescore {
    public class UploadVideo : Message {
        public string FilePath { get; set; }
        public string VimeoProjectId { get; set; }
    }
}

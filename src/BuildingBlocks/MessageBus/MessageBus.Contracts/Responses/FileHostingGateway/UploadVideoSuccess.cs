namespace MessageBus.Contracts.Responses.FileHostingGateway {
    public class UploadVideoSuccess : Message {
        public string VideoId { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}

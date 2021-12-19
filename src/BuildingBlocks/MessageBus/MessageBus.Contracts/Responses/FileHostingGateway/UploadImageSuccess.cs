namespace MessageBus.Contracts.Responses.FileHostingGateway {
    public class UploadImageSuccess : Message {
        public string ImageUrl { get; set; }
    }
}

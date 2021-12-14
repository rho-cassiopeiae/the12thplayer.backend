namespace Admin.Application.Common.Interfaces {
    public interface ISuperuserSignatureVerifier {
        bool Verify(string payload, string signature);
    }
}

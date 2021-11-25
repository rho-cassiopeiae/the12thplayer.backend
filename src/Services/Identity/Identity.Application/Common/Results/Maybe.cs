namespace Identity.Application.Common.Results {
    public class Maybe<T> where T : class {
        public T Error { get; }

        public bool IsError => Error != null;

        private Maybe(T error = null) {
            Error = error;
        }

        public static implicit operator Maybe<T>(T error)
            => new Maybe<T>(error: error);
    }
}

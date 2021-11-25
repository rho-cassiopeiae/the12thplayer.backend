namespace Identity.Application.Common.Results {
    public class Either<T1, T2>
        where T1 : class
        where T2 : class {
        public T1 Error { get; }
        public T2 Data { get; }
        public bool IsError => Error != null;

        private Either(T1 error = null, T2 data = null) {
            Error = error;
            Data = data;
        }

        public static implicit operator Either<T1, T2>(T1 left)
            => new Either<T1, T2>(error: left);

        public static implicit operator Either<T1, T2>(T2 right)
            => new Either<T1, T2>(data: right);
    }
}

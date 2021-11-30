namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public abstract class ResponseDto {
        public class MetaDto {
            public class PaginationDto {
                public int CurrentPage { get; set; }
                public int TotalPages { get; set; }
            }

            public PaginationDto Pagination { get; set; }
        }

        public class ErrorDto {
            public string Message { get; set; }
            public int Code { get; set; }
        }

        public MetaDto Meta { get; set; }
        public ErrorDto Error { get; set; }
    }
}

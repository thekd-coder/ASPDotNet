namespace JwtAuthDotNet9.Dto
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }

    }
}

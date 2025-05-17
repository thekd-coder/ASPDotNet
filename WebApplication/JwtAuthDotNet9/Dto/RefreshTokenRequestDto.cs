namespace JwtAuthDotNet9.Dto
{
    public class RefreshTokenRequestDto
    {
        public Guid UserId { get; set; }
        public required string  RefreshToken { get; set; }
    }
}

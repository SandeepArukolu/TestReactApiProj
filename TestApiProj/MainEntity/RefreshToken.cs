namespace TestApiProj.MainEntity
{
    public class RefreshToken
    {
          public int Id { get; set; }

            public string Token { get; set; } = string.Empty;

            public DateTime Expires { get; set; }

            public bool? IsExpired => DateTime.UtcNow >= Expires;

            public string? UserId { get; set; } = string.Empty;
        }
    }

namespace BlazorAuthentication.Shared
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; } = default!;
        public string Token { get; set; } = default!;
    }
}

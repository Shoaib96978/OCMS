namespace OCMS.Shared
{
    public class AppResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? RedirectUrl { get; set; }
        public object? Data { get; set; }

        public static AppResponse Ok(string message,
                                     string? redirectUrl = null,
                                     object? data = null)
            => new()
            {
                Success = true,
                Message = message,
                RedirectUrl = redirectUrl,
                Data = data
            };

        public static AppResponse Fail(string message)
            => new() { Success = false, Message = message };
    }
}


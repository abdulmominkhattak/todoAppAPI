namespace toDoApp.Configuration
{
    public class AuthResult
    {
        /// <summary>
        /// class Resposible for DTOs
        /// </summary>
        public string Token  { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}

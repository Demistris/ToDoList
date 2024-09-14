namespace ToDoList.Shared.CustomExceptions
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException() : base("Email is already registered") { }
    }
}

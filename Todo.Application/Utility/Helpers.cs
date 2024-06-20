namespace Todo.Application.Utility;

internal static class Helpers
{
    internal static string SetRequiredErrorMessage(string requiredProperty)
    {
        return $"{requiredProperty} is required";
    }
}

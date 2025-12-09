namespace Database_Project.Helpers;

public static class SearchHelper
{
    /// <summary>
    /// Search for customer's name.
    /// </summary>
    /// <returns>Query for the search.</returns>
    public static async Task<string> SearchNameAsync()
    {
        Console.Write("Search for customer's name: ");
        var search = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(search))
        {
            return "";
        }

        return search;
    }
}
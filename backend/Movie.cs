namespace backend;

/// <summary>
/// Represents a movie with its associated director
/// </summary>
public record Movie(int Id, string Title, int DirectorId, int Year, string Genre);

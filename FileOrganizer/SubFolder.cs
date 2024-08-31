namespace FileOrganizer;

public sealed record SubFolder(List<string> Extensions, string FileNameContains, string FolderName, bool CamelCaseSensitive);
namespace FileOrganizer;

public sealed record Configuration(string BaseFolderPath, List<Group> Groups, List<SubFolder> SubFolders);
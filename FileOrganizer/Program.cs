using FileOrganizer;
using System.Text;
using System.Text.Json;

var isFileLocked = (FileInfo file) =>
{
    try
    {
        using FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
        stream.Close();
    }
    catch (IOException)
    {
        Logger.Instance.Log($"File {file.Name} is beign used by another process.");
        return true;
    }

    return false;
};

var moveFile = (string originalFile, string fullNewPath) =>
{
    var originalFileName = Path.GetFileName(originalFile);
    string newDestiniyFile = Path.Combine(fullNewPath, originalFileName);
    File.Move(originalFile, newDestiniyFile);
    string log = $"File: {originalFileName} \n moved from: {originalFile} \n\tto: {newDestiniyFile}";
    Logger.Instance.Log(log);
};

var basicValidations = (Configuration config) =>
{
    if (config is null)
    {
        string configNotValidMessage = "Config not valid, exiting program.";
        Logger.Instance.Log(configNotValidMessage);
        Console.WriteLine(configNotValidMessage);
        return false;
    }

    if (string.IsNullOrEmpty(config.BaseFolderPath))
    {
        string folderNullMessage = "Folder entered is null, exiting program.";
        Console.WriteLine(folderNullMessage);
        Logger.Instance.Log(folderNullMessage);
        return false;
    }

    if (!Path.Exists(config.BaseFolderPath))
    {
        string baseFolderNotExistsMessage = $"Path entered -{config.BaseFolderPath}- does not exists, exiting program.";
        Console.WriteLine(baseFolderNotExistsMessage);
        Logger.Instance.Log(baseFolderNotExistsMessage);
        return false;
    }

    return true;
};

string jsonConfiguration = string.Empty;
using (StreamReader streamReader = new("Configuration.json", Encoding.UTF8))
{
    jsonConfiguration = await streamReader.ReadToEndAsync();
}

BaseConfiguration? localConfiguration = JsonSerializer.Deserialize<BaseConfiguration>(jsonConfiguration);

if (localConfiguration is null)
{
    string baseConfigurationNotValid = "Could not read base configuration, exiting program.";
    Console.WriteLine(baseConfigurationNotValid);
    Logger.Instance.Log(baseConfigurationNotValid);
    return;
}

foreach (var config in localConfiguration.Configurations)
{
    if (!basicValidations(config))
    {
        return;
    }

    var files = Directory.GetFiles(config.BaseFolderPath);

    if (files.Length == 0)
    {
        string noFilesMessage = $"There are no files in the directory {config.BaseFolderPath}, exiting program.";
        Console.WriteLine(noFilesMessage);
        Logger.Instance.Log(noFilesMessage);
        return;
    }

    int cont = 0;

    foreach (var originalFile in files)
    {
        string? extension = Path.GetExtension(originalFile);
        string? originalFileName = Path.GetFileName(originalFile);
        
        if (string.IsNullOrEmpty(extension) || string.IsNullOrEmpty(originalFileName))
        {
            string noExtensionMessage = $"The file {originalFile} has no extension, reading next file.";
            Console.WriteLine(noExtensionMessage);
            Logger.Instance.Log(noExtensionMessage);
            continue;
        }

        extension = extension.Replace(".", "");

        var currentGroup = config.Groups.Find(x => x.Extensions.Exists(z => z.Equals(extension)));
        //var currentSubFolder = config.SubFolders.Find(x => x.Extensions.Exists(z => z.Equals(extension)));
        var currentSubFolder = config.SubFolders
            .Find(x => originalFileName.Contains(x.FileNameContains,
                                    x.CamelCaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)
                    && x.Extensions.Exists(z => z.Equals(extension)));

        string fullNewPath = currentGroup is null 
            ? Path.Combine(config.BaseFolderPath, extension)
            : Path.Combine(config.BaseFolderPath, currentGroup.FolderName);

        if (currentSubFolder is not null)
        {
            fullNewPath = Path.Combine(fullNewPath, currentSubFolder.FolderName);
        }

        if (!Directory.Exists(fullNewPath))
        {

            try
            {
                Directory.CreateDirectory(fullNewPath);
                string createdDirectoryMessage = $"Directorio creado: {fullNewPath}";
                Console.WriteLine(createdDirectoryMessage);
                Logger.Instance.Log(createdDirectoryMessage);
            }
            catch (Exception ex)
            {
                string creatingFolderException = $"Exception when creating {fullNewPath} folder";
                Console.WriteLine(creatingFolderException);
                Logger.Instance.Log(creatingFolderException);
                Logger.Instance.Log(ex.Message);
                continue;
            }
        }

        if (!isFileLocked(new FileInfo(originalFile)))
        {
            try
            {
                moveFile(originalFile, fullNewPath);
                cont++;
            }
            catch (Exception ex)
            {
                string movingFileExceptionMessage = $"Excepction when moving file {originalFile} to {fullNewPath}";
                Console.WriteLine(movingFileExceptionMessage);
                Logger.Instance.Log(movingFileExceptionMessage);
                Logger.Instance.Log(ex.Message);
                continue;
            }
        }
        else
        {
            string fileOpenByAnotherApplicationMessage = $"El archivo {originalFile} esta siendo utilizado por otra aplicacion.";
            Console.WriteLine(fileOpenByAnotherApplicationMessage);
            Logger.Instance.Log(fileOpenByAnotherApplicationMessage);
        }
    }

    string finalMessage = $"Cantidad de archivos movidos: {cont}";
    Console.WriteLine(finalMessage);
    Logger.Instance.Log(finalMessage);
}


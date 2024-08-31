# FileOrganizer
A project to organize files by type in the indicated directories

## Config structure
The file has to have an array of objects named `Configurations`. Each object representing a folder to organize.

### Properties
`BaseFolderPath`: should have the full path to the folder to organize.
`Groups`: is an array of objects that indicates what files should be put together
#### Groups
`FolderName` indicates the name of the folder that will contain the files
`Extensions` is an array of strings that indicates what extensions will affect this configuration

Example:
```
[
  {
    "FolderName": "images",
    "Extensions": [ "png", "jpg", "jpeg" ]
  },
  {
    "FolderName": "documents",
    "Extensions": [ "docx", "pdf" ]
  }
]
```
`SubFolders` is an array of objects that indicates if some files should be put in a specific sub folder
#### SubFolders
`FileNameContains` indicates a string to compare to the filename (do not include extension), the text is a like, not a literal
`CamelCaseSensitive` indicates if the comparison should take camel case into account
`Extensions` is an array of strings that indicates what extensions will affect this configuration
`FolderName` indicates the name of the folder that will contain the files

Example:
```
[
  {
    "FileNameContains": "parcial",
    "CamelCaseSensitive": false,
    "Extensions": [ "docx", "pdf" ],
    "FolderName": "Parciales"
  }
]
```

## Full File Example
```
{
  "Configurations": [
    {
      "BaseFolderPath": "C:\\Users\\walte\\Downloads",
      "Groups": [
        {
          "FolderName": "images",
          "Extensions": [ "png", "jpg", "jpeg" ]
        },
        {
          "FolderName": "documents",
          "Extensions": [ "docx", "pdf" ]
        }
      ],
      "SubFolders": [
        {
          "FileNameContains": "parcial",
          "CamelCaseSensitive": false,
          "Extensions": [ "docx", "pdf" ],
          "FolderName": "Parciales"
        }
      ]
    }
  ]
}
```

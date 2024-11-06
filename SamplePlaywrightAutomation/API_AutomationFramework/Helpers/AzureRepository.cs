using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace API_AutomationFramework.Helpers
{
    public class AzureRepository
    {
        public static bool VerifyFileinAzureRepository(string connectionString, string fileShareName, string fileName, List<string> subDirectories)
        {
            bool flag;
            ShareServiceClient service = new ShareServiceClient(connectionString);
            service.GetPropertiesAsync();
            flag = GetFolders(connectionString, fileShareName, fileName, subDirectories);
            return flag;
        }
        public static bool GetFolders(string connectionString, string fileShareName, string fileName, List<string> subDirList)
        {
            bool flag = false;
            ShareClient share = new ShareClient(connectionString, fileShareName);
            ShareDirectoryClient subDirectory = null!;

            if (subDirList.Count > 1)
            {
                for (int i = 0; i < subDirList.Count; i++)
                {
                    if (i == 0)
                    {
                        subDirectory = share.GetDirectoryClient(subDirList[i]);
                    }
                    else
                    {
                        subDirectory = subDirectory.GetSubdirectoryClient(subDirList[i]);
                    }
                }
            }

            else if (subDirList.Count == 1)
            {
                subDirectory = share.GetDirectoryClient(subDirList[0]);
            }

            if (subDirectory != null)
            {
                foreach (ShareFileItem item in subDirectory.GetFilesAndDirectories())
                {
                    if (!item.IsDirectory && item.Name == fileName)
                    {
                        flag = true;
                        Console.WriteLine("File " + item.Name + " found at the directory " + subDirectory.Uri + " in Azure Portal");
                        break;
                    }
                }
            }
            return flag;
        }       
    }
}


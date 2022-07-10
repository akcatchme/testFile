using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ConsoleApp1CopyFile
{
    class Program
    {
        static void Main(string[] args)
        {

            string zipFullPath = Dts.Variables["User::ZipFullPath"].Value.ToString();

            string unzip =Dts.Variables["$Package::unzip"].Value.ToString();

            string policies = Dts.Variables["$Package::policies"].Value.ToString();

            string processed = Dts.Variables["$Package::processed"].Value.ToString();


            if (!Directory.Exists(unzip))
            {
                Directory.CreateDirectory(unzip);
            }

            var unzipFolderName = Path.GetFileNameWithoutExtension(zipFullPath);
            string unzipIndividualFolder = Path.Combine(unzip, unzipFolderName);

            Directory.Delete(unzipIndividualFolder, true);
            Directory.CreateDirectory(unzipIndividualFolder);


            using (ZipArchive arch = ZipFile.OpenRead(zipFullPath))
            {

                foreach (ZipArchiveEntry entry in arch.Entries)
                {
                    string fileFullPath = Path.Combine(unzipIndividualFolder, entry.FullName);

                    string pathWithOutFileName = Path.GetDirectoryName(fileFullPath);

                    if (!Directory.Exists(pathWithOutFileName))
                        Directory.CreateDirectory(pathWithOutFileName);

                    if (!string.IsNullOrEmpty(entry.Name))
                        entry.ExtractToFile(Path.Combine(unzipIndividualFolder, entry.FullName));
                }
            }

            if (!Directory.Exists(processed))
            {
                Directory.CreateDirectory(processed);
            }


            File.Move(zipFullPath, Path.Combine(processed, Path.GetFileName(zipFullPath)));



            if (!Directory.Exists(policies))
            {
                Directory.CreateDirectory(policies);
            }


            string[] filePaths = Directory.GetFiles(unzipIndividualFolder, "*.*", SearchOption.AllDirectories);

            foreach (var filepath in filePaths)
            {

                string newFilePath = filepath.Replace(unzip, policies); // Path.Combine(policies, Path.GetFileName(filepath));

                if (!File.Exists(newFilePath))
                {
                    string pathWithOutFileName = Path.GetDirectoryName(newFilePath);

                    if (!Directory.Exists(pathWithOutFileName))
                        Directory.CreateDirectory(pathWithOutFileName);

                    File.Copy(filepath, newFilePath, true);
                }
            }
        }
    }
}

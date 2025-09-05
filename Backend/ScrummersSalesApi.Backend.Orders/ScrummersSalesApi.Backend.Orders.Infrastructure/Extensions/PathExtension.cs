using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrummersSalesApi.Backend.Orders.Infrastructure.Extensions
{
    public static class PathExtension
    {
        // Método para encontrar la carpeta base de la solución
        public static string FindSolutionBaseDirectory()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string solutionFolderName = ""; // Nombre de la carpeta de la solución

            while (!currentDirectory.EndsWith(solutionFolderName) && !string.IsNullOrEmpty(currentDirectory))
            {
                DirectoryInfo parentDirectory = Directory.GetParent(currentDirectory);
                currentDirectory = parentDirectory?.FullName;
            }

            return currentDirectory;
        }
    }
}

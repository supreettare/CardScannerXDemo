using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardScannerX
{
    public interface ISaveStreamToStorage
    {
        void SaveImageToFile(Stream imageStream, string localPath);

        Stream OpenFile(string txtFilePath);
    }
}

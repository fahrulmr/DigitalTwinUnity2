
using Ionic.Zip;
using System.IO;

public static class ZipUtil
{

	public static void Unzip (string zipFilePath, string location)
	{

		Directory.CreateDirectory (location);
		
		using (ZipFile zip = ZipFile.Read (zipFilePath)) {
			
			zip.ExtractAll (location, ExtractExistingFileAction.OverwriteSilently);
		}

	}
}

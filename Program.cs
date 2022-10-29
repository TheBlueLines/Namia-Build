using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Namia_Build
{
	internal class Program
	{
		static void Main(string[] args)
		{
			DirectoryInfo nzx = Directory.CreateDirectory("Binaries");
			string path = args.Length > 0 ? args[0] : "Source";
			string[] order = Directory.GetFiles(path);
			if (File.Exists("order.txt"))
			{
				order = File.ReadLines("order.txt").ToArray();
			}
			for (int i = 0; i < order.Length; i++)
			{
				order[i] = Path.GetFileNameWithoutExtension(order[i]);
			}
			CreateBinaries(path, order);
			MakeOS(nzx.Name, order);
			Directory.Delete(nzx.FullName, true);
		}
		private static void CreateBinaries(string path, string[] order)
		{
			foreach (string value in order)
			{
				Process process = new Process();
				process.StartInfo.FileName = "nasm";
				process.StartInfo.Arguments = "-f bin \"" + Path.Combine(path, value + ".asm") + "\" -o \"" + "Binaries\\" + value + ".bin\"";
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				process.Start();
				process.WaitForExit();
				process.Dispose();
			}
		}
		private static void MakeOS(string path, string[] order)
		{
			List<byte> data = new List<byte>();
			foreach (string value in order)
			{
				byte[] bytes = File.ReadAllBytes(Path.Combine(path, value + ".bin"));
				data.AddRange(bytes);
				while (data.Count % 512 != 0)
				{
					data.Add(0);
				}
			}
			File.WriteAllBytes("namia.iso", data.ToArray());
		}
	}
}
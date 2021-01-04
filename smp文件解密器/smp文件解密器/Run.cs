using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SmpFileDecryptor
{
    class Run
    {
        static private string read_path;//目标读取路径
        static private string read_suffix;//读取后缀名
        static private DirectoryInfo root;
        static private Queue<FileInfo> files;
        static private Queue<FileInfo> erroFiles;

        static private string save_path;//目标保存路径
        static private string save_suffix;//保存后缀名

        static private Queue<TargetFile> file_list;

        static private bool pattern;//true 批量模式 flase 单文件模式
        static private int counter;//计数
        static private int num;//总数

        static Thread readFiles;
        static Thread decodeFiles;

        static string key;

        static private void menu()
        {

            while (true)
            {
                Console.WriteLine("**********smp转mp3文件解密器*************");
                Console.WriteLine("选择模式: 1.批量模式 2.单文件模式");
                int i = Convert.ToInt32(Console.ReadLine());
                if (i == 1) pattern = true;
                else if (i == 2) pattern = false;
                else continue;
                break;
            }
            while (true)
            {
                Console.WriteLine("请输入读取路径:");
                read_path = Console.ReadLine();
                if (pattern)
                {
                    if (!System.IO.Directory.Exists(read_path))
                    {
                        Console.WriteLine("路径不存在");
                        continue;
                    }
                    break;
                }
                else
                {
                    if (!File.Exists(read_path))
                    {
                        Console.WriteLine("文件不存在");
                        continue;
                    }
                    break;

                }
            }
            while (true)
            {
                Console.WriteLine("请输入保存路径:");
                save_path = Console.ReadLine();
                if (!System.IO.Directory.Exists(save_path))
                {
                    Console.WriteLine("路径不存在");
                    continue;
                }
                break;
            }

            Console.WriteLine("请输入已知密匙(未知输入N)");
            key = Console.ReadLine();
            if (key == "N" || key == "n") key = "";

            Console.WriteLine("设定成功，文件读取中...");
        }
        static private void readFile()
        {
            while (files.Count != 0)
            {
                FileInfo j = files.Dequeue();
                if (j.Extension == read_suffix)
                    try
                    {
                        file_list.Enqueue(new TargetFile(j.FullName));
                    }
                    catch
                    {
                        erroFiles.Enqueue(j);
                        continue;
                    }
                if (file_list.Count == 10) Thread.Sleep(0);
            }
            Console.WriteLine("文件读取完成，文件数量：" + file_list.Count);
        }
        static private void decoder()
        {
            while (file_list.Count != 0 || files.Count != 0)
            {
                if (file_list.Count == 0) readFiles.Join(100);
                else
                {
                    Decryptor.decode(file_list.Dequeue(), save_path, save_suffix,key);
                    counter++;
                    Console.Clear();
                   Console.WriteLine("进度:" + $"{((double)counter / (double)num):P}");
                }
            }
                Console.WriteLine("破解完成，文件已保存至" + save_path);
        }
        static void Main()
        {
            read_suffix = ".smp";
            save_suffix = ".mp3";
            counter = 0;
            menu();
            erroFiles = new Queue<FileInfo>();
            if (pattern)
            {
                files = new Queue<FileInfo>();
                root = new DirectoryInfo(read_path);
                FileInfo[] f = root.GetFiles();
                foreach (FileInfo i in f)
                {
                    files.Enqueue(i);
                }
                num = files.Count;

                readFiles = new Thread(readFile);
                decodeFiles = new Thread(decoder);
                try
                {
                    file_list = new Queue<TargetFile>();
                    readFiles.Start();
                    decodeFiles.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: {ex}");
                }
                if(erroFiles.Count!=0)
                {
                    Console.WriteLine("文件损坏数量：" + erroFiles.Count);
                    while(erroFiles.Count!=0)
                    {
                        Console.WriteLine(erroFiles.Dequeue().Name);
                    }
                }
            }
            else
            {
                TargetFile file =new TargetFile(read_path);
                Decryptor.decode(file,save_path,save_suffix,key);
            }

        }
    }
}

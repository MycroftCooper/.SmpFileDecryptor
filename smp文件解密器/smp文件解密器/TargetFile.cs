using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmpFileDecryptor
{
    class TargetFile
    {
        private string name;
        private byte[] key;
        private byte[] target_file;

        public byte[] Key { get => key; set => key = value; }
        public byte[] Target_file { get => target_file; set => target_file = value; }

        public TargetFile(string file_path)
        {
            try
            {
                target_file = File.ReadAllBytes(file_path);
                name = file_path.Substring(file_path.LastIndexOf('\\')+1, file_path.LastIndexOf('.')-file_path.LastIndexOf('\\')-1);
            }
            catch (Exception ex) { throw ex; }
            
        }
        public void saveFile(string file_path, string save_suffix)
        {
            try
            {
                File.WriteAllBytes(file_path + @"\" + name+save_suffix, target_file);
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
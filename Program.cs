using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace ZhihuFormulaConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("参数数量不对。\n");
                PrintUsage();
                return;
            }
            var fn = args[0].Trim();
            if (fn == "help")
            {
                PrintUsage();
                return;
            }
            if (!File.Exists(fn))
            {
                Console.WriteLine("文件不存在: {0}", fn);
                return;
            }
            var newFn = ConvertFile(fn);
            if (newFn != default)
            {
                Console.WriteLine("转换成功，文件名保存为 {0}", newFn);
                return;
            }
            else
            {
                Console.WriteLine("转换失败。");
                return;
            }
        }

        private static object ConvertFile(string fn)
        {
            string contents = default;
            string newFn = default;
            try
            {
                contents = File.ReadAllText(fn);
                contents = Regex.Replace(contents, @"\$\$\n*((.|\n)*?)\n*\$\$", delegate (Match match)
                {
                    var o = match.Groups[1].Value;
                    var f = HttpUtility.UrlEncode(o);
                    return $"\n<img src=\"https://www.zhihu.com/equation?tex={f}\" alt=\"[公式]\" eeimg=\"1\" data-formula=\"{o.Replace("\n", "")}\">\n";
                });
                contents = Regex.Replace(contents, @"\$(.*?)\$", delegate (Match match)
                {
                    var o = match.Groups[1].Value;
                    var f = HttpUtility.UrlEncode(o);
                    return $"<img src=\"https://www.zhihu.com/equation?tex={f}\" alt=\"[公式]\" eeimg=\"1\" data-formula=\"{o.Replace("\n", "")}\">";
                });
                var dir = Path.GetDirectoryName(fn);
                newFn = dir + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(fn) + ".conv" + Path.GetExtension(fn);
                File.WriteAllText(newFn, contents);
            }
            catch (Exception ex)
            {
                Console.WriteLine("出现错误：" + ex.Message);
            }

            return newFn;
        }

        static void PrintUsage()
        {
            Console.WriteLine("说明：\n\t本程序用于将 Mathjax 公式替换为知乎图片公式。" +
                "\n用法：\n\t./zhconv 文件.md" +
                "\n作者：\n\tPluveto (https://www.pluvet.com)" +
                "\n发布与更新：\n\tPluveto (https://github.com/pluveto/ZhihuFormulaConvert)"); 
        }
    }
}

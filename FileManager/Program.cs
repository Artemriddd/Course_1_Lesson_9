using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class Program
    {
        const int WINDOW_HEIGHT = 30;
        const int WINDOW_WIDHT = 120;
        private static string currentDir = Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Title = "FileManager";
            Console.SetWindowSize(WINDOW_WIDHT, WINDOW_HEIGHT);
            Console.SetBufferSize(WINDOW_WIDHT, WINDOW_HEIGHT);

            DrowWindow(0,0,WINDOW_WIDHT,18);
            DrowWindow(0, 18, WINDOW_WIDHT, 8);
            UpdateConsole();


            Console.ReadLine();
        }
        /// <summary>
        /// Вовзрат позиции курсора
        /// </summary>
        /// <returns></returns>
        static (int left, int top) GetCursorPosition()
        {
            return (Console.CursorLeft, Console.CursorTop);
        }


        /// <summary>
        /// Обработка процесса ввода данных с консоли
        /// </summary>
        /// <param name="width">Длинна строки ввода</param>
        static void ProcessEnterCommand(int width)
        {
            (int left, int top) = GetCursorPosition();
            StringBuilder command = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            char key;
            do
            {
                keyInfo = Console.ReadKey();
                key = keyInfo.KeyChar;
                if(keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.UpArrow)
                {
                    command.Append(key);
                }
                (int currentLeft, int currentTop) = GetCursorPosition();
                if(currentLeft == width -2)
                {
                    Console.SetCursorPosition(currentLeft - 1, top);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, top);

                }
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if(command.Length > 0)
                    {
                        command.Remove(command.Length - 1, 1);
                    }
                    if(currentLeft>=left)
                    {
                        Console.SetCursorPosition(currentLeft, top);
                        Console.Write(" ");
                        Console.SetCursorPosition(currentLeft, top);
                    }
                    else
                    {
                        command.Clear();
                        Console.SetCursorPosition(left, top);
                    }
                }
            }
            while (keyInfo.Key != ConsoleKey.Enter);
            ParseCommandString(command.ToString());
        }
        static void ParseCommandString(string command)
        {
            string[] commandParams = command.ToLower().Split(' ');
            if(commandParams.Length>0)
            {
                switch (commandParams[0])
                {
                    case "cd":
                        if(commandParams.Length>1)
                        {
                            if(Directory.Exists(commandParams[1]))
                            {
                                currentDir = commandParams[1];
                                DrawTree(new DirectoryInfo(commandParams[1]), 1);
                            }
                        }
                        break;
                    case "ls":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int n))
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), n);
                            }
                            else
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), 1);
                            }
                        break;
                    case "cp":
                        if (commandParams.Length > 2 && File.Exists(commandParams[1]))
                        {
                            FileCopy(commandParams[1], commandParams[2]);
                        }
                        else if (commandParams.Length > 2 && Directory.Exists(commandParams[1]))
                        {
                            CopyDirectory(commandParams[1], commandParams[2], true);
                        }
                        break;
                    case "rm":
                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                        {
                            FileDelete(commandParams[1]);
                        }
                        else if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                            DeleteDirectory(commandParams[1], true);
                        }
                        break;
                    case "file":
                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                        {
                            FileInfo(commandParams[1]);
                        }
                        break;
                }
            }
            UpdateConsole();
        }
        /// <summary>
        /// Информация о файле
        /// </summary>
        /// <param name="source"></param>
        static void FileInfo(string source)
        {
            FileInfo info = new FileInfo(source);
            if (info.Exists)
            {
                Console.SetCursorPosition(1, 19);
                Console.WriteLine($"Путь к файлу: {info.FullName}");
                Console.SetCursorPosition(1, 20);
                Console.WriteLine($"Дата создания файла: {info.CreationTime}");
                Console.SetCursorPosition(1, 21);
                Console.WriteLine($"Последнее изменение: {info.LastWriteTime}");
                Console.SetCursorPosition(1, 22);
                Console.WriteLine($"Размер файла: {info.Length} байта");
            }
        }
        /// <summary>
        /// Удаляет файл
        /// </summary>
        /// <param name="targer"></param>
        static void FileDelete(string targer)
        {
            File.Delete(targer);
        }
        /// <summary>
        /// Удаляет директорию и все файлы в ней
        /// </summary>
        /// <param name="source"></param>
        /// <param name="lastDir"></param>
        static void DeleteDirectory(string source, bool lastDir)
        {
            var dir = new DirectoryInfo(source); // Создаем новую объект класса типа DirectoryInfo          
            DirectoryInfo[] dirs = dir.GetDirectories(); // Добавляем в массив все поддиректории в исходной директории 
            foreach (FileInfo file in dir.GetFiles())  // Удаляем все файлы из исходной папки
            {
                file.Delete();
            }
            if (lastDir) // Рекурсивно вызываем данный метод для всех поддиректорий
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    DeleteDirectory(subDir.FullName, true);
                }
                Directory.Delete(source);
            }
        }

        /// <summary>
        /// Копирует файл
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targer"></param>
        static void FileCopy(string source, string targer)
        {
            File.Copy(source, targer);
        }
        /// <summary>
        /// Копирует директорию и все файлы в ней
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="lastDir"></param>
        static void CopyDirectory(string source, string target, bool lastDir)
        {
            var dir = new DirectoryInfo(source); // Создаем новую объект класса типа DirectoryInfo          
            DirectoryInfo[] dirs = dir.GetDirectories(); // Добавляем в массив все поддиректории в исходной директории 
            Directory.CreateDirectory(target); // Создаем новую папку, куда идет копирование
            foreach (FileInfo file in dir.GetFiles())  // Копируем все файлы из исходной папки
            {
                string targetFilePath = Path.Combine(target, file.Name);
                file.CopyTo(targetFilePath);
            }
            if (lastDir) // Рекурсивно вызываем данный метод для всех поддиректорий
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(target, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        /// <summary>
        /// Обновление ввода с консоли
        /// </summary>
        static void UpdateConsole()
        {
            DrawConsole(currentDir, 0, 26, WINDOW_WIDHT, 3);
            ProcessEnterCommand(WINDOW_WIDHT);
        }
        /// <summary>
        /// Отрисовка консоли
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="widht"></param>
        /// <param name="height"></param>
        static void DrawConsole(string dir, int x, int y, int widht, int height)
        {
            DrowWindow(x, y, widht, height);
            Console.SetCursorPosition(x+1, y+height/2);
            Console.Write($"{dir}>");
        }
        /// <summary>
        /// Отрисовка окна
        /// </summary>
        /// <param name="x">Начальная позиция по оси Х</param>
        /// <param name="y">Начальная позиция по оси Y</param>
        /// <param name="widht">Ширина окна</param>
        /// <param name="height">Высота окна</param>
        static void DrowWindow(int x, int y, int widht, int height)
        {
            // шапка
            Console.SetCursorPosition(x, y);
            Console.Write("┌");
            for (int i = 0; i<widht-2;i++)
            {
                Console.Write("─");
            }
            Console.Write("┐");

            // окно
            Console.SetCursorPosition(x, y+1);
             for (int i = 0; i < height - 2; i++)
            {
                Console.Write("│");
                for (int j=x+1; j<x+widht-1;j++)
                {
                    Console.Write(" ");
                }
                Console.Write("│");
            }
            // подвал
            Console.Write("└");
            for (int i = 0; i < widht - 2; i++)
            {
                Console.Write("─");
            }
            Console.Write("┘");
            Console.SetCursorPosition(x, y);
        }
        static void DrawTree(DirectoryInfo dir, int page)
        {
            StringBuilder tree = new StringBuilder();
            GetTree(tree, dir, "", true);
            DrowWindow(0, 0, WINDOW_WIDHT, 18);
            (int currentLeft, int currentTop) = GetCursorPosition();
            int pageLines = 16;
            string[] lines = tree.ToString().Split('\n');
            int pageTotal = (lines.Length + pageLines - 1) / pageLines;
            if (page > pageTotal)
                page = pageTotal;

            for (int i = (page - 1) * pageLines, counter = 0; i < page * pageLines; i++, counter++)
            {
                if (lines.Length - 1 > i)
                {
                    Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                    Console.WriteLine(lines[i]);
                }
            }

            // Отрисуем footer
            string footer = $"┤ {page} of {pageTotal} ├";
            Console.SetCursorPosition(WINDOW_WIDHT / 2 - footer.Length / 2, 17);
            Console.WriteLine(footer);

        }

        static void GetTree(StringBuilder tree, DirectoryInfo dir, string indent, bool lastDirectory)
        {
            tree.Append(indent);
            if (lastDirectory)
            {
                tree.Append("└─");
                indent += "  ";
            }
            else
            {
                tree.Append("├─");
                indent += "│ ";
            }

            tree.Append($"{dir.Name}\n");


            FileInfo[] subFiles = dir.GetFiles();
            for (int i = 0; i < subFiles.Length; i++)
            {
                if (i == subFiles.Length - 1)
                {
                    tree.Append($"{indent}└─{subFiles[i].Name}\n");
                }
                else
                {
                    tree.Append($"{indent}├─{subFiles[i].Name}\n");
                }
            }


            DirectoryInfo[] subDirects = dir.GetDirectories();
            for (int i = 0; i < subDirects.Length; i++)
                GetTree(tree, subDirects[i], indent, i == subDirects.Length - 1);
        }
    }
}

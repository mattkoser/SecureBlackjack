using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SecureBlackjack
{
    class Player
    {
        FileSystemWatcher Listener = new FileSystemWatcher();
        string Directory = @"C:\Blackjack";
        public string Folder { get; }
        public string Name;
        public int Count { get; set; }
        int Sent = 0;
        public Player(String name)
        {
            DirectoryInfo folderMaker = new DirectoryInfo(Directory + "\\" + name.ToUpper());

            try
            {
                if (folderMaker.Exists)
                {
                    Console.WriteLine("There is already a user with that name! Please try again.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                folderMaker.Create();
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.ToString()}.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Name = name;
            Count = 0;
            Directory = Directory + "\\" + name.ToUpper();

            Listener.Filter = "*.txt";
            Listener.Path = Directory;
            Listener.Created += Recieved;
            Listener.EnableRaisingEvents = true;  


            Wait();
        }

        private void Wait()
        {
            
        }

        private void Communicate(String message)
        {

            //ALAN - Encrypt the message that is being sent to the controller

            string destination = @"C:\Blackjack\CONTROLLER" + "\\" + Name.ToUpper() + Sent.ToString() + ".txt";
            using (StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }
            Sent++;
        }

        private void Recieved(object source, FileSystemEventArgs file)
        {

        }


    }
}

using System;
using System.Threading;


namespace ChatApp
{

    public partial class Program
    {

        public static void Main()
        {
            Console.WriteLine("\n" +
                              "This program generate p2p chat.      \n " +
                              "For connecting to the chat press 'C' \n ");

            Char key = Console.ReadKey().KeyChar;
            Console.WriteLine("\n");

            if (key == 'C' || key == 'c')
            {
                Chat chat = new Chat();
                CancellationTokenSource TokenSource = new CancellationTokenSource();
                CancellationToken myToken = TokenSource.Token;

                Random randomID = new Random();
                int userId = randomID.Next(1000, 9999);

                chat.Connect(userId, TokenSource.Token);

                Console.WriteLine("\n Welcome! \n " +
                                  "Your ID is " + userId +     "\n" +
                                  "At any time to write a message press 'M' \n " +
                                  "Press 'Q' to quit the dialog \n "); ;
                

                while (!myToken.IsCancellationRequested)
                {
                    Char keyMessage;
                    keyMessage = Char.ToUpper(Console.ReadKey().KeyChar);

                    switch (keyMessage)
                    {
                        case 'M':
                            Console.WriteLine("\n" + userId + " : ");
                            String message = Console.ReadLine();

                            if (message != "")
                                chat.Send(userId, message);
                            Console.WriteLine("\n");
                            break;

                        case 'Q':
                            TokenSource.Cancel();
                            break;

                        default:
                            Console.WriteLine("\n" +
                              "Unknown key-message.          \n " +
                              "Press 'M' to wrire a message. \n " +
                              "Press 'Q' to quit.            \n ");
                            break;

                    }
                }
                TokenSource.Dispose();
            }
            Console.ReadKey();
        }
    }
}

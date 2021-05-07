using System;

namespace SteeringWheelTest
{
    class Program
    {
        static void Main(string[] args)
        {
            new Main();
        }
    }

    class Main
    {
        public Main()
        {
            Random rand = new Random();
            bool initialized = false;
            bool forward = true;
            LogitechGSDK.LogiSteeringInitialize(false);

            if (!LogitechGSDK.LogiIsConnected(0))
            {
                Console.WriteLine("Not connected");
            }

            while (true)
            {
                if (LogitechGSDK.LogiUpdate())
                {
                    // Random bumpy road effect
                    if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD))
                    {
                        LogitechGSDK.LogiStopBumpyRoadEffect(0);
                    }

                    // Stop button at left bottom button on wheel
                    if (LogitechGSDK.LogiButtonTriggered(0, 6))
                    {
                        break;
                    }

                    if (!initialized)
                    {
                        // Reset to center
                        Console.WriteLine("Initializing steering wheel... (4000 ms)");
                        LogitechGSDK.LogiPlaySpringForce(0, 0, 50, 50);
                        System.Threading.Thread.Sleep(2500);
                        Console.WriteLine("Starting simulation:");
                        initialized = true;
                        continue;
                    }

                    int offsetPercentage;

                    if (forward)
                    {
                        offsetPercentage = 30;
                    } 
                    else
                    {
                        offsetPercentage = -30;
                    }

                    LogitechGSDK.LogiPlaySpringForce(0, offsetPercentage, 10, 100);

                    // Invert direction
                    forward = !forward;

                    Console.WriteLine("Turning " + ((!forward) ? "backwards" : "forwards") + " (1500 ms)");

                    // Random bumpy road effect
                    if (rand.Next(0, 100) > 80)
                    {
                        Console.WriteLine("Brrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
                        LogitechGSDK.LogiPlayBumpyRoadEffect(0, 30);
                    }

                    System.Threading.Thread.Sleep(1500);
                }
            }

            // Shutdown
            LogitechGSDK.LogiSteeringShutdown();
        }
    }
}

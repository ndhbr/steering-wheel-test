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
        // USB Steering Wheel Device ID
        const int DEVICE_ID = 1;
        const int LOOP_DELAY_MS = 1000;

        // State
        private int rotation = 0;
        private readonly bool initialized = false;

        // Random
        private readonly Random random = new Random();

        public Main()
        {
            Initialize();

            if (!LogitechGSDK.LogiIsConnected(DEVICE_ID))
            {
                ErrorPrint("[ERROR] Lenkrad nicht verbunden, oder falsche Device ID angegeben");
                return;
            }


            while (true)
            {
                if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(DEVICE_ID))
                {
                    if (LogitechGSDK.LogiButtonIsPressed(DEVICE_ID, 7))
                    {
                        DebugPrint("Exit button pressed");
                        break;
                    }

                    if (!initialized)
                    {
                        CenterWheel();
                        initialized = true;
                    }
                    else
                    {
                        int newRotation = random.Next(20, 50);
                        int saturation = random.Next(10, 30);
                        int coefficient = random.Next(10, 30);

                        if (rotation > 0 || rotation < 0)
                        {
                            rotation = 0;
                            CenterWheel(saturation, coefficient);
                        }
                        else
                        {
                            rotation = newRotation;

                            if (random.Next(0, 2) == 1)
                            {
                                rotation = -rotation;
                            }

                            RotateWheel(rotation, saturation, coefficient);
                        }
                    }

                    System.Threading.Thread.Sleep(LOOP_DELAY_MS);
                }
            }

            Shutdown();
        }

        void Initialize()
        {
            Console.WriteLine("************** WELCOME! **************");
            DebugPrint("Initializing wheel...");
            LogitechGSDK.LogiSteeringInitialize(true);
        }

        void Shutdown()
        {
            CenterWheel();
            DebugPrint("Shutting down system...");
            Console.WriteLine("************** GOOD BYE! **************");
            LogitechGSDK.LogiSteeringShutdown();
        }

        void CenterWheel(int saturation = 100, int coefficient = 100)
        {
            DebugPrint("Centering wheel");
            rotation = 0;
            LogitechGSDK.LogiPlaySpringForce(DEVICE_ID, 0, saturation, coefficient);
        }

        void RotateWheel(int offset, int saturation = 100, int coefficient = 100)
        {
            DebugPrint("Rotating wheel to " + offset + " (saturation: " + saturation + ", coefficient: " + coefficient + ")");
            LogitechGSDK.LogiPlaySpringForce(DEVICE_ID, offset, saturation, coefficient);
        }

        void DebugPrint(String msg)
        {
            Console.WriteLine("[DEBUG] " + msg);
        }

        void ErrorPrint(String msg)
        {
            Console.WriteLine("[ERROR] " + msg);
        }
    }
}

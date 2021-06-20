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
                    StopBumpWheel();

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

                            if (random.Next(0, 2) == 1)
                            {
                                BumpWheel();
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(LOOP_DELAY_MS);
                }
            }

            Shutdown();
        }

        /// <summary>
        /// Initializes the wheel
        /// </summary>
        void Initialize()
        {
            Console.WriteLine("************** WELCOME! **************");
            DebugPrint("Initializing wheel...");
            LogitechGSDK.LogiSteeringInitialize(true);
        }

        /// <summary>
        /// Centers and shuts down the wheel
        /// </summary>
        void Shutdown()
        {
            CenterWheel();
            DebugPrint("Shutting down system...");
            Console.WriteLine("************** GOOD BYE! **************");
            LogitechGSDK.LogiSteeringShutdown();
        }

        /// <summary>
        /// Centers the wheel
        /// </summary>
        /// <param name="saturation">Saturation 0 - 100</param>
        /// <param name="coefficient">Coefficient Percentage -100 - 100</param>
        void CenterWheel(int saturation = 100, int coefficient = 100)
        {
            DebugPrint("Centering wheel");
            rotation = 0;
            LogitechGSDK.LogiPlaySpringForce(DEVICE_ID, 0, saturation, coefficient);
        }

        /// <summary>
        /// Rotates the wheel
        /// </summary>
        /// <param name="offset">Offset -100 - 100</param>
        /// <param name="saturation">Saturation 0 - 100</param>
        /// <param name="coefficient">Coefficient Percentage -100 - 100</param>
        void RotateWheel(int offset, int saturation = 100, int coefficient = 100)
        {
            DebugPrint("Rotating wheel to " + offset + " (saturation: " + saturation + ", coefficient: " + coefficient + ")");
            LogitechGSDK.LogiPlaySpringForce(DEVICE_ID, offset, saturation, coefficient);
        }

        /// <summary>
        /// Starts bumping the wheel
        /// </summary>
        void BumpWheel()
        {
            LogitechGSDK.LogiPlayBumpyRoadEffect(DEVICE_ID, 10);
        }

        /// <summary>
        /// This method stops bumping the wheel (in case it is bumping)
        /// </summary>
        void StopBumpWheel()
        {
            if (LogitechGSDK.LogiIsPlaying(DEVICE_ID, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD))
            {
                LogitechGSDK.LogiStopBumpyRoadEffect(DEVICE_ID);
            }
        }

        /// <summary>
        /// This method prints debug messages to the console
        /// </summary>
        /// <param name="msg">Message</param>
        void DebugPrint(String msg)
        {
            Console.WriteLine("[DEBUG] " + msg);
        }

        /// <summary>
        /// This method prints error messages to the console
        /// </summary>
        /// <param name="msg">Message</param>
        void ErrorPrint(String msg)
        {
            Console.WriteLine("[ERROR] " + msg);
        }
    }
}

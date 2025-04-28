using EnvironmentControlinator.Infra;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace EnvironmentControlinator.Controllers
{
    class KeyPressedController
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int key);

        private bool _leftMouseButtonHold = false;

        private bool _buttonX1Hold = false;

        private bool _buttonX2Hold = false;

        private bool isFPSGame = true;

        public void Start()
        {
            while (true)
            {
                Thread.Sleep(10);


                int middleMouseButton = GetAsyncKeyState(4);

                if (middleMouseButton == -32767)
                    isFPSGame = !isFPSGame;

                if (!isFPSGame) continue;

                int keyState = GetAsyncKeyState(1); // left button click

                var isHold = keyState != 0;
                ChangeLeftMouseButtonHold(isHold);

                if (_leftMouseButtonHold) continue;

                //keyState = GetAsyncKeyState(5); // button x 1



                keyState = GetAsyncKeyState(6) ; //button x 2

                isHold = keyState != 0 || GetAsyncKeyState(160) != 0;

                ChangeButonX2Hold(isHold);
            }
        }

        public void ChangeLeftMouseButtonHold(bool newValue)
        {
            if (newValue == _leftMouseButtonHold) return;

            _leftMouseButtonHold = newValue;

            var message = _leftMouseButtonHold ? "4" : "5";

            SendToArduino(message);
        }

        public void ChangeButonX2Hold(bool newValue)
        {
            if (newValue == _buttonX2Hold) return;

            _buttonX2Hold = newValue;

            var message = _buttonX2Hold ? "6" : "7";

            SendToArduino(message);//65536
        }

        private void SendToArduino(String message)
        {
            ArduinoHandler.SendMessage(message);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class TetrisInputReader : IComponentData, IDisposable
    {
        private object boxedInput;
        private TetrisInput m_input => boxedInput as TetrisInput;

        public static bool simulatePause = false;
        public static bool allowGameInput = true;

        private int lastMove;
        private float timeToMoveAgain;
        private int lastRotate;
        private bool lastDrop;
        private bool lastHold;
        private bool lastPause;

        public void Initialize()
        {
            // Create the input and enable the game input
            boxedInput = new TetrisInput();
            m_input.Enable();
            m_input.Game.Enable();
        }

        public void Dispose()
        {
            if (m_input != null)
                m_input.Dispose();
        }

        public void UpdateValues(ref InputValues values, in GameData settings, float deltaTime)
        {
            // If no input is allowed, just return the default values
            if (!allowGameInput)
            {
                values = new InputValues();
                return;
            }

            UpdateMove(ref values, settings, deltaTime);
            UpdateRotate(ref values);
            values.fall = m_input.Game.Fall.IsPressed();
            UpdateDrop(ref values);
            UpdateHold(ref values);
            UpdatePause(ref values);
        }

        private void UpdateMove(ref InputValues values, in GameData settings, float deltaTime)
        {
            var moveInput = Mathf.RoundToInt(m_input.Game.Move.ReadValue<float>());

            if (moveInput != 0)
            {
                // Start a movement
                if (lastMove != moveInput)
                {
                    values.move = moveInput;
                    timeToMoveAgain = settings.moveRepeatDelay;
                }
                else
                {
                    // Update the timer
                    timeToMoveAgain -= deltaTime;

                    if (timeToMoveAgain <= 0f)
                    {
                        // Repeat the movement
                        values.move = moveInput;
                        timeToMoveAgain = settings.moveRepeatPeriod;
                    }
                    else
                    {
                        // Don't move this frame
                        values.move = 0;
                    }
                }
            }
            else
            {
                // Reset everything
                values.move = 0;
                timeToMoveAgain = settings.moveRepeatDelay;
            }

            lastMove = moveInput;
        }

        private void UpdateRotate(ref InputValues values)
        {
            var rotateInput = Mathf.RoundToInt(m_input.Game.Rotate.ReadValue<float>());

            if (rotateInput != 0 && rotateInput != lastRotate)
                values.rotate = rotateInput;
            else
                values.rotate = 0;

            lastRotate = rotateInput;
        }

        private void UpdateDrop(ref InputValues values)
        {
            var dropInput = m_input.Game.Drop.IsPressed();

            values.drop = (dropInput && !lastDrop);

            lastDrop = dropInput;
        }

        private void UpdateHold(ref InputValues values)
        {
            var holdInput = m_input.Game.Hold.IsPressed();

            values.hold = holdInput && !lastHold;

            lastHold = holdInput;
        }

        private void UpdatePause(ref InputValues values)
        {
            var pauseInput = simulatePause ? true : m_input.Game.Pause.IsPressed();
            simulatePause = false;

            values.pause = pauseInput && !lastPause;

            lastPause = pauseInput;
        }
    }
}
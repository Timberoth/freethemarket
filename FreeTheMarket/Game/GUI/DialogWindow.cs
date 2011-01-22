using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GarageGames.Torque.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// TODO NEED TO REMOVE
//using System.Timers;

namespace FreeTheMarket
{
    public class DialogWindow
    {
        int NUM_LINES = 4;

        float[,] GUI_LINE_OFFSETS = new float[4,2]{ {15.0f, 10.0f},
                                       {15.0f, 32.0f},
                                       {15.0f, 54.0f},
                                       {15.0f, 76.0f}};

        // Dictionary of all English characters and their pixel widths.  This gets filled
        // as new text comes in.
        Dictionary<Char, float> _charWidths;
        
        // Normal stuff
        String _text;
        float _x;
        float _y;
        float _height;
        float _width;
        Queue<String> _textLines;

        // TorqueX stuff
        GUITextStyle _dialogWindowTextStyle;
        GUIBitmapStyle _dialogBitmapStyle;
        GUIBitmap _windowBitmap;
        GUIControl _parentGUIControl;

        // Torque GUIText lines
        GUIText[] _guiTexts;

        // TODO NEED TO REMOVE
        //System.Timers.Timer aTimer;

        // Constructor
        public DialogWindow( GUIControl parentGUIControl, String text, float x, float y, float width, float height )
        {
            // Can't do anything without a GUIControl to add this dialog to.
            Debug.Assert(parentGUIControl != null, "DialogWindows must be added to an existing GUIControl.");

            _charWidths = new Dictionary<Char, float>();

            _parentGUIControl = parentGUIControl;
            _x = x;
            _y = y;
            _height = height;
            _width = width;
            _text = text;
            _textLines = new Queue<string>();

            _dialogBitmapStyle = new GUIBitmapStyle();
            _dialogBitmapStyle.SizeToBitmap = false;

            _windowBitmap = new GUIBitmap();
            _windowBitmap.Style = _dialogBitmapStyle;
            _windowBitmap.Size = new Vector2(_width, _height);
            _windowBitmap.Bitmap = @"data\images\dialog_window";
            _windowBitmap.Folder = parentGUIControl;
            _windowBitmap.Visible = true;
            _windowBitmap.Position = new Vector2(_x,_y);


            // Create the text style for all the text.
            _dialogWindowTextStyle = new GUITextStyle();
            _dialogWindowTextStyle.FontType = "Arial14"; // @"data\images\MyFont";
            _dialogWindowTextStyle.TextColor[0] = Color.White;
            _dialogWindowTextStyle.SizeToText = true;
            _dialogWindowTextStyle.Alignment = TextAlignment.JustifyLeft;
            _dialogWindowTextStyle.PreserveAspectRatio = true;            

            // Create all the GUI text lines.
            _guiTexts = new GUIText[NUM_LINES];
            
            // Something is off with the calcuations because the 60.0 doesn't make logical sense.
            BreakTextIntoLines(text, _width-60.0f, _height);

            // This has to be modified to work with text that spans multiple lines.
            string line = "";
            for (int i = 0; i < NUM_LINES; ++i)
            {                
                _guiTexts[i] = new GUIText();
                _guiTexts[i].Style = _dialogWindowTextStyle;
                // The text needs to be inside the dialog window so the position should
                // be offset from the dialog window's position.
                _guiTexts[i].Position = new Vector2(_x + GUI_LINE_OFFSETS[i,0], _y + GUI_LINE_OFFSETS[i,1]);
                _guiTexts[i].Visible = true;
                _guiTexts[i].Folder = parentGUIControl;

                line = "";
                if (_textLines.Count > 0)
                    line = _textLines.Dequeue();

                _guiTexts[i].Text = line;
            }

            /*
            // TODO NEED TO REMOVE
            aTimer = new System.Timers.Timer();

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // Set the Interval to 2 seconds (2000 milliseconds).
            aTimer.Interval = 4000;
            aTimer.Enabled = true;
            aTimer.AutoReset = true;
             */
        }

        public void ShowWindow()
        {
            _windowBitmap.Visible = true;
            for (int i = 0; i < NUM_LINES; ++i)
            {               
                _guiTexts[i].Visible = true;                
            }
        }

        public void HideWindow()
        {
            _windowBitmap.Visible = false;
            for (int i = 0; i < NUM_LINES; ++i)
            {
                _guiTexts[i].Visible = false;
            }
        }

        /*
        // TODO NEED TO REMOVE
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (_textLines.Count <= 0)
                aTimer.Stop();
                
            AdvanceText();            
        }
         */

        // Load up the next 4 lines of text if tere is any or close the window if the dialog is done.
        public void AdvanceText()
        {
            if (_textLines.Count <= 0)
            {
                // Hide the window
                HideWindow();
                return;
            }

            string line = "";
            for (int i = 0; i < NUM_LINES; ++i)
            {                
                // The text needs to be inside the dialog window so the position should
                // be offset from the dialog window's position.
                _guiTexts[i].Position = new Vector2(_x + GUI_LINE_OFFSETS[i, 0], _y + GUI_LINE_OFFSETS[i, 1]);                

                line = "";
                if (_textLines.Count > 0)
                    line = _textLines.Dequeue();

                _guiTexts[i].Text = line;
            }
        }


        // Take the complete string and break it into multiple lines which can be advanced through.
        private void BreakTextIntoLines(String text, float boxWidth, float boxHeight)
        {
            // String Pixel length
            float stringPixelLength = 0.0f;

            // Create dummy GUIText to calculate character widths.
            GUIText dummyText = new GUIText();
            dummyText.Style = _dialogWindowTextStyle;
            dummyText.Visible = false;
            dummyText.Folder = _parentGUIControl;
            dummyText.Text = "A";
            float charHeight = dummyText.Size.Y;

            // Keep track of the last space encountered so it can
            // be used as a line break index.
            int lastSpaceIndex = 0;

            int i = 0;
            bool doneProcessingString = false;
            while ( !doneProcessingString )
            {
                // Check if the string is done processing.
                if (i >= text.Length)
                {
                    // The remain text is the last line.
                    _textLines.Enqueue(text);

                    doneProcessingString = true;

                    break;
                }

                // Check if the character is already in the dictionary
                Char character = text[i];
                if (_charWidths.ContainsKey(character))
                {
                    stringPixelLength += _charWidths[character];
                    
                }
                else
                {
                    // Calculate the width and add it to the dictionary
                    dummyText.Text = character.ToString();
                    _charWidths.Add(character, dummyText.Size.X);
                    stringPixelLength += _charWidths[character];
       
                }

                // Track last encountered space
                if (character == ' ')
                {
                    lastSpaceIndex = i;
                }

                // Figure out if the string needs to be split into it's own line.
                if ( stringPixelLength >= boxWidth )
                {
                    String completeLine = text.Substring(0, lastSpaceIndex);
                    text = text.Substring(lastSpaceIndex+1);
                    lastSpaceIndex = 0;
                    stringPixelLength = 0.0f;
                    i = -1;

                    _textLines.Enqueue(completeLine);
                }

                // Increment counter
                ++i;
            }
        }
    }   
}

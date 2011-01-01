﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GarageGames.Torque.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace FreeTheMarket
{
    public class DialogWindow
    {                
        float[,] GUI_LINE_OFFSETS = new float[3,2]{ {15.0f, 10.0f},
                                       {15.0f, 32.0f},
                                       {15.0f, 54.0f} };

        // Dictionary of all English characters and their pixel widths.  This gets filled
        // as new text comes in.
        Dictionary<Char, float> charWidths;
        
        // Normal stuff
        String _text;
        float _x;
        float _y;
        float _height;
        float _width;
        Queue<String> _textLines;

        // TorqueX stuff
        GUITextStyle dialogWindowTextStyle;
        GUIBitmapStyle dialogBitmapStyle;
        GUIBitmap _windowBitmap;
        GUIControl _parentGUIControl;

        // Torque GUIText lines
        GUIText[] _guiTexts;
        
        public DialogWindow( GUIControl parentGUIControl, String text, float x, float y, float width, float height )
        {
            // Can't do anything without a GUIControl to add this dialog to.
            Debug.Assert(parentGUIControl != null, "DialogWindows must be added to an existing GUIControl.");

            charWidths = new Dictionary<Char, float>();

            _parentGUIControl = parentGUIControl;
            _x = x;
            _y = y;
            _height = height;
            _width = width;
            _text = text;
            _textLines = new Queue<string>();

            dialogBitmapStyle = new GUIBitmapStyle();
            dialogBitmapStyle.SizeToBitmap = false;

            _windowBitmap = new GUIBitmap();
            _windowBitmap.Style = dialogBitmapStyle;
            _windowBitmap.Size = new Vector2(_width, _height);
            _windowBitmap.Bitmap = @"data\images\dialog_window";
            _windowBitmap.Folder = parentGUIControl;
            _windowBitmap.Visible = true;
            _windowBitmap.Position = new Vector2(_x,_y);


            // Create the text style for all the text.
            dialogWindowTextStyle = new GUITextStyle();
            dialogWindowTextStyle.FontType = "Arial14"; // @"data\images\MyFont";
            dialogWindowTextStyle.TextColor[0] = Color.White;
            dialogWindowTextStyle.SizeToText = true;
            dialogWindowTextStyle.Alignment = TextAlignment.JustifyLeft;
            dialogWindowTextStyle.PreserveAspectRatio = true;            

            // Create all the GUI text lines.
            _guiTexts = new GUIText[3];

            //String testText = "Here's a test string of characters that should be long enough to roll on the second line of the dialog window and maybe even a third line.";
            String testText = "Let's see how the dialog window handles this longer piece of text with different spacing.  It looks like this string has been broken up into multiple lines, but the question is how many?";
            
            // Something is off with the calcuations because the 60.0 doesn't make logical sense.
            BreakTextIntoLines(testText, _width-60.0f, _height);

            // This has to be modified to work with text that spans multiple lines.
            for (int i = 0; i < 3; ++i)
           {
                _guiTexts[i] = new GUIText();
                _guiTexts[i].Style = dialogWindowTextStyle;
                // The text needs to be inside the dialog window so the position should
                // be offset from the dialog window's position.
                _guiTexts[i].Position = new Vector2(_x + GUI_LINE_OFFSETS[i,0], _y + GUI_LINE_OFFSETS[i,1]);
                _guiTexts[i].Visible = true;
                _guiTexts[i].Folder = parentGUIControl;
                _guiTexts[i].Text = _textLines.Dequeue();
            }    
        }

        private void BreakTextIntoLines(String text, float boxWidth, float boxHeight)
        {
            // String Pixel length
            float stringPixelLength = 0.0f;

            // Create dummy GUIText to calculate character widths.
            GUIText dummyText = new GUIText();
            dummyText.Style = dialogWindowTextStyle;
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
                if (charWidths.ContainsKey(character))
                {
                    stringPixelLength += charWidths[character];
                    
                }
                else
                {
                    // Calculate the width and add it to the dictionary
                    dummyText.Text = character.ToString();
                    charWidths.Add(character, dummyText.Size.X);
                    stringPixelLength += charWidths[character];
       
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
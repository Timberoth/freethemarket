using System;
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
        float[] FIRST_LINE_OFFSET = {15.0f, 10.0f};

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
        GUIText _guiText;        
        GUIBitmap _windowBitmap;
        GUIControl _parentGUIControl;

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
            _guiText = new GUIText();
            _guiText.Style = dialogWindowTextStyle;
            // The text needs to be inside the dialog window so the position should
            // be offset from the dialog window's position.
            _guiText.Position = new Vector2( _x+FIRST_LINE_OFFSET[0], _y+FIRST_LINE_OFFSET[1]);
            _guiText.Visible = true;
            _guiText.Folder = parentGUIControl;
            //_guiText.Text = _text;
            //_guiText.Text = "wwwwwwwwwWwwwwwwwwwWwwwwwwwwwWwwwwwwwwwWwwwwwwwwwW";                       
            _guiText.Text = "Here's a test string of characters that should be long enough to roll on the second line of the dialog window and maybe even a third line.";

            BreakTextIntoLines(_guiText.Text, _width, _height);
        }

        private void BreakTextIntoLines(String text, float boxWidth, float boxHeight)
        {
            // Total pixel length
            float totalPixelLength = 0.0f;

            // Create dummy GUIText to calculate character widths.
            GUIText dummyText = new GUIText();
            dummyText.Style = dialogWindowTextStyle;
            dummyText.Visible = false;
            dummyText.Folder = _parentGUIControl;
            dummyText.Text = "A";
            float charHeight = dummyText.Size.Y;
            
            // Go through each character and keep track of character width if not already stored.
            for (int i = 0; i < text.Length; i++)
            {
                // Check if the character is already in the dictionary
                Char character = text[i];
                if (charWidths.ContainsKey(character))
                {
                    totalPixelLength += charWidths[character];
                }
                else
                {
                    // Calculate the width and add it to the dictionary
                    dummyText.Text = character.ToString();
                    charWidths.Add(character, dummyText.Size.X);
                    totalPixelLength += dummyText.Size.X;
                }
            }

            // Based on the total pixel length and the length of the dialog box, calculate
            // how many lines would be required.
            int numLines = (int)Math.Ceiling(totalPixelLength / boxWidth);

            // Based on this rough estimate, figure out how to break string into lines.
            
            // If there is only one line, that's all you have to do.
            if (numLines == 1)
            {
                _textLines.Enqueue(text);
            }

            // If there are more lines, we have to figure out exactly where to break the string.
            else
            {
                // TODO
            }
        }
    }   
}

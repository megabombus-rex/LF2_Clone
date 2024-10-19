using LF2Clone.Base;
using LF2Clone.Misc.Helpers;
using System.Text;
using Raylib = Raylib_cs.Raylib;

namespace LF2Clone.UI
{
    public class Textbox : UIComponent
    {
        private StringBuilder _sb;
        private string _displayedText;
        private bool _isSelected;
        private int _cursorIndex;

        public Textbox(float rotation, Node? node, bool isActive, int id) 
            : base(rotation, node, isActive, id)
        {
            _sb = new StringBuilder();
            _displayedText = string.Empty;
            _isSelected = false;
        }

        public void AddLetter(char c)
        {
            _sb.Append(c);
        }

        public void RemoveLetter(int index) 
        {
        
        }

        public void RemoveLetter()
        {

        }

        public void ChangeSelection()
        {
            _isSelected = !_isSelected;
        }
    }
}

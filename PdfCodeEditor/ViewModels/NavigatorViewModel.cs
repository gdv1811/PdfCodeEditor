using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using PdfCodeEditor.Models;

namespace PdfCodeEditor.ViewModels
{
    internal class NavigatorViewModel : ViewModelBase
    {
        #region Fields

        private int _caretOffset;

        private readonly History<int> _history = new History<int>();
        private ICommand _forwardCommand;
        private ICommand _backwardCommand;
        private ICommand _goToDefinitionCommand;

        #endregion

        #region Properties

        public int CaretOffset
        {
            get { return _caretOffset; }
            set
            {
                _caretOffset = value;
                OnPropertyChanged(nameof(CaretOffset));
            }
        }

        #endregion

        #region Property-commands

        public ICommand ForwardCommand
        {
            get { return _forwardCommand ?? (_forwardCommand = new RelayCommand(arg => Forward())); }
        }

        public ICommand BackwardCommand
        {
            get { return _backwardCommand ?? (_backwardCommand = new RelayCommand(arg => Backward())); }
        }

        public ICommand GoToDefinitionCommand
        {
            get { return _goToDefinitionCommand ?? (_goToDefinitionCommand = new RelayCommand(arg => GoToDefinition(CaretOffset))); }
        }

        public TextDocument Document { get; set; }

        #endregion

        #region Constructor

        public NavigatorViewModel(TextDocument document)
        {
            Document = document;
        }

        #endregion

        #region Private methods

        private string GetReference(int carreteOffset)
        {
            var spacesCount = 0;
            var token = "";
            var step = 1;
            for (var currentOffset = carreteOffset; ; currentOffset += step)
            {
                var currentChar = Document.GetCharAt(currentOffset);
                if (currentChar == 'R')
                {
                    token += currentChar;
                    currentOffset = carreteOffset;
                    step = -1;
                    continue;
                }
                if (currentChar == ' ')
                {
                    spacesCount++;
                    if (spacesCount == 3)
                        break;
                }
                else if (!char.IsDigit(currentChar))
                    break;

                if (step == 1)
                    token += currentChar;
                else
                    token = currentChar + token;
            }

            return spacesCount > 1 ? token : null;
        }

        private void GoToDefinition(string reference)
        {
            var definition = reference.Replace("R", "obj");
            var regex = new Regex("[^0-9]" + definition);
            var match = regex.Match(Document.Text);

            if (!match.Success)
                return;

            CaretOffset = match.Index + 1;
            AddPositionInHistory(CaretOffset);
        }

        private void GoToDefinition(int offset)
        {
            var reference = GetReference(offset);
            if (reference == null)
                return;
            AddPositionInHistory(offset);
            GoToDefinition(reference);
        }

        private void AddPositionInHistory(int position)
        {
            if (Math.Abs(position - _history.CurrentItem) > 20)
                _history.Add(position);
        }

        private void Forward()
        {
            if (!_history.CanRedo)
                return;
            CaretOffset = _history.Redo();
        }

        private void Backward()
        {
            AddPositionInHistory(CaretOffset);
            if (!_history.CanUndo)
                return;
            CaretOffset = _history.Undo();
        } 

        #endregion
    }
}

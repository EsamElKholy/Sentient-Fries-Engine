﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentient_Editor.Utilities
{
    public interface IUndoRedo
    {
        public string Name { get; }
        public void Undo();
        public void Redo();
    }

    public class UndoRedoAction : IUndoRedo
    {
        private Action undoAction;
        private Action redoAction;

        public string Name { get; }

        public void Redo() => redoAction();

        public void Undo() => undoAction();

        public UndoRedoAction(string name)
        {
            Name = name;
        }

        public UndoRedoAction(Action undo, Action redo, string name)
            : this(name)
        {
            Debug.Assert(undo != null && redo != null);

            undoAction = undo;
            redoAction = redo;
        }

        public UndoRedoAction(string property, object instance, object undoValue, object redoValue, string name)
            : this
            (
                () => instance.GetType().GetProperty(property).SetValue(instance, undoValue),
                () => instance.GetType().GetProperty(property).SetValue(instance, redoValue),
                name
            )
        {}
    }

    public class UndoRedo
    {
        private bool enableAdd = true;
        
        private ObservableCollection<IUndoRedo> redoList = new ObservableCollection<IUndoRedo>();
        private ObservableCollection<IUndoRedo> undoList = new ObservableCollection<IUndoRedo>();

        public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }
        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }
        
        public UndoRedo()
        {
            RedoList = new ReadOnlyObservableCollection<IUndoRedo>(redoList);
            UndoList = new ReadOnlyObservableCollection<IUndoRedo>(undoList);
        }

        public void Reset() 
        {
            redoList.Clear();
            undoList.Clear();
        }

        public void Undo() 
        {
            if (undoList.Any())
            {
                var command = undoList.Last();
                undoList.RemoveAt(undoList.Count - 1);
                
                enableAdd = false;
                command.Undo();
                enableAdd = true;

                redoList.Insert(0, command);
            }
        }

        public void Redo() 
        {
            if (redoList.Any())
            {
                var command = redoList.First();
                redoList.RemoveAt(0);

                enableAdd = false;
                command.Redo();
                enableAdd = true;

                undoList.Add(command);
            }
        }

        public void Add(IUndoRedo command) 
        {
            if (enableAdd)
            {
                undoList.Add(command);

                redoList.Clear();
            }
        }
    }
}

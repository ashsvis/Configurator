using ModelHolder;
using System;

namespace BaseEditor
{
    /// <summary>
    /// Управление операциями отмены/возврата действий
    /// </summary>
    public class UndoRedoController
    {
        private ModelRoot _snapshot;
        private readonly ModelRoot _root;
        private string _operationName;

        /// <summary>
        /// Конструктор запоминает рабочий контекст, с которым будет работать undo/redo
        /// </summary>
        /// <param name="root"></param>
        public UndoRedoController(ModelRoot root)
        {
            _root = root;
        }

        /// <summary>
        /// Выполняем перед началом операций изменения контента
        /// </summary>
        /// <param name="operationName"></param>
        public void OnStartOperation(string operationName)
        {
            // сначала запоминаем копию контекста по значению в локальной переменной
            _snapshot = _root.DeepClone();
            // запоминаем также наименование операции
            _operationName = operationName;
        }

        /// <summary>
        /// Выполняем после окончания операций изменения контента
        /// </summary>
        public void OnFinishOperation()
        {
            var afterOperationSnapshot = _root.DeepClone();
            var beforeOperationSnapshot = _snapshot; // захват переменной при выполнении тела акций

            Action undo = () =>
            {
                _root.Id = beforeOperationSnapshot.DeepClone().Id;
                _root.Name = beforeOperationSnapshot.DeepClone().Name;
                _root.Childs = beforeOperationSnapshot.DeepClone().Childs;
                _root.Properies = beforeOperationSnapshot.DeepClone().Properies;
            };

            Action redo = () =>
            {
                _root.Id = afterOperationSnapshot.DeepClone().Id;
                _root.Name = afterOperationSnapshot.DeepClone().Name;
                _root.Childs = afterOperationSnapshot.DeepClone().Childs;
                _root.Properies = afterOperationSnapshot.DeepClone().Properies;
            };

            UndoRedoManager.Instance.Add(new ActionCommand(undo, redo) { Name = _operationName });
        }
    }
}

using ModelHolder;
using ModelHolder.Common;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BaseEditor
{
    public partial class BaseEditorForm : Form
    {
        private UndoRedoController _undoRedoController;
        // здесь храним состояние модели
        private ModelRoot _modelRoot;
        private string _defaultFileName;

        public BaseEditorForm()
        {
            InitializeComponent();
            // имя файла для сохранения состояния по умолчанию
            _defaultFileName = Path.ChangeExtension(Application.ExecutablePath, ".bin");
            _modelRoot = new ModelRoot();
            _modelRoot.Properies.Add(new ModelProperty() { Name = "Name", Value = "Root", Type = typeof(string) });
        }

        /// <summary>
        /// Первоначальная главной загрузка формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseEditorForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(_defaultFileName))
                _modelRoot = SaverLoader.LoadFromFile(_defaultFileName);
            // инициализация менеджера отмены/возврата состояния
            _undoRedoController = new UndoRedoController(_modelRoot);
            // заполнение дерева
            FillTree(treeView);
        }

        /// <summary>
        /// Перед закрытием формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // сохраняем состояние в файл
            SaverLoader.SaveToFile(_defaultFileName, _modelRoot);
        }

        /// <summary>
        /// Заполнение дерева сущностей
        /// </summary>
        /// <param name="tv"></param>
        private void FillTree(TreeView tv)
        {
            tv.BeginUpdate();
            try
            {
                tv.Nodes.Clear();
                var rootNode = new TreeNode(_modelRoot.Name) { Tag = _modelRoot };
                tv.Nodes.Add(rootNode);
                // заполняем дерево рекурсивно
                AddChildNodes(rootNode, _modelRoot);
                tv.ExpandAll();
            }
            finally
            {
                tv.EndUpdate();
            }
        }

        /// <summary>
        /// Рекурсивное заполнение дерева вложенными сущностями
        /// </summary>
        /// <param name="node"></param>
        /// <param name="modelItem"></param>
        private void AddChildNodes(TreeNode node, ModelItem modelItem)
        {
            foreach (var item in modelItem.Childs)
            {
                var childNode = new TreeNode(item.Name) { Tag = item };
                node.Nodes.Add(childNode);
                // продолжаем заполнение для дочерних узлов
                AddChildNodes(childNode, item);
            }
        }

        /// <summary>
        /// Жмём мышкой на дереве
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            treeView.SelectedNode = treeView.GetNodeAt(e.Location);
            if (treeView.SelectedNode == null)
                listView.Items.Clear();
        }

        /// <summary>
        /// Перед открытием контекстного меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var nodeFound = treeView.SelectedNode != null;
            tsmiAddItem.Visible = nodeFound;
            tsmiRenameItem.Visible = nodeFound;
            tsmiDeleteItem.Enabled = nodeFound && treeView.SelectedNode.Parent != null;
            tsmiDeleteItem.Visible = nodeFound;
            // если нет выбранного узла, то меню не показываем совсем
            e.Cancel = !nodeFound;
        }

        /// <summary>
        /// Переименование сущности
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiRenameItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            var frm = new StringEditorForm();
            var item = (ModelItem)treeView.SelectedNode.Tag;
            frm.tbValue.Text = item.Name;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                _undoRedoController.OnStartOperation("Rename item");
                var nameProp = item.Properies.FirstOrDefault(x => x.Name == "Name");
                if (nameProp != null)
                    nameProp.Value = frm.tbValue.Text;
                else
                {
                    var prop = new ModelProperty() { Name = "Name", Type = typeof(string), Value = frm.tbValue.Text };
                    item.Properies.Add(prop);
                }
                _undoRedoController.OnFinishOperation();
                treeView.SelectedNode.Text = frm.tbValue.Text;
                FillList(listView);
            }
        }


        /// <summary>
        /// Добавим вложенный элемент
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiAddItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            var item = (ModelItem)treeView.SelectedNode.Tag;
            // создадим элемент
            _undoRedoController.OnStartOperation("Add item");
            var childItem = new ModelItem() { Parent = item };
            item.Childs.Add(childItem);
            _undoRedoController.OnFinishOperation();
            // внесём изменения в визуальный интерфейс
            treeView.SelectedNode.Expand();
            var childNode = new TreeNode(childItem.Name) { Tag = childItem };
            treeView.SelectedNode.Nodes.Add(childNode);
            treeView.SelectedNode = childNode;
        }

        /// <summary>
        /// Удаление узла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiDeleteItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            var item = (ModelItem)treeView.SelectedNode.Tag;
            if (item.Parent == null) return;
            if (MessageBox.Show(this, "Are you sure to delete this item (with his childs)?", "Delete item(s)", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            _undoRedoController.OnStartOperation("Remove item");
            item.Parent.Childs.Remove(item);
            _undoRedoController.OnFinishOperation();
            // внесём изменения в визуальный интерфейс
            var parentNode = treeView.SelectedNode.Parent;
            parentNode.Nodes.Remove(treeView.SelectedNode);
            treeView.SelectedNode = parentNode;
        }
        /// <summary>
        /// Отменить последнее изменение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoRedoManager.Instance.Undo();
            FillTree(treeView);
            FillList(listView);
        }

        /// <summary>
        /// Вернуть отменённое состояние
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoRedoManager.Instance.Redo();
            FillTree(treeView);
            FillList(listView);
        }

        /// <summary>
        /// Обновление состояний пунктов меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            undoToolStripMenuItem.Enabled = UndoRedoManager.Instance.CanUndo;
            redoToolStripMenuItem.Enabled = UndoRedoManager.Instance.CanRedo;
        }

        /// <summary>
        /// Обновление состояний кнопочек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            tsbUndo.Enabled = UndoRedoManager.Instance.CanUndo;
            tsbRedo.Enabled = UndoRedoManager.Instance.CanRedo;
            //
            if (listView.Focused)
            {
                tsbMoveUp.Enabled = listView.SelectedIndices.Count == 1 && listView.SelectedItems[0].Index > 0;
                tsbMoveDown.Enabled = listView.SelectedIndices.Count == 1 && listView.SelectedItems[0].Index < listView.Items.Count - 1;
                tsbDeleteProp.Enabled = listView.SelectedIndices.Count == 1;
                tsbAddProp.Enabled = treeView.SelectedNode != null;
            }
            else
            {
                tsbMoveUp.Enabled = false;
                tsbMoveDown.Enabled = false;
                tsbDeleteProp.Enabled = false;
                tsbAddProp.Enabled = false;
            }
        }

        /// <summary>
        /// При выборе узла в дереве
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FillList(listView);
        }
        
        /// <summary>
        /// Заполнение списка свойств
        /// </summary>
        /// <param name="lv"></param>
        private void FillList(ListView lv)
        {
            lv.BeginUpdate();
            try
            {
                lv.Items.Clear();
                if (treeView.SelectedNode == null) return;
                var item = (ModelItem)treeView.SelectedNode.Tag;
                ListViewItem lvi;
                foreach (var prop in item.Properies)
                {
                    lvi = new ListViewItem(prop.Name) { Tag = prop };
                    lvi.SubItems.Add($"{prop}");
                    lvi.SubItems.Add($"{prop.Value}");
                    lv.Items.Add(lvi);
                }
            }
            finally
            {
                lv.EndUpdate();
            }
        }

        private void listContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (treeView.SelectedNode == null)
            {
                e.Cancel = true;
                return;
            }
            var item = (ModelItem)treeView.SelectedNode.Tag;

            var propFound = listView.SelectedIndices.Count == 1;
            tsmiRenameProp.Visible = propFound;
            tsmiDeleteProp.Visible = propFound;
            tsmiChangeValue.Visible = propFound;
        }

        private void tsmiAddProp_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            var item = (ModelItem)treeView.SelectedNode.Tag;
            var frm = new PropertyEditorForm();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var prop = new ModelProperty() { Name = frm.tbName.Text, Type = ((TypeItem)frm.cbDataKind.SelectedItem).Type };
                item.Properies.Add(prop);
                FillList(listView);
                RestorePropSeletion(prop);
            }
        }

        private void tsmiDeleteProp_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            var item = (ModelItem)treeView.SelectedNode.Tag;
            if (listView.SelectedIndices.Count != 1) return;
            var prop = (ModelProperty)listView.SelectedItems[0].Tag;
            if (prop == null) return;
            if (MessageBox.Show(this, "Are you sure to delete this property?", "Delete property",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            _undoRedoController.OnStartOperation("Remove prop");
            item.Properies.Remove(prop);
            _undoRedoController.OnFinishOperation();
            FillList(listView);
        }

        private void tsmiRenameProp_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            var item = (ModelItem)treeView.SelectedNode.Tag;
            if (listView.SelectedIndices.Count != 1) return;
            var prop = (ModelProperty)listView.SelectedItems[0].Tag;
            if (prop == null) return;
            var frm = new StringEditorForm();
            frm.tbValue.Text = prop.Name;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                _undoRedoController.OnStartOperation("Rename prop");
                prop.Name = frm.tbValue.Text;
                _undoRedoController.OnFinishOperation();
                FillList(listView);
                RestorePropSeletion(prop);
            }
        }

        /// <summary>
        /// Перемещение элемента списка вверх
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMoveUp_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            var item = (ModelItem)treeView.SelectedNode.Tag;
            // для списка свойств
            if (listView.Focused)
            {
                if (listView.SelectedIndices.Count != 1) return;
                if (listView.SelectedItems[0].Index > 0)
                {
                    var index = listView.SelectedItems[0].Index;
                    var prop = item.Properies[index];
                    _undoRedoController.OnStartOperation("Move prop to up");
                    item.Properies.RemoveAt(index);
                    item.Properies.Insert(index - 1, prop);
                    _undoRedoController.OnFinishOperation();
                    FillList(listView);
                    RestorePropSeletion(prop);
                }
            }
        }

        /// <summary>
        /// восстановление выбора свойства
        /// </summary>
        /// <param name="prop"></param>
        private void RestorePropSeletion(ModelProperty prop)
        {
            var lvi = listView.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Tag == prop);
            if (lvi != null)
                lvi.Selected = true;
        }

        /// <summary>
        /// Перемещение элемента списка вниз
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMoveDown_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            var item = (ModelItem)treeView.SelectedNode.Tag;
            // для списка свойств
            if (listView.Focused)
            {
                if (listView.SelectedIndices.Count != 1) return;
                if (listView.SelectedItems[0].Index < listView.Items.Count - 1)
                {
                    var index = listView.SelectedItems[0].Index;
                    var prop = item.Properies[index];
                    _undoRedoController.OnStartOperation("Move prop to down");
                    item.Properies.RemoveAt(index);
                    item.Properies.Insert(index + 1, prop);
                    _undoRedoController.OnFinishOperation();
                    FillList(listView);
                    RestorePropSeletion(prop);
                }
            }
        }

        /// <summary>
        /// Изменить значение свойства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiChangeValue_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            var item = (ModelItem)treeView.SelectedNode.Tag;
            if (listView.SelectedIndices.Count != 1) return;
            var prop = (ModelProperty)listView.SelectedItems[0].Tag;
            if (prop == null) return;
            var frm = new StringEditorForm();
            frm.tbValue.Text = $"{prop.Value}";
            if (frm.ShowDialog() == DialogResult.OK)
            {
                _undoRedoController.OnStartOperation("Change prop");
                prop.Value = frm.tbValue.Text;
                _undoRedoController.OnFinishOperation();
                FillList(listView);
                RestorePropSeletion(prop);
            }
        }
    }
}

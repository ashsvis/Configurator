using ModelHolder;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BaseEditor
{
    public partial class BaseEditorForm : Form
    {
        private ModelRoot modelRoot = new ModelRoot();
        private string defaultFileName;

        public BaseEditorForm()
        {
            InitializeComponent();
            defaultFileName = Path.ChangeExtension(Application.ExecutablePath, ".bin");
        }

        private void BaseEditorForm_Load(object sender, System.EventArgs e)
        {
            if (File.Exists(defaultFileName))
                modelRoot = SaverLoader.LoadFromFile(defaultFileName);
            FillTree(treeView);
        }

        private void BaseEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaverLoader.SaveToFile(defaultFileName, modelRoot);
        }

        /// <summary>
        /// Заполнение дерева сущностей
        /// </summary>
        /// <param name="treeView"></param>
        private void FillTree(TreeView treeView)
        {
            treeView.BeginUpdate();
            try
            {
                treeView.Nodes.Clear();
                var rootNode = new TreeNode(modelRoot.Name) { Tag = modelRoot };
                treeView.Nodes.Add(rootNode);
                AddChildNodes(rootNode, modelRoot);
                treeView.ExpandAll();
            }
            finally
            {
                treeView.EndUpdate();
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
                item.Name = frm.tbValue.Text;
                treeView.SelectedNode.Text = frm.tbValue.Text;
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
            var childItem = new ModelItem() { Name = "Child", Parent = item };
            item.Childs.Add(childItem);
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
            item.Parent.Childs.Remove(item);
            // внесём изменения в визуальный интерфейс
            var parentNode = treeView.SelectedNode.Parent;
            parentNode.Nodes.Remove(treeView.SelectedNode);
            treeView.SelectedNode = parentNode;
        }
    }
}

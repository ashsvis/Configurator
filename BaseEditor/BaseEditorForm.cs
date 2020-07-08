using ModelHolder;
using System;
using System.Linq;
using System.Windows.Forms;

namespace BaseEditor
{
    public partial class BaseEditorForm : Form
    {
        private ModelRoot modelRoot = new ModelRoot();

        public BaseEditorForm()
        {
            InitializeComponent();
        }

        private void BaseEditorForm_Load(object sender, System.EventArgs e)
        {
            FillTree(treeView);
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

        private void BaseEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {

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
            item.Parent.Childs.Remove(item);
            // внесём изменения в визуальный интерфейс
            var parentNode = treeView.SelectedNode.Parent;
            parentNode.Nodes.Remove(treeView.SelectedNode);
            treeView.SelectedNode = parentNode;
        }
    }
}

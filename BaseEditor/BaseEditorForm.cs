using ModelHolder;
using System;
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
    }
}

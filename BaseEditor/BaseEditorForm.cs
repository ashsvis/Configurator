﻿using ModelHolder;
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
            // если нет выбранного узла, то меню не показываем совсем
            e.Cancel = !nodeFound;
        }
    }
}
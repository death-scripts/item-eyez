using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace item_eyez
{
    /// <summary>
    /// Interaction logic for items_view.xaml
    /// </summary>
    public partial class items_view : UserControl
    {
        public items_view()
        {
            InitializeComponent();
        }
        private void datagrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Temporarily unsubscribe from the event to avoid stack overflow
                this.datagrid.CellEditEnding -= this.datagrid_CellEditEnding;

                datagrid.CommitEdit(DataGridEditingUnit.Row, true);
                DataRowView row = e.Row.Item as DataRowView;
                if (row != null)
                {
                    try
                    {
                        // Update the database with the new values
                        ItemEyezDatabase.Instance().UpdateItem(
                            (Guid)row["id"],
                            (string)row["name"],
                            (string)row["description"],
                            (decimal)row["value"],
                            (string)row["catagories"]
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                // Re-subscribe to the event
                this.datagrid.CellEditEnding += datagrid_CellEditEnding;
            }
        }
    }
}

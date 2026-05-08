import re

filepath = 'mRemoteNG/UI/Window/ConnectionTreeWindow.cs'
with open(filepath, 'r') as f:
    content = f.read()

pattern = r'private void TvConnections_KeyDown\(object sender, KeyEventArgs e\)\s+\{\s+try\s+\{\s+if \(e\.KeyCode == Keys\.Enter\)\s+\{\s+e\.Handled = true;\s+var connectionsToOpen = ConnectionTree\.SelectedObjects\s+\.OfType<ConnectionInfo\(\)>\s+\.Where\(node => node\.GetTreeNodeType\(\) == TreeNodeType\.Connection\)\s+\.Take\(Settings\.Default\.MaxBulkConnect\)\s+\.ToList\(\);\s+foreach \(var connection in connectionsToOpen\)\s+\{\s+Runtime\.ConnectionInitiator\.OpenConnection\(connection\);\s+\}\s+\}\s+\}\s+else if \(e\.Control && e\.KeyCode == Keys\.F\)'

# The above regex is too fragile. Let's use a simpler approach.
start_marker = 'private void TvConnections_KeyDown(object sender, KeyEventArgs e)'
end_marker = '#endregion' # End of the search region

start_idx = content.find(start_marker)
end_idx = content.find(end_marker, start_idx)

method_body = """private void TvConnections_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;

                    var connectionsToOpen = ConnectionTree.SelectedObjects
                                                         .OfType<ConnectionInfo>()
                                                         .Where(node => node.GetTreeNodeType() == TreeNodeType.Connection)
                                                         .Take(Settings.Default.MaxBulkConnect)
                                                         .ToList();

                    foreach (var connection in connectionsToOpen)
                    {
                        Runtime.ConnectionInitiator.OpenConnection(connection);
                    }
                }
                else if (e.Control && e.KeyCode == Keys.F)
                {
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Runtime.MessageCollector.AddExceptionStackTrace("tvConnections_KeyDown (UI.Window.ConnectionTreeWindow) failed", ex);
            }
        }

        """

new_content = content[:start_idx] + method_body + content[end_idx:]
with open(filepath, 'w') as f:
    f.write(new_content)
print("Method fixed.")
